using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using JSMBaseC;

namespace JSMBase.Algorithm
{
    public static class BaseAlgorithms
    {
        /// <summary>Алгоритм Норриса</summary><param name="lstObjectsMain">Список</param><returns>Гипотезы</returns>
        public static void AlgorithmNorris<T, TData>(IEnumerable<TData> lstObjectsMain, List<Hypothesis<T, TData>> result, Hypothesis<T, TData> startH, SaveMode sm, CancellationToken ct, Func<Hypothesis<T, TData>, Boolean> filter = null, CustomProgress cp = null) where TData : JSMDataBase<T>
        {
            List<Hypothesis<T, TData>> new_hyps = new List<Hypothesis<T, TData>>();
            uint indexer = 1;
            int num = lstObjectsMain.Count();
            int objectIndex = 0;
            foreach (var o in lstObjectsMain)//Цикл по объектам
            {
                Stopwatch sw = Stopwatch.StartNew();
                Hypothesis<T, TData> hh = new Hypothesis<T, TData>(o);
                hh.Index = indexer++;
                if (startH != null)
                {
                    Hypothesis<T, TData> hNew;
                    var res = hh.Intersect(startH, out hNew);
                    if (res == IntersectResult.CreateNew)
                        hh = hNew;
                    else if (res != IntersectResult.IsNull)
                        hh.ParentList.AddRange(hNew.ParentList.Except(hh.ParentList));
                    else continue;
                }
                    
                List<Hypothesis<T, TData>> buffer = new List<Hypothesis<T, TData>>();
                List<Hypothesis<T, TData>> removedRes = new List<Hypothesis<T, TData>>();
                foreach (Hypothesis<T, TData> h in result)//Цикл по уже порожденным гипотезам
                {
                    Hypothesis<T, TData> h1;
                    var res = h.Intersect(hh, out h1);
                    
                    if (res == IntersectResult.IsNull) continue;
                    if (res == IntersectResult.CreateNew)
                    {
                        if (filter == null || filter(h1))
                        {
                            bool needAdd = true;
                            foreach (var hb in buffer)
                            {
                                if (h1.Body.IsEqual(hb.Body))
                                {
                                    needAdd = false;
                                    hb.AddToParentList(h1.ParentList);
                                    break;
                                }
                            }
                            if (needAdd)
                            {
                                h1.Index = indexer++;
                                buffer.Add(h1);
                            }
                        }
                    }
                    else
                    {
                        h.AddToParentList(h1.ParentList);
                    }
                    if (ct.IsCancellationRequested) return;
                }
                if (ct.IsCancellationRequested) return;
                //Удаляем лишние
                //foreach (var rem in removedRes)
                //    result.Remove(rem);
                //Добавляем буффер
                foreach (var hb in buffer)
                {
                    bool needAdd = true;
                    foreach (var hr in result)
                    {
                        if (hr.Body.IsEqual(hb.Body))
                        {
                            needAdd = false;
                            hr.AddToParentList(hb.ParentList);
                            break;
                        }
                    }
                    if (needAdd)
                    {
                        hb.Index = indexer++;
                        result.Add(hb);
                    }
                }
                //result.AddRange(buffer);
                //Добавляем себя
                result.Add(hh);
                if (ct.IsCancellationRequested) return;
                //Если экономим
                if (!sm.Equals(SaveMode.None))
                {
                    //По количеству родителей
                    if (sm.ThresholdParent != null)
                    {
                        int leave = num - objectIndex;
                        //чистим гипотезы, которые уже точно не дотянут до количества родителей (без второго шага)
                        if (sm.ThresholdParent.Operator == JSMBaseC.CompareOperator.MoreOrEquals && leave < sm.ThresholdParent.Value && (leave % sm.Step == sm.Step - 1))
                        {
                            List<Hypothesis<T, TData>> spared = new List<Hypothesis<T, TData>>();
                            foreach (Hypothesis<T, TData> h in result)
                            {
                                if (!h.IsRealHyp && h.ParentList.Count < sm.ThresholdParent.Value - leave)
                                    spared.Add(h);
                            }
                            foreach (var sp in spared)
                                result.Remove(sp);
                        }
                        //Чистим гипотезы, у которых слишком много родителей
                        if (sm.ThresholdParent.Operator == JSMBaseC.CompareOperator.LessOrEquals)
                        {
                            List<Hypothesis<T, TData>> spared = new List<Hypothesis<T, TData>>();
                            foreach (Hypothesis<T, TData> h in result)
                            {
                                if (!h.IsRealHyp && h.ParentList.Count > sm.ThresholdParent.Value)
                                    spared.Add(h);
                            }
                            foreach (var sp in spared)
                                result.Remove(sp);
                        }
                    }
                   //По количеству признаков
                    if (sm.ThresholdProps != null)
                    {
                        if (sm.ThresholdProps.Operator == JSMBaseC.CompareOperator.MoreOrEquals)
                        {
                            List<Hypothesis<T, TData>> spared = new List<Hypothesis<T, TData>>();
                            foreach (Hypothesis<T, TData> h in result)
                            {
                                if (!h.IsRealHyp && h.Body.CountNonEmptyProps < sm.ThresholdProps.Value)
                                    spared.Add(h);
                            }
                            foreach (var sp in spared)
                                result.Remove(sp);
                        }
                    }
                }

                //
                objectIndex++;
                sw.Stop();
                Debug.WriteLine(o.ID);
                if (cp != null) cp.IncreaseAndReport();
                Debug.WriteLine("secs : "+sw.ElapsedMilliseconds+"; count : "+result.Count);
            }
        }

