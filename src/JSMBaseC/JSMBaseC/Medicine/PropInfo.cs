using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSMBase.Medicine
{
    public class PropInfo
    {
        public String GroupId { get; set; }
        public Int32 Index { get; set; }
        public PropTypes Type { get; set; }
        public String Name { get; set; }
        public List<TupleEx<Int32, String>> Values { get; set; }

        public PropInfo()
        {
            Values = new List<TupleEx<Int32, String>>();
        }

        public override string ToString()
        {
            return Index + "-" + Name;
        }
    }

    public class JSMStructure : List<PropInfo> { }
}
