using JSMBase;
using JSMBaseC;
using System;
using System.Collections.Generic;
using System.Text;

namespace JSMSolver.Research
{
    /// <summary>Данные для исследования</summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public sealed class ResearchData<T, TData> where TData : JSMDataBase<T>
    {
        /// <summary>Массив данных</summary>
        public IEnumerable<TData> Data { get; set; }
        /// <summary>Ограничения на родителей</summary>
        public Dictionary<Signs, CompareData> Restrictions { get; set; }

    }
}
