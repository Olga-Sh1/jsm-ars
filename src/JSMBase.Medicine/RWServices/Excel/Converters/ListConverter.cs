using BFSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSMBase.RNK;
using System.Data;

namespace JSMBase.Medicine.RWServices.Excel.Converters
{
    public sealed class ListConverter : BaseConverter<ListPropSetting>
    {
        PropInfoList pil;
        public ListConverter(ListPropSetting ob) : base(ob)
        {
        }

        public override IPropInfo CreateInfo()
        {
            if (pil ==null)
            {
                pil = new PropInfoList();
                pil.Name = pil.Description = inner.Name;
                pil.Values = inner.Values;
                if (inner.Categories != null && inner.Categories.Length > 0) pil.GroupId = inner.Categories[0];
            }
           
            return pil;
        }

        public override IPropValue CreateValue(DataRow dr)
        {
            Object ob = dr[inner.ExcelColumn];
            PropValueList pvl = new PropValueList((PropInfoList)CreateInfo(), TryConvert<String>(ob));
            return pvl;
        }
    }
}
