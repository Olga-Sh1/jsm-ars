using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBaseC
{
    public class CustomProgress
    {
        IProgress<int> pr;
        int k = 0;
        public CustomProgress(IProgress<int> pr)
        {
            this.pr = pr;
        }

        public void IncreaseAndReport()
        {
            k++;
            if (pr != null) pr.Report(k);
        }
    }
}
