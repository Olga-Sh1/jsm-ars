using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseElements
{
    /// <summary>Двумерная матрица</summary>
    /// <typeparam name="T1">Тип первого измерения</typeparam>
    /// <typeparam name="T2">Тип второго измерения</typeparam>
    public class Matrix2<T1, T2>
    {
        /// <summary>Размерность по первому измерению</summary>
        public int Length1 { get { return Matrix.GetLength(0); } }
        /// <summary>Размерность по второму измерению</summary>
        public int Length2 { get { return Matrix.GetLength(1); } }
        public bool this[int d1, int d2]
        {
            get { return Matrix[d1, d2]; }
            set { Matrix[d1, d2] = value; }
        }
        /// <summary>Данные по первому измерению</summary>
        public IEnumerable<T1> Dimension1 { get; private set; }
        /// <summary>Данные по второму измерению</summary>
        public IEnumerable<T2> Dimension2 { get; private set; }
        bool[,] Matrix;

        public Matrix2(IEnumerable<T1> dim1, IEnumerable<T2> dim2)
        {
            Dimension1 = dim1;
            Dimension2 = dim2;
            Matrix = new bool[dim1.Count(), dim2.Count()];
        }
    }
}
