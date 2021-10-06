using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.RNK
{
    public class PropValueString : IPropValue
    {
        public PropInfoString PropInfo { get; private set; }
        public String Value { get; private set; }
        public PropValueString(PropInfoString pr, string val)
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
            PropValueString _other = other as PropValueString;
            if (_other == null) return null;
            if (this.Value == _other.Value)
                return new PropValueString(this.PropInfo, this.Value);
            return null;
        }

        object IPropValue.Value
        {
            get { return this.Value; }
        }

        bool IPropValue.IsEmpty
        {
            get { return String.IsNullOrEmpty(this.Value); }
        }

        bool IPropValue.IsEquals(IPropValue other)
        {
            PropValueString _other = other as PropValueString;
            if (_other == null) return false;
            return this.Value == _other.Value;
        }

        bool IPropValue.IsEnclosed(IPropValue other)
        {
            return ((IPropValue)this).IsEquals(other);
        }

        IPropValue IPropValue.FindDifference(IPropValue other)
        {
            return ((IPropValue)this).FindSimilarity(other);
        }
    }
}
