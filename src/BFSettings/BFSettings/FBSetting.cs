using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BFSettings
{
    /// <summary>Настройки БФ</summary>
    public class FBSetting
    {
       public GeneralSettings Settings { get; set; }
        /// <summary>Массив настроек по признакам</summary>
        public PropSetting[] Array { get; set; }
    }
}
