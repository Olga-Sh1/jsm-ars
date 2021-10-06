using JSMBase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace JSMBaseC
{
    /// <summary>Кандидат в гипотезы</summary>
    /// <typeparam name="T"></typeparam>
    public sealed class Candidate<T, TData> where TData : JSMDataBase<T>
    {
        private bool IsNullable
        {
            get 
            {
                return parents_pre != null;
            }
        }
        IEnumerable<TData> parents_pre;
        TData inner;
        /// <summary>Список "родителей"</summary>
        public IEnumerable<TData> Parents { get; private set; }
        public Candidate(){ }

        public Candidate(TData parent) : this(new TData[] { parent }, parent) { }
        public Candidate(IEnumerable<TData> parents)
        {
            InitParents(parents);
        }
        private Candidate(IEnumerable<TData> parents, TData inner)
        {
            InitParents(parents);
            this.inner = inner;
        }

        private Candidate(IEnumerable<TData> parents, IEnumerable<TData> parentsother, TData inner)
        {
            InitParents(parents);
            parents_pre = parentsother;
            this.inner = inner;
        }

        public IEnumerable<Candidate<T, TData>> DoStep(CandidateGroupData cgroups)
        {
            List<Candidate<T, TData>> lst = new List<Candidate<T, TData>>();
            String[] groups = cgroups.GroupsNames;
            lst.AddRange(_FillInit(groups));
            foreach (var p in Parents)
            {
                List<Candidate<T, TData>> buffer = new List<Candidate<T, TData>>(lst.Count + 1);
                List<Candidate<T, TData>> remove = new List<Candidate<T, TData>>();
                Candidate<T, TData> c1 = new Candidate<T, TData>(p);
                foreach (var h in lst)
                {
                    if (!h.IsNullable)
                    {
                        var h1 = h.inner.Intersect(c1.inner, groups);
                        if (h1 != null && !h1.IsEmpty) buffer.Add(new Candidate<T, TData>(h.Parents.Union(c1.Parents).ToArray(), h1.Sum(inner) as TData));
                        if (h1.IsEqual(h.inner)) remove.Add(h);
                    }
                    
                }
                buffer.Add(c1);
                Dictionary<Candidate<T, TData>, List<Candidate<T, TData>>> grs = new Dictionary<Candidate<T, TData>, List<Candidate<T, TData>>>();
                foreach (Candidate<T, TData> bf in buffer)
                {
                    bool b = false;
                    foreach (var entry in grs)
                    {
                        if (entry.Key.inner.IsEqual(bf.inner))
                        {
                            entry.Value.Add(bf);
                            b = true;
                            break;
                        }
                    }
                    if (!b) grs.Add(bf, new List<Candidate<T, TData>>() { bf });
                }
                foreach (var entry in grs)
                {
                    Candidate<T, TData> c = entry.Key;
                    var parents = entry.Value.SelectMany(v => v.Parents).Distinct().ToArray();
                    c.InitParents(parents);
                    lst.Add(c);
                }
                foreach (var r in remove)
                    lst.Remove(r);
            }
            return lst.Where(l => l.Parents.Count() > 1).ToArray();
        }

        private IEnumerable<Candidate<T, TData>> _FillInit(params String[] groups)
        {
            if (parents_pre != null)
            {
                TData hMain = parents_pre.First();
                for (int i = 1; i < Parents.Count(); i++)
                {
                    hMain = hMain.Intersect(Parents.ElementAt(i), groups) as TData;
                }
                return new[] { new Candidate<T, TData>(Parents, hMain.Sum(inner) as TData) };
            }
            return Enumerable.Empty<Candidate<T, TData>>();
        }

        public Hypothesis<T, TData> ToHypothesis(int index)
        {
            Hypothesis<T, TData> h = new Hypothesis<T, TData>((uint)index, index.ToString());
            h.Body = inner;
            h.ParentList.AddRange(this.Parents);
            return h;
        }

        public Candidate<T, TData> DoLastStep(CandidateGroupData grouplast)
        {
            var wrps = Parents.ToArray();
            JSMDataBase<T> inter = wrps.First();
            for (int i = 1; i < wrps.Length; i++)
            {
                inter = inter.Intersect(wrps[i], grouplast.GroupsNames);
                if (inter.IsEmpty) return null;
            }
            
            return new Candidate<T, TData>(Parents, inter.Sum(inner) as TData);
        }


        public void InitParents(IEnumerable<TData> parents)
        {
            Parents = parents;
        }
    }
}
