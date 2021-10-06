using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.RNK
{
    /// <summary>Информация о значении числового признака</summary>
    public sealed class PropValueNumeric : IPropValue
    {
        private PropValueNumeric() { }
        public PropValueNumeric(PropInfoNumeric info, double d)
        {
            this.Info = info;
            MinValue = MaxValue = d;
        }
        /// <summary>Вернее значение интервала</summary>
        public Double MaxValue { get; set; }

        /// <summary>Нижнее значение интервала</summary>
        public Double MinValue { get; set; }

        /// <summary>Информация</summary>
        public PropInfoNumeric Info { get; set; }

        /// <summary>Находим сходство</summary><param name="other"></param><returns></returns>
        public PropValueNumeric FindSimilarity(PropValueNumeric other)
        {
            if (other == null) throw new ArgumentNullException("other");
            if (!other.Info.Equals(this.Info)) throw new ArgumentException("Значения относятся к разным признакам");
            var mn = Math.Min(this.MinValue, other.MinValue);
            var mx = Math.Max(this.MaxValue, other.MaxValue);

            if (Math.Abs(mx - mn) <= this.Info.Delta)
                return new PropValueNumeric() { Info = this.Info, MinValue = mn, MaxValue = mx };
            return null;
        }

        #region IPropValue
        IPropInfo IPropValue.PropInfo
        {
            get { return Info; }
        }

        IPropValue IPropValue.FindSimilarity(IPropValue other)
        {
            return this.FindSimilarity(other as PropValueNumeric);
        }

        object IPropValue.Value
        {
            get { return ToString(); }
        }

        bool IPropValue.IsEmpty
        {
            get { return (Double.IsNaN(MinValue) || Double.IsNaN(MaxValue)); }
        }

        public bool IsEquals(IPropValue other)
        {
            var other2 = other as PropValueNumeric;
            /*
            var mn = Math.Min(this.MinValue, other2.MinValue);
            var mx = Math.Max(this.MaxValue, other2.MaxValue);

            return Math.Abs(mx - mn) <= this.Info.Delta;
            */
            return this.MinValue == other2.MinValue &&
                this.MaxValue == other2.MaxValue;
        }

        public bool IsEnclosed(IPropValue other)
        {
            var other2 = other as PropValueNumeric;
            return this.MaxValue >= other2.MaxValue && this.MinValue <= other2.MinValue;
            //return !(this.MaxValue < other2.MinValue || this.MinValue > other2.MaxValue);
        }

        public IPropValue FindDifference(IPropValue other)
        {
            if (other == null)
                return new PropValueNumeric() { MinValue = this.MinValue, MaxValue = this.MaxValue };
            var other2 = other as PropValueNumeric;
            var right = Math.Min(this.MaxValue, this.MinValue);
            if (this.MinValue >= right) return null;
            return new PropValueNumeric() { MinValue = this.MinValue, MaxValue = right };
        }
        #endregion

        private static readonly string _format = "{0}-{1}";
        public override string ToString()
        {
            if (((IPropValue)this).IsEmpty) return "-";
            if (MinValue == MaxValue) return MinValue.ToString();
            return String.Format(_format, MinValue, MaxValue);
        }
    }
}
