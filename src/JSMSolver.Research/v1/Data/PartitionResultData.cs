using JSMBase;
using System;
using System.Collections.Generic;
using System.Text;

namespace JSMSolver.Research.v1.Data
{
    public sealed class PartitionResultData<T, TData> where TData : JSMDataBase<T>
    {
        public PartitionResultData(Addings pl, Addings mn, ResearchDataHyp<T, TData> hyp)
        {
            Stratagy = new Stratagy(pl, mn);
            Data = hyp;
        }
        public Stratagy Stratagy { get; private set; }

        public ResearchDataHyp<T, TData> Data { get; private set; }
    }
}
