using JSMBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSMSolver.Research.v2
{
    /// <summary>Data for one World</summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public sealed class ResearchDataWorld<T, TData> where TData : JSMDataBase<T>
    {
        public ResearchDataWorld(Object[] worldData)
        {
            WorldData = worldData.Cast<ResearchDataHyp<T, TData>>().ToArray();
        }
        public ResearchDataWorld(ResearchDataHyp<T, TData>[] worldData)
        {
            WorldData = worldData;
        }
        /// <summary>Data for Any BF Extension in World</summary>
        public ResearchDataHyp<T, TData>[] WorldData { get; private set; }
    }
}
