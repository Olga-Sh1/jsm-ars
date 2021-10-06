using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.RNK
{
    public sealed class PropValueBit : IPropValue
    {
        public PropInfoBit PropInfo { get; private set; }
        public bool? Value { get; private set; }
        public PropValueBit(PropInfoBit pr, bool? val)
        {
            this.PropInfo = pr;
            Value = val;
        }

        IPropInfo IPropValue.PropInfo
        {
            get
            {
                return this.PropInfo;
            }
        }

        object IPropValue.Value
        {
            get
            {
                return this.Value;
            }
        }

        bool IPropValue.IsEmpty
        {
            get
            {
                return !this.Value.HasValue;
            }
        }

        IPropValue IPropValue.FindSimilarity(IPropValue other)
        {
            if (this.Equals(other)) return new PropValueBit(this.PropInfo, this.Value);
            return null;
        }

        bool IPropValue.IsEquals(IPropValue other)
        {
            PropValueBit _other = other as PropValueBit;
            if (_other == null) return false;
            return this.Value.HasValue && _other.Value.HasValue && (this.Value.Value == _other.Value.Value);
        }

        bool IPropValue.IsEnclosed(IPropValue other)
        {
            return this.Equals(other);
        }

        IPropValue IPropValue.FindDifference(IPropValue other)
        {
            throw new NotImplementedException();
        }
    }
}
