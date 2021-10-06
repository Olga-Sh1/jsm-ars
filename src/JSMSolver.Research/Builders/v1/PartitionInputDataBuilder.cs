using JSMBase;
using JSMSolver.Research.v1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMSolver.Research.Builders.v1
{
    public sealed class PartitionInputDataBuilder<T, TData> : PartitionInputDataBuilderBase<T, TData> where TData : JSMDataBase<T>
    {
        List<HypBatchBase<T, TData>> hypBatches = new List<HypBatchBase<T, TData>>();
        public PartitionInputDataBuilder(){ }
        public PartitionInputDataBuilder(int capacity) : base(capacity)
        {
            hypBatches.Capacity = capacity;
        }
        public void Add(Addings pl, Addings mn, HypBatchBase<T, TData> h, String path)
        {
            pls.Add(pl);
            mns.Add(mn);
            hypBatches.Add(h);
            pathes.Add(path);
        }

        public override async Task<PartitionInputData<T, TData>[]> Build()
        {
            List<PartitionInputData<T, TData>> res = new List<PartitionInputData<T, TData>>();
            for (int i = 0; i < pathes.Count; i++)
            {
                var dhs = await new ResearchDataHypBuilder<T, TData>()
                    .Add(hypBatches[i], pathes[i])
                    .Build();

                res.AddRange(dhs.Select(d => new PartitionInputData<T, TData>(pls[i], mns[i], d)));
            }

            return res.ToArray();
        }

        public override Dictionary<String, Signs> CollectAllPreds()
        {
            Dictionary<String, Signs> dh = new Dictionary<string, Signs>();
            foreach (var hhs in hypBatches)
            {
                var taus = hhs.WrappedCollection.Where(c => c.Sign == Signs.Tau).ToArray();
                foreach(var tau in taus)
                {
                    if (!dh.ContainsKey(tau.ID))
                        dh.Add(tau.ID, Signs.None);
                }
                
            }
            return dh;
        }
    }
}
