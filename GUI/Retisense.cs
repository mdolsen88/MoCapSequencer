using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
namespace MoCapSequencer.GUI
{
    public partial class Retisense : UserControl,tabInterface
    {
        public class Insole
        {
            public class InsolePacket : MDOL.IO.DataSaver.DataPacket
            {
                protected override string PacketStr => "Insole";
                public InsolePacket(bool isLeft, byte[] bytes)
                {
                    AddData("isLeft", isLeft);
                    AddData("bytes", bytes);
                }
            }
            public class PressureFrame
            {
                public int Time;
                public byte[] Bytes;
                public int[] Raw = null;
                public int[] Pressure = null;
                public PressureFrame(byte[] bytes)
                {
                    Bytes = bytes;
                    Time = BitConverter.ToInt32(Bytes, 0);

                    Raw = new int[8];
                    for (int i = 0; i < 8; i++)
                        Raw[i] = 255 - Bytes[i + 17];

                    Pressure = Raw;
                }
            }
            // Make global to prevent disposal
            GattCharacteristic gattCharacteristic;
            GattCharacteristic gattCharacteristicFPS;
            public int[] pressure = new int[8];
            public string HardwareVersionString = "";
            BluetoothLEDevice device = null;
            DateTime[] timeStamps = new DateTime[10];
            int timeStamps_i = 0;

            public event EventHandler<PressureFrame> PressureChanged = null;
            public Insole()
            {
            }
            public void Disconnect()
            {
                if (device != null)
                    device.Dispose();
            }
            bool isLeft;
            public string ID;
            public Insole(BluetoothLEDevice device,string ID, GattDeviceService[] Services, bool isLeft)
            {
                this.device = device;
                this.isLeft = isLeft;
                this.ID = ID;
                //ID = device.DeviceId.Replace("BluetoothLE#BluetoothLE", "").Replace(':', '_');
                ReadHardwareVersionString(Services);
                StartNotification(Services);
            }

