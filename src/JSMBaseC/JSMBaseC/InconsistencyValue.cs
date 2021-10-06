using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase
{
    /// <summary>Оценка непротиворечивости</summary>
    public class InconsistencyValue
    {
        public InconsistencyValue(Dictionary<HypCollectionType, Dictionary<Signs, double>> inn, Dictionary<HypCollectionType, Dictionary<Signs, List<InconsistencyDetails>>> dets = null)
        {
            this.inner = inn;
            this.dets = dets;
        }
        Dictionary<HypCollectionType, Dictionary<Signs, double>> inner;
        Dictionary<HypCollectionType, Dictionary<Signs, List<InconsistencyDetails>>> dets;

        public Dictionary<Signs, double> this[HypCollectionType t]
        {
            get
            {
                return inner.ContainsKey(t) ? inner[t] : null;
            }
        }

        public double this[HypCollectionType t, Signs s]
        {
            get
            {
                var d = this[t];
                if (d == null) return 0;
                return d.ContainsKey(s) ? d[s] : 0;
            }
        }

        public List<InconsistencyDetails> GetDetails(HypCollectionType t, Signs s)
        {
            if (dets == null) return null;
            if (!dets.ContainsKey(t)) return null;
            var ins = dets[t];
            if (!ins.ContainsKey(s)) return null;
            return ins[s];
        }
    }
}
