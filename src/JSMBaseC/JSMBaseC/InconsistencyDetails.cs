using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase
{
    public class InconsistencyDetails
    {
        public InconsistencyDetails(Signs opp, Int32 nn)
        {
            this.OppSign = opp;
            this.Num = nn;
        }
        public Signs OppSign { get; private set; }
        public Int32 Num { get; private set; }
    }
}
