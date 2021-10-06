using JSMBase;
using JSMSolver.Research.ER;
using JSMSolver.Research.ER.Services;
using JSMSolver.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JSMSolver.Research.v2
{
    public sealed class Researcher<T, TData> where TData : JSMDataBase<T>
    {
        ERCreateService er = new ERCreateService();
        JSMSolver.Research.v1.Researcher<T, TData> researcher = new JSMSolver.Research.v1.Researcher<T, TData>();

        public List<ResearchData<T, TData>[]> CreateWorlds(ResearchData<T, TData>[] exts)
        {
            var arr = exts.Select(e => e.Data).ToArray();
            List<IEnumerable<TData>[]> perms = createPermutations(arr);
            List<ResearchData<T, TData>[]> result = new List<ResearchData<T, TData>[]>(perms.Count);
            for(int i = 0; i < perms.Count; i++)
            {
                ResearchData<T, TData>[] d = perms[i].Select(p => new ResearchData<T, TData>() { Data = p, Restrictions = exts[i].Restrictions }).ToArray();
                result[i] = d;
            }
            return result;
        }
        public async Task<Dictionary<ERType, IEnumerable<ChainData<T, TData>>>> Count(CancellationToken token, IProgress<int> pr, Addings plus, Addings minus, ResearchData<T, TData>[] exts, Dictionary<String, Signs> verified)
        {
            List<ResearchData<T, TData>[]>  worlds = CreateWorlds(exts);
            ResearchDataWorld<T, TData>[] hypWorlds = new ResearchDataWorld<T, TData>[worlds.Count];
            for(int  i = 0; i < worlds.Count; i++)
            {
                var r = await researcher.CountSolver(token, pr, plus, minus, worlds[i]);
                ResearchDataWorld<T, TData> rdw = new ResearchDataWorld<T, TData>(r);
                hypWorlds[i] = rdw;
            }
            return await Count(token, pr, hypWorlds, verified);
        }

        public async Task<Dictionary<ERType, IEnumerable<ChainData<T, TData>>>> Count(CancellationToken token, IProgress<int> pr, ResearchDataWorld<T, TData>[] worlds, Dictionary<String, Signs> verified) 
        {
            if (worlds.Length == 0) throw new ArgumentException("Нет расширений - no extensions");
            Dictionary<ChainData<T, TData>, Tuple<ERTypeInner, bool>[]> resultInner = new Dictionary<ChainData<T, TData>, Tuple<ERTypeInner, bool>[]>();
            Boolean isMonotonousPl = true;
            Boolean isMonotonousMn = true;
            //For the first (real) world
            ResearchResult<T, TData> rReal = await researcher.Count(token, pr, worlds[0].WorldData, verified);
            isMonotonousPl = rReal.IsSteady(Signs.Plus);
            isMonotonousMn = rReal.IsSteady(Signs.Minus);
            foreach (var dc in rReal.Data)
            {
                if (!dc.IsVerifiedByPreds.GetValueOrDefault()) continue;
                ERTypeInner type = findERInner(dc);
                if (type != ERTypeInner.None)
                {
                    Tuple<ERTypeInner, bool>[] allTypes = new Tuple<ERTypeInner, bool>[worlds.Length];
                    allTypes[0] = Tuple.Create(type, dc.Chain.Last() == Signs.Plus ? isMonotonousPl : isMonotonousMn);
                    resultInner.Add(dc, allTypes);
                }
            }

            for (int i = 1; i < worlds.Length; i++)
            {
                ResearchResult<T, TData>  r = await researcher.Count(token, pr, worlds[i].WorldData, verified);
                isMonotonousPl = r.IsSteady(Signs.Plus);
                isMonotonousMn = r.IsSteady(Signs.Minus);
                foreach (var dc in r.Data)
                {
                    if (!dc.IsVerifiedByPreds.GetValueOrDefault()) continue;
                    if (resultInner.Any(rr => rr.Key.Equals(dc)))
                    {
                        var pair = resultInner.First(rr => rr.Key.Equals(dc));
                        ERTypeInner type = findERInner(dc);
                        if (type != ERTypeInner.None)
                            resultInner[pair.Key][i] = new Tuple<ERTypeInner, bool>(type, dc.Chain.Last() == Signs.Plus ? isMonotonousPl : isMonotonousMn);
                        else
                            resultInner.Remove(pair.Key);
                    }
                }

                List<ChainData<T, TData>> spare = new List<ChainData<T, TData>>();
                foreach (var pair2 in resultInner)
                {
                    if (pair2.Value[i] == null)
                        spare.Add(pair2.Key);
                }

                foreach(var k in spare)
                {
                    resultInner.Remove(k);
                }
            }


            Dictionary<ERType, List<ChainData<T, TData>>> result2 = new Dictionary<ERType, List<ChainData<T, TData>>>();
            foreach (var pair in resultInner)
            {
                ERType t = convertTypes(pair.Value);
                if (!result2.ContainsKey(t)) result2.Add(t, new List<ChainData<T, TData>>());
                result2[t].Add(pair.Key);
            }

            return result2.ToDictionary(k => k.Key, v => v.Value as IEnumerable<ChainData<T, TData>>);
        }

        public async Task<Dictionary<ERType, IEnumerable<ChainDataBase>>> Count(CancellationToken token, IProgress<int> pr, Object[] worlds, Dictionary<String, Signs> verified)
        {
            var d = await Count(token, pr, worlds.Cast<ResearchDataWorld<T, TData>>().ToArray(), verified);
            Dictionary<ERType, IEnumerable<ChainDataBase>> dres = new Dictionary<ERType, IEnumerable<ChainDataBase>>();
            foreach(var p in d)
            {
                dres.Add(p.Key, p.Value);
            }
            return dres;
        }

        private ERTypeInner findERInner(ChainData<T, TData> chain)
        {
            if (chain.IsLaw) return ERTypeInner.EL;
            if (chain.IsTrend)
            {
                if (chain.IsHalfMore) return ERTypeInner.ET;
                return ERTypeInner.WET;
            }
            return ERTypeInner.None;
        }

        private ERType convertTypes(Tuple<ERTypeInner, bool>[] chain)
        {
            /*All the rules for typification
             * ab) 2 и больше EL, нет WET
             * cd) 2 и больше EL, есть WET
             * ef) Один EL, нет WET
             * gh) Один EL, есть WET
             * ij) Только ET
             * kl) ET и WET
             * mn) Только WET
            */
            bool allm = chain.All(c => c.Item2);
            if (chain.All(c => c.Item1 == ERTypeInner.WET))
                if (allm) return ERType.m;
                else return ERType.n;

            if (chain.All(c => c.Item1 == ERTypeInner.ET))
                if (allm) return ERType.i;
                else return ERType.j;

            int countER = chain.Count(c => c.Item1 == ERTypeInner.EL);

            if (countER == 0)
                if (allm) return ERType.k;
                else return ERType.l;

            if (countER == 1)
            {
                if (chain.Any(c => c.Item1 == ERTypeInner.WET))
                {
                    if (allm) return ERType.g;
                    else return ERType.h;
                }
                else
                {
                    if (allm) return ERType.e;
                    else return ERType.f;
                }
            }

            if (chain.Any(c => c.Item1 == ERTypeInner.WET))
            {
                if (allm) return ERType.c;
                else return ERType.d;
            }

            if (allm) return ERType.a;
            return ERType.b;
            


        }

        private List<IEnumerable<TData>[]> createPermutations(IEnumerable<TData>[] datas)
        {
            List<IEnumerable<TData>> diffs = new List<IEnumerable<TData>>();
            diffs.Add(datas[0]);
            for (int i = 1; i < datas.Length; i++)
            {
                diffs.Add(datas[i].Except(datas[i - 1]).ToArray());
            }
            return heapPermutation(diffs.ToArray(), diffs.Count);
        }

        //Generating permutation using Heap Algorithm 
        private List<TArr[]> heapPermutation<TArr>(TArr[] a, int size)
        {
            // if size becomes 1 then prints the obtained permutation 
            if (size == 1)
            {
                TArr[] copy = new TArr[a.Length];
                a.CopyTo(copy, 0);
                return new List<TArr[]>() { copy };
            }
               

            List<TArr[]> result = new List<TArr[]>();
            for (int i = 0; i < size; i++)
            {
                var res = heapPermutation(a, size - 1);
                result.AddRange(res);
                // if size is odd, swap first and last element 
                if (size % 2 == 1)
                {
                    TArr temp = a[0];
                    a[0] = a[size - 1];
                    a[size - 1] = temp;
                }

                // If size is even, swap ith and last element 
                else
                {
                    TArr temp = a[i];
                    a[i] = a[size - 1];
                    a[size - 1] = temp;
                }
            }

            return result;
        }

        private enum ERTypeInner
        {
            None, EL, ET, WET
        }
    }
}
