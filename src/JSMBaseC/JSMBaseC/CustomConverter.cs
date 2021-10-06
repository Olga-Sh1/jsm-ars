using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase
{
    public static class CustomConverter
    {
        /// <summary>Конвертировать</summary>
        /// <param name="b">Объект, который конвертируем</param>
        /// <param name="t">Тип, в который конвертируем</param>
        /// <returns></returns>
        public static Object TryConvert(Object b, Type t)
        {
            if (b == null || Convert.IsDBNull(b))
            {
                if (t.IsValueType)
                {
                    return Activator.CreateInstance(t);
                }
                return null;
            }
            if (t.Equals(typeof(DateTime)) && b.GetType().Equals(typeof(Double)))
                return DateTime.FromOADate((Double)b);
            return Convert.ChangeType(b, t);
        }
    }
}
