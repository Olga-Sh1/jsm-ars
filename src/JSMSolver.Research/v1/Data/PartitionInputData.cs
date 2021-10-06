using JSMBase;
using System;
using System.Collections.Generic;
using System.Text;

namespace JSMSolver.Research.v1.Data
{
    public sealed class PartitionInputData<T, TData> where TData : JSMDataBase<T>
    {
        public PartitionInputData(Addings pl, Addings mn, ResearchDataHyp<T, TData> data)
        {
            Stratagy = new Stratagy(pl, mn);
            Data = data;
        }
        public Stratagy Stratagy { get; private set; }
        public ResearchDataHyp<T, TData> Data { get; private set; }
    }
}
