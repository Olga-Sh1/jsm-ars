using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace BaseElements
{
    /// <summary>Расширения для атрибутов</summary>
    public static class AttributeExtension
    {
        public static T GetAttribute<T>(MethodInfo mi) where T : Attribute
        {
            return Attribute.GetCustomAttribute(mi, typeof(T)) as T;
        }

        public static T GetAttribute<T>(Enum en) where T : Attribute
        {
            return Attribute.GetCustomAttribute(en.GetType().GetField(en.ToString()), typeof(T)) as T;
        }
    }
}
