using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase
{
    public interface IHypothesis<T, TData> : IJSMObject where TData : JSMDataBase<T>
    {
        UInt32 Index { get; }
        List<TData> ParentList { get; }
        TData Body { get; }
    }
}
