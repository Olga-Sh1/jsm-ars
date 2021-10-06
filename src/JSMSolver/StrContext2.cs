using JSMBase;
using JSMBase.SpecialAlgorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMSolver
{
    class StrContext2<T, TBody> where TBody : JSMBase.JSMDataBase<T>
    {
        [AddingInfo(Addings.Contr)]
        public Boolean ContrPredicate(Hypothesis<T, TBody> h, IEnumerable<TBody> sameObjs, IEnumerable<TBody> oppositeObjs)
        {
            foreach (var ob in oppositeObjs)
            {
                if (h.Body.IsEnclosed(ob))
                    return false;
            }
            return true;
        }
        [AddingInfo(Addings.Difference)]
        private Boolean DiffPredicate(Hypothesis<T, TBody> h, IEnumerable<TBody> sameObjs, IEnumerable<TBody> oppositeObjs)
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
        private Boolean SimpleDiffPredicate(Hypothesis<T, TBody> h, IEnumerable<TBody> sameObjs, IEnumerable<TBody> oppositeObjs)
        {
            //h - кандидат в гипотезы (сходство), который проверяем на упрощение метода сходства-различия
            //h.ParentList - список "родителей": примеров, которые участвовали в порождении сходства
            var arr = sameObjs.Except(h.ParentList).ToArray();//Массив примеров, которые имеют тот же знак, за вычетом "родительских"
            foreach (var par in h.ParentList)//Проверяем для каждого "родителя"
            {
                var diff = par.Difference(h.Body);//Находим разность
                if (diff.IsEmpty) continue;
                foreach (var other in arr)//Проверяем, что нет ни одного примера, в который входит разность
                {
                    if (diff.IsEnclosed(other))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        [AddingInfo(Addings.DiffSimilarity)]
        private Boolean SimDiffPredicate(Hypothesis<T, TBody> h, IEnumerable<TBody> sameObjs, IEnumerable<TBody> oppositeObjs)
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
    }
}
