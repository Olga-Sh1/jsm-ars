using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.RNK
{
    public sealed class PropValueDate : IPropValue
    {
        public PropInfoDate PropInfo { get; private set; }
        public DateTime Value { get; private set; }
        public PropValueDate(PropInfoDate pr, DateTime val)
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
                return DateTime.MinValue == this.Value;
            }
        }

        IPropValue IPropValue.FindSimilarity(IPropValue other)
        {
            if (this.Equals(other)) return new PropValueDate(this.PropInfo, this.Value);
            return null;
        }

        bool IPropValue.IsEquals(IPropValue other)
        {
            PropValueDate _other = other as PropValueDate;
            if (_other == null) return false;
            return this.Value.Date == _other.Value.Date;
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
