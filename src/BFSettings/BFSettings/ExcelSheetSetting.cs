using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BFSettings
{
    public sealed class ExcelSheetSetting
    {
        [DisplayName("Название листа")]
        public String Name { get; set; }
        [DisplayName("Количество строк")]
        public int HeadersCount { get; set; }

    }
}
