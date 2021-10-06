//using CliqueLib;
using JSMBase;
using JSMSolver.Research.v1.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace JSMSolver.Research.v1.Partitions
{
    public abstract class BaseWrapper<T, TData>  where TData : JSMDataBase<T>
    {
        public PartitionInputData<T, TData> Inner { get; protected set; }
        protected Dictionary<String, Signs> verified;
        protected Signs[] sgs = new Signs[] { Signs.Plus, Signs.Minus, Signs.Null };
        public BaseWrapper(PartitionInputData<T, TData> inner, Dictionary<String, Signs> verified)
        {
            this.Inner = inner;
            this.verified = verified;
        }
        public abstract bool CheckInconsistency(Object seq2);

        protected Signs getSign(Dictionary<Signs, List<Hypothesis<T, TData>>> hs, String id)
        {
            foreach (Signs sg in sgs)
            {
                var lst = hs[sg];
                foreach (var h in lst)
                    if (h.ID == id) return sg;
            }
            return Signs.None;
        }

        protected Signs getSign(Dictionary<Signs, List<Prediction<T, TData>>> hs, String id)
        {
            foreach (Signs sg in sgs)
            {
                var lst = hs[sg];
                foreach (var h in lst)
                    if (h.Body.ID == id) return sg;
            }
            return Signs.None;
        }

        public bool? IsWeaker(Object seq2)
        {
            
            //StrongInconsistencyWrapper<T, TData> other = seq2 as StrongInconsistencyWrapper<T, TData>;
            //bool? b1 = isWeaker(inner.Stratagy.Plus, other.inner.Stratagy.Plus);
            //bool? b2 = isWeaker(inner.Stratagy.Minus, other.inner.Stratagy.Minus);
            //if (!b1.HasValue || !b2.HasValue) return null;
            //if (b1.Value == b2.Value) return b1.Value;
            
            return null;
        }

        private bool? isWeaker(Addings a1, Addings a2)
        {
            if ((a2 & a1) == a1) return true;
            if ((a2 & a1) == a2) return false;
            return null;
        }
    }
}

