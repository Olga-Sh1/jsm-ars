using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase
{
    public class BitBaseIndicesPair
    {
        public BitBaseIndicesPair(BitArrayBase _base, UInt32[] _ind)
        {
            Base = _base;
            Indices = _ind;
        }
        public BitArrayBase Base { get; private set; }
        public UInt32[] Indices { get; private set; }
    }
}