            async void ReadHardwareVersionString(GattDeviceService[] Services)
            {
                foreach (GattDeviceService service in Services)
                    if (service.Uuid.ToString().Contains("180a"))
                    {
                        GattCharacteristicsResult charResults = await service.GetCharacteristicsAsync();
                        if (charResults.Status == GattCommunicationStatus.Success)
                        {
                            foreach (GattCharacteristic characteristic in charResults.Characteristics)
                                if (characteristic.Uuid.ToString().Contains("2a27"))
                                {
                                    GattReadResult readResult = await characteristic.ReadValueAsync();
                                    DataReader reader = DataReader.FromBuffer(readResult.Value);
                                    HardwareVersionString = reader.ReadString(reader.UnconsumedBufferLength);
                                }
                        }
                    }
            }
            async void StartNotification(GattDeviceService[] Services)
            {
                foreach (GattDeviceService service in Services)
                    if (service.Uuid.ToString().Contains("1814"))
                    {
                        GattCharacteristicsResult CharResults = await service.GetCharacteristicsAsync();

                        if (CharResults.Status == GattCommunicationStatus.Success)
                        {
                            var characteristics = CharResults.Characteristics;
                            foreach (GattCharacteristic characteristic in characteristics)
                            {
                                if (characteristic.Uuid.ToString().Contains("2a53"))
                                {
                                    gattCharacteristic = characteristic;
                                    var status = await gattCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                                    if (status == GattCommunicationStatus.Success)
                                        gattCharacteristic.ValueChanged += (s, e) =>
                                        {
                                            timeStamps[timeStamps_i++] = DateTime.Now;
                                            if (timeStamps_i == timeStamps.Length)
                                                timeStamps_i = 0;

                                            DataReader reader = DataReader.FromBuffer(e.CharacteristicValue);
                                            byte[] bytes = new byte[reader.UnconsumedBufferLength];
                                            reader.ReadBytes(bytes);

                                            MDOL.IO.DataSaver.AddPacket(new InsolePacket(isLeft, bytes));

                                            PressureFrame frame = new PressureFrame(bytes);
                                            pressure = frame.Pressure;
                                        };
                                }
                                else if (characteristic.Uuid.ToString().Contains("ff02"))
                                {
                                    gattCharacteristicFPS = characteristic;
                                }
                            }
                        }
                    }
            }
            public double GetFPS()
            {
                long[] times = timeStamps.Select(timeStamp => timeStamp.Ticks).ToArray();
                double mean = 0;
                int n = 0;
                for (int i = 0; i < times.Length;i++)
                {
                    long before;
                    long now = times[i];
                    if (i == 0)
                        before = times.Last();
                    else
                        before = times[i - 1];
                    if(now> before)
                    {
                        mean += (now - before) / (10000.0*1000.0);
                        n++;
                    }
                }
                if (n == 0)
                    return 0;
                else
                    return 1 / (mean / n);
            }
            public async void SetFPS(int FPS)
            {
                byte[] data = new byte[6];
                data[1] = 11;
                data[5] = (byte)(1000 / FPS);
                await gattCharacteristicFPS.WriteValueAsync(data.AsBuffer());
            }
        }
        static Insole Left = null;
        static Insole Right = null;
        static DeviceWatcher deviceWatcher;
        Queue<PointF>[] Trajectory = new Queue<PointF>[2] { new Queue<PointF>(), new Queue<PointF>() };
        double[] Locations = MDOL.IO.ReadAll<double>("LeftLocations.bin");
        public Retisense()
        {
            InitializeComponent();
            MDOL.FormResizer.FillParent(this, picPressure);

            chkAutoConnect.Checked = Settings.RetisenseStartup;
            chkAutoConnect.CheckedChanged += (s, e) =>
              {
                  Settings.RetisenseStartup = chkAutoConnect.Checked;
                  Settings.Save();
              };

            cmdConnect.Click += (s, e) =>
              {
                  Connect();
              };
            cmdScan.Click += (s, e) =>
              {
                  Scan();
              };
            cmdSet.Click += (s, e) =>
              {
                  foreach(BLDevice blDevice in lstDevices.SelectedItems)
                  {
                      if (blDevice.Name.Contains("L"))
                          Settings.RetisenseL = blDevice.MAC;
                      if (blDevice.Name.Contains("R"))
                          Settings.RetisenseR = blDevice.MAC;
                  }
                  Settings.Save();
              };
        }

        class BLDevice
        {
            public string Name;
            public string MAC;
            public BLDevice(string Name,string MAC)
            {
                this.Name = Name;
                this.MAC = MAC;
            }
            public override string ToString()
            {
                return Name + " - " + MAC;
            }
        }

        void Scan()
        {
            if (deviceWatcher != null)
                deviceWatcher.Stop();
            string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };
            deviceWatcher =
            DeviceInformation.CreateWatcher(
                    BluetoothLEDevice.GetDeviceSelectorFromPairingState(false),
                    requestedProperties,
                    DeviceInformationKind.AssociationEndpoint);
            deviceWatcher.Added += (s, e) =>
            {
                if (e.Name.Contains("INSIGHT"))
                    if (!blDevices.Any(bldevice => bldevice.MAC == e.Id))
                    {
                        blDevices.Add(new BLDevice(e.Name, e.Id));
                        if (lstDevices.InvokeRequired)
                            lstDevices.Invoke(new Action(() => { UpdateList(); }));
                        else
                            UpdateList();
                    }
            };
            deviceWatcher.Start();
        }

        void UpdateList()
        {
            lstDevices.Items.Clear();
            foreach (BLDevice blDevice in blDevices)
                lstDevices.Items.Add(blDevice);
        }

