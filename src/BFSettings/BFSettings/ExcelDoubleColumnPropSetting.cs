using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BFSettings
{
    /// <summary>Свойство, считываемое из двух колонок в Excel</summary>
    public abstract class ExcelDoubleColumnPropSetting : PropSetting
    {
       
        [DisplayName("Колонка1"), Category("Excel")]
        public String ExcelColumn1 { get; set; }
        [DisplayName("Колонка2"), Category("Excel")]
        public String ExcelColumn2 { get; set; }
    }
}