        #region Пересечения гипотез
        private static List<Hypothesis<T, TData>> IntersectTwoSet<T, TData>(List<Hypothesis<T, TData>> first, List<Hypothesis<T, TData>> second) where TData : JSMDataBase<T>
        {
            List<Hypothesis<T, TData>> result = new List<Hypothesis<T, TData>>(first.Count*second.Count);
            int num1 = first.Count;
            int num2 = second.Count;
            foreach (var h1 in first)
            {
                List<Hypothesis<T, TData>> buffer = new List<Hypothesis<T, TData>>(second.Count);
                foreach (var h2 in second)
                {
                    if (h1.Body.IsEqual(h2.Body))
                    {
                        foreach (var par in h2.ParentList)
                            if (!h1.ParentList.Contains(par))
                                h1.ParentList.Add(par);
                        buffer.Add(h1);
                        continue;
                    }
                    Hypothesis<T, TData> hNew = null;
                    var res = h1.Intersect(h2, out hNew);
                    if (res == IntersectResult.CreateNew)
                    {
                        Boolean needAdd = true;
                        foreach(var hb in buffer)
                            if (hb.Body.IsEqual(hNew.Body))
                            {
                                foreach (var par in hNew.ParentList)
                                    if (!hb.ParentList.Contains(par))
                                        hb.ParentList.Add(par);
                                needAdd = false;
                                break;
                            }
                        if (needAdd) buffer.Add(hNew);
                    }
                }
                result.AddRange(buffer);
            }
            return result;
        }

        /// <summary>Общее пересечение для массивов гипотез</summary><typeparam name="T">Тип</typeparam><param name="arr">Массивы</param><returns>Ядро</returns>
        public static IEnumerable<Hypothesis<T, TData>> IntersectSets<T, TData>(params IEnumerable<Hypothesis<T, TData>>[] arr) where TData : JSMDataBase<T>
        {
            if (arr.Any(a => a == null)) return Enumerable.Empty<Hypothesis<T, TData>>();
            List<Hypothesis<T, TData>> result = new List<Hypothesis<T, TData>>();
            if (arr.Length > 0)
            {
                IEnumerable<Hypothesis<T, TData>> a1 = arr[0];
                int len = arr.Length;
                foreach(var h in a1)
                {
                    bool b = true;
                    for (int i = 1; i < len; i++)
                    {
                        IEnumerable<Hypothesis<T, TData>> a = arr[i];
                        foreach (var h2 in a)
                        {
                            if (h2.Body.Equals(h.Body))
                            {
                                b = true;
                                break;
                            }
                            b = false;
                        }
                        if (!b) break;
                    }
                    if (b) result.Add(h);
                }
            }

            return result;
        }
        /// <summary>Найти множество гипотез встречающихся хотя бы в двух множествах</summary><typeparam name="T">Тип</typeparam><param name="arr">Массив множеств гипотез</param><returns></returns>
        public static IEnumerable<Hypothesis<T, TData>> IntersectSetsTwo<T, TData>(params IEnumerable<Hypothesis<T, TData>>[] arr) where TData : JSMDataBase<T>
        {
            List<Hypothesis<T, TData>> result = new List<Hypothesis<T, TData>>();
            if (arr.Length > 0)
            {
                for (int j = 0; j < arr.Length; j++)
                {
                    IEnumerable<Hypothesis<T, TData>> a1 = arr[j];
                    int len = arr.Length;
                    foreach (var h in a1)
                    {
                        for (int i = j + 1; i < len; i++)
                        {
                            IEnumerable<Hypothesis<T, TData>> a = arr[i];
                            bool b = false;
                            foreach (var h2 in a)
                            {
                                if (h2.Body.Equals(h.Body))
                                {
                                    bool b2 = false;
                                    for (int l = 0; l < result.Count; l++)
                                    {
                                        if (result[l].Body.Equals(h.Body))
                                        {
                                            b2 = true;
                                            break;
                                        }
                                    }
                                    if (!b2)
                                        result.Add(h);
                                    b = true;
                                    break;
                                }
                            }
                            if (b) break;
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>Найти объединение множеств гипотез</summary><typeparam name="T">Тип</typeparam><param name="arr">Массив множеств гипотез</param><returns></returns>
        public static IEnumerable<Hypothesis<T, TData>> UnionSets<T, TData>(params IEnumerable<Hypothesis<T, TData>>[] arr) where TData : JSMDataBase<T>
        {
            List<Hypothesis<T, TData>> result = new List<Hypothesis<T, TData>>();

            if (arr.Length > 0)
            {
               
                int len = arr.Length;
                int[] sp = new int[len];
                for (int j = 0; j < len; j++)
                {
                    int c = 0;
                    var a1 = arr[j];
                    if (a1 == null) continue;
                    foreach (var h in a1)
                    {
                        int len2 = result.Count;
                        bool b = false;
                        for (int i = 0; i < len2; i++)
                        {
                            if (result[i].Body.IsEqual(h.Body))
                            {
                                b = true;
                                c++;
                                break;
                            }
                        }
                        if (!b) result.Add(h);
                    }
                    sp[j] = c;
                }
            }

            return result;
        }
        #endregion
    }
}