using JSMBase;
using JSMBaseC.Services.RW.v2;
using JSMSolver.Research.v1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMSolver.Research.Builders.v2
{
    public sealed class PartitionInputDataBuilder<T, TData, TServ> : PartitionInputDataBuilderBase<T, TData> where TData : JSMDataBase<T> where TServ : IDataService, new()
    {
        List<String> data_pathes = new List<string>();
        Dictionary<String, TServ> cache = new Dictionary<string, TServ>();
        public void Add(Addings pl, Addings mn, String data_path, String path)
        {
            pls.Add(pl);
            mns.Add(mn);
            pathes.Add(path);
            data_pathes.Add(data_path);
        }

        public override async Task<PartitionInputData<T, TData>[]> Build()
        {
            List<PartitionInputData<T, TData>> res = new List<PartitionInputData<T, TData>>();
            for (int i = 0; i < pathes.Count; i++)
            {
                if (!cache.ContainsKey(data_pathes[i]))
                {
                    TServ dserv = new TServ();
                    await dserv.Open(data_pathes[i]);
                    cache.Add(data_pathes[i], dserv);
                }

                var dhs = await new ResearchDataHypBuilder<T, TData>()
                    .Add(cache[data_pathes[i]], pathes[i])
                    .Build();

                res.AddRange(dhs.Select(d => new PartitionInputData<T, TData>(pls[i], mns[i], d)));
            }

            return res.ToArray();
        }

        public override Dictionary<string, Signs> CollectAllPreds()
        {
            Dictionary<String, Signs> dh = new Dictionary<string, Signs>();
            foreach (var ds in cache.Values)
            {
                var taus = (ds.Data as List<TData>).Where(t => t.Sign == Signs.Tau).ToList();
                foreach (var tau in taus)
                {
                    if (!dh.ContainsKey(tau.ID))
                        dh.Add(tau.ID, Signs.None);
                }

            }
            return dh;
        }
    }
}
