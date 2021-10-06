using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBaseC.Services.RW.v2.Serialization
{
    public interface IRestorable
    {
        Object Restore(IList all);
        Object Restore();
        Object RestorePred(IList hyp);
    }
}
