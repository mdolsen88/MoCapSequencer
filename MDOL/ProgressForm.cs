using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
namespace MoCapSequencer.MDOL
{
    public class ProgressForm : Form
    {
        public static int itemHeight = 13;
        public static int space = 10;
        //public static int lblWidth = 100;
        public static int pgbWidth = 200;

        ProgressBar[] PGB;
        public ProgressForm(string Title, params string[] Actions) : base()
        {
            int lblWidth = Actions.Max(action => action.Length) * 6;
            this.Text = Title;
            this.Height = 39 + Actions.Length * (space + itemHeight) + space;
            this.Width = 16 + (space + pgbWidth + space + lblWidth + space);
            PGB = new ProgressBar[Actions.Length];
            for (int i = 0; i < Actions.Length; i++)
            {
                PGB[i] = new ProgressBar();
                PGB[i].Minimum = 0;
                PGB[i].Maximum = 100;
                if (Actions[i] != "")
                {
                    Label lbl = new Label();
                    lbl.Bounds = new System.Drawing.Rectangle(space, space + i * (itemHeight + space), lblWidth, itemHeight);
                    this.Controls.Add(lbl);
                    lbl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                    lbl.Text = Actions[i];

                    PGB[i].Bounds = new System.Drawing.Rectangle(space + lblWidth + space, space + i * (itemHeight + space), pgbWidth, itemHeight);
                }
                else
                    PGB[i].Bounds = new System.Drawing.Rectangle(space, space + i * (itemHeight + space), pgbWidth, itemHeight);
                this.Controls.Add(PGB[i]);
            }
        }
        public void SetValue(int id, int percentage)
        {
            PGB[id].Value = percentage;
        }
    }
}
