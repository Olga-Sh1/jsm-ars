using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BFSettings;
using JSMBase.RNK;

namespace JSMBase.Medicine.RWServices.Excel.Converters
{
    public sealed class LogicalThreeConverter : BaseConverter<BFSettings.LogicalTreePropSetting>
    {
        PropInfoList pil;
        private static string YES = "Да";
        private static string NO = "Нет";
        public LogicalThreeConverter(LogicalTreePropSetting ob) : base(ob)
        {
        }

        public override IPropInfo CreateInfo()
        {
            if (pil == null)
            {
                pil = new PropInfoList();
                pil.Name = pil.Description = inner.Name;
                pil.Values = new string[]
                {
                    YES, NO
                };
                if (inner.Categories != null && inner.Categories.Length > 0) pil.GroupId = inner.Categories[0];
            }
            return pil;
        }

        public override IPropValue CreateValue(DataRow dr)
        {
            Object ob = dr[inner.ExcelColumn];
            String tr = TryConvert<String>(ob);
            String val = null;
            if (inner.StringTrueValues.Contains(tr))
                val = YES;
            else if (inner.StringFalseValues.Contains(tr))
                val = NO;
            PropValueList pvl = new PropValueList((PropInfoList)CreateInfo(), val);
            return pvl;
        }
    }
}
