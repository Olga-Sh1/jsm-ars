using JSMBase;
using JSMBase.RNK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.Medicine.Models.CSV
{
    /// <summary>БФ для данных РНК</summary>
    public sealed class CSVBaseOfFacts : BaseOfFacts2<MedicineExample>
    {
        public CSVBaseOfFacts(IEnumerable<MedicineExample> inner): base(inner)
        {

        }
        List<global::JSMBase.JSMDataBase<MedicineExample>> res = null;
        private static readonly string diffIndex = "illness";
        public override IEnumerable<global::JSMBase.JSMDataBase<MedicineExample>> Divide()
        {
            if (res != null) return res;
            res = new List<global::JSMBase.JSMDataBase<MedicineExample>>();
            foreach (var med in Inner)
            {
                JSMDataMedRnk rnk = new JSMDataMedRnk(med, med.ID);
                String val = med[diffIndex].ToString();
                if (val == "1")
                    rnk.Sign = Signs.Plus;
                else if (val == "0")
                    rnk.Sign = Signs.Minus;
                res.Add(rnk);
            }
            List<int> usedInd = new List<int>();
            for(int i = 0; i < MedicineExample.Infos.Length;i++)
            {
                IPropInfo prinf = MedicineExample.Infos[i];
                if (prinf.Name != diffIndex && prinf.Name != "ID")
                    usedInd.Add(i);
            }
            JSMDataMedRnk.UsedIndices = usedInd.ToArray();
            return res;
        }
    }
}
