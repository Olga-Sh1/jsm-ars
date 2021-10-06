using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSMBase.Medicine
{
    public class PropData
    {
        const Char PlusSign = '+';
        const Char MinusSign = '-';
        const Char TauSign = 't';

        public PropInfo Info { get; private set; }
        public Int32[] PlusValuesIndeces { get; set; }
        public Int32[] MinusValuesIndeces { get; set; }
        public PropData(PropInfo _info, String s)
        {
            if (_info.Values.Count != s.Length)
                throw new ArgumentOutOfRangeException(String.Format("Длина разбираемой строки не равна количеству возможных значений : значение {0}, свойство {1}", s, _info.ToString() ));
            Int32[] pl = new int[s.Count(c => c == PlusSign)];
            Int32[] min = new int[s.Count(c => c == MinusSign)];
            int indexPl = 0;
            int indexMin = 0;
            int indexAll  = 0;
            foreach(Char ch in s)
            {
                switch (ch)
                {
                    case PlusSign:
                        pl[indexPl++] = indexAll;
                        break;
                    case MinusSign:
                        min[indexMin++] = indexAll;
                        break;
                }
                indexAll++;
            }
            _Initialize(_info, pl, min);
        }
        public PropData(PropInfo _info, Int32[] pl, Int32[] min)
        {
            _Initialize(_info, pl, min);
        }
        internal void _Initialize(PropInfo _info, Int32[] pl, Int32[] min)
        {
            Info = _info;
            PlusValuesIndeces = pl;
            MinusValuesIndeces = min;
        }

        public Boolean IsEmpty
        {
            get { return PlusValuesIndeces.Length == 0 & MinusValuesIndeces.Length == 0; }
        }

        public override string ToString()
        {
            Char[] chars = new Char[Info.Values.Count];
            for(int i = 0; i < Info.Values.Count; i++)
                chars[i] = TauSign;
            foreach (Int32 i in PlusValuesIndeces)
               chars[i] = PlusSign;
            foreach (Int32 i in MinusValuesIndeces)
                chars[i] = MinusSign;
            return new String(chars);
        }
    }
}
