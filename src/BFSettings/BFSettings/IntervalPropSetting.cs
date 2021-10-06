using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BFSettings
{
    /// <summary>Интервал</summary>
    [Description("Число в интервал")]
    public class IntervalPropSetting : ExcelOneColumnPropSetting
    {
        /// <summary>Ключевые точки</summary>
        [Description("Ключевые точки")]
        [DisplayName("Ключевые точки")]
        public double[] Points { get; set; }
    }
}
