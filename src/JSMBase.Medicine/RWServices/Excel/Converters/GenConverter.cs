using BFSettings;
using JSMBase.RNK;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.Medicine.RWServices.Excel.Converters
{
    public sealed class GenConverter : BaseConverter<TwoLetterGenPropSetting>
    {
        PropInfoGen2Letter pig;
        public GenConverter(TwoLetterGenPropSetting ob) : base(ob)
        {
        }
        public override IPropInfo CreateInfo()
        {
            if (pig == null)
            {
                pig = new PropInfoGen2Letter();
                pig.Name = pig.Description = inner.Name;
                if (inner.Categories != null && inner.Categories.Length > 0) pig.GroupId = inner.Categories[0];
            }

            return pig;
        }

        public override IPropValue CreateValue(DataRow dr)
        {
            Object o = dr[inner.ExcelColumn];
            PropValueGen2Letter pvn = new PropValueGen2Letter((PropInfoGen2Letter)CreateInfo(), o.ToString());
            return pvn;
        }
    }
}
