using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BFSettings
{
    public abstract class PropSetting
    {
        [DisplayName("Индекс"), Category("Общее")]
        public int Index { get; set; }
        [DisplayName("Название"), Category("Общее")]
        public String Name { get; set; }
        [DisplayName("Категории"), Category("Общее")]
        public String[] Categories { get; set; }
        [DisplayName("Имя листа"), Category("Excel")]
        public String ExcelSheetName { get; set; }

        public override string ToString()
        {
            return String.Format("{0} - {1}", Index, Name); 
        }
    }
}
