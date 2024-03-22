using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoCapSequencer.MDOL
{
    static class Extension
    {
        public static System.Drawing.Bitmap ToBitmap(this byte[] rgb, int Width, int Height, System.Drawing.Imaging.PixelFormat PixelFormat = System.Drawing.Imaging.PixelFormat.Undefined)
        {
            int pixelSize = rgb.Length / (Height * Width);
            if (PixelFormat == System.Drawing.Imaging.PixelFormat.Undefined)
                switch (pixelSize)
                {
                    case 1:
                        PixelFormat = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
                        byte[] rgbNew = new byte[rgb.Length * 3];
                        for (int i = 0; i < rgb.Length; i++)
                            rgbNew[i * 3] = rgbNew[i * 3 + 1] = rgbNew[i * 3 + 2] = rgb[i];
                        rgb = rgbNew;
                        break;
                    case 3:
                        PixelFormat = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
                        break;
                    case 4:
                        PixelFormat = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
                        break;
                }
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(Width, Height, PixelFormat);
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, bmp.PixelFormat);
            if (bmpData.Stride != bmpData.Width * pixelSize)
            {
                byte[] rgbNew = new byte[bmpData.Stride * bmpData.Height];
                for (int r = 0; r < bmpData.Height; r++)
                    Buffer.BlockCopy(rgb, r * bmpData.Width * pixelSize, rgbNew, r * bmpData.Stride, bmpData.Width * pixelSize);
                System.Runtime.InteropServices.Marshal.Copy(rgbNew, 0, bmpData.Scan0, rgb.Length);
            }
            else
                System.Runtime.InteropServices.Marshal.Copy(rgb, 0, bmpData.Scan0, rgb.Length);

            bmp.UnlockBits(bmpData);
            return bmp;
        }
        public static double Sqr(this double d)
        {
            return d * d;
        }
        public static float Sqr(this float d)
        {
            return d * d;
        }
        public static double Mean(this IEnumerable<double> A)
        {
            if (A.Count() == 0)
                return double.NaN;
            return A.Sum() / A.Count();
        }
        public static double Std(this IEnumerable<double> A)
        {
            return Math.Sqrt(A.Var());
        }
        public static double Var(this IEnumerable<double> A)
        {
            if (A.Count() == 0)
                return double.NaN;
            double mean = A.Mean();
            return A.Sum(a => (a - mean).Sqr()) / A.Count();
        }
        public static T Max<T>(params T[] values) where T : IComparable
        {
            T max = values.FirstOrDefault();
            foreach (T t in values)
                if (t.CompareTo(max) > 0)
                    max = t;
            return max;
        }
        public static string ToStringF(this float value, int decimalplaces = int.MaxValue)
        {
            if (decimalplaces != int.MaxValue)
            {
                string format = "0.";
                for (int i = 0; i < decimalplaces; i++)
                    format += "0";
                return value.ToString(format, culture);
            }
            else
                return value.ToString(culture);
        }
        public static double Std(this IEnumerable<double> A, out double Mean)
        {
            return Math.Sqrt(A.Var(out Mean));
        }
        public static double Var(this IEnumerable<double> A, out double Mean)
        {
            Mean = double.NaN;
            if (A.Count() == 0)
                return double.NaN;
            double mean = A.Mean();
            double var = A.Sum(a => (a - mean).Sqr()) / A.Count();
            Mean = mean;
            return var;
        }
        public static T Median<T>(this IEnumerable<T> A) where T : IComparable
        {
            if (A.Count() == 0)
                return default(T);
            int half_i = (int)Math.Floor((A.Count() - 1) * 0.5);
            return A.OrderBy(t => t).ElementAt(half_i);
        }
        public static IntPtr Pointer<T>(this T[] a)
        {
            return System.Runtime.InteropServices.Marshal.UnsafeAddrOfPinnedArrayElement(a, 0);
        }
        static System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.InvariantCulture;
        public static string ToStringD(this double value, int decimalplaces = int.MaxValue)
        {
            if (decimalplaces != int.MaxValue)
            {
                string format = "0.";
                for (int i = 0; i < decimalplaces; i++)
                    format += "0";
                return value.ToString(format, culture);
            }
            else
                return value.ToString(culture);
        }
        public static float ToFloat(this string text)
        {
            return float.Parse(text, culture);
        }
        public static double ToDouble(this string text)
        {
            return double.Parse(text, culture);
        }
    }
}
