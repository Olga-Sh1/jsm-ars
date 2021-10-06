using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBaseC
{
    public sealed class CompareData
    {
        public CompareData(CompareOperator op, Int32 val)
        {
            Operator = op;
            Value = val;
        }
        public CompareOperator Operator { get; private set; }
        public Int32 Value { get; private set; }
    }
}
