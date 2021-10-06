using JSMBase;
using JSMSolver.Research.ER;
using JSMSolver.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace JSMSolver.Research
{
    /// <summary>Result of a research</summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public sealed class ResearchResult<T, TData> where TData : JSMDataBase<T>
    {
        public ResearchResult(IEnumerable<ChainData<T, TData>> data, Dictionary<Signs, Div>[] exp)
        {
            Data = data;
            Explicability = exp;
        }
        /// <summary>Data (codes)</summary>
        public IEnumerable<ChainData<T, TData>> Data { get; private set; }
        public Dictionary<Signs, Div>[] Explicability { get; private set; }

        public bool IsSteady(Signs sgs)
        {
            double prev = double.MinValue;
            for(int i = 0; i  < Explicability.Length; i++)
            {
                if (!Explicability[i].ContainsKey(sgs)) return false;
                if (prev > Explicability[i][sgs].Value) return false;
                prev = Explicability[i][sgs].Value;
            }
            return true;
        }
    }
}
