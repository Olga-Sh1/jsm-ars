using JSMBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSMSolver.Research.ER
{
    public abstract class ChainDataBase
    {
        public abstract IEnumerable<Signs> Chain { get; }
        public abstract IList Data { get; }
        public abstract Boolean IsLaw { get; }
        public abstract bool IsTrend { get; }
        public bool? IsVerifiedByPreds { get; protected set; }
        public bool IsHalfMore
        {
            get 
            {
                int nt = Chain.Count(s => s == Signs.Tau);
                int np = Chain.Count(s => s == Signs.Plus);
                int nm = Chain.Count(s => s == Signs.Minus);
                int n = Chain.Count();
                if (nt + np == n || nt + nm == n)
                {
                    if (np == 0) return nm > nt;
                    if (nm == 0) return np > nt;
                }
                return false;
            }
        }

        public override string ToString()
        {
            return String.Join("->", Chain.Select(ch => ch.ToSymbolString())) + "\t" + Data.Count;
        }
    }
}
