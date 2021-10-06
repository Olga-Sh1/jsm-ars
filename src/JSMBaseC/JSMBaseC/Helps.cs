using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase
{
    public static class Helps
    {
        /// <summary>Знак в строку</summary>
        /// <param name="s">Знак</param>
        /// <returns>Строка</returns>
        public static String ToSymbolString(this Signs s)
        {
            switch (s)
            {
                case Signs.Plus: return "+";
                case Signs.Minus: return "‒";
                case Signs.Tau: return "t";
                case Signs.Null: return "0";
            }
            return null;
        }
        /// <summary>Строка в знак</summary>
        /// <param name="s">Строка</param>
        /// <returns></returns>
        public static Signs FromSymbolString(this String s)
        {
            Char[] arr = s.ToCharArray();
            switch ((int)arr[0])
            {
                case 43: return Signs.Plus;
                case 48: return Signs.Null;
                case 116: return Signs.Tau;
                case 45: return Signs.Minus;
                case 8210: return Signs.Minus;
            }
            //switch (s)
            //{
            //    case "+": return Signs.Plus;
            //    case "‒": return Signs.Minus;
            //    case "t" : return Signs.Tau;
            //    case "0" : return Signs.Null;
            //}
            throw new NotSupportedException();
        }

        public static T[] GetEnumValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToArray();
        }
    }
}
