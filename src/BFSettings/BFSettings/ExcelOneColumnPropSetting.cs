using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BFSettings
{
    /// <summary>Свойство, считываемое из одной колонки в Excel</summary>
    public abstract class ExcelOneColumnPropSetting : PropSetting
    {
        [DisplayName("Колонка"), Category("Excel")]
        public String ExcelColumn { get; set; }
    }
}
