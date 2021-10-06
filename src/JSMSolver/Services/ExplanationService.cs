using JSMBase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMSolver.Services
{
    /// <summary>Результаты теста на обощенный метод и объясняемости</summary>
    public class ExplanationService<T, TData> where TData : JSMDataBase<T>
    {
        readonly Dictionary<Signs, ReadOnlyCollection<TData>> _data;
        readonly Dictionary<Signs, IEnumerable<Hypothesis<T, TData>>> _hyps;
        public ExplanationService(Dictionary<Signs, ReadOnlyCollection<TData>> data, Dictionary<Signs, IEnumerable<Hypothesis<T, TData>>> hyps)
        {
            _data = data;
            _hyps = hyps;
        }

        public ExplanationService(Dictionary<Signs, IList> data, Dictionary<Signs, IList> hyps)
        {
            _data = new Dictionary<Signs, ReadOnlyCollection<TData>>();
            foreach (var p in data)
                _data.Add(p.Key, p.Value as ReadOnlyCollection<TData>);
            _hyps = new Dictionary<Signs, IEnumerable<Hypothesis<T, TData>>>();
            foreach (var p in hyps)
                _hyps.Add(p.Key, p.Value as IEnumerable<Hypothesis<T, TData>>);
        }
        /// <summary>Объясняемость</summary><returns></returns>
        public Dictionary<Signs, Div>  GetExplanation()
        {
            Dictionary<Signs, Div> ds = new Dictionary<Signs, Div>();
            ds.Add(Signs.Plus, explanationCoeff(_hyps[Signs.Plus], _data[Signs.Plus]));
            ds.Add(Signs.Minus, explanationCoeff(_hyps[Signs.Minus], _data[Signs.Minus]));
            return ds;
        }
        /// <summary>Значение теста на обобщенный метод</summary><returns></returns>
        public Dictionary<Signs, Div> GetGenTest()
        {
            Dictionary<Signs, Div> ds = new Dictionary<Signs, Div>();
            ds.Add(Signs.Plus, explanationCoeff(_hyps[Signs.Minus], _data[Signs.Plus]));
            ds.Add(Signs.Minus, explanationCoeff(_hyps[Signs.Plus], _data[Signs.Minus]));
            return ds;
        }

        /// <summary>Разделить массив на подмассивы</summary>
        /// <remarks>Первый массив - для обощенного метода, второй массив - для запрета на контрпример</remarks>
        /// <returns></returns>
        public Dictionary<Signs, IEnumerable<TData>>[] DivideData()
        {
            var pls = divide(_hyps[Signs.Minus], _data[Signs.Plus]);
            var mns = divide(_hyps[Signs.Plus], _data[Signs.Minus]);
            Dictionary<Signs, IEnumerable<TData>>[] arr = new Dictionary<Signs, IEnumerable<TData>>[2];
            arr[0] = new Dictionary<Signs, IEnumerable<TData>>()
            {
                {  Signs.Plus, pls.First() },
                {  Signs.Minus, mns.First() }
            };
            arr[1] = new Dictionary<Signs, IEnumerable<TData>>()
            {
                {  Signs.Plus, pls.Last() },
                {  Signs.Minus, mns.Last() }
            };
            return arr;
        }

        private static IEnumerable<TData>[] divide(IEnumerable<Hypothesis<T, TData>> hyps, IEnumerable<TData> data)
        {
            IEnumerable<TData>[] res = new IEnumerable<TData>[2];
            List<TData> lst1 = new List<TData>();
            List<TData> lst2 = new List<TData>();
            foreach (var pl in data)
            {
                bool b = false;
                foreach (var hh in hyps)
                    if (hh.Body.IsEnclosed(pl))
                    {
                        lst1.Add(pl);
                        b = true;
                        break;
                    }
                if (!b)
                    lst2.Add(pl);
            }
            res[0] = lst1;
            res[1] = lst2;
            return res;
        }

        private static Div explanationCoeff(IEnumerable<Hypothesis<T, TData>> hyps, IEnumerable<JSMDataBase<T>> data)
        {
            int counter = 0;
            foreach (var pl in data)
            {
                foreach (var hh in hyps)
                    if (hh.Body.IsEnclosed(pl))
                    {
                        counter++;
                        break;
                    }
            }
            return new Div(counter, data.Count());
        }

    }

    /// <summary>Значения деления</summary>
    public class Div
    {
        public Div(int nom, int denom)
        {
            Nominator = nom;
            Denominator = denom;
            Value = (double)Nominator / (double)Denominator;
        }
        /// <summary>Числитель</summary>
        public int Nominator { get; private set; }
        /// <summary>Знаменатель</summary>
        public int Denominator { get; private set; }
        /// <summary>Значение </summary>
        public double Value { get; private set; }
    }
}
