using JSMBase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JSMSolver.Research.Builders
{
    public abstract class ResearchDataHypBuilderBase<T, TData> where TData : JSMDataBase<T>
    {
        protected List<String> pathes = new List<string>();
        public abstract Task<List<ResearchDataHyp<T, TData>>> Build();
    }
}
