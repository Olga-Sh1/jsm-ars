using JSMBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace JSMBaseC.Services.Diagram
{
    public sealed class DiagramGirdleService<T, TData> : IDiagramGirdleService where TData : JSMDataBase<T> 
    {
        public IEnumerable<Hypothesis<T, TData>> FindMax(IEnumerable<Hypothesis<T, TData>> alls)
        {
            List<Hypothesis<T, TData>> res = new List<Hypothesis<T, TData>>(alls.Count() / 3);
            foreach (Hypothesis<T, TData> h in alls)
            {
                bool isMax = true;
                foreach (Hypothesis<T, TData> h2 in alls)
                {
                    if (h != h2)
                    {
                        if (h.Body.IsEnclosed(h2.Body))
                        {
                            isMax = false;
                            break;
                        }
                    }
                }

                if (isMax) res.Add(h);
            }
            return res;
        }

        /// <summary>Находим минимальные гипотезы из массива</summary>
        /// <param name="alls">Все</param>
        /// <returns></returns>
        public IEnumerable<Hypothesis<T, TData>> FindMins(IEnumerable<Hypothesis<T, TData>> alls)
        {
            if (alls == null) return Enumerable.Empty<Hypothesis<T, TData>>();
               List <Hypothesis<T, TData>> res = new List<Hypothesis<T, TData>>(alls.Count() /3);
            foreach (Hypothesis<T, TData> h in alls)
            {
                bool isMin = true;
                foreach(Hypothesis<T, TData> h2 in alls)
                {
                    if (h != h2)
                    {
                        if (h2.Body.IsEnclosed(h.Body))
                        {
                            isMin = false;
                            break;
                        }
                    }
                }

                if (isMin) res.Add(h);
            }
            return res;
        }

        /// <summary>Найти массив гипотез, которые вкладываются в данную</summary>
        /// <param name="alls"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public IEnumerable<Hypothesis<T, TData>> FindSubHypotheses(IEnumerable<Hypothesis<T, TData>> alls, Hypothesis<T, TData> parent)
        {
            List<Hypothesis<T, TData>> res = new List<Hypothesis<T, TData>>(alls.Count() / 3);
            foreach (Hypothesis<T, TData> h in alls)
            {
                if (h!= parent && h.Body.IsEnclosed(parent.Body))
                    res.Add(h);
            }
            return res;
        }

        /// <summary>Найти массив гипотез, в которые вкладывается данная</summary>
        /// <param name="alls"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        public IEnumerable<Hypothesis<T, TData>> FindSuperHypotheses(IEnumerable<Hypothesis<T, TData>> alls, Hypothesis<T, TData> child)
        {
            List<Hypothesis<T, TData>> res = new List<Hypothesis<T, TData>>(alls.Count() / 3);
            foreach (Hypothesis<T, TData> h in alls)
            {
                if (h != child && child.Body.IsEnclosed(h.Body))
                    res.Add(h);
            }
            return res;
        }

        public bool HasAnySuper(IEnumerable<Hypothesis<T, TData>> alls, Hypothesis<T, TData> child)
        {
            return alls.Any(h => h != child && child.Body.IsEnclosed(h.Body));
        }

        IEnumerable IDiagramGirdleService.FindMins(IEnumerable arr)
        {
            return this.FindMins(arr.Cast<Hypothesis<T, TData>>().ToArray());
        }

        IEnumerable IDiagramGirdleService.FindSuper(IEnumerable alls, object child)
        {
            return FindSuperHypotheses(alls.OfType<Hypothesis<T, TData>>().ToArray(), child as Hypothesis<T, TData>);
        }
    }
}
