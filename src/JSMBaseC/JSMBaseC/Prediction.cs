using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase
{
    public sealed class Prediction<T, TData> : IObjectable, IID where TData : JSMDataBase<T>
    {
        public Prediction(TData body, List<IHypothesis<T, TData>> preds)
        {
            this.ParentList = preds;
            this.Body = body;
        }
        public List<IHypothesis<T, TData>> ParentList { get; private set; }

        public TData Body { get; private set; }

        public object Object { get { return Body; } }

        public string ID => Body.ID;

        public override string ToString()
        {
            return Body.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj is Prediction<T, TData>)  return base.Equals(obj);
            if (obj is TData) return Body.Equals(obj);
            return false;
        }
    }
}
