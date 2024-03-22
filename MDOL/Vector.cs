using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MoCapSequencer.MDOL
{
    public class Dimensions
    {
        public class II : Dimensions
        {
        }
        public class III : Dimensions
        {
        }
        public class N : Dimensions
        {
        }
    }

    public class Vector<Tn> where Tn : Dimensions
    {
        public double[] Data;
        public readonly int n;
        public Vector(params double[] data)
        {
            Data = data;
            n = data.Length;
        }

        public double X
        {
            get { return Data[0]; }
            set { Data[0] = value; }
        }
        public double Y
        {
            get { return Data[1]; }
            set { Data[1] = value; }
        }
        public double Z
        {
            get { return Data[2]; }
            set { Data[2] = value; }
        }

        public Vector(int n = 0)
        {
            if (typeof(Tn) == typeof(Dimensions.II))
                n = 2;
            else if (typeof(Tn) == typeof(Dimensions.III))
                n = 3;
            this.n = n;
            Data = new double[n];
        }
        public Vector(string str)
        {
            Data = str.Split(';').Select(s => s.ToDouble()).ToArray();
            n = Data.Length;
        }
        public override string ToString()
        {
            return string.Join(";", Data.Select(d => d.ToStringD()));
        }
        public string ToString(int decimals)
        {
            return string.Join(";", Data.Select(d => d.ToStringD(decimals)));
        }
        public IO.XML ToXML()
        {
            return new IO.XML("Vector",
                new IO.XML("Data", string.Join(";", Data.Select(data => data.ToStringD()))));
        }
        public static Vector<Tn> FromXML(IO.XML xml)
        {
            xml = xml.getElement("Vector");
            double[] Data = xml.getString("Data", null).Split(';').Select(element => element.ToDouble()).ToArray();
            return new Vector<Tn>(Data);
        }
        public double this[int i]
        {
            get { return Data[i]; }
            set { Data[i] = value; }
        }
        public static Vector<Tn> Zero(int n = 0)
        {
            if (typeof(Tn) == typeof(Dimensions.II))
                n = 2;
            else if (typeof(Tn) == typeof(Dimensions.III))
                n = 3;
            return new Vector<Tn>(n);
        }

        public double Dot(Vector<Tn> v)
        {
            double dot = 0;
            for (int i = 0; i < n; i++)
                dot += this[i] * v[i];
            return dot;
        }
        public double Angle(Vector<Tn> v1, Vector<Tn> v2)
        {
            double dot = v1.Normalize().Dot(v2.Normalize());
            dot = dot < -1 ? -1 : dot;
            dot = dot > 1 ? 1 : dot;
            return Math.Acos(dot);
        }
        public double LengthSqr
        {
            get
            {
                return Data.Sum(d => d * d);
            }
        }
        public double Length
        {
            get
            {
                return Math.Sqrt(LengthSqr);
            }
        }
        public Vector<Tn> Normalize()
        {
            double l = Length;
            if (l == 0)
                return new Vector<Tn>(n);
            else
                return this / l;
        }

        #region Operators
        // Add
        public static Vector<Tn> operator +(Vector<Tn> v1, Vector<Tn> v2)
        {
            double[] data = new double[v1.n];
            for (int i = 0; i < data.Length; i++)
                data[i] = v1[i] + v2[i];
            return new Vector<Tn>(data);
        }
        public static Vector<Tn> operator +(Vector<Tn> v, double value)
        {
            return new Vector<Tn>(v.Data.Select(d => d + value).ToArray());
        }
        public static Vector<Tn> operator +(double value, Vector<Tn> v) { return v + value; }

        // Subtract
        public static Vector<Tn> operator -(Vector<Tn> v1, Vector<Tn> v2)
        {
            double[] data = new double[v1.n];
            for (int i = 0; i < data.Length; i++)
                data[i] = v1[i] - v2[i];
            return new Vector<Tn>(data);
        }
        public static Vector<Tn> operator -(Vector<Tn> v, double value)
        {
            return new Vector<Tn>(v.Data.Select(d => d - value).ToArray());
        }
        public static Vector<Tn> operator -(double value, Vector<Tn> v) { return v - value; }

        //Negation
        public static Vector<Tn> operator -(Vector<Tn> v)
        {
            return -1.0 * v;
        }

        // Multiply
        public static Vector<Tn> operator *(Vector<Tn> v, double value)
        {
            return new Vector<Tn>(v.Data.Select(d => d * value).ToArray());
        }
        public static Vector<Tn> operator *(double value, Vector<Tn> v) { return v * value; }

        // Divide
        public static Vector<Tn> operator /(Vector<Tn> v, double value)
        {
            return new Vector<Tn>(v.Data.Select(d => d / value).ToArray());
        }
        public static Vector<Tn> operator /(double value, Vector<Tn> v)
        {
            return new Vector<Tn>(v.Data.Select(d => value / d).ToArray());
        }
        #endregion
        public Vector<Tn> Concat(Vector<Tn> v)
        {
            Vector<Tn> C = new Vector<Tn>(n + v.n);
            for (int i = 0; i < n; i++)
                C.Data[i] = Data[i];
            for (int i = 0; i < v.n; i++)
                C.Data[n + i] = v.Data[i];
            return C;
        }
        public Vector<Tn> Copy()
        {
            return new Vector<Tn>(Data);
        }
    }
    public static class Vector3
    {
        public static Vector<Dimensions.III> XAxis = new Vector<Dimensions.III>(1, 0, 0);
        public static Vector<Dimensions.III> YAxis = new Vector<Dimensions.III>(0, 1, 0);
        public static Vector<Dimensions.III> ZAxis = new Vector<Dimensions.III>(0, 0, 1);
        public static Vector<Dimensions.III> Cross(this Vector<Dimensions.III> v1, Vector<Dimensions.III> v2)
        {
            return new Vector<Dimensions.III>(
                v1.Y * v2.Z - v1.Z * v2.Y,
                v1.Z * v2.X - v1.X * v2.Z,
                v1.X * v2.Y - v1.Y * v2.X);
        }
        public static Matrix<Dimensions.III> CrossMatrix(this Vector<Dimensions.III> v)
        {
            return new Matrix<Dimensions.III>(0, -v.Z, v.Y, v.Z, 0, -v.X, -v.Y, v.X, 0);
        }
        public static Vector<Dimensions.III>[] Normals(this Vector<Dimensions.III> v)
        {
            Vector<Dimensions.III> b = new Vector<Dimensions.III>();
            Vector<Dimensions.III> c = new Vector<Dimensions.III>();
            Vector<Dimensions.III> aN = v.Normalize();
            if (aN[2] != 0)
                b = new Vector<Dimensions.III>(1, 1, (aN[0] * 1 + aN[1]) / -aN[2]).Normalize();
            else if (aN[1] != 0)
                b = new Vector<Dimensions.III>(1, (aN[0] + aN[2]) / -aN[1], 1).Normalize();
            else if (aN[0] != 0)
                b = new Vector<Dimensions.III>((aN[1] + aN[2]) / -aN[0], 1, 1).Normalize();
            c = aN.Cross(b);
            return new Vector<Dimensions.III>[2] { b, c };
        }

        public static Vector<Dimensions.II> InHomogenize(this Vector<Dimensions.III> v)
        {
            return new Vector<Dimensions.II>(v.X / v.Z, v.Y / v.Z);
        }
        public static Vector<Dimensions.II> XY(this Vector<Dimensions.III> v)
        {
            return new Vector<Dimensions.II>(v.X, v.Y);
        }
    }

    public static class Vector2
    {
        public static Vector<Dimensions.III> Homogenize(this Vector<Dimensions.II> v)
        {
            return new Vector<Dimensions.III>(v.X, v.Y, 1);
        }
        public static double Cross(this Vector<Dimensions.II> v1, Vector<Dimensions.II> v2)
        {
            return v1.X * v2.Y - v1.Y * v2.X;
        }
        public static Vector<Dimensions.II> LineLineIntersect(Vector<Dimensions.II> P0, Vector<Dimensions.II> P, Vector<Dimensions.II> Q0, Vector<Dimensions.II> Q)
        {
            double p, q;
            return LineLineIntersect(P0, P, Q0, Q, out p, out q);
        }
        public static Vector<Dimensions.II> LineLineIntersect(Vector<Dimensions.II> P0, Vector<Dimensions.II> P, Vector<Dimensions.II> Q0, Vector<Dimensions.II> Q, out double p)
        {
            double q;
            return LineLineIntersect(P0, P, Q0, Q, out p, out q);
        }
        public static Vector<Dimensions.II> LineLineIntersect(Vector<Dimensions.II> P0, Vector<Dimensions.II> P, Vector<Dimensions.II> Q0, Vector<Dimensions.II> Q, out double p, out double q)
        {
            Vector<Dimensions.II> w = P0 - Q0;
            Vector<Dimensions.II> P_norm = P.Normalize();
            Vector<Dimensions.II> Q_norm = Q.Normalize();

            p = w.Cross(Q_norm) / (Q_norm.Cross(P));
            q = w.Cross(P_norm) / (P_norm.Cross(Q));
            return P0 + P * p;
        }
        public static double Angle(this Vector<Dimensions.II> v)
        {
            return Math.Atan2(v.Y, v.X);
        }
        public static PointF ToPointF(this Vector<Dimensions.II> v)
        {
            return new PointF((float)v.X, (float)v.Y);
        }
    }

    public static class VectorT
    {
        public static Vector<Tn>[] Diff<Tn>(this IEnumerable<Vector<Tn>> Vectors) where Tn : Dimensions
        {
            int n = Vectors.Count();
            if (n == 0)
                return null;
            Vector<Tn>[] diff = new Vector<Tn>[n - 1];
            for (int i = 0; i < diff.Length; i++)
                diff[i] = Vectors.ElementAt(i + 1) - Vectors.ElementAt(i);
            return diff;
        }
        public static Vector<Tn> Sum<Tn>(this IEnumerable<Vector<Tn>> Vectors) where Tn : Dimensions
        {
            Vector<Tn> sum = new Vector<Tn>();
            foreach (Vector<Tn> vector in Vectors)
            {
                for (int i = 0; i < vector.n; i++)
                    sum[i] += vector[i];
            }
            return sum;
        }
        public static Vector<Tn> Max<Tn>(this IEnumerable<Vector<Tn>> Vectors) where Tn : Dimensions
        {
            Vector<Tn> max = new Vector<Tn>(new double[Vectors.ElementAt(0).n].Select(d => double.NegativeInfinity).ToArray());
            foreach (Vector<Tn> vector in Vectors)
                for (int i = 0; i < vector.n; i++)
                    max[i] = vector[i] > max[i] ? vector[i] : max[i];
            return max;
        }
        public static Vector<Tn> WhereIMax<Tn>(this IEnumerable<Vector<Tn>> Vectors, int I) where Tn : Dimensions
        {
            double max = double.NegativeInfinity;
            int iMax = -1;
            for (int i = 0; i < Vectors.Count(); i++)
                if (Vectors.ElementAt(i)[I] > max)
                {
                    max = Vectors.ElementAt(i)[I];
                    iMax = i;
                }
            return Vectors.ElementAt(iMax);
        }
        public static Vector<Tn> Min<Tn>(this IEnumerable<Vector<Tn>> Vectors) where Tn : Dimensions
        {
            Vector<Tn> min = new Vector<Tn>(new double[Vectors.ElementAt(0).n].Select(d => double.PositiveInfinity).ToArray());
            foreach (Vector<Tn> vector in Vectors)
                for (int i = 0; i < vector.n; i++)
                    min[i] = vector[i] < min[i] ? vector[i] : min[i];
            return min;
        }
        public static Vector<Tn> WhereIMin<Tn>(this IEnumerable<Vector<Tn>> Vectors, int I) where Tn : Dimensions
        {
            double min = double.PositiveInfinity;
            int iMin = -1;
            for (int i = 0; i < Vectors.Count(); i++)
                if (Vectors.ElementAt(i)[I] < min)
                {
                    min = Vectors.ElementAt(i)[I];
                    iMin = i;
                }
            return Vectors.ElementAt(iMin);
        }
        public static Vector<Tn> Mean<Tn>(this IEnumerable<Vector<Tn>> Vectors) where Tn : Dimensions
        {
            return Vectors.Sum() / Vectors.Count();
        }
        public static Vector<Tn> Median<Tn>(this IEnumerable<Vector<Tn>> Vectors) where Tn : Dimensions
        {
            int n = Vectors.ElementAt(0).n;
            double[][] datas = new double[n][];
            Vector<Tn> median = new Vector<Tn>(n);
            for (int i = 0; i < n; i++)
                datas[i] = Vectors.Select(v => v[i]).ToArray();
            return new Vector<Tn>(datas.Select(d => d.Median()).ToArray());
        }
    }
}
