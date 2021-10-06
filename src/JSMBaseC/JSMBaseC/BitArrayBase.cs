using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace JSMBase
{
    /// <summary></summary>
    public class BitArrayBase
    {
        const double _base = 2.0;
        public Int32 Count { get; private set; }
        private UInt32[] Lengthes;
        private UInt64[] m_Arrays;
        public UInt64 this[int pos]
        {
            get
            {
                return m_Arrays[pos];
            }
        }

        #region Конструкторы
        public BitArrayBase(Boolean[][] _Arrays)
        {
            if (_Arrays == null) throw new ArgumentNullException();
            Count = _Arrays.Length;
            m_Arrays = new UInt64[Count];
            Lengthes = new UInt32[Count];
            for (int i = 0; i < Count; i++)
            {
                UInt32 arrVal = 0;
                for (int j = 0; j < _Arrays[i].Length; j++)
                    if (_Arrays[i][j])
                        arrVal += (uint)Math.Pow(_base, j);
                m_Arrays[i] = arrVal;
                Lengthes[i] = (uint)_Arrays[i].Length;
            }
        }
        /// <summary>Создать массив битовых строк из заданного с индексами</summary>
        /// <param name="other">Заданный массив</param>
        /// <param name="spsIndices">Индексы</param>
        /// <param name="b">TRUE - из заданных индексов, FALSE - за исключением заданных индексов</param>
        public BitArrayBase(BitArrayBase other, uint[] spsIndices, bool b = false)
        {
            if (b)
            {
                Count = spsIndices.Length;
                Lengthes = new UInt32[Count];
                m_Arrays = new UInt64[Count];
                uint i = 0;
                for (uint j = 0; j < other.Lengthes.Length; j++)
                    if (spsIndices.Contains(j))
                    {
                        this.Lengthes[i] = other.Lengthes[j];
                        this.m_Arrays[i] = other.m_Arrays[j];
                        i++;
                    }
            }
            else
            {
                Count = other.Lengthes.Length - spsIndices.Length;
                Lengthes = new UInt32[Count];
                m_Arrays = new UInt64[Count];
                uint i = 0;
                for (uint j = 0; j < other.Lengthes.Length; j++)
                    if (!spsIndices.Contains(j))
                    {
                        this.Lengthes[i] = other.Lengthes[j];
                        this.m_Arrays[i] = other.m_Arrays[j];
                        i++;
                    }
            }
        }

        public BitArrayBase(params BitBaseIndicesPair[] data)
        {
            uint max = 0;
            for(int i = 0; i < data.Length; i++)
                for(int j = 0; j < data[i].Indices.Length; j++)
                {
                    if (max < data[i].Indices[j]) max = data[i].Indices[j];
                }

            Count = (int)(max + 1);
            Lengthes = new UInt32[Count];
            m_Arrays = new UInt64[Count];
            for(int i = 0; i < data.Length; i++)
                for(int j = 0; j < data[i].Indices.Length; j++)
                {
                    uint index = data[i].Indices[j];
                    Lengthes[index] = data[i].Base.Lengthes[j];
                    m_Arrays[index] = data[i].Base.m_Arrays[j];
                }
        }


        public BitArrayBase(UInt32[] _Lengthes)
        {
            Lengthes = _Lengthes;
            Count = _Lengthes.Length;
            m_Arrays = new UInt64[Count];
        }
        private BitArrayBase(int n)
        {
            Count = n;
            m_Arrays = new UInt64[Count];
            Lengthes = new UInt32[Count];
        }

        #endregion
        public BitArrayBase Intersect(BitArrayBase other)
        {
            if (this.Count!= other.Count)
                throw new ArgumentException("Количество массивов отличается");
            BitArrayBase item = new BitArrayBase(this.Lengthes);
            for (int i = 0; i < Count; i++ )
                item.m_Arrays[i] = this.m_Arrays[i] & other.m_Arrays[i];
            return item;
        }

        public BitArrayBase Intersect(BitArrayBase other, int[] grs)
        {
            if (this.Count != other.Count)
                throw new ArgumentException("Количество массивов отличается");
            BitArrayBase item = new BitArrayBase(this.Lengthes);
            for (int i = 0; i < Count; i++)
                if (grs.Contains(i))
                    item.m_Arrays[i] = this.m_Arrays[i] & other.m_Arrays[i];
                else
                    item.m_Arrays[i] = 0;
            return item;
        }

        public BitArrayBase Sum(BitArrayBase other)
        {
            if (this.Count != other.Count)
                throw new ArgumentException("Количество массивов отличается");
            BitArrayBase item = new BitArrayBase(this.Lengthes);
            for (int i = 0; i < Count; i++)
                item.m_Arrays[i] = this.m_Arrays[i] | other.m_Arrays[i];
            return item;
        }

        public BitArrayBase Difference(BitArrayBase other)
        {
            if (this.Count != other.Count)
                throw new ArgumentException("Количество массивов отличается");
            BitArrayBase item = new BitArrayBase(this.Lengthes);
            for (int i = 0; i < Count; i++)
                item.m_Arrays[i] = (this.m_Arrays[i] ^ other.m_Arrays[i]) & this.m_Arrays[i];
            return item;
        }

        public Boolean HasIntersection(BitArrayBase other)
        {
            if (this.Count != other.Count)
                throw new ArgumentException("Количество массивов отличается");
            for (int i = 0; i < this.Count; i++)
            {
                if ((this.m_Arrays[i] & other.m_Arrays[i]) != 0)
                    return true;
            }
            return false;
        }

        public Boolean HasIntersection(int ii, BitArrayBase other)
        {
            return (this.m_Arrays[ii] & other.m_Arrays[ii]) != 0;
        }

        public Boolean IsEnclosed(BitArrayBase other)
        {
            if (this.Count != other.Count)
                throw new ArgumentException("Количество массивов отличается");
            for (int i = 0; i < this.Count; i++)
            {
                if ((this.m_Arrays[i] & other.m_Arrays[i]) != this.m_Arrays[i])
                    return false;
            }
            return true;
        }
        public override bool Equals(object obj)
        {
            BitArrayBase other = obj as BitArrayBase;
            if (other == null)
                return false;
            return EqualsInner(other);
        }
        private Boolean EqualsInner(BitArrayBase other)
        {
            if (other == null) return false;
            if (this.Count != other.Count)
                throw new ArgumentException("Количество массивов отличается");
            for (int i = 0; i < Count; i++)
                if (this.m_Arrays[i] != other.m_Arrays[i]) return false;
            return true;
        }
        public Boolean IsEmpty()
        {
            for (int i = 0; i < Count; i++)
                if (this.m_Arrays[i] != 0) return false;
            return true;
        }
        public Int32 CountNonEmpty()
        {
            return m_Arrays.Count(v => v > 0);
        }
        public Boolean[][] GetArrays()
        {
            Boolean[][] result = new Boolean[Count][];
            for(int i = 0; i < Count; i++)
            {
                Boolean[] b = new Boolean[Lengthes[i]];
                for(int j = 0; j < Lengthes[i]; j++)
                {
                    ulong pos = (ulong)Math.Pow(_base, j);
                    if (m_Arrays[i] < pos)
                    {
                        for (int k = j; k < Lengthes[i]; k++)
                            b[k] = false;
                        continue;
                    }
                    b[j] = (m_Arrays[i] & pos) == pos;
                }
                result[i] = b;
            }
            return result;
        }
    }
}
