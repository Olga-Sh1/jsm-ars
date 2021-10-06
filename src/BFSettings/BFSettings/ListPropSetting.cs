using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BFSettings
{
    /// <summary>Списочное значение</summary>
    [Description("Списочное значение")]
    public class ListPropSetting : ExcelOneColumnPropSetting
    {
        /// <summary>Значения</summary>
        [Description("Значения")]
        [DisplayName("Значения")]
        public String[] Values { get; set; }
    }
}
