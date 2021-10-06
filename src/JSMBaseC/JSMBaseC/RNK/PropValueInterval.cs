using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.RNK
{
    public sealed class PropValueInterval : IPropValue
    {
        private PropValueInterval() { }
        public PropValueInterval(PropInfoInterval info, double dSt, double dEnd)
        {
            this.Info = info;
            MinValue = dSt;
            MaxValue = dEnd;
        }
        /// <summary>Информация</summary>
        public PropInfoInterval Info { get; set; }
        /// <summary>Начальное значение диапазона</summary>
        public Double MinValue { get; set; }
        /// <summary>Конечное значение диапазона</summary>
        public Double MaxValue { get; set; }

        /// <summary>Находим сходство</summary><param name="other"></param><returns></returns>
        public PropValueInterval FindSimilarity(PropValueInterval other)
        {
            if (other == null) throw new ArgumentNullException("other");
            if (!other.Info.Equals(this.Info)) throw new ArgumentException("Значения относятся к разным признакам");
            var mn = Math.Max(this.MinValue, other.MinValue);
            var mx = Math.Min(this.MaxValue, other.MaxValue);

            if (mn <= mx)
                return new PropValueInterval() { Info = this.Info, MinValue = mn, MaxValue = mx };
            return null;
        }

        #region IPropValue
        IPropInfo IPropValue.PropInfo
        {
            get
            {
                return this.Info;
            }
        }

        object IPropValue.Value
        {
            get
            {
                return ToString();
            }
        }

        bool IPropValue.IsEmpty
        {
            get
            { 
                return (MaxValue == MinValue) && Double.IsNaN(MaxValue);
            }
        }

        IPropValue IPropValue.FindSimilarity(IPropValue other)
        {
            return this.FindSimilarity(other as PropValueInterval);
        }

        bool IPropValue.IsEquals(IPropValue other)
        {
            var other2 = other as PropValueInterval;
            return this.MaxValue == other2.MaxValue && this.MinValue == other2.MinValue;
        }

        bool IPropValue.IsEnclosed(IPropValue other)
        {
            var other2 = other as PropValueInterval;
            return this.MaxValue >= other2.MaxValue && this.MinValue <= other2.MinValue;
        }

        IPropValue IPropValue.FindDifference(IPropValue other)
        {
            var other2 = other as PropValueInterval;
            var right = Math.Min(this.MaxValue, this.MinValue);
            if (this.MinValue >= right) return null;
            return new PropValueInterval() { MinValue = this.MinValue, MaxValue = right };
        }
        #endregion

        private static readonly string _format = "{0}-{1}";
        public override string ToString()
        {
            if (MinValue == MaxValue) return MinValue.ToString();
            return String.Format(_format, MinValue, MaxValue);
        }
    }
}
