using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase
{
    [Flags]
    public enum HypFeatures
    {
        None,
        [Description("Пол.")]
        Useful,
        [Description("Об.")]
        Explaining
    }
}
