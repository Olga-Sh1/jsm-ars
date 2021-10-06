using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSMBase
{
    public interface IJSMObject : IID
    {
        Object Object { get; }
        IJSMObject Clone(BitArrayBase inner, IContext context);
    }
}
