using JSMBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.Medicine.Models
{
    /// <summary>База фактов</summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseOfFacts2<T>
    {
        /// <summary>Имена групп</summary>
        public Dictionary<Signs, String> GroupNames { get; private set; }
        /// <summary>Базовый массив</summary>
        public IEnumerable<T> Inner { get; private set; }
        public BaseOfFacts2(IEnumerable<T> inner)
        {
            this.Inner = inner;
        }
        /// <summary>Разделение по знакам</summary>
        /// <returns></returns>
        public abstract IEnumerable<global::JSMBase.JSMDataBase<T>> Divide();
    }
}
