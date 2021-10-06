using JSMBase;
using JSMBaseC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSMSolver.Research
{
    public static class Defaults
    {
        public static Dictionary<Signs, CompareData>[] CreateThresholdRestrict<T, TData>(double persentage, params IEnumerable<TData>[] arr) where TData : JSMDataBase<T>
        {
            if (persentage > 1.0 || persentage < 0.0) throw new ArgumentOutOfRangeException("persentage", "Value should be between 0 and 1");
            Dictionary<Signs, CompareData>[] res = new Dictionary<Signs, CompareData>[arr.Length];
            Signs[] sgs = new Signs[] { Signs.Plus, Signs.Minus, Signs.Null };
            for (int i = 0; i < arr.Length; i++)
            {
                Dictionary<Signs, CompareData> d = new Dictionary<Signs, CompareData>();
                foreach (Signs sg in sgs)
                {
                    AddToDict<T, TData>(sg, persentage, arr[i], d);
                }
                res[i] = d;
            }
            return res;
        }

        public static Dictionary<Signs, CompareData> CreateThresholdRestrict<T, TData>(double persentage, IEnumerable<TData> arr) where TData : JSMDataBase<T>
        {
            Signs[] sgs = new Signs[] { Signs.Plus, Signs.Minus, Signs.Null };
            Dictionary<Signs, CompareData> d = new Dictionary<Signs, CompareData>();
            foreach (Signs sg in sgs)
            {
                AddToDict<T, TData>(sg, persentage, arr, d);
            }
            return d;
        }

        private static void AddToDict<T, TData>(Signs s, double persentage, IEnumerable<TData> datas, Dictionary<Signs, CompareData> d) where TData : JSMDataBase<T>
        {
            int num = (int)Math.Round((double)datas.Count(a => a.Sign == s) * persentage);
            d.Add(s, new CompareData(CompareOperator.MoreOrEquals, num));
        }
    }
}
