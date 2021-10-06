using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase
{
    /// <summary>База для набора гипотез</summary><typeparam name="T"></typeparam>
    public abstract class HypBatchBase<T, TData> where TData : JSMDataBase<T>
    {
        public IEnumerable<T> Collection { get; protected set; }
        public IEnumerable<TData> WrappedCollection { get; protected set; }
        public String Comment { get; set; }
        protected Dictionary<HypCollectionType, Dictionary<Signs, IEnumerable<Hypothesis<T, TData>>>> inner;
        public IEnumerable<Hypothesis<T, TData>> this[HypCollectionType t, Signs s]
        {
            get
            {
                if (!inner.ContainsKey(t)) return null;
                var d = inner[t];
                return d.ContainsKey(s) ? d[s] : null;
            }
        }

        /// <summary>Проверка на противоречивость</summary><param name="other">Массив гипотез</param><returns>Значения</returns>
        public InconsistencyValue CheckInconsistency(HypBatchBase<T, TData> other)
        {
            Dictionary<HypCollectionType, Dictionary<Signs, Double>> result = new Dictionary<HypCollectionType, Dictionary<Signs, Double>>();
            Dictionary<HypCollectionType, Dictionary<Signs, List<InconsistencyDetails>>> details = new Dictionary<HypCollectionType, Dictionary<Signs, List<InconsistencyDetails>>>(); 
            HypCollectionType[] hts = Helps.GetEnumValues<HypCollectionType>();
            Signs[] sngs = new Signs[] { Signs.Plus, Signs.Minus, Signs.Null };
            foreach (HypCollectionType ht in hts)
            {
                Dictionary<Signs, Double> d = new Dictionary<Signs, double>();
                Dictionary<Signs, List<InconsistencyDetails>> det_in = new Dictionary<Signs, List<InconsistencyDetails>>();
                foreach (Signs s in sngs)
                {
                    var arr1 = this[ht, s];
                    if (arr1 == null)
                    {
                        d.Add(s, double.NaN);
                        continue;
                    }
                    //складываем противоречивые
                    List<Hypothesis<T, TData>> lst = new List<Hypothesis<T, TData>>();
                    List<InconsistencyDetails> lstDet = new List<InconsistencyDetails>();
                    //противоположные знаки
                    foreach (Signs s2 in sngs)
                    {
                        if (s == s2) continue;
                        var arr2 = other[ht, s2];
                        if (arr1 == null || arr2 == null) continue;
                        int n = 0;
                        //Сравнение "тел" гипотез
                        foreach (var ob in arr1)
                        {
                            foreach (var ob2 in arr2)
                            {
                                if (ob.Body.IsEqual(ob2.Body))
                                {
                                    if (!lst.Contains(ob))
                                    {
                                        lst.Add(ob);
                                        n++;
                                    }
                                    break;
                                }

                            }
                        }

                        InconsistencyDetails indet = new InconsistencyDetails(s2, n);
                        lstDet.Add(indet);
                    }
                    double dd = (double)lst.Count / (double)arr1.Count();
                   
                    if (dd != 0.0 && dd != Double.NaN)
                    {
                        
                    }
                    d.Add(s, dd);
                    det_in.Add(s, lstDet);
                }
                result.Add(ht, d);
                details.Add(ht, det_in);
            }

            InconsistencyValue res = new InconsistencyValue(result, details);
            return res;
        }
        /// <summary>Проверка на сохранимость знака</summary><param name="other"></param><returns></returns>
        public InconsistencyValue CheckKeepSign(HypBatchBase<T, TData> other)
        {
            Dictionary<HypCollectionType, Dictionary<Signs, Double>> result = new Dictionary<HypCollectionType, Dictionary<Signs, Double>>();
            HypCollectionType[] hts = Helps.GetEnumValues<HypCollectionType>();
            Signs[] sngs = new Signs[] { Signs.Plus, Signs.Minus, Signs.Null };
            foreach (HypCollectionType ht in hts)
            {
                Dictionary<Signs, Double> d = new Dictionary<Signs, double>();
                foreach (Signs s in sngs)
                {
                    //сравниваем знак
                    var h1 = this[ht, s];
                    var h2 = other[ht, s];
                    if (h1 == null || h2 == null)
                    {
                        d.Add(s, double.NaN);
                        continue;
                    }
                    //скаладываем те, которые поменяли знак
                    List<Hypothesis<T, TData>> lst = new List<Hypothesis<T, TData>>();
                    foreach (var hh in h1)
                    {
                        bool hasEquals = false;
                        foreach (var hh2 in h2)
                        {
                            if (hh.Body.Equals(hh2.Body))
                            {
                                hasEquals = true;
                                break;
                            }
                        }
                        if (!hasEquals)
                            lst.Add(hh);
                    }
                    d.Add(s, ((double)lst.Count / (double)h1.Count()));
                }
                result.Add(ht, d);
            }

            InconsistencyValue res = new InconsistencyValue(result);
            return res;
        }
        /// <summary>Заполнение из файла</summary><param name="path">Путь</param>
        public abstract Task Read(String path);
        /// <summary>Запись в файл</summary><param name="path"></param>
        public abstract Task Write(String path);

        public void Add(HypCollectionType t, Signs s, IEnumerable<Hypothesis<T, TData>> hyps)
        {
            if (inner == null) inner = new Dictionary<HypCollectionType, Dictionary<Signs, IEnumerable<Hypothesis<T, TData>>>>();
            if (!inner.ContainsKey(t))
                inner.Add(t, new Dictionary<Signs, IEnumerable<Hypothesis<T, TData>>>()
                    {
                        { s, hyps.ToArray() }
                    });
            else
            {
                Dictionary<Signs, IEnumerable<Hypothesis<T, TData>>> d = inner[t];
                if (!d.ContainsKey(s))
                {
                    d.Add(s, hyps.ToArray());
                }
                else
                {
                    d[s] = d[s].Union(hyps).ToArray();
                }
            }
        }
    }
}
