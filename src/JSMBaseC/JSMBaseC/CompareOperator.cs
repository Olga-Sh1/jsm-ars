using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBaseC
{
    /// <summary>Оператор сравнения</summary>
    public enum CompareOperator
    {
        /// <summary>Не задано</summary>
        [Description("Не задано")]
        None,
        /// <summary>Больше или равно</summary>
        [Description("Больше или равно")]
        MoreOrEquals,
        /// <summary>Меньше или равно</summary>
        [Description("Меньше или равно")]
        LessOrEquals
    }
}
