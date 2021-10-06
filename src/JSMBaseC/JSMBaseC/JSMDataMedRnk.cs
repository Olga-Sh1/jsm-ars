using JSMBase.RNK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase
{
    public sealed class JSMDataMedRnk : JSMDataBase<MedicineExample>, IObjectable, ICloneable, IPropertyEqual
    {
        public static int[] UsedIndices { get; set; }
        public JSMDataMedRnk() { }
        public JSMDataMedRnk(MedicineExample ob, String id) : base(ob) { this.ID = id; }
        public override JSMDataBase<MedicineExample> Intersect(JSMDataBase<MedicineExample> other)
        {
            JSMDataMedRnk other2 = other as JSMDataMedRnk;
            int len = other2.Inner.PropValues.Length;
            IPropValue[] inn = new IPropValue[len];
            foreach (int i in UsedIndices)
            {
                if (this.Inner.PropValues[i] == null || other.Inner.PropValues[i] == null)
                    inn[i] = null;
                else
                    inn[i] = this.Inner.PropValues[i].FindSimilarity(other.Inner.PropValues[i]);
            }
            MedicineExample ex = new MedicineExample(inn);
            return new JSMDataMedRnk(ex, null);
        }

        public override JSMDataBase<MedicineExample> Difference(JSMDataBase<MedicineExample> other)
        {
            JSMDataMedRnk other2 = other as JSMDataMedRnk;
            int len = other2.Inner.PropValues.Length;
            IPropValue[] inn = new IPropValue[len];
            foreach (int i in UsedIndices)
            {
                if (this.Inner.PropValues[i] == null)
                    inn[i] = null;
                inn[i] = this.Inner.PropValues[i].FindDifference(other.Inner.PropValues[i]);
            }
            MedicineExample ex = new MedicineExample(inn);
            return new JSMDataMedRnk(ex, null);
        }

        public override bool IsEnclosed(JSMDataBase<MedicineExample> other)
        {
            JSMDataMedRnk other2 = other as JSMDataMedRnk;
            int len = other2.Inner.PropValues.Length;
            IPropValue[] inn = new IPropValue[len];
            foreach (int i in UsedIndices)
            {
                if (this.Inner.PropValues[i] == null) continue;
                if (other2.Inner.PropValues[i] == null) return false;
                if (!this.Inner.PropValues[i].IsEnclosed(other2.Inner.PropValues[i])) return false;
            }
            return true;
        }

        public override bool IsEmpty
        {
            get 
            {
                int len = this.Inner.PropValues.Length;
                IPropValue[] inn = new IPropValue[len];
                foreach (int i in UsedIndices)
                {
                    if (this.Inner.PropValues[i] != null && !this.Inner.PropValues[i].IsEmpty) return false;
                }
                return true;
            }
        }

        public override bool IsEqual(JSMDataBase<MedicineExample> other)
        {
            JSMDataMedRnk other2 = other as JSMDataMedRnk;
            int len = other2.Inner.PropValues.Length;
          
            foreach (int i in UsedIndices)
            {
                if (this.Inner.PropValues[i] == null && other.Inner.PropValues[i] == null) continue;
                if (this.Inner.PropValues[i] == null ||other.Inner.PropValues[i] == null) return false;
                if (!this.Inner.PropValues[i].IsEquals(other.Inner.PropValues[i])) return false;
            }
            return true;
        }

        public override bool IsJSMEqual(JSMDataBase<MedicineExample> other)
        {
            return IsEqual(other);
        }

        public override string ToString()
        {
            return ID;
        }

        #region ICloneable
        public object Clone()
        {
            JSMDataMedRnk clone = new JSMDataMedRnk(Inner, this.ID);
            clone.Sign = this.Sign;
            return clone;
        }
        #endregion

        public object Object
        {
            get { return this; }
        }

        public override int CountNonEmptyProps
        {
            get { return this.Inner.PropValues.Count(v => v != null && !v.IsEmpty ); }
        }

        #region IPropertyEqual
        bool IPropertyEqual.IsEqualProp(IPropertyEqual other, int[] prs)
        {
            JSMDataMedRnk o = other as JSMDataMedRnk;
            int[] reals = new int [prs.Length];
            int j = 0;
            foreach (int i in prs)
            {
                for(int k = 0; k < MedicineExample.Infos.Length;k++)
                {
                    if (MedicineExample.Infos[k].Index == i)
                    {
                        reals[j] = k;
                        break;
                    }
                }
                j++;
            }
            foreach (int i in reals)
            {
                if (this.Inner.PropValues[i] == null && o.Inner.PropValues[i] == null) continue;
                if (this.Inner.PropValues[i] == null || o.Inner.PropValues[i] == null) return false;
                if (!this.Inner.PropValues[i].IsEquals(o.Inner.PropValues[i])) return false;
            }
            return true;
        }

        public override JSMDataBase<MedicineExample> Intersect(JSMDataBase<MedicineExample> other, string[] groups)
        {
            JSMDataMedRnk other2 = other as JSMDataMedRnk;
            int len = other2.Inner.PropValues.Length;
            IPropValue[] inn = new IPropValue[len];
            foreach (int i in UsedIndices)
            {
                if (this.Inner.PropValues[i] != null && groups.Contains(this.Inner.PropValues[i].PropInfo.GroupId))
                {
                    if (other.Inner.PropValues[i] == null)
                        inn[i] = null;
                    else
                        inn[i] = this.Inner.PropValues[i].FindSimilarity(other.Inner.PropValues[i]);
                }
                else
                    inn[i] = null;

            }
            MedicineExample ex = new MedicineExample(inn);
            return new JSMDataMedRnk(ex, null);
        }

        public override JSMDataBase<MedicineExample> Sum(JSMDataBase<MedicineExample> other)
        {
            JSMDataMedRnk other2 = other as JSMDataMedRnk;
            if (other2 == null)
            {
                MedicineExample ex2 = new MedicineExample(this.Inner.PropValues);
                return new JSMDataMedRnk(ex2, null);
            }
            int len = other2.Inner.PropValues.Length;
            IPropValue[] inn = new IPropValue[len];
            for(int i = 0;i < len; i++)
            {
                if (this.Inner.PropValues[i] != null) 
                    inn[i] = this.Inner.PropValues[i];
                else
                    inn[i] = other2.Inner.PropValues[i];
            }
            MedicineExample ex = new MedicineExample(inn);
            return new JSMDataMedRnk(ex, null);
        }

        public static readonly string PROPNAME = "ДСМ-пример";
        public override void Complete() 
        { 
            if (MedicineExample.Infos == null)
            {
                MedicineExample.Infos = Inner.PropValues.Select(pv => pv.PropInfo).ToArray();
            }
            if (UsedIndices == null)
            {
                int[] used = new int[Inner.PropValues.Length - 1];
                int index1 = 0;
                int index2 = 0;
                foreach (var p in Inner.PropValues)
                {
                    if (p.PropInfo.Name != PROPNAME)
                        used[index2++] = index1;
                    index1++;
                }
                JSMDataMedRnk.UsedIndices = used;
            }
        }
        #endregion
    }
}
