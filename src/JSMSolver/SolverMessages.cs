using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMSolver
{
    [Flags]
    public enum SolverMessages
    {
        None,
        [Description("Обязательные (+)-гипотезы несовместны")]
        PlusOblNotCompatible,
        [Description("Обязательные (-)-гипотезы несовместны")]
        MinusOblNotCompatible,
    }
}
