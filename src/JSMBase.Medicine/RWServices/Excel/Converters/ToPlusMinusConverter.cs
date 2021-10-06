using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSMBase.RNK;
using BFSettings;

namespace JSMBase.Medicine.RWServices.Excel.Converters
{
    public sealed class ToPlusMinusConverter : BaseConverter<PropSetting>
    {
        public static readonly string PROPNAME = JSMDataMedRnk.PROPNAME;// "ДСМ-пример";
        public static readonly string PROPVALPLUS = "Положительный (+)";
        public static readonly string PROPVALMINUS = "Отрицательный (-)";
        String colNamePl;
        String colNameMin;
        String tn;
        PropInfoList pil;
        public ToPlusMinusConverter(String colNamePl, String colNameMin, String tn) : base(null)
        {
            this.colNameMin = colNameMin;
            this.colNamePl = colNamePl;
            this.tn = tn;
        }
        public override bool CheckTable(DataTable dt)
        {
            return String.Compare(dt.TableName, tn, true) == 0;
        }

        public override IPropInfo CreateInfo()
        {
            if (pil == null)
            {
                pil = new PropInfoList();
                pil.Name = pil.Description = PROPNAME;
                pil.Values = new string[]
                {
                   PROPVALPLUS,
                    PROPVALMINUS
                };
            }

            return pil;
        }

        public override IPropValue CreateValue(DataRow dr)
        {
            Object val = dr[colNamePl];
            bool b1 = TryConvert<bool>(val);
            String raw = null;
            PropInfoList pil2 = CreateInfo() as PropInfoList;
            if (b1)
                raw = pil2.Values[0];
            else
            {
                Object val2 = dr[colNameMin];
                bool b2 = TryConvert<bool>(val2);
                if (b2)
                    raw = pil2.Values[1];
            }
            return new PropValueList(pil2, raw);
        }
    }
}
