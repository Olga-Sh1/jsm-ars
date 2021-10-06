using JSMBase;
using JSMBaseC.Services.RW.v2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMSolver.Research.Builders.v2
{
    public sealed class ResearchDataHypBuilder<T, TData> : ResearchDataHypBuilderBase<T, TData> where TData : JSMDataBase<T>
    {
        List<IDataService> dataServices = new List<IDataService>();
        public ResearchDataHypBuilder<T, TData> Add(IDataService dataService, String path)
        {
            pathes.Add(path);
            dataServices.Add(dataService);
            return this;
        }
        public override async Task<List<ResearchDataHyp<T, TData>>> Build()
        {
            int i = 0;
            List<ResearchDataHyp<T, TData>> res = new List<ResearchDataHyp<T, TData>>(pathes.Count);
            foreach (String path in pathes)
            {
                RWHypothesesService serv = new RWHypothesesService(dataServices[i]);
                var r2 = await Task.Run(() => serv.Read(path));

                Dictionary<Signs, List<Hypothesis<T, TData>>> d2 = new Dictionary<Signs, List<Hypothesis<T, TData>>>();
                d2.Add(Signs.Plus, r2.Item1[Signs.Plus] as List<Hypothesis<T, TData>>);
                d2.Add(Signs.Minus, r2.Item1[Signs.Minus] as List<Hypothesis<T, TData>>);
                d2.Add(Signs.Null, r2.Item1[Signs.Null] as List<Hypothesis<T, TData>>);

               
                Dictionary<Signs, List<Prediction<T, TData>>> d3 = new Dictionary<Signs, List<Prediction<T, TData>>>();
               d3.Add(Signs.Plus, r2.Item2[Signs.Plus] as List<Prediction<T, TData>>);
               d3.Add(Signs.Minus, r2.Item2[Signs.Minus] as List<Prediction<T, TData>>);
               d3.Add(Signs.Null, r2.Item2[Signs.Null] as List<Prediction<T, TData>>);

                res.Add(new ResearchDataHyp<T, TData>(dataServices[i].JSMWrapperDataType as IEnumerable<TData>, d2, d3));
                i++;
            }
            return res;
        }
    }
}
