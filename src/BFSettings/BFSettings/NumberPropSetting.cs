using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BFSettings
{
    [Description("Числовое значение")]
    public sealed class NumberPropSetting : ExcelOneColumnPropSetting
    {
        /// <summary>Дельта</summary>
        [Description("Дельта")]
        [DisplayName("Дельта")]
        public Double Delta { get; set; }
    }
}
