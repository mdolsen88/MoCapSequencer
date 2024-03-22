using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoCapSequencer.MDOL
{
    public static class IO
    {
        public static T[] ReadAll<T>(string path)
        {
            return FromBytes<T>(System.IO.File.ReadAllBytes(path));
        }
        public static T[] FromBytes<T>(byte[] bytes)
        {
            T[] data = new T[bytes.Length / Buffer.ByteLength(new T[1])];
            Buffer.BlockCopy(bytes, 0, data, 0, bytes.Length);
            return data;
        }
        public static class Ports
        {
            public static string TTL = "USB Serial Port";
            public static string Arduino = "USB-SERIAL CH340";
            public static int[] GetPorts(string MustContain = "")
            {
                List<int> ports = new List<int>();
                System.Management.ManagementClass processClass = new System.Management.ManagementClass("Win32_PnPEntity");
                System.Management.ManagementObjectCollection Ports = processClass.GetInstances();
                foreach (System.Management.ManagementObject property in Ports)
                    if (property.GetPropertyValue("Name") != null)
                        if (MustContain == "" || property.GetPropertyValue("Name").ToString().Contains(MustContain))
                        {
                            string name = property.GetPropertyValue("Name").ToString();
                            string[] elements = name.Split(new string[] { "(", ")" }, StringSplitOptions.RemoveEmptyEntries);
                            if (elements.Length > 1 && elements[1].Contains("COM"))
                                ports.Add(int.Parse(elements[1].Replace("COM", "")));
                        }
                return ports.ToArray();
            }
        }
        public class XML
        {
            public readonly string mTag;
            readonly List<XML> mElements = new List<XML>();
            readonly string mValue = "";

            public static XML Read(string Filename)
            {
                return new XML(System.IO.File.ReadAllText(Filename));
            }
            public XML(string Tag, string Value)
            {
                mTag = Tag;
                mValue = Value;
            }
            public XML(string Tag, params XML[] elements)
            {
                mTag = Tag;
                foreach (XML element in elements)
                    addElement(element);
            }
            public void addElement(XML element)
            {
                mElements.Add(element);
            }
            public bool hasElement(string Tag)
            {
                foreach (XML element in mElements)
                    if (element.mTag.Equals(Tag))
                        return true;
                return false;
            }
            public List<XML> getElements()
            {
                return mElements;
            }
            public XML getElement(string Tag, bool Contains = false)
            {
                foreach (XML element in mElements)
                    if (element.mTag.Equals(Tag) || (Contains && element.mTag.Contains(Tag)))
                        return element;
                return null;
            }
            public int getInt(string Tag, int def)
            {
                if (hasElement(Tag))
                    return int.Parse(getElement(Tag).mValue);
                else
                    return def;
            }
            public double getDouble(string Tag, double def)
            {
                if (hasElement(Tag))
                    return Extension.ToDouble(getElement(Tag).mValue);
                else
                    return def;
            }
            public float getFloat(string Tag, float def)
            {
                if (hasElement(Tag))
                    return Extension.ToFloat(getElement(Tag).mValue);
                else
                    return def;
            }
            public long getLong(string Tag, long def)
            {
                if (hasElement(Tag))
                    return long.Parse(getElement(Tag).mValue);
                else
                    return def;
            }
            public bool getBool(string Tag, bool def)
            {
                if (hasElement(Tag))
                    return bool.Parse(getElement(Tag).mValue);
                else
                    return def;
            }
            public string getString(string Tag, string def)
            {
                if (hasElement(Tag))
                    return getElement(Tag).mValue;
                else
                    return def;
            }

            readonly int iCurrent;
            public XML(string str) : this(str.Replace("\t", "").Replace("\r", "").Replace("\n", ""), 0)
            {
            }
            private XML(string str, int icurrent)
            {
                if (str[0] != '<')
                {
                    mTag = str;
                }
                else
                {
                    iCurrent = icurrent;
                    int iStart = iCurrent + 1;
                    while (str[iCurrent] != '>')
                        iCurrent++;
                    int iEnd = iCurrent - 1;
                    iCurrent++;
                    mTag = str.Substring(iStart, iEnd + 1 - iStart);
                    if (str[iCurrent] == '<')
                    {
                        while (!str.Substring(iCurrent, mTag.Length + 3).Equals("</" + mTag + ">"))
                        {
                            XML element = new XML(str, iCurrent);
                            mElements.Add(element);
                            iCurrent = element.iCurrent;
                        }
                        iCurrent += mTag.Length + 3;
                    }
                    else
                    {
                        iStart = iCurrent;
                        while (!str.Substring(iCurrent, mTag.Length + 3).Equals("</" + mTag + ">"))
                        {
                            iCurrent++;
                        }
                        iEnd = iCurrent - 1;
                        mValue = str.Substring(iStart, iEnd + 1 - iStart);
                        iCurrent += mTag.Length + 3;
                    }
                }
            }

            public override string ToString()
            {
                string str = "<" + mTag + ">";
                if (mElements.Count == 0)
                    str += mValue;
                else
                    foreach (XML value in mElements)
                    {
                        str += value.ToString();
                    }
                str += "</" + mTag + ">";
                return str;
            }

            public string ToString(int Indent)
            {
                string str = "";
                for (int i = 0; i < Indent; i++)
                    str += "\t";
                str += "<" + mTag + ">\r\n";
                if (mElements.Count == 0)
                {
                    for (int i = 0; i < Indent + 1; i++)
                        str += "\t";
                    str += mValue + "\r\n";
                }
                else
                    foreach (XML value in mElements)
                        str += value.ToString(Indent + 1);
                for (int i = 0; i < Indent; i++)
                    str += "\t";
                str += "</" + mTag + ">\r\n";
                return str;
            }

            public static XML[] ToXML<T>(T t)
            {
                System.Reflection.FieldInfo[] fields = t.GetType().GetFields();
                return fields.Select(field =>
                {
                    return new XML(field.Name, field.GetValue(t).ToString());
                }).ToArray();
            }
            public static T FromXML<T>(XML xml) where T : new()
            {
                System.Reflection.FieldInfo[] fields = typeof(T).GetFields();
                T t = new T();

                foreach (System.Reflection.FieldInfo field in fields)
                    if (field.FieldType == typeof(int))
                        field.SetValue(t, xml.getInt(field.Name, -1));
                    else if (field.FieldType == typeof(double))
                        field.SetValue(t, xml.getDouble(field.Name, double.NaN));
                    else if (field.FieldType == typeof(float))
                        field.SetValue(t, xml.getFloat(field.Name, float.NaN));
                    else if (field.FieldType == typeof(bool))
                        field.SetValue(t, xml.getBool(field.Name, false));
                    else if (field.FieldType == typeof(long))
                        field.SetValue(t, xml.getLong(field.Name, -1));
                    else if (field.FieldType == typeof(string))
                        field.SetValue(t, xml.getString(field.Name, null));
                    else if (field.FieldType.IsEnum)
                        field.SetValue(t, Enum.Parse(field.FieldType, xml.getString(field.Name, null)));
                    else
                        throw new TypeAccessException();
                return t;
            }
            public static void SetFromXML<T>(XML xml, T t)
            {
                System.Reflection.FieldInfo[] fields = t.GetType().GetFields();
                foreach (System.Reflection.FieldInfo field in fields)
                    if (field.FieldType == typeof(int))
                        field.SetValue(t, xml.getInt(field.Name, -1));
                    else if (field.FieldType == typeof(double))
                        field.SetValue(t, xml.getDouble(field.Name, double.NaN));
                    else if (field.FieldType == typeof(float))
                        field.SetValue(t, xml.getFloat(field.Name, float.NaN));
                    else if (field.FieldType == typeof(bool))
                        field.SetValue(t, xml.getBool(field.Name, false));
                    else if (field.FieldType == typeof(long))
                        field.SetValue(t, xml.getLong(field.Name, -1));
                    else if (field.FieldType == typeof(string))
                        field.SetValue(t, xml.getString(field.Name, null));
                    else if (field.FieldType.IsEnum)
                        field.SetValue(t, Enum.Parse(field.FieldType, xml.getString(field.Name, null)));
                    else
                        throw new TypeAccessException();
            }
        }

        public static class DataSaver
        {
            public interface byteAble
            {
                byte[] getBytes();
            }
            static System.IO.BinaryWriter BW = null;
            static Queue<DataPacket> dataPackets = new Queue<DataPacket>();
            static List<Type> knownHeaders = new List<Type>();
            static System.Threading.Thread thread = null;
            static bool stopThread = false;
            public static void NewFile(string Filename, bool Threaded = false)
            {
                dataPackets.Clear();
                knownHeaders.Clear();
                BW = new System.IO.BinaryWriter(System.IO.File.OpenWrite(Filename));
                if (Threaded)
                {
                    stopThread = false;
                    thread = new System.Threading.Thread(() =>
                    {
                        while (!stopThread)
                        {
                            int nPackets = dataPackets.Count;
                            for (int i = 0; i < nPackets; i++)
                                WritePacket(dataPackets.Dequeue());
                            System.Threading.Thread.Sleep(1000); // Take a break and wait for more packages to pile up
                        }
                    });
                    thread.Start();
                }
            }
            static void WritePacket(DataPacket dataPacket)
            {
                if (dataPacket != null)
                {
                    int PacketID;
                    if (!knownHeaders.Contains(dataPacket.GetType()))
                    {
                        PacketID = knownHeaders.Count;
                        dataPacket.WriteHeader(BW, PacketID);
                        knownHeaders.Add(dataPacket.GetType());
                    }
                    else
                        PacketID = knownHeaders.IndexOf(dataPacket.GetType());
                    dataPacket.Write(BW, PacketID);
                }
            }
            public static void Close()
            {
                if (thread != null)
                {
                    stopThread = true;
                    thread.Join();
                }
                if (BW != null)
                {
                    BW.Close();
                    BW = null;
                }
            }
            public static void AddPacket(DataPacket dataPacket)
            {
                if (BW != null)
                {
                    dataPackets.Enqueue(dataPacket);
                    if (thread == null) // Not threaded - write to file now
                    {
                        lock (dataPackets)
                        {
                            int nPackets = dataPackets.Count;
                            for (int i = 0; i < nPackets; i++)
                                WritePacket(dataPackets.Dequeue());
                        }
                    }
                }
            }
            public abstract class DataPacket
            {
                private List<byte[]> Data = new List<byte[]>();
                private List<string> Headers = new List<string>();
                protected abstract string PacketStr
                {
                    get;
                }

                long ticks = DateTime.Now.Ticks;
                public void Write(System.IO.BinaryWriter BW, int PacketID)
                {
                    BW.Write(ticks);
                    BW.Write(PacketID);
                    BW.Write(Data.Sum(d => d.Length));
                    foreach (byte[] datum in Data)
                        BW.Write(datum);
                }
                public void WriteHeader(System.IO.BinaryWriter BW, int PacketID)
                {
                    BW.Write(ticks);
                    BW.Write(PacketID);

                    byte[] bytes = Encoding.UTF8.GetBytes(PacketStr);
                    BW.Write(bytes.Length);
                    BW.Write(bytes);

                    BW.Write(Headers.Count);
                    foreach (string str in Headers)
                    {
                        bytes = Encoding.UTF8.GetBytes(str);
                        BW.Write(bytes.Length);
                        BW.Write(bytes);
                    }
                }
                protected void AddData(string header, byte data)
                {
                    Data.Add(new byte[] { data });
                    Headers.Add(header + ";" + data.GetType());
                }
                protected void AddData(string header, byte[] data)
                {
                    Data.Add(BitConverter.GetBytes(data.Length));
                    Data.Add(data);
                    Headers.Add(header + ";" + data.GetType());
                }
                protected void AddData(string header, bool data)
                {
                    Data.Add(BitConverter.GetBytes(data));
                    Headers.Add(header + ";" + data.GetType());
                }
                protected void AddData(string header, bool[] data)
                {
                    Data.Add(BitConverter.GetBytes(data.Length));
                    foreach (bool datum in data)
                        Data.Add(BitConverter.GetBytes(datum));
                    Headers.Add(header + ";" + data.GetType());
                }
                protected void AddData(string header, int data)
                {
                    Data.Add(BitConverter.GetBytes(data));
                    Headers.Add(header + ";" + data.GetType());
                }
                protected void AddData(string header, int[] data)
                {
                    Data.Add(BitConverter.GetBytes(data.Length));
                    foreach (int datum in data)
                        Data.Add(BitConverter.GetBytes(datum));
                    Headers.Add(header + ";" + data.GetType());
                }
                protected void AddData(string header, double data)
                {
                    Data.Add(BitConverter.GetBytes(data));
                    Headers.Add(header + ";" + data.GetType());
                }
                protected void AddData(string header, double[] data)
                {
                    Data.Add(BitConverter.GetBytes(data.Length));
                    foreach (double datum in data)
                        Data.Add(BitConverter.GetBytes(datum));
                    Headers.Add(header + ";" + data.GetType());
                }
                protected void AddData(string header, float data)
                {
                    Data.Add(BitConverter.GetBytes(data));
                    Headers.Add(header + ";" + data.GetType());
                }
                protected void AddData(string header, float[] data)
                {
                    Data.Add(BitConverter.GetBytes(data.Length));
                    foreach (float datum in data)
                        Data.Add(BitConverter.GetBytes(datum));
                    Headers.Add(header + ";" + data.GetType());
                }
                protected void AddData(string header, short data)
                {
                    Data.Add(BitConverter.GetBytes(data));
                    Headers.Add(header + ";" + data.GetType());
                }
                protected void AddData(string header, short[] data)
                {
                    Data.Add(BitConverter.GetBytes(data.Length));
                    foreach (short datum in data)
                        Data.Add(BitConverter.GetBytes(datum));
                    Headers.Add(header + ";" + data.GetType());
                }
                protected void AddData(string header, long data)
                {
                    Data.Add(BitConverter.GetBytes(data));
                    Headers.Add(header + ";" + data.GetType());
                }
                protected void AddData(string header, long[] data)
                {
                    Data.Add(BitConverter.GetBytes(data.Length));
                    foreach (long datum in data)
                        Data.Add(BitConverter.GetBytes(datum));
                    Headers.Add(header + ";" + data.GetType());
                }
                protected void AddData(string header, string data)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(data);
                    Data.Add(BitConverter.GetBytes(bytes.Length));
                    Data.Add(bytes);
                    Headers.Add(header + ";" + data.GetType());
                }
                protected void AddData(string header, string[] data)
                {
                    Data.Add(BitConverter.GetBytes(data.Length));
                    foreach (string datum in data)
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(datum);
                        Data.Add(BitConverter.GetBytes(bytes.Length));
                        Data.Add(bytes);
                    }

                    Headers.Add(header + ";" + data.GetType());
                }
                protected void AddData(string header, ushort data)
                {
                    Data.Add(BitConverter.GetBytes(data));
                    Headers.Add(header + ";" + data.GetType());
                }
                protected void AddData(string header, ushort[] data)
                {
                    Data.Add(BitConverter.GetBytes(data.Length));
                    foreach (ushort datum in data)
                        Data.Add(BitConverter.GetBytes(datum));
                    Headers.Add(header + ";" + data.GetType());
                }
                protected void AddData(string header, uint data)
                {
                    Data.Add(BitConverter.GetBytes(data));
                    Headers.Add(header + ";" + data.GetType());
                }
                protected void AddData(string header, uint[] data)
                {
                    Data.Add(BitConverter.GetBytes(data.Length));
                    foreach (uint datum in data)
                        Data.Add(BitConverter.GetBytes(datum));
                    Headers.Add(header + ";" + data.GetType());
                }
                protected void AddData(string header, ulong data)
                {
                    Data.Add(BitConverter.GetBytes(data));
                    Headers.Add(header + ";" + data.GetType());
                }
                protected void AddData(string header, ulong[] data)
                {
                    Data.Add(BitConverter.GetBytes(data.Length));
                    foreach (ulong datum in data)
                        Data.Add(BitConverter.GetBytes(datum));
                    Headers.Add(header + ";" + data.GetType());
                }
                protected void AddData(string header, System.Drawing.PointF data)
                {
                    Data.Add(BitConverter.GetBytes(data.X));
                    Data.Add(BitConverter.GetBytes(data.Y));
                    Headers.Add(header + ";" + data.GetType());
                }
                protected void AddData(string header, System.Drawing.PointF[] data)
                {
                    Data.Add(BitConverter.GetBytes(data.Length));
                    foreach (System.Drawing.PointF datum in data)
                    {
                        Data.Add(BitConverter.GetBytes(datum.X));
                        Data.Add(BitConverter.GetBytes(datum.Y));
                    }
                    Headers.Add(header + ";" + data.GetType());
                }
                protected void AddData(string header, byteAble data)
                {
                    Data.Add(data.getBytes());
                    Headers.Add(header + ";" + data.GetType());
                }
                protected void AddData(string header, byteAble[] data)
                {
                    Data.Add(BitConverter.GetBytes(data.Length));
                    foreach (byteAble datum in data)
                        Data.Add(datum.getBytes());
                    Headers.Add(header + ";" + data.GetType());
                }
                protected void AddData<T>(string header, T data, Func<T, byte[]> BitConverter)
                {
                    Data.Add(BitConverter(data));
                    Headers.Add(header + ";" + data.GetType());
                }
                protected void AddData<T>(string header, T[] data, Func<T, byte[]> BitConverterT)
                {
                    Data.Add(BitConverter.GetBytes(data.Length));
                    foreach (T datum in data)
                        Data.Add(BitConverterT(datum));
                    Headers.Add(header + ";" + data.GetType());
                }
            }
        }
    }
}
