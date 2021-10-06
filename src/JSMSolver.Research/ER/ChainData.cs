using JSMBase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections;

namespace JSMSolver.Research.ER
{
    public class ChainData<T, TData> : ChainDataBase, IEquatable<ChainData<T, TData>> where TData : JSMDataBase<T>
    {
        protected Signs[] _Chain;
        /// <summary>Последовательность знаков</summary>
        public override IEnumerable<Signs> Chain { get { return _Chain; } }
        List<Hypothesis<T, TData>> _Hyps;
        /// <summary>Гипотезы</summary>
        public IEnumerable<Hypothesis<T, TData>> Hyps { get { return _Hyps; } }
        /// <summary>Количество гипотез</summary>
        public int Count { get { return _Hyps.Count; } }
        /// <summary>Длина цепи</summary>
        public int Dimension { get { return _Chain.Length; } }
        protected ChainData()
        {
            _Hyps = new List<Hypothesis<T, TData>>();
        }

        public ChainData(int size) : this()
        {
            _Chain = new Signs[size];
        }

        public ChainData(IEnumerable<Signs> sgs)
            : this()
        {
            _Chain = new Signs[sgs.Count()];
            int j = 0;
            foreach (Signs s in sgs)
            {
                _Chain[j] = s;
                j++;
            }
        }

        public ChainData(IEnumerable<Signs> sgs, List<Hypothesis<T, TData>> hs, bool isVerified)
           : this(sgs)
        {
            _Hyps = hs;
            IsVerifiedByPreds = isVerified;
        }

        public void Add(Hypothesis<T, TData> h)
        {
            _Hyps.Add(h);
        }

        public bool Has(Hypothesis<T, TData> h)
        {
            foreach (var hh in _Hyps)
                if (hh.Body.Equals(h.Body))
                    return true;
            return false;
        }

        public bool IsChain(IEnumerable<Signs> sgs)
        {
            if (sgs.Count() != this.Dimension) return false;
            for (int i = 0; i < this.Dimension; i++)
                if (sgs.ElementAt(i) != this._Chain[i]) return false;
            return true;
        }

        public bool Equals(ChainData<T, TData> other)
        {
            return IsChain(other.Chain);
        }


        public override string ToString()
        {
            return String.Join("->", _Chain.Select(ch => ch.ToSymbolString())) + "\t" + Count;
        }

        /// <summary>Закон</summary>
        public override bool IsLaw
        {
            get { return Chain.All(s => s == Chain.First()); }
        }
        /// <summary>Тенденция</summary>
        public override bool IsTrend
        {
            get
            {
                int ts = Chain.Count(s => s == Signs.Tau);
                if (ts > 0)
                {
                    Signs current = Signs.Tau;
                    for(int i = 0; i < Chain.Count(); i++)
                    {
                        if (current == Signs.Null) return false;
                        if (i > 0 && Chain.ElementAt(i) != Signs.Tau && Chain.ElementAt(i - 1) == Signs.Tau)
                        {
                            current = Chain.ElementAt(i);
                            continue;
                        }
                        if (Chain.ElementAt(i) != current) return false;
                    }
                    return true;
                }
                return false;
            }
        }


        public override IList Data => _Hyps;
    }
}
