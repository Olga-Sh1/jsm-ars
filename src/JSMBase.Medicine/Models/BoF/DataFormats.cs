using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.Medicine.Models.BoF
{
    /// <summary>Формат данных, которые можно вгрузить в систему</summary>
    public enum DataFormats
    {
       
        [Description("База данных SPSS")]
        sav,
        [Description("DMODEL")]
        old,
        [Description("Архивированный DMODEL")]
        jbof
    }
}
