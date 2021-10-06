using BFSettings;
using JSMBase;
using JSMBase.Medicine.RWServices.Excel;
using JSMBase.RNK;
using Ninject;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XlsxReader4_6;

namespace JSMBase.Medicine.DAL
{
    public sealed class ExcelReader : IReader
    {
        [Inject]
        public ExcelParser parser { get; set; }
        [Inject]
        public SettingsSaverService ssaver { get; set; }
        
        public IList Read(string path)
        {
            string modelName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path), "model.xml");
            FBSetting setts = ssaver.Read(modelName);
            var meds = parser.Read(path, setts);
            int index = 0;
            List<JSMDataMedRnk> rnks = new List<JSMDataMedRnk>();

            foreach(MedicineExample med in meds)
            {
                var j = new JSMDataMedRnk(med, med.ID);
                Object val = j.Inner[global::JSMBase.Medicine.RWServices.Excel.Converters.ToPlusMinusConverter.PROPNAME];
                if (val != null)
                {
                    if (val.ToString() == global::JSMBase.Medicine.RWServices.Excel.Converters.ToPlusMinusConverter.PROPVALPLUS)
                        j.Sign = Signs.Plus;
                    else if (val.ToString() == global::JSMBase.Medicine.RWServices.Excel.Converters.ToPlusMinusConverter.PROPVALMINUS)
                        j.Sign = Signs.Minus;
                }
                else
                    j.Sign = Signs.Tau;
               
                rnks.Add(j);
                index++;
            }
            var msample = meds.ElementAt(0);
            int[] used = new int[msample.PropValues.Length - 1];
            int index1 = 0;
            int index2 = 0;
            foreach(var p in msample.PropValues)
            {
                if (p.PropInfo.Name != global::JSMBase.Medicine.RWServices.Excel.Converters.ToPlusMinusConverter.PROPNAME)
                    used[index2++] = index1;
                index1++;
            }
            JSMDataMedRnk.UsedIndices = used;
            return rnks;
        }
    }
}
