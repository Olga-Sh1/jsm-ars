using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BFSettings
{
    public sealed class GeneralSettings
    {
        [DisplayName("Заголовки"), Description("Количество заголовков по листам"), Category("Excel")]
        public ExcelSheetSetting[] Headers { get; set; }
        [DisplayName("Имя листа"), Description("Имя листа для (+)/(-)-примеров"), Category("БФ")]
        public String ExcelSheetName { get; set; }
        [DisplayName("(+)-примеры"), Description("Выражение для (+)-примеров"), Category("БФ")]
        public String ExpressionPlus { get; set; }
        [DisplayName("(-)-примеры"), Description("Выражение для (-)-примеров"), Category("БФ")]
        public String ExpressionMinus { get; set; }
    }
}
