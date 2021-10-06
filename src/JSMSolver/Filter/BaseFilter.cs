using JSMBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMSolver.Filter
{
    public interface IBaseFilter<T, TData> where TData : JSMDataBase<T>
    {
        Boolean FilterPlus(Hypothesis<T, TData> m);
        Boolean FilterMin(Hypothesis<T, TData> m);
    }
}
