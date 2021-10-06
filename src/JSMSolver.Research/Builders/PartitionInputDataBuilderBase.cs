using JSMBase;
using JSMSolver.Research.v1.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JSMSolver.Research.Builders
{
    public abstract class PartitionInputDataBuilderBase<T, TData> where TData : JSMDataBase<T>
    {
        protected List<Addings> pls = new List<Addings>();
        protected List<Addings> mns = new List<Addings>();
        protected List<String> pathes = new List<string>();
        public PartitionInputDataBuilderBase() { }
        public PartitionInputDataBuilderBase(int capacity)
        {
            pls.Capacity = mns.Capacity = pathes.Capacity = capacity;
        }

        public abstract Dictionary<String, Signs> CollectAllPreds();

        public abstract Task<PartitionInputData<T, TData>[]> Build();
    }
}
