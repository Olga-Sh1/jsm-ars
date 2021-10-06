using JSMBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace JSMSolver.Research
{
    public sealed class ResearchDataHyp<T, TData> where TData : JSMDataBase<T>
    {
        public ResearchDataHyp(IEnumerable<TData> data, Dictionary<Signs, IList> hs, Dictionary<Signs, IList> prds)
        {
            Data = data;
            Hyps = new Dictionary<Signs, List<Hypothesis<T, TData>>>();
            foreach (var p in hs)
                Hyps.Add(p.Key, p.Value as List<Hypothesis<T, TData>>);
            Preds = new Dictionary<Signs, List<Prediction<T, TData>>>();
            foreach (var p in prds)
                Preds.Add(p.Key, p.Value as List<Prediction<T, TData>>);
        }
        public ResearchDataHyp(IEnumerable<TData> data, Dictionary<Signs, List<Hypothesis<T, TData>>> hs, Dictionary<Signs, List<Prediction<T, TData>>> prds)
        {
            Data = data;
            Hyps = hs;
            Preds = prds;
        }
        /// <summary>Data array</summary>
        public IEnumerable<TData> Data { get; set; }
        /// <summary>Hypotheses</summary>
        public Dictionary<Signs, List<Hypothesis<T, TData>>> Hyps { get; set; }
        /// <summary>Predictions</summary>
        //public Dictionary<Signs, List<Hypothesis<T, TData>>> Preds { get; set; }

        /// <summary>Predictions</summary>
        public Dictionary<Signs, List<Prediction<T, TData>>> Preds { get; set; }

    }
}
