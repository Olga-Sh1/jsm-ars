using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace JSMBase
{
    public enum Signs
    {
        [Description("Не задано")]
        None,
        [Description("Положительный")]
        Plus,
        [Description("Отрицательный")]
        Minus,
        [Description("Совмещает оба")]
        Null,
        [Description("Неизвестно")]
        Tau
    }
}