using JSMBase;
using JSMBase.Medicine;
using JSMSolver.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMSolver
{
    public class Filters : JSMSolver.Filter.IBaseFilter<JSMMedicine, JSMBase.JSMBase>
    {
        PropInfo[] PlusConjFilter;
        PropInfo[] PlusDizFilter;
        PropInfo[] MinConjFilter;
        PropInfo[] MinDizFilter;
        public Filters(PropInfo[] pl_conj, PropInfo[] pl_diz, PropInfo[] min_conj, PropInfo[] min_diz)
        {
            PlusConjFilter = pl_conj;
            PlusDizFilter = pl_diz;
            MinConjFilter = min_conj;
            MinDizFilter = min_diz;
        }
        public Boolean FilterPlus(Hypothesis<JSMMedicine, JSMBase.JSMBase> m)
        {
            return FilterInner(m, PlusConjFilter, PlusDizFilter);
        }
        public Boolean FilterMin(Hypothesis<JSMMedicine, JSMBase.JSMBase> m)
        {
            return FilterInner(m, MinConjFilter, MinDizFilter);
        }
        private Boolean FilterInner(Hypothesis<JSMMedicine, JSMBase.JSMBase> m, PropInfo[] filterConj, PropInfo[] filterDiz)
        {
            if (filterConj != null)
                foreach (PropInfo pi in filterConj)
                    if (m.Body.IsEmptyProp(JSMMedicine.GetRealIndex(pi)))
                        return false;
            if (filterDiz != null)
            {
                Boolean b = filterDiz.Length == 0;
                foreach (PropInfo pi in filterDiz)
                    b |= !m.Body.IsEmptyProp(JSMMedicine.GetRealIndex(pi));
                return b;
            }
            return true;
        }
    }
}
