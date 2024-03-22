using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml;

using MoCapSequencer.MDOL;
namespace MoCapSequencer
{
    class Trial
    {
        public event EventHandler TrialStopped = null;
        public float Speed = 0;
        public float Radius = 0.1f;
        public PointF Start = new PointF();
        public float EndTime = float.NaN;
        public float EndY = -5;
        public float EndUser = float.NaN;

        public int ID = 0;
        Series serie;
        PointF current = new PointF();

        Timer tmr = new Timer()
        {
            Interval = 30,
        };
        public Trial(int ID)
        {
            this.ID = ID;

            serie = new Series(ID.ToString())
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Blue,
            };

            serie.Points.AddXY(Start.X-Radius, Start.Y-Radius);
            serie.Points.AddXY(Start.X + Radius, Start.Y - Radius);
            serie.Points.AddXY(Start.X + Radius, Start.Y + Radius);
            serie.Points.AddXY(Start.X - Radius, Start.Y + Radius);
            serie.Points.AddXY(Start.X - Radius, Start.Y - Radius);

            tmr.Tick += Tmr_Tick;
        }
        public Trial Clone()
        {
            return new Trial(ToXML());
        }
        public Trial(IO.XML xml) : this(xml.getInt("ID", -1))
        {
            Speed = xml.getFloat("Speed", 0);
            Start.X = xml.getFloat("StartX", 0);
            Start.Y = xml.getFloat("StartY", 0);
            Radius = xml.getFloat("Radius", 0);
            EndTime = xml.getFloat("EndTime", float.NaN);
            EndY = xml.getFloat("EndY", float.NaN);
            EndUser = xml.getFloat("EndUser", float.NaN);
        }
        public IO.XML ToXML()
        {
            return new IO.XML("Trial",
                new IO.XML("ID", ID.ToString()),
                new IO.XML("Speed", Speed.ToStringF()),
                new IO.XML("StartX", Start.X.ToStringF()),
                new IO.XML("StartY", Start.Y.ToStringF()),
                new IO.XML("Radius", Radius.ToStringF()),
                new IO.XML("EndTime", EndTime.ToStringF()),
                new IO.XML("EndY", EndY.ToStringF()),
                new IO.XML("EndUser", EndUser.ToStringF()));
        }

        private void Tmr_Tick(object sender, EventArgs e)
        {
            current.Y += -Speed / 1000 * tmr.Interval;

            serie.Points[0].XValue = current.X - Radius;
            serie.Points[1].XValue = current.X + Radius;
            serie.Points[2].XValue = current.X + Radius;
            serie.Points[3].XValue = current.X - Radius;
            serie.Points[4].XValue = current.X - Radius;

            serie.Points[0].YValues[0] = current.Y - Radius;
            serie.Points[1].YValues[0] = current.Y - Radius;
            serie.Points[2].YValues[0] = current.Y + Radius;
            serie.Points[3].YValues[0] = current.Y + Radius;
            serie.Points[4].YValues[0] = current.Y - Radius;

            if (!float.IsNaN(EndTime) && sw.ElapsedMilliseconds > EndTime * 1000)
                Stop();
            if (!float.IsNaN(EndY) && current.Y <= EndY)
                Stop();
        }
        public void Check(PointF pointF)
        {
            if (!float.IsNaN(EndUser) && Extension.Sqr(current.X - pointF.X) + Extension.Sqr(current.Y - pointF.Y) < EndUser.Sqr())
                Stop();
        }
        public override string ToString()
        {
            return ID.ToString();
        }

        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        public void Play()
        {
            /*if (FormVisual.CHART.Series.Contains(serie))
                FormVisual.CHART.Series.Remove(serie);
            FormVisual.CHART.Series.Add(serie);
            serie.Enabled = true;
            current = Start;
            sw.Restart();

            Tmr_Tick(null, null);
            tmr.Start();*/
        }
        public void Stop()
        {
            /*if(FormVisual.CHART.Series.Contains(serie))
                FormVisual.CHART.Series.Remove(serie);*/
            serie.Enabled = false;
            tmr.Stop();
            TrialStopped?.Invoke(this, null);
        }
    }
}
