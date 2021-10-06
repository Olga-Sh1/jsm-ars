//using CliqueLib;
using JSMBase;
using JSMSolver.Research.v1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSMSolver.Research.v1.Partitions
{
    
    public sealed class StrongInconsistencyWrapper<T, TData> : BaseWrapper<T, TData> where TData : JSMDataBase<T>
    {
        public StrongInconsistencyWrapper(PartitionInputData<T, TData> inner, Dictionary<String, Signs> verified)
            :base(inner, verified)
        {

        }
        public override bool CheckInconsistency(Object seq2)
        {
            StrongInconsistencyWrapper<T, TData> other = seq2 as StrongInconsistencyWrapper<T, TData>;
            foreach (var pair in verified)
            {
                Signs sg1 = getSign(this.Inner.Data.Preds, pair.Key);
                Signs sg2 = getSign(other.Inner.Data.Preds, pair.Key);
                if (sg1 != sg2 && sg1 != Signs.None && sg2 != Signs.None) return false;
            }

            return true;

        }
    }
}
