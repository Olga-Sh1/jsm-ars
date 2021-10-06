using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.RNK
{
    /// <summary>Информация о значении одного пункта из списка</summary>
    public sealed class PropValueList : IPropValue
    {
        private string rawData;
        private PropValueList() { }
        public PropValueList(PropInfoList info, string rawData)
        {
            this.Info = info;
            this.rawData = rawData;
            SetDataIndex();
        }
        /// <summary>Информация</summary>
        public PropInfoList Info { get; set; }
        /// <summary>Значение выбранного признака</summary>
        public UInt32 IndexInList { get; set; }
        /// <summary>Находим сходство</summary><param name="other"></param><returns></returns>
        public PropValueList FindSimilarity(PropValueList other)
        {
            if (other == null) throw new ArgumentNullException("other");
            if (!other.Info.Equals(this.Info)) throw new ArgumentException("Значения относятся к разным признакам");
            if (IndexInList != UInt32.MaxValue && this.IndexInList == other.IndexInList)
                return new PropValueList(){ Info = this.Info, IndexInList = this.IndexInList };
            return null;
        }

        /// <summary>Находим сходство</summary><param name="other"></param><returns></returns>
        public PropValueList FindDifference(PropValueList other)
        {
            if (other == null) return new PropValueList() { Info = this.Info, IndexInList = this.IndexInList };
            if (!other.Info.Equals(this.Info)) throw new ArgumentException("Значения относятся к разным признакам");
            if (IndexInList == UInt32.MaxValue || this.IndexInList == other.IndexInList)
                return null;
            return new PropValueList() { Info = this.Info, IndexInList = this.IndexInList };
        }
        #region IPropValue
        IPropInfo IPropValue.PropInfo
        {
            get { return Info; }
        }

        IPropValue IPropValue.FindSimilarity(IPropValue other)
        {
            return this.FindSimilarity(other as PropValueList);
        }
       
        object IPropValue.Value
        {
            get { return IndexInList == UInt32.MaxValue ? null : Info.Values[IndexInList]; }
        }
       
        bool IPropValue.IsEmpty
        {
            get { return IndexInList == UInt32.MaxValue; }
        }

        bool IPropValue.IsEquals(IPropValue other)
        {
            return this.IndexInList == (other as PropValueList).IndexInList;
        }

        bool IPropValue.IsEnclosed(IPropValue other)
        {
           return ((IPropValue)this).IsEquals(other);
        }

        IPropValue IPropValue.FindDifference(IPropValue other)
        {
            return this.FindDifference(other as PropValueList);
        }
        #endregion

        public void SetDataIndex()
        {
            for (int i = 0; i < Info.Values.Length; i++)
                if (Info.Values[i] == rawData)
                {
                    IndexInList = (uint)i;
                    return;
                }
            IndexInList = uint.MaxValue;
        }
    }
}
