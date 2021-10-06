using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.RNK
{
    /// <summary>Значение кода гена (2 буквы)</summary>
    public sealed class PropValueGen2Letter : IPropValue
    {
        private Char[] _arr;
        private PropValueGen2Letter(PropInfoGen2Letter info, Char[] rawData) : base()
        {
            this._arr = rawData;
            this.Info = info;
        }

        public PropValueGen2Letter(PropInfoGen2Letter info, String rawData) : base()
        {
            this._arr = rawData.Where(o => !Char.IsWhiteSpace(o)).ToArray();
            this.Info = info;
        }

        /// <summary>Информация</summary>
        public PropInfoGen2Letter Info { get; set; }

        public PropValueGen2Letter FindSimilarity(PropValueGen2Letter other)
        {
            List<Char> r = new List<char>(_arr.Length);
            int k = 0;
            for(int i = 0; i < _arr.Length;i++)
            {
                for (int j = k; j < other._arr.Length; j++)
                    if (_arr[i] == other._arr[j])
                    {
                        r.Add(_arr[i]);
                        k = j + 1;
                        break;
                    }
            }
            if (r.Count == 0) return null;
            return new PropValueGen2Letter(Info, r.ToArray());
        }

        public PropValueGen2Letter FindDifference(PropValueGen2Letter other)
        {
            List<Char> r = new List<char>(_arr.Length);
            for (int i = 0; i < _arr.Length; i++)
            {
                for (int j = 0; j < other._arr.Length; j++)
                    if (_arr[i] != other._arr[j])
                    {
                        r.Add(_arr[i]);
                        break;
                    }
            }
            return new PropValueGen2Letter(Info, r.ToArray());
        }

        public bool IsEnclosed(PropValueGen2Letter other)
        {
            int k = 0;
            for (int i = 0; i < _arr.Length; i++)
            {
                bool b = false;
                for (int j = k; j < other._arr.Length; j++)
                {
                    if (_arr[i] == other._arr[j])
                    {
                        b = true;
                        k = j+1;
                        break;
                    }
                }
                if (!b) return false;
            }
            return true;
        }

        public bool Equals(PropValueGen2Letter other)
        {
            if (_arr.Length != other._arr.Length) return false;
            for (int i = 0; i < _arr.Length; i++)
            {
                if (_arr[i] != other._arr[i])
                    return false;
            }
            return true;
        }

        #region Реализация IPropValue
        IPropInfo IPropValue.PropInfo => this.Info;

        object IPropValue.Value => new String (_arr);

        bool IPropValue.IsEmpty => _arr == null || _arr.Length == 0;

        IPropValue IPropValue.FindDifference(IPropValue other)
        {
            return this.FindDifference(other as PropValueGen2Letter);
        }

        IPropValue IPropValue.FindSimilarity(IPropValue other)
        {
            return this.FindSimilarity(other as PropValueGen2Letter);
        }

        bool IPropValue.IsEnclosed(IPropValue other)
        {
            return this.IsEnclosed(other as PropValueGen2Letter);
        }

        bool IPropValue.IsEquals(IPropValue other)
        {
            return Equals(other as PropValueGen2Letter);
        }
        #endregion
    }
}
