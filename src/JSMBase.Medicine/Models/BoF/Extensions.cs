using BaseElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.Medicine.Models.BoF
{
    public static class Extensions
    {
        public static string GetFilterStringByExt(this DataFormats dtf)
        {
            String desc = Globals.GetDescription(dtf);
            string ext = dtf == DataFormats.old ? "txt" : dtf.ToString();
            return desc + "|" + "*." + ext;
        }
    }
}