        List<BLDevice> blDevices = new List<BLDevice>();
        public void onLoad()
        {
            if (Settings.RetisenseStartup)
                Connect();
        }
        public void Destroy()
        {
            Disconnect();
        }
        public string[] IsReady()
        {
            List<string> msg = new List<string>();
            if (Left == null)
                msg.Add("Left insole was not found");
            if(Right == null)
                msg.Add("Right insole was not found");
            return msg.ToArray();
        }
        public void UpdateGUI()
        {
            if (Left != null)
            {
                lblRetisenseL.Text = "Left: Connected (" + MDOL.Extension.ToStringD(Left.GetFPS(),1) + " Hz)";
                lblRetisenseL.ForeColor = Color.Green;
            }
            if (Right != null)
            {
                lblRetisenseR.Text = "Right: Connected (" + MDOL.Extension.ToStringD(Right.GetFPS(), 1) + " Hz)";
                lblRetisenseR.ForeColor = Color.Green;
            }

            int[][] pressure = new int[2][];
            if (Left != null)
                pressure[0] = Left.pressure;
            else
                pressure[0] = null;
            if (Right != null)
                pressure[1] = Right.pressure;
            else
                pressure[1] = null;

            Bitmap bmp;
            if (System.IO.File.Exists("Foot.png"))
                bmp = (Bitmap)Image.FromFile("Foot.png");
            else
                bmp = new Bitmap(512, 512, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(bmp))
                for (int i = 0; i < 2; i++)
                    if (pressure[i] != null)
                    {
                        int sum = pressure[i].Sum();
                        PointF Center = new PointF();
                        for (int j = 0; j < 8; j++)
                        {
                            int p = pressure[i][j];
                            int X = (int)((1 + (i == 0 ? 1 : -1) * Locations[j * 2]) * 512 / 2);
                            int Y = (int)(Locations[j * 2 + 1] * 512);

                            if (sum != 0)
                            {
                                Center.X += X * p / sum;
                                Center.Y += Y * p / sum;
                            }
                            p /= 3;
                            RectangleF rect = new RectangleF(X - p, Y - p, p * 2 + 1, p * 2 + 1);

                            // Create a path that consists of a single ellipse.
                            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
                            path.AddEllipse(rect);

                            System.Drawing.Drawing2D.PathGradientBrush pthGrBrush = new System.Drawing.Drawing2D.PathGradientBrush(path);

                            pthGrBrush.CenterColor = i == 0 ? Color.Red : Color.Green;

                            Color[] colors = { Color.Transparent };
                            pthGrBrush.SurroundColors = colors;

                            g.FillEllipse(pthGrBrush, rect);
                        }
                        g.FillEllipse(new SolidBrush(i == 0 ? Color.Red : Color.Green), Center.X - 10, Center.Y - 10, 21, 21);
                        Trajectory[i].Enqueue(Center);
                        if (Trajectory[i].Count >= 2)
                            g.DrawLines(new Pen(i == 0 ? Color.Red : Color.Green), Trajectory[i].ToArray());
                        if (Trajectory[i].Count > 10)
                            Trajectory[i].Dequeue();
                    }
            MDOL.Helper.SetImage(picPressure, bmp);
        }

        public static void Connect()
        {
            if (Settings.RetisenseL != "" && (Left == null || (Left.ID != Settings.RetisenseL)))
                connect(Settings.RetisenseL, true);
            if (Settings.RetisenseR != "" && (Right == null || (Right.ID != Settings.RetisenseR)))
                connect(Settings.RetisenseR, false);
        }

        static async void connect(string Id, bool isLeft)
        {
            BluetoothLEDevice device = await BluetoothLEDevice.FromIdAsync(Id);
            GattDeviceServicesResult ServicesResult = await device.GetGattServicesAsync();
            if (ServicesResult.Status == GattCommunicationStatus.Success)
            {
                if (isLeft)
                    Left = new Insole(device,Id, ServicesResult.Services.ToArray(), true);
                else
                    Right = new Insole(device,Id, ServicesResult.Services.ToArray(), false);
            }
        }
        public static void Disconnect()
        {
            if (Left != null)
                Left.Disconnect();
            if (Right != null)
                Right.Disconnect();
        }
    }
}
