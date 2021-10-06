using JSMBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSMSolver
{
    public sealed class Predictor<T, TData> where TData : JSMDataBase<T>
    {
        public IList Predict(IList data, IList hyps)
        {
            return Predict2(data.OfType<TData>().ToArray(), hyps.OfType<Hypothesis<T, TData>>().ToArray());
        }

        public List<Prediction<T, TData>> Predict2(IEnumerable<TData> data, IEnumerable<Hypothesis<T, TData>> hyps)
        {
            List<Prediction<T, TData>> result = new List<Prediction<T, TData>>();
            var taus = data.Where(d => d.Sign == Signs.Tau).ToArray();
            foreach(var t in taus)
            {
                List<IHypothesis<T, TData>> hh = new List<IHypothesis<T, TData>>();
                foreach (var h in hyps)
                {
                    if (h.Body.IsEnclosed(t)) hh.Add(h);
                }
                if (hh.Count > 0)
                    result.Add(new Prediction<T, TData>(t, hh));
            }
            return result;
        }
    }
}
