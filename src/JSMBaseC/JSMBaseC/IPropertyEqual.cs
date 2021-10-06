using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase
{
    /// <summary>Интерефйс для равенства по признакам</summary>
    public interface IPropertyEqual
    {
        bool IsEqualProp(IPropertyEqual other, int[] prs);
    }
}
