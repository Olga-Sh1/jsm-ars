using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.RNK
{
    public sealed class PropValueId : IPropValue
    {
        public PropInfoId PropInfo { get; set; }
        public String Value { get; set; }
        public PropValueId()
        {

        }
        public PropValueId(PropInfoId pr, string val)
        {
            this.PropInfo = pr;
            Value = val;
        }
        IPropInfo IPropValue.PropInfo
        {
            get { return this.PropInfo; }
        }

        IPropValue IPropValue.FindSimilarity(IPropValue other)
        {
            return null;
        }

        object IPropValue.Value
        {
            get { return this.Value; }
        }

        bool IPropValue.IsEmpty
        {
            get { return true; }
        }

        bool IPropValue.IsEquals(IPropValue other)
        {
            return false;
        }

        bool IPropValue.IsEnclosed(IPropValue other)
        {
            return false;
        }

        IPropValue IPropValue.FindDifference(IPropValue other)
        {
            return null;
        }
    }
}
