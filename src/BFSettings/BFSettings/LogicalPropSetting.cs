using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BFSettings
{

    /// <summary>Логическое значение</summary>
    [Description("Логическое значение")]
    public class LogicalPropSetting : ExcelOneColumnPropSetting
    {
        /// <summary>Строковые представления значений "Да"</summary>
        [Description("Строковые представления значений \"Да\"")]
        [DisplayName("Строковые представления значений \"Да\"")]
        public String[] StringTrueValues { get; set; }
    }
}
