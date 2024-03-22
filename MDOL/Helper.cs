using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoCapSequencer.MDOL
{
    public class Helper
    {
        public static void DrawCenteredString(System.Drawing.Image img, string String)
        {
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(img))
            {
                for (int i = 1; i < 1000; i++)
                {
                    System.Drawing.Font font = new System.Drawing.Font(System.Drawing.FontFamily.Families.FirstOrDefault(family => family.Name.Contains("Arial")), i);
                    System.Drawing.SizeF size = g.MeasureString(String, font);
                    if (size.Width > img.Width || size.Height > img.Height)
                    {
                        font = new System.Drawing.Font(System.Drawing.FontFamily.Families.FirstOrDefault(family => family.Name.Contains("Arial")), i - 1);
                        size = g.MeasureString(String, font);
                        g.DrawString(String, font, System.Drawing.Brushes.Black, (img.Width - size.Width) / 2, (img.Height - size.Height) / 2);
                        break;
                    }
                }
            }
        }
        public static void SetImage(System.Windows.Forms.PictureBox pic, System.Drawing.Bitmap bmp)
        {
            if (pic.Image != null)
                pic.Image.Dispose();
            pic.Image = bmp;
        }
        public static void SetImage(System.Windows.Forms.Form form, System.Drawing.Bitmap bmp)
        {
            if (form.BackgroundImage != null)
                form.BackgroundImage.Dispose();
            form.BackgroundImage = bmp;
        }
        public static System.Drawing.Color Hue2Color(double h)
        {
            double var_h = h * 6;
            if (var_h == 6)
                var_h = 0;
            int var_i = (int)Math.Floor(var_h);
            int var_1 = 0;
            int var_2 = (int)(255 * (1 - (var_h - var_i)));
            int var_3 = (int)(255 * (1 - (1 - (var_h - var_i))));
            int r = 255, g = 255, b = 255;
            if (var_i == 0)
            {
                g = var_3;
                b = var_1;
            }
            else if (var_i == 1)
            {
                r = var_2;
                b = var_1;
            }
            else if (var_i == 2)
            {
                r = var_1;
                b = var_3;
            }
            else if (var_i == 3)
            {
                r = var_1;
                g = var_2;
            }
            else if (var_i == 4)
            {
                r = var_3;
                g = var_1;
            }
            else
            {
                g = var_1;
                b = var_2;
            }

            return System.Drawing.Color.FromArgb(r, g, b);
        }

        public static System.Drawing.Color weigth2Color(double weight)
        {
            int r = 255, g = 255, b = 0;
            weight = Math.Max(0, Math.Min(1, weight));
            if (weight <= 0.5)
            {
                r = (int)((weight * 2) * 255);
            }
            if (weight > 0.5)
            {
                g = (int)((2 - 2 * weight) * 255);
            }
            return System.Drawing.Color.FromArgb(r, g, b);
        }

        public static void RunProcess(string Filename, string Arguments, Action<string, bool> OnOutput = null)
        {
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = Filename,
                Arguments = Arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            System.Diagnostics.Process proc = new System.Diagnostics.Process()
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };
            if (OnOutput != null)
            {
                proc.OutputDataReceived += (s, e) =>
                {
                    OnOutput(e.Data, false);
                };
                proc.ErrorDataReceived += (s, e) =>
                {
                    OnOutput(e.Data, true);
                };
            }

            proc.Start();
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();
            proc.WaitForExit();
        }
    }
}
