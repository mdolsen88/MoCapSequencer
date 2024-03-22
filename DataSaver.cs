using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Linq;
namespace MoCapSequencer
{
    /*public static class DataSavers
    {
        static System.IO.BinaryWriter BW = null;
        static Queue<DataPacket> dataPackets = new Queue<DataPacket>();
        static List<Type> knownHeaders = new List<Type>();
        static Thread thread = null;
        static bool stopThread = false;
        public static void NewFile(MoCapForm.MoCapSystem mocapSystem, string PostFix = "")
        {
            dataPackets.Clear();
            knownHeaders.Clear();
            //dataPackets.Enqueue(new MoCapForm.MoCapPacket.Info(mocapSystem));
            if (!System.IO.Directory.Exists("Recordings"))
                System.IO.Directory.CreateDirectory("Recordings");
            BW = new System.IO.BinaryWriter(System.IO.File.OpenWrite("Recordings/" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + (PostFix != "" ? "_" + PostFix : "") + ".bin"));
            thread = new Thread(() =>
            {
                while (!stopThread)
                {
                    int nPackets = dataPackets.Count;
                    for(int i = 0; i < nPackets;i++)
                    {
                        DataPacket packet = dataPackets.Dequeue();
                        if (packet != null)
                        {
                            if (!knownHeaders.Contains(packet.GetType()))
                            {
                                packet.WriteHeader(BW);
                                knownHeaders.Add(packet.GetType());
                            }
                            packet.Write(BW);
                        }
                    }
                    Thread.Sleep(1000); // Take a break and wait for more packages to pile up
                }
            });
            stopThread = false;
            thread.Start();
        }
        public static void Close()
        {
            if (thread != null)
            {
                stopThread = true;
                thread.Join();
                if (BW != null)
                {
                    BW.Close();
                    BW = null;
                }
            }
        }
        public static void AddPacket(DataPacket dataPacket)
        {
            if(BW != null)
                dataPackets.Enqueue(dataPacket);
        }
        public abstract class DataPacket
        {
            protected enum Primaries { MoCap, Visual, Treadmill, RandomWalk,Retisense}
            protected List<byte[]> data = new List<byte[]>();

            public abstract int Primary
            {
                get;
            }
            public abstract int Secondary
            {
                get;
            }

            long ticks = DateTime.Now.Ticks;
            public void Write(System.IO.BinaryWriter BW)
            {
                BW.Write(ticks);
                BW.Write(Primary);
                BW.Write(Secondary);
                BW.Write(data.Sum(d=>d.Length));
                foreach (byte[] datum in data)
                    BW.Write(datum);
            }
            public void WriteHeader(System.IO.BinaryWriter BW)
            {
                BW.Write(ticks);
                BW.Write(Primary);
                BW.Write(Secondary);
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(((Primaries)Primary).ToString() + ";" + header());
                BW.Write(-bytes.Length);
                BW.Write(bytes);
            }
            protected abstract string header();
        }
    }*/
}
