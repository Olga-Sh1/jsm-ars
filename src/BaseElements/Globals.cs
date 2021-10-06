using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseElements
{
    public static class Globals
    {
        public static T[] CreateArrayExceptOne<T>(IEnumerable<T> arr, T main)
        {
            if (!arr.Contains(main)) throw new IndexOutOfRangeException("Экземпляр объекта не содержится в массиве");
            T[] Arr = new T[arr.Count() - 1];
            int ii = 0;
            foreach (T ex in arr)
            {
                if (!ex.Equals(main))
                {
                    Arr[ii] = ex;
                    ii++;
                }
            }
            return Arr;
        }

        public static String GetDescription(Enum val)
        {
            DescriptionAttribute attr = AttributeExtension.GetAttribute<DescriptionAttribute>(val);
            if (attr != null)
                return attr.Description;
            return val.ToString();
        }

        public static T[] GetEnumValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToArray();
        }

        public static KeyValuePair<T, String>[] GetEnumKeyValuePairs<T>()
        {
            var arr = Enum.GetValues(typeof(T));
            List<KeyValuePair<T, String>> res = new List<KeyValuePair<T, string>>();
            foreach (Object t in arr)
                res.Add(new KeyValuePair<T,string>((T)t, GetDescription((Enum)t)));
            return res.ToArray();
        }

        public static KeyValuePair<Enum, String>[] GetEnumKeyValuePairs(Type t)
        {
            var arr = Enum.GetValues(t);
            List<KeyValuePair<Enum, String>> res = new List<KeyValuePair<Enum, string>>();
            foreach (Enum tt in arr)
                res.Add(new KeyValuePair<Enum, string>(tt, GetDescription(tt)));
            return res.ToArray();
        }

       
    }
}
