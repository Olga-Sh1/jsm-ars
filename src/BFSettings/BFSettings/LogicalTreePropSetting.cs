using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BFSettings
{
    /// <summary>Логическое значение (три режима)</summary>
    [Description("Логическое значение (три режима)")]
    public sealed class LogicalTreePropSetting : LogicalPropSetting
    {
        /// <summary>Строковые представления значений "Нет"</summary>
        [Description("Строковые представления значений \"Нет\"")]
        [DisplayName("Строковые представления значений \"Нет\"")]
        public String[] StringFalseValues { get; set; }
    }
}
