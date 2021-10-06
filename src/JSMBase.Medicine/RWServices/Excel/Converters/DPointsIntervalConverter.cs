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
    public sealed class DPointsIntervalConverter : BaseConverter<DoublePointInterval>
    {
        PropInfoInterval pi;
        public DPointsIntervalConverter(DoublePointInterval dpi) : base(dpi)
        {

        }
        public override IPropInfo CreateInfo()
        {
            if (pi == null)
            {
                pi = new PropInfoInterval();
                pi.Name = pi.Description = inner.Name;
                if (inner.Categories != null && inner.Categories.Length > 0) pi.GroupId = inner.Categories[0];
            }
            return pi;
        }

        public override IPropValue CreateValue(DataRow dr)
        {
            Object o1 = dr[inner.ExcelColumn1];
            Object o2 = dr[inner.ExcelColumn2];
            PropValueInterval pvi = new PropValueInterval((PropInfoInterval)CreateInfo(), (double)TryConvert(o1, typeof(double)), (double)TryConvert(o2, typeof(double)));
            return pvi;
        }
    }
}
