using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace JSMBase
{
    public sealed class Hypothesis<T, TData> : IHypothesis<T, TData>, IObjectable where TData : JSMDataBase<T>
    {
        /// <summary>Замкнуто ли пересечение пересечениях</summary>
        public Boolean IsLeaf { get; set; }
        /// <summary>Вес негативный</summary>
        public Double Weight { get; set; }
        /// <summary>Вес собственный</summary>
        public Double WeightOwn { get; set; }
        public UInt32 Index { get; internal set; }
        public TData Body { get; set; }
        public List<TData> ParentList { get; private set; }
        public Boolean IsRealHyp { get; set; }
        public HypFeatures Type { get; set; }
        public HypGain GainType { get; set; }
        public Hypothesis(TData ob)
        {
            Body = ob;
            ParentList = new List<TData>() { ob };
        }
        public Hypothesis(uint index, String id)
        {
            //Body = new JSMDataBase<T>(new T());
            ParentList = new List<TData>();
            Index = index;
            ID = id;
        }
        //public Hypothesis(String id)
        //{
        //    Body = new T();
        //    ParentList = new List<T>();
        //    ID = id;
        //}
        private Hypothesis() { }
        public IntersectResult Intersect(Hypothesis<T, TData> other, out Hypothesis<T, TData> newEx) 
        {
            newEx = null;
            //if (this.Body.IsEqual(other.Body))
            //{
            //    other.ParentList.AddRange(this.ParentList);
            //    return IntersectResult.Equals;
            //}
            var _body = this.Body.Intersect(other.Body) as TData;
            if (_body == null || _body.IsEmpty) return IntersectResult.IsNull;
            
            newEx = new Hypothesis<T, TData>()
            {
                Body = _body,
                ParentList = this.ParentList.Union(other.ParentList).ToList()
            };

            if (_body.IsJSMEqual(this.Body))
                return IntersectResult.Equals;
            return IntersectResult.CreateNew;

        }

        public Object Object
        {
            get { return Body; }
        }

        public override string ToString()
        {
            if (GainType == HypGain.Break)
                return "Тормоз №" + Index;
            return "Гипотеза №" + Index;
            //return String.Format(_format, Index, WeightOwn, Weight);
        }
        private readonly string _format = "Гипотеза№{0} wo={1:N2} wn={2:N2}";
        public string _ID;
        public string ID
        {
            get 
            {
                if (String.IsNullOrEmpty(_ID))
                    return ToString();
                return _ID;
            }
            private set { _ID = value; }
        }


        public IJSMObject Clone(BitArrayBase inner, IContext context)
        {
            throw new NotSupportedException();
            //Hypothesis<T> h = new Hypothesis<T>();
            //h.Body = this.Body.Clone(inner, context) as T;
            //h.ParentList = this.ParentList;
            //h.Index = this.Index;
            //h.IsRealHyp = this.IsRealHyp;
            //return h;
        }

        public void AddToParentList(IEnumerable<TData> newList)
        {
            this.ParentList = this.ParentList.Union(newList).ToList();
        }
    }
}
