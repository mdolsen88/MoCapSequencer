using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoCapSequencer.MDOL
{
    public class FormResizer
    {
        public static int HORIZONTAL = 12;
        public static int VERTICAL = 18;
        public static int HORIZONTAL_FORM = 28;
        public static int VERTICAL_FORM = 51;
        public static void FillParent(System.Windows.Forms.Control parent, params System.Windows.Forms.Control[] controls)
        {
            int Top = controls.Min(control => control.Top);
            int Left = controls.Min(control => control.Left);
            int Right = controls.Max(control => control.Right);
            int Bottom = controls.Max(control => control.Bottom);

            int Width = Right - Left;
            int Height = Bottom - Top;

            double[] widths = controls.Select(control => (double)control.Width / Width).ToArray();
            double[] heights = controls.Select(control => (double)control.Height / Height).ToArray();
            double[] lefts = controls.Select(control => (double)((control.Left - Left)) / Width).ToArray();
            double[] tops = controls.Select(control => (double)((control.Top - Top)) / Height).ToArray();

            parent.Resize += (s, e) =>
            {
                int newWidth = parent.Width - Left;
                int newHeight = parent.Height - Top;
                if (parent.GetType().IsSubclassOf(typeof(System.Windows.Forms.Form)))
                {
                    newWidth -= HORIZONTAL_FORM;
                    newHeight -= VERTICAL_FORM;
                }
                else
                {
                    newWidth -= HORIZONTAL;
                    newHeight -= VERTICAL;
                }
                for (int i = 0; i < controls.Length; i++)
                {
                    controls[i].Location = new System.Drawing.Point((int)(Left + lefts[i] * newWidth), (int)(Top + tops[i] * newHeight));
                    controls[i].Size = new System.Drawing.Size((int)(widths[i] * newWidth), (int)(heights[i] * newHeight));
                }
            };
        }
        public static void FillBottom(System.Windows.Forms.Control parent, params System.Windows.Forms.Control[] controls)
        {
            parent.Resize += (s, e) =>
            {
                for (int i = 0; i < controls.Length; i++)
                {
                    controls[i].Size = new System.Drawing.Size(controls[i].Width, parent.Height - controls[i].Top - (parent.GetType().IsSubclassOf(typeof(System.Windows.Forms.Form)) ? VERTICAL_FORM : VERTICAL));
                }
            };
        }
    }
}
