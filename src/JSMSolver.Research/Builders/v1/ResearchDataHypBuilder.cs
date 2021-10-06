using JSMBase;
using JSMBase.Medicine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMSolver.Research.Builders
{
    public sealed class ResearchDataHypBuilder<T, TData> : ResearchDataHypBuilderBase<T, TData> where TData : JSMDataBase<T>
    {
       
        List<HypBatchBase<T, TData>> hyps = new List<HypBatchBase<T, TData>>();

        public ResearchDataHypBuilder<T, TData> Add(HypBatchBase<T, TData> hyp, String path)
        {
            pathes.Add(path);
            hyps.Add(hyp);
            return this;
        }

        public override async Task<List<ResearchDataHyp<T, TData>>> Build()
        {
            int i = 0;
            List<ResearchDataHyp<T, TData>> res = new List<ResearchDataHyp<T, TData>>(pathes.Count);
            foreach (String path in pathes)
            {
                HypBatchBase<T, TData> h = hyps[i];
                await h.Read(path);
                var arri = h.WrappedCollection;
                Dictionary<Signs, List<Hypothesis<T, TData>>> d2 = new Dictionary<Signs, List<Hypothesis<T, TData>>>();
                d2.Add(Signs.Plus, h[HypCollectionType.Reason, Signs.Plus].ToList());
                d2.Add(Signs.Minus, h[HypCollectionType.Reason, Signs.Minus].ToList());
                d2.Add(Signs.Null, h[HypCollectionType.Reason, Signs.Null].ToList());

                if (needTryPredict(h)) tryPredict(h);

                Dictionary<Signs, List<Prediction<T, TData>>> d3 = new Dictionary<Signs, List<Prediction<T, TData>>>();
                d3.Add(Signs.Plus, h[HypCollectionType.Redefined, Signs.Plus].Select(r => convert(r)).ToList());
                d3.Add(Signs.Minus, h[HypCollectionType.Redefined, Signs.Minus].Select(r => convert(r)).ToList());
                d3.Add(Signs.Null, h[HypCollectionType.Redefined, Signs.Null].Select(r => convert(r)).ToList());

                res.Add(new ResearchDataHyp<T, TData>(arri, d2, d3));
                i++;
            }
            return res;
        }

        private void tryPredict(HypBatchBase<T, TData> hhs)
        {
            var taus = hhs.WrappedCollection.Where(c => c.Sign == Signs.Tau).ToArray();
            var hsPl = hhs[HypCollectionType.Reason, Signs.Plus] ?? new List<Hypothesis<T, TData>>();
            var hsMn = hhs[HypCollectionType.Reason, Signs.Minus] ?? new List<Hypothesis<T, TData>>();
            var hsNl = hhs[HypCollectionType.Reason, Signs.Null] ?? new List<Hypothesis<T, TData>>();
            Dictionary<Signs, List<Hypothesis<T, TData>>> d = new Dictionary<Signs, List<Hypothesis<T, TData>>>();
            d.Add(Signs.Plus, new List<Hypothesis<T, TData>>());
            d.Add(Signs.Minus, new List<Hypothesis<T, TData>>());
            d.Add(Signs.Null, new List<Hypothesis<T, TData>>());
            foreach (var tau in taus)
            {
                Signs current = Signs.None;
                foreach (var h in hsPl)
                {
                    if (h.Body.IsEnclosed(tau))
                    {
                        current = Signs.Plus;
                        break;
                    }
                }
                foreach (var h in hsMn)
                {
                    if (h.Body.IsEnclosed(tau))
                    {
                        current = current == Signs.None ? Signs.Minus : Signs.Null;
                        break;
                    }
                }
                foreach (var h in hsNl)
                {
                    if (h.Body.IsEnclosed(tau))
                    {
                        current = Signs.Null;
                        break;
                    }
                }
                if (current != Signs.None)
                {
                    Hypothesis<T, TData> hnew = new Hypothesis<T, TData>(0, tau.ID);
                    hnew.Body = tau;
                    d[current].Add(hnew);
                }
            }

            foreach (var pair in d)
            {
                hhs.Add(HypCollectionType.Redefined, pair.Key, pair.Value);
            }
        }

        private bool needTryPredict(HypBatchBase<T, TData> hbb)
        {
            return hbb[HypCollectionType.Redefined, Signs.Plus] == null &&
                hbb[HypCollectionType.Redefined, Signs.Minus] == null;
        }

        private Prediction<T, TData> convert (Hypothesis<T, TData> h)
        {
            Prediction<T, TData> pr = new Prediction<T, TData>(h.Body, null);
            return pr;
        }
    }
}
