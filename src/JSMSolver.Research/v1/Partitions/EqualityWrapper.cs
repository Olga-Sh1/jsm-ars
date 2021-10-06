//using CliqueLib;
using JSMBase;
using JSMSolver.Research.v1.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace JSMSolver.Research.v1.Partitions
{
    public sealed class EqualityWrapper<T, TData> : BaseWrapper<T, TData> where TData : JSMDataBase<T>
    {
        public EqualityWrapper(PartitionInputData<T, TData> inner, Dictionary<String, Signs> verified)
            : base(inner, verified)
        {

        }
        public override bool CheckInconsistency(Object seq2)
        {
            EqualityWrapper<T, TData> other = seq2 as EqualityWrapper<T, TData>;
            foreach (var pair in verified)
            {
                Signs sg1 = getSign(this.Inner.Data.Preds, pair.Key);
                Signs sg2 = getSign(other.Inner.Data.Preds, pair.Key);
                if (sg1 != sg2) return false;
            }

            return true;

        }
    }
}
