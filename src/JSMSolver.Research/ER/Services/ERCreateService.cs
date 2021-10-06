using JSMBase;
using JSMBase.Algorithm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSMSolver.Research.ER.Services
{
    public sealed class ERCreateService
    {
        /// <summary>Построение кодов гипотез</summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TData"></typeparam>
        /// <param name="arr">Массив</param>
        /// <returns>Коды</returns>
        public IEnumerable<ChainData<T, TData>> FindCodes<T, TData>(Dictionary<Signs, IList>[] arr, Dictionary<Signs, IList>[] preds = null, Dictionary<String, Signs> verified = null) where TData : JSMDataBase<T>
        {
            List<ChainData<T, TData>> result = new List<ChainData<T, TData>>();
            Signs[] sgs = new Signs[] { Signs.Plus, Signs.Minus, Signs.Null };
            foreach (Signs sg in sgs)
            {
                List<List<Hypothesis<T, TData>>> allbysgn = new List<List<Hypothesis<T, TData>>>();
                foreach (var arr1 in arr)
                {
                    if (arr1.ContainsKey(sg))
                        allbysgn.Add(arr1[sg] as List<Hypothesis<T, TData>>);
                }
                var d1 = BaseAlgorithms.UnionSets(allbysgn.ToArray());
                foreach (var h in d1)
                {
                    List<Signs> chain = new List<Signs>();
                    foreach (var a in arr)
                    {
                        bool isStopped = false;
                        foreach (var sg2 in sgs)
                        {
                            if (a.ContainsKey(sg2))
                            {
                                var data = a[sg2];
                                foreach (Hypothesis<T, TData> h2 in data)
                                    if (h2.Body.IsEqual(h.Body))
                                    {
                                        chain.Add(sg2);
                                        isStopped = true;
                                        break;
                                    }
                                if (isStopped) break;
                            }
                        }
                        if (!isStopped)
                            chain.Add(Signs.Tau);
                    }
                    ChainData<T, TData> ch = result.FirstOrDefault(r => r.IsChain(chain));
                    if (ch == null)
                    {
                        ch = new ChainData<T, TData>(chain);
                        result.Add(ch);
                    }
                    if (!ch.Has(h))
                        ch.Add(h);
                }
            }

            if (preds != null && verified != null)
            {
                List<ChainData<T, TData>> adds = new List<ChainData<T, TData>>();
                foreach (var ch in result)
                {
                    if (ch.IsLaw || ch.IsTrend)
                    {
                        List<Hypothesis<T, TData>> hsNV = new List<Hypothesis<T, TData>>(ch.Count);
                        for (int j = 0; j < ch.Dimension; j++)
                        {
                            Signs sgs_current = ch.Chain.ElementAt(j);
                            if (sgs_current == Signs.Tau) continue;
                            if (!preds[j].ContainsKey(sgs_current))
                                break;
                            var plst = preds[j][sgs_current];
                            foreach(Hypothesis<T, TData> h1 in ch.Data)
                            {
                                if (hsNV.Contains(h1)) continue;
                                bool b = false;
                                foreach(Prediction<T, TData> hp in plst)
                                {
                                    bool isRight = !verified.ContainsKey(hp.ID) || verified[hp.ID] == sgs_current;
                                    if (h1.Body.IsEnclosed(hp.Body))
                                    {
                                        if (isRight)
                                        {
                                            b = true;
                                        }
                                        else
                                        {
                                            b = false;
                                            break;
                                        }    
                                            
                                    }
                                }
                                if (b && preds[j].ContainsKey(Signs.Null))
                                {
                                    foreach (Prediction<T, TData> hp in preds[j][Signs.Null])
                                    {
                                        if (h1.Body.IsEnclosed(hp.Body))
                                        {
                                            b = false;
                                            break;
                                        }
                                    }
                                }
                                if (!b) hsNV.Add(h1);
                            }
                        }
                        if (hsNV.Count > 0)
                        {
                            ChainData<T, TData> nchain = new ChainData<T, TData>(ch.Chain, hsNV, false);
                            adds.Add(nchain);
                        }
                        var hsv = ch.Data.OfType<Hypothesis<T, TData>>().Except(hsNV).ToList();
                        if (hsv.Count > 0)
                        {
                            ChainData<T, TData> nchain2 = new ChainData<T, TData>(ch.Chain, hsv, true);
                            adds.Add(nchain2);
                        }
                    }
                }
                result.AddRange(adds);
            }

            return result.ToArray();
        }

    }
}
