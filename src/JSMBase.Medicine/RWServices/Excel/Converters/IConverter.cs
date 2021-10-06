using JSMBase.RNK;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.Medicine.RWServices.Excel.Converters
{
    public interface IConverter
    {
        Boolean CheckTable(DataTable dt);
        IPropInfo CreateInfo();
        IPropValue CreateValue(DataRow dr);
    }
}
