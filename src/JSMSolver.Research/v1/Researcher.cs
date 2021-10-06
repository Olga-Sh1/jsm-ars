using JSMBase;
using JSMSolver.Research.ER;
using JSMSolver.Research.ER.Services;
using JSMSolver.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JSMSolver.Research.v1
{
    public sealed class Researcher<T, TData> where TData : JSMDataBase<T>
    {
        ERCreateService er = new ERCreateService();
        IEnumerable<ChainData<T, TData>> chainData = null;

        public async Task<ResearchResult<T, TData>> Count(CancellationToken token, IProgress<int> pr, Addings plus, Addings minus, params ResearchData<T, TData>[] exts)
        {
            if (exts.Length == 0) throw new ArgumentException("Нет расширений - no extensions");
            IEnumerable<TData> data = Enumerable.Empty<TData>();
            Dictionary<Signs, IList>[] arrExtentions = new Dictionary<Signs, System.Collections.IList>[exts.Length];
            Dictionary<Signs, Div>[] exp = new Dictionary<Signs, Div>[exts.Length];
            for (int i = 0; i < exts.Length;i++)
            {
                data = data.Union(exts[i].Data);
                Solver<T, TData> solv = new Solver<T, TData>();
                solv.Data[Signs.Plus] = new ReadOnlyCollection<TData>(data.Where(d => d.Sign == Signs.Plus).ToArray());
                solv.Data[Signs.Minus] = new ReadOnlyCollection<TData>(data.Where(d => d.Sign == Signs.Minus).ToArray());
                solv.Data[Signs.Tau] = new ReadOnlyCollection<TData>(data.Where(d => d.Sign == Signs.Tau).ToArray());
                solv.Data[Signs.Null] = new ReadOnlyCollection<TData>(data.Where(d => d.Sign == Signs.Null).ToArray());
                solv.Init(exts[i].Restrictions[Signs.Plus], exts[i].Restrictions[Signs.Minus], null, null, plus, minus, null, false, null);
                await solv.JSMSimple(token, pr);

                Dictionary<Signs, IList> dataStep = convert(solv.Hypotheses);
                arrExtentions[i] = dataStep;

                ExplanationService<T, TData> expwr = new ExplanationService<T, TData>(solv.Data, convert2(solv.Hypotheses));
                var expdict = expwr.GetExplanation();
                exp[i] = expdict;
            }

            chainData = er.FindCodes<T, TData>(arrExtentions);
            return new ResearchResult<T, TData>(chainData, exp);
        }

        internal async Task<ResearchDataHyp<T, TData>[]> CountSolver(CancellationToken token, IProgress<int> pr, Addings plus, Addings minus, params ResearchData<T, TData>[] exts)
        {
            if (exts.Length == 0) throw new ArgumentException("Нет расширений - no extensions");
            IEnumerable<TData> data = Enumerable.Empty<TData>();
            Dictionary<Signs, IList>[] arrExtentions = new Dictionary<Signs, System.Collections.IList>[exts.Length];
            Dictionary<Signs, Div>[] exp = new Dictionary<Signs, Div>[exts.Length];
            ResearchDataHyp<T, TData>[] result = new ResearchDataHyp<T, TData>[exts.Length];
            for (int i = 0; i < exts.Length; i++)
            {
                data = data.Union(exts[i].Data);
                Solver<T, TData> solv = new Solver<T, TData>();
                solv.Data[Signs.Plus] = new ReadOnlyCollection<TData>(data.Where(d => d.Sign == Signs.Plus).ToArray());
                solv.Data[Signs.Minus] = new ReadOnlyCollection<TData>(data.Where(d => d.Sign == Signs.Minus).ToArray());
                solv.Data[Signs.Tau] = new ReadOnlyCollection<TData>(data.Where(d => d.Sign == Signs.Tau).ToArray());
                solv.Data[Signs.Null] = new ReadOnlyCollection<TData>(data.Where(d => d.Sign == Signs.Null).ToArray());
                solv.Init(exts[i].Restrictions[Signs.Plus], exts[i].Restrictions[Signs.Minus], null, null, plus, minus, null, false, null);
                await solv.JSMSimple(token, pr);

                result[i] = new ResearchDataHyp<T, TData>(exts[i].Data, solv.Hypotheses, (solv as ISolver).Predictions as Dictionary<Signs, List<Prediction<T, TData>>>);
            }

            return result;
        }

        public async Task<ResearchResult<T, TData>> Count(CancellationToken token, IProgress<int> pr, ResearchDataHyp<T, TData>[] exts, Dictionary<String, Signs> verified)
        {
            if (exts.Length == 0) throw new ArgumentException("Нет расширений - no extensions");
            Dictionary<Signs, IList>[] arrExtentions = new Dictionary<Signs, IList>[exts.Length];
            Dictionary<Signs, IList>[] predsExtentions = new Dictionary<Signs, IList>[exts.Length];
            Dictionary<Signs, Div>[] exp = new Dictionary<Signs, Div>[exts.Length];
            for (int i = 0; i < exts.Length;i++)
            {
                Dictionary<Signs, IList> d = convert(exts[i].Hyps);
                arrExtentions[i] = d;

                Dictionary<Signs, IList> d2 = convert(exts[i].Preds);
                predsExtentions[i] = d2;

                ExplanationService<T, TData> expwr = new ExplanationService<T, TData>(convert(exts[i]), convert2(exts[i].Hyps));
                var expdict = expwr.GetExplanation();
                exp[i] = expdict;
            }
            chainData = er.FindCodes<T, TData>(arrExtentions, predsExtentions, verified);
            
            return new ResearchResult<T, TData>(chainData, exp);
        }

        public Task<ResearchResult<T, TData>> Count(CancellationToken token, IProgress<int> pr, Object[] exts, Dictionary<String, Signs> verified)
        {
            return Count(token, pr, exts.Cast<ResearchDataHyp<T, TData>>().ToArray(), verified);
        }

            /// <summary></summary>
            /// <param name="type">Regularity type; if None, then all</param>
            /// <param name="sgs">Sign type; if None, then all</param>
            /// <returns></returns>
            public IEnumerable<Hypothesis<T, TData>> GetRegularities(ERType type, Signs sgs)
        {
            switch(type)
            {
                case ERType.EL:
                    break;
            }
            throw new NotImplementedException();
        }

        private Dictionary<Signs, IList> convert (Dictionary<Signs, List<Hypothesis<T, TData>>> arr)
        {
            Dictionary<Signs, IList> dataStep = new Dictionary<Signs, IList>();
            dataStep.Add(Signs.Plus, arr[Signs.Plus]);
            dataStep.Add(Signs.Minus, arr[Signs.Minus]);
            dataStep.Add(Signs.Null, arr[Signs.Null]);
            return dataStep;
        }

        private Dictionary<Signs, IList> convert(Dictionary<Signs, List<Prediction<T, TData>>> arr)
        {
            Dictionary<Signs, IList> dataStep = new Dictionary<Signs, IList>();
            Signs[] sgs = new Signs[] { Signs.Plus, Signs.Minus, Signs.Null };
            foreach(var sg in sgs)
            {
                if (arr.ContainsKey(sg))
                    dataStep.Add(sg, arr[sg]);
            }
            return dataStep;
        }

        private Dictionary<Signs, IEnumerable<Hypothesis<T, TData>>> convert2(Dictionary<Signs, List<Hypothesis<T, TData>>> arr)
        {
            Dictionary<Signs, IEnumerable<Hypothesis<T, TData>>> dataStep = new Dictionary<Signs, IEnumerable<Hypothesis<T, TData>>>(arr.Count);
            foreach(var pair in arr)
            {
                dataStep.Add(pair.Key, pair.Value);
            }
            return dataStep;
        }

        private Dictionary<Signs, ReadOnlyCollection<TData>> convert(ResearchDataHyp<T, TData> d)
        {
            Dictionary<Signs, ReadOnlyCollection<TData>> res = new Dictionary<Signs, ReadOnlyCollection<TData>>();
            res.Add(Signs.Plus, new ReadOnlyCollection<TData>(d.Data.Where(dd => dd.Sign == Signs.Plus).ToList()));
            res.Add(Signs.Minus, new ReadOnlyCollection<TData>(d.Data.Where(dd => dd.Sign == Signs.Minus).ToList()));
            res.Add(Signs.Null, new ReadOnlyCollection<TData>(d.Data.Where(dd => dd.Sign == Signs.Null).ToList()));
            return res;
        }
    }
}
