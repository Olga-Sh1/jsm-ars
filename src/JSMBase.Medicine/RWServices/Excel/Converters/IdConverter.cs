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
    public class IdConverter : BaseConverter<BFSettings.IdPropSetting>
    {
        PropInfoId pii;
        public IdConverter(IdPropSetting ob) : base(ob)
        {
        }

        public override IPropInfo CreateInfo()
        {
            if (pii == null)
            {
                pii = new PropInfoId();
                pii.Name = pii.Description = inner.Name;
            }
            return pii;
        }

        public override IPropValue CreateValue(DataRow dr)
        {
            Object ob = dr[inner.ExcelColumn];
            PropValueId pvi = new PropValueId((PropInfoId)CreateInfo(), TryConvert<String>(ob));
            return pvi;
        }
    }
}
