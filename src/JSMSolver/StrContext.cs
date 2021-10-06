using JSMBase;
using JSMBase.Medicine;
using JSMSolver.Predicates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMSolver
{
    /// <summary>Класс для стратегий</summary>
    public class StrContext<T, TData> where TData : JSMDataBase<T>
    {
        Solver<T, TData> _solv = null;
        public StrContext(Solver<T, TData> solv)
        {
            _solv = solv;
            Initialize();
        }
        private void Initialize()
        {
           
        }

        //Dictionary<Addings, PredicateContainer<T>> _Addings;
        [AddingInfo(Addings.Contr)]
        public Boolean ContrPredicate(Hypothesis<T, TData> h, IEnumerable<JSMDataBase<T>> sameObjs, IEnumerable<TData> oppositeObjs)
        {
            foreach (var ob in oppositeObjs)
            {
                if (h.Body.IsEnclosed(ob))
                    return false;
            }
            return true;
        }
        [AddingInfo(Addings.Difference)]
        private Boolean DiffPredicate(Hypothesis<T, TData> h, IEnumerable<JSMDataBase<T>> sameObjs, IEnumerable<TData> oppositeObjs)
        {
            foreach (var par in h.ParentList)
            {
                var diff = par.Difference(h.Body);
                foreach (var other in oppositeObjs)
                {
                    if (diff.IsEnclosed(other))
                        return true;
                }
            }
            return false;
        }
        [AddingInfo(Addings.SimpleDiffSimilarity)]
        private Boolean SimpleDiffPredicate(Hypothesis<T, TData> h, IEnumerable<TData> sameObjs, IEnumerable<TData> oppositeObjs)
        {
            //foreach (T par in h.ParentList)
            //{
            //    T diff = par.Difference<T>(h.Body);
            //    bool b = true;
            //    foreach (T other in oppositeObjs)
            //    {
            //        if (!h.Body.IsEnclosed(other))
            //        {
            //            b = false;
            //            if (diff.IsEnclosed(other))
            //                return true;
            //        }
            //    }
            //    if (b) return true;
            //}
            //return false;

            var arr = sameObjs.Except(h.ParentList).ToArray();
            foreach (var par in h.ParentList)
            {
                var diff = par.Difference(h.Body);
                if (diff.IsEmpty) continue;
                foreach (var other in arr)
                {
                    if (/*!h.Body.IsEnclosed(other) &&*/ diff.IsEnclosed(other))
                    {
                        return false;
                    }
                }
            }
            return true;

            //foreach (T par in h.ParentList)
            //{
            //    T diff = par.Difference<T>(h.Body);
            //    foreach (T other in myObjs)
            //    {
            //        if (other != par)
            //        {
            //            if (diff.IsEnclosed(other) && !h.Body.IsEnclosed(other))
            //                return false;
            //        }
            //    }
            //}
            //return true;
        }
        [AddingInfo(Addings.DiffSimilarity)]
        private Boolean SimDiffPredicate(Hypothesis<T, TData> h, IEnumerable<TData> sameObjs, IEnumerable<TData> oppositeObjs)
        {
            foreach (var par in h.ParentList)
            {
                //разница между родителем и гипотезой
                var d = par.Difference(h.Body);
                int count = 0;
                //проверка на противоположные объекты
                foreach (var opp in oppositeObjs)
                {
                    //если разница вкладывается, а сама гипотеза не вкладывается
                    if (d.IsEnclosed(opp) && !h.Body.IsEnclosed(opp))
                    {
                        count++;
                        //набираем 2 или больше примеров
                        if (count >= 2) { return true; }
                    }
                }
            }
            return false;
        }

        public void ThroughAddings2(Dictionary<Signs, PredAllExpression<T, TData>> d)
        {
            if (d.ContainsKey(Signs.Plus))
            {
                var func = d[Signs.Plus].GetExpression(_solv);
                _solv.Hypotheses[Signs.Plus].RemoveAll(h =>
                    h.IsRealHyp && !func(h, _solv.Data[Signs.Plus], _solv.Data[Signs.Minus]));
            }

            if (d.ContainsKey(Signs.Minus))
            {
                var func = d[Signs.Minus].GetExpression(_solv);
                _solv.Hypotheses[Signs.Minus].RemoveAll(h =>
                    h.IsRealHyp && !func(h, _solv.Data[Signs.Minus], _solv.Data[Signs.Plus]));
            }
        }

        //[AddingInfo(Addings.WeightedContr)]
        //private Boolean WeightedContrPredicate(Hypothesis<T> h, ICollection<T> sameObjs, ICollection<T> oppositeObjs)
        //{
        //    int index = 0;
        //    foreach (T ob in oppositeObjs)
        //    {
        //        if (h.Body.IsEnclosed(ob))
        //            index++;
        //    }
        //    double weight = (double)(oppositeObjs.Count - index) / (double)oppositeObjs.Count;
        //    h.Weight = weight;
        //    h.WeightOwn = (double)h.ParentList.Count / (double)sameObjs.Count;
        //    return weight > 0.85;
        //}
        }

    public class AddingInfoAttribute : Attribute
    {
        public Addings Adding { get; private set; }
        public AddingInfoAttribute(Addings add)
        {
            Adding = add;
        }
    }
}
