using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.Medicine
{
    public class TupleEx<T1, T2>
    {
        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }
        internal TupleEx(T1 it1, T2 it2)
        {
            Item1 = it1;
            Item2 = it2;
        }
    }
    public class TupleEx
    {
        public static TupleEx<T1, T2> Create<T1, T2>(T1 ind, T2 ind2)
        {
            TupleEx<T1, T2> t = new TupleEx<T1, T2>(ind, ind2);
            return t;
        }
    }
}
