using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoCapSequencer.MDOL
{
    public class Coordinate
    {
        public int i;
        public int R;
        public int C;
        public Coordinate(int i, int Cols)
        {
            this.i = i;
            C = i % Cols;
            R = (i - C) / Cols;
        }
    }

    public class Matrix<Tn, Tt> where Tn : Dimensions
    {
        public Tt[,] Data = null;
        public int Rows = 0;
        public int Cols = 0;

        public Matrix(int Rows = 0, int Cols = 0)
        {
            if (typeof(Tn) == typeof(Dimensions.II))
                Rows = Cols = 2;
            else if (typeof(Tn) == typeof(Dimensions.III))
                Rows = Cols = 3;
            Data = new Tt[Rows, Cols];
            this.Rows = Rows;
            this.Cols = Cols;
        }
        public Matrix(int Rows, int Cols, params Tt[] values)
        {
            Data = new Tt[Rows, Cols];
            this.Rows = Rows;
            this.Cols = Cols;
            for (int r = 0, i = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++, i++)
                    Data[r, c] = values[i];
        }
        public Matrix(params Tt[] values)
        {
            Rows = Cols = (int)Math.Sqrt(values.Length);
            Data = new Tt[Rows, Cols];
            for (int r = 0, i = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++, i++)
                    Data[r, c] = values[i];
        }
        public Matrix(Tt[,] A)
        {
            Data = A;
            Rows = A.GetLength(0);
            Cols = A.GetLength(1);
        }
        public Tt this[int r, int c]
        {
            get { return Data[r, c]; }
            set { Data[r, c] = value; }
        }
        public Coordinate[] Find(Func<Tt, bool> Condition)
        {
            List<Coordinate> I = new List<Coordinate>();
            for (int r = 0, i = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++, i++)
                    if (Condition(Data[r, c]))
                        I.Add(new Coordinate(i, Cols));
            return I.ToArray();
        }
        public Matrix<Tn, Tt> Copy()
        {
            return new Matrix<Tn, Tt>(this.Data);
        }
    }

    public class Matrix<Tn> : Matrix<Tn, double> where Tn : Dimensions
    {
        public Matrix(int Rows = 0, int Cols = 0)
        {
            if (typeof(Tn) == typeof(Dimensions.II))
                Rows = Cols = 2;
            else if (typeof(Tn) == typeof(Dimensions.III))
                Rows = Cols = 3;
            Data = new double[Rows, Cols];
            this.Rows = Rows;
            this.Cols = Cols;
        }
        public Matrix(int Rows, int Cols, params double[] values)
        {
            Data = new double[Rows, Cols];
            this.Rows = Rows;
            this.Cols = Cols;
            for (int r = 0, i = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++, i++)
                    if (i < values.Length)
                        Data[r, c] = values[i];
        }
        public Matrix(params double[] values)
        {
            Rows = Cols = (int)Math.Sqrt(values.Length);
            Data = new double[Rows, Cols];
            for (int r = 0, i = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++, i++)
                    Data[r, c] = values[i];
        }
        public Matrix(double[,] A)
        {
            Data = A;
            Rows = A.GetLength(0);
            Cols = A.GetLength(1);
        }
        public Matrix<Tn> ConcatRight(Matrix<Tn> A)
        {
            Matrix<Tn> C = new Matrix<Tn>(Rows, Cols + A.Cols);
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                    C.Data[r, c] = Data[r, c];
                for (int c = 0; c < A.Cols; c++)
                    C.Data[r, Cols + c] = A.Data[r, c];
            }
            return C;
        }
        public Matrix<Tn> ConcatBelow(Matrix<Tn> A)
        {
            Matrix<Tn> C = new Matrix<Tn>(Rows + A.Rows, Cols);
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    C.Data[r, c] = Data[r, c];
            for (int r = 0; r < A.Rows; r++)
                for (int c = 0; c < Cols; c++)
                    C.Data[Rows + r, c] = A.Data[r, c];
            return C;
        }
        public Vector<Tn> XAxis
        {
            get { return Col(0); }
            set
            {
                for (int r = 0; r < Rows; r++)
                    Data[r, 0] = value[r];
            }
        }
        public Vector<Tn> YAxis
        {
            get { return Col(1); }
            set
            {
                for (int r = 0; r < Rows; r++)
                    Data[r, 1] = value[r];
            }
        }
        public Vector<Tn> ZAxis
        {
            get { return Col(2); }
            set
            {
                for (int r = 0; r < Rows; r++)
                    Data[r, 2] = value[r];
            }
        }
        public void NormalizeColumns()
        {
            XAxis = XAxis.Normalize();
            YAxis = YAxis.Normalize();
            ZAxis = ZAxis.Normalize();
        }
        public Matrix<Tn> Transpose
        {
            get
            {
                Matrix<Tn> transpose = new Matrix<Tn>(Cols, Rows);
                for (int i = 0; i < Rows; i++)
                    for (int j = 0; j < Cols; j++)
                        transpose[j, i] = this[i, j];
                return transpose;
            }
        }
        public Vector<Tn> Col(int c)
        {
            double[] col = new double[Rows];
            for (int r = 0; r < Rows; r++)
                col[r] = Data[r, c];
            return new Vector<Tn>(col);
        }
        public Vector<Tn> Row(int r)
        {
            double[] row = new double[Cols];
            for (int c = 0; c < Cols; c++)
                row[c] = Data[r, c];
            return new Vector<Tn>(row);
        }

        public static implicit operator double[,](Matrix<Tn> x)
        {
            return x.Data;
        }
        double trace = double.NaN;
        public double Trace
        {
            get
            {
                if (double.IsNaN(trace))
                {
                    trace = 0;
                    int iMax = Math.Min(Rows, Cols);
                    for (int i = 0; i < iMax; i++)
                        trace += this[i, i];
                }
                return trace;
            }
        }
        public static Matrix<Tn> Diag(params double[] diag)
        {
            Matrix<Tn> A = new Matrix<Tn>(diag.Length, diag.Length);
            for (int i = 0; i < diag.Length; i++)
                A[i, i] = diag[i];
            return A;
        }
        public static Matrix<Tn> operator *(Matrix<Tn> A, double value)
        {
            Matrix<Tn> B = new Matrix<Tn>(A.Rows, A.Cols);
            for (int r = 0; r < A.Rows; r++)
                for (int c = 0; c < A.Cols; c++)
                    B[r, c] = A[r, c] * value;
            return B;
        }
        public static Matrix<Tn> operator *(double value, Matrix<Tn> A) { return A * value; }
        public static Matrix<Tn> operator /(Matrix<Tn> A, double value)
        {
            Matrix<Tn> B = new Matrix<Tn>(A.Rows, A.Cols);
            for (int r = 0; r < A.Rows; r++)
                for (int c = 0; c < A.Cols; c++)
                    B[r, c] = A[r, c] / value;
            return B;
        }
        public static Matrix<Tn> operator /(double value, Matrix<Tn> A) { return 1 / value * A; }
        //Negation
        public static Matrix<Tn> operator -(Matrix<Tn> A)
        {
            return A * -1;
        }
        public static Matrix<Tn> operator -(Matrix<Tn> A, Matrix<Tn> B)
        {
            Matrix<Tn> C = new Matrix<Tn>(A.Rows, A.Cols);
            for (int r = 0; r < A.Rows; r++)
                for (int c = 0; c < A.Cols; c++)
                    C[r, c] = A[r, c] - B[r, c];
            return C;
        }
        public static Matrix<Tn> operator +(Matrix<Tn> A, Matrix<Tn> B)
        {
            Matrix<Tn> C = new Matrix<Tn>(A.Rows, A.Cols);
            for (int r = 0; r < A.Rows; r++)
                for (int c = 0; c < A.Cols; c++)
                    C[r, c] = A[r, c] + B[r, c];
            return C;
        }

        public static Matrix<Tn> Identity(int n = 0)
        {
            if (typeof(Tn) == typeof(Dimensions.II))
                n = 2;
            else if (typeof(Tn) == typeof(Dimensions.III))
                n = 3;
            return Diag(new double[n].Select(i => 1.0).ToArray());
        }

        public static Matrix<Tn> FromXML(IO.XML xml)
        {
            xml = xml.getElement("Matrix");
            int Rows = xml.getInt("Rows", 0);
            int Cols = xml.getInt("Cols", 0);
            double[] Data = xml.getString("Data", null).Split(';').Select(element => element.ToDouble()).ToArray();
            return new Matrix<Tn>(Rows, Cols, Data);
        }
        public static Matrix<Tn> FromString(string s)
        {
            string[] elements = s.Split(':');
            int[] RowsCols = elements[0].Split(';').Select(element => int.Parse(element)).ToArray();
            double[] Data = elements[1].Split(';').Select(element => element.ToDouble()).ToArray();
            return new Matrix<Tn>(RowsCols[0], RowsCols[1], Data);
        }
    }

    public static class Matrix3
    {
        public static double Det(this Matrix<Dimensions.III> A)
        {
            double[,] Data = A.Data;
            return Data[0, 0] * (Data[1, 1] * Data[2, 2] - Data[1, 2] * Data[2, 1]) - Data[0, 1] * (Data[1, 0] * Data[2, 2] - Data[1, 2] * Data[2, 0]) + Data[0, 2] * (Data[1, 0] * Data[2, 1] - Data[1, 1] * Data[2, 0]);
        }
        public static Vector<Dimensions.III> Multiply(this Matrix<Dimensions.III> A, Vector<Dimensions.III> x)
        {
            return new Vector<Dimensions.III>(
                A[0, 0] * x[0] + A[0, 1] * x[1] + A[0, 2] * x[2],
                A[1, 0] * x[0] + A[1, 1] * x[1] + A[1, 2] * x[2],
                A[2, 0] * x[0] + A[2, 1] * x[1] + A[2, 2] * x[2]);
        }
    }
    public static class Matrix2
    {
        public static double Det(this Matrix<Dimensions.II> A)
        {
            double[,] Data = A.Data;
            return Data[0, 0] * Data[1, 1] - Data[0, 1] * Data[1, 0];
        }
    }
}
