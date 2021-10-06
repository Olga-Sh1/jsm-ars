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
    public sealed class NumberConverter : BaseConverter<NumberPropSetting>
    {
        PropInfoNumeric pin;
        public NumberConverter(NumberPropSetting ob) : base(ob)
        {
        }

        public override IPropInfo CreateInfo()
        {
            if (pin == null)
            {
                pin = new PropInfoNumeric();
                pin.Name = pin.Description = inner.Name;
                pin.Delta = inner.Delta;
                if (inner.Categories != null && inner.Categories.Length > 0) pin.GroupId = inner.Categories[0];
            }

            return pin;
        }

        public override IPropValue CreateValue(DataRow dr)
        {
            Object o = dr[inner.ExcelColumn];
            PropValueNumeric pvn = new PropValueNumeric((PropInfoNumeric)CreateInfo(), (double)TryConvert(o, typeof(double)));
            return pvn;
        }
    }
}
