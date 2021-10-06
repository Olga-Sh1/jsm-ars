using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using JSMBase.Medicine;

namespace JSMBase
{
    public class JSMBase : JSMDataBase<JSMMedicine>, IJSMObject, IObjectable
    {
        private JSMBase() { }
        public JSMBase(JSMMedicine med) : base(med) { this.ID = med.ID; }
        public JSMBase(String id, BitArrayBase bab)
        {
            Arrays = bab;
            ID = id;
        }

        public BitArrayBase Arrays { get; protected set; }

        public override Boolean IsEmpty
        {
            get { return Arrays.IsEmpty(); }
        }

        public override bool Equals(object obj)
        {

            if (obj == null) return false;
            JSMBase other = obj as JSMBase;
            if (obj != null && other == null) return false;
            if (other == null) throw new ArgumentNullException();
            return EqualsInner(other);

        }

        internal Boolean EqualsInner(JSMBase other)
        {
            if (other == null) return false;
            return this.Arrays.Equals(other.Arrays);
        }

        public override JSMDataBase<JSMMedicine> Intersect(JSMDataBase<JSMMedicine> other)
        {
            return new JSMBase() { Arrays = this.Arrays.Intersect((other as JSMBase).Arrays) };
        }

        public override JSMDataBase<JSMMedicine> Difference(JSMDataBase<JSMMedicine> other)
        {
            return new JSMBase() { Arrays = this.Arrays.Difference((other as JSMBase).Arrays) };
        }

        public override bool IsEnclosed(JSMDataBase<JSMMedicine> other)
        {
            return this.Arrays.IsEnclosed((other as JSMBase).Arrays);
        }

        public override bool IsEqual(JSMDataBase<JSMMedicine> other)
        {
            return EqualsInner(other as JSMBase);
        }

        public void ConvertToBitArray(IContext context = null)
        {
            Arrays = new BitArrayBase(GetArrays(context));
        }
        public void ConvertFromBitArray(IContext context = null)
        {
            SetArrays(Arrays.GetArrays(), context);
        }

        //Кодирование
        protected Boolean[][] GetArrays(IContext context)
        {
            MedicineContext ctxt = context as MedicineContext;
            int[] _RealIndices = ctxt == null ? JSMMedicine.RealIndices : ctxt.GetRealIndices();
            if (_RealIndices != null)
            {
                Boolean[][] arr = new Boolean[_RealIndices.Length][];
                for (int i = 0; i < _RealIndices.Length; i++)
                {
                    Boolean[] in_arr = new Boolean[JSMMedicine.Infos[_RealIndices[i]].Values.Count * 2];
                    for (int j = 0; j < JSMMedicine.Infos[_RealIndices[i]].Values.Count; j++)
                    {
                        if (Inner.Data[_RealIndices[i]].PlusValuesIndeces.Contains(j))
                        {
                            in_arr[j * 2] = true;
                            in_arr[j * 2 + 1] = false;
                        }
                        else if (Inner.Data[_RealIndices[i]].MinusValuesIndeces.Contains(j))
                        {
                            in_arr[j * 2] = false;
                            in_arr[j * 2 + 1] = true;
                        }
                    }
                    arr[i] = in_arr;
                }
                return arr;
            }
            else
            {
                Boolean[][] arr = new Boolean[JSMMedicine.Infos.Count][];
                for (int i = 0; i < JSMMedicine.Infos.Count; i++)
                {
                    Boolean[] in_arr = new Boolean[JSMMedicine.Infos[i].Values.Count * 2];
                    for (int j = 0; j < JSMMedicine.Infos[i].Values.Count; j++)
                    {
                        if (Inner.Data[i] != null)
                        {
                            if (Inner.Data[i].PlusValuesIndeces.Contains(j))
                            {
                                in_arr[j * 2] = true;
                                in_arr[j * 2 + 1] = false;
                            }
                            else if (Inner.Data[i].MinusValuesIndeces.Contains(j))
                            {
                                in_arr[j * 2] = false;
                                in_arr[j * 2 + 1] = true;
                            }
                        }
                    }
                    arr[i] = in_arr;
                }
                return arr;
            }
        }
        //Декодирование
        protected void SetArrays(Boolean[][] arr, IContext context)
        {
            MedicineContext ctxt = context as MedicineContext;
            int[] _RealIndices = ctxt == null ? JSMMedicine.RealIndices : ctxt.GetRealIndices();
            JSMStructure _Infos = ctxt == null ? JSMMedicine.Infos : ctxt.Infos;
            if (Inner == null)
                Inner = new JSMMedicine();
            Inner.Data = new PropData[_Infos.Count];
            if (_RealIndices != null)
            {
                for (int i = 0; i < _RealIndices.Length; i++)
                {
                    int real_index = _RealIndices[i];
                    Boolean[] cur = arr[i];
                    List<Int32> pluses = new List<int>();
                    List<Int32> minuses = new List<int>();
                    for (int j = 0; j < _Infos[real_index].Values.Count; j++)
                    {
                        Boolean b1 = cur[j * 2];
                        Boolean b2 = cur[j * 2 + 1];
                        if (b1 | b2)
                        {
                            if (b1)
                                pluses.Add(j);
                            else
                                minuses.Add(j);
                        }
                    }
                    Inner.Data[real_index] = new PropData(_Infos[real_index], pluses.ToArray(), minuses.ToArray());
                }
            }
            else
            {
                for (int i = 0; i < _Infos.Count; i++)
                {
                    Boolean[] cur = arr[i];
                    List<Int32> pluses = new List<int>();
                    List<Int32> minuses = new List<int>();
                    for (int j = 0; j < _Infos[i].Values.Count; j++)
                    {
                        Boolean b1 = cur[j * 2];
                        Boolean b2 = cur[j * 2 + 1];
                        if (b1 | b2)
                        {
                            if (b1)
                                pluses.Add(j);
                            else
                                minuses.Add(j);
                        }
                    }
                    Inner.Data[i] = new PropData(_Infos[i], pluses.ToArray(), minuses.ToArray());
                }
            }
        }


        public Boolean HasIntersection(int ii, JSMBase other)
        {
            return this.Arrays.HasIntersection(ii, other.Arrays);
        }

        public Boolean HasIntersection(JSMBase other)
        {
            return this.Arrays.HasIntersection(other.Arrays);
        }

        public Boolean IsEmptyProp(int ii)
        {
            return this.Arrays[ii] == 0;
        }

        public int CountNotEmpty()
        {
            int r = 0;
            int l = Arrays.GetArrays().Length;
            for (int i = 0; i < l; i++)
                if (!IsEmptyProp(i))
                    r++;
            return r;
        }

        public Boolean IsEqual(int ii, JSMBase other)
        {
            return this.Arrays[ii] == other.Arrays[ii];
        }


        internal void Fill(String id, BitArrayBase _base)
        {
            this.ID = id;
            this.Arrays = _base;
        }

        #region IJSMObject members
        public Object Object
        {
            get { return this; }
        }

        public override int CountNonEmptyProps
        {
            get { return Arrays.CountNonEmpty(); }
        }

        public JSMBase Clone(BitArrayBase inner, IContext context)
        {
            JSMMedicine med = new JSMMedicine(Inner.Index, this.ID, "");
            JSMBase b = new JSMBase(med);
            b.Arrays = inner;
            b.ConvertFromBitArray(context);
            return b;
        }

        #endregion

        public override bool IsJSMEqual(JSMDataBase<JSMMedicine> other)
        {
            return IsEqual(other);
        }

        public JSMBase Clone()
        {
            return new JSMBase(this.Inner.Clone());
        }

        IJSMObject IJSMObject.Clone(BitArrayBase inner, IContext context)
        {
            return this.Clone(inner, context);
        }

        public override string ToString()
        {
            return Inner.ToString();
        }

        public override JSMDataBase<JSMMedicine> Intersect(JSMDataBase<JSMMedicine> other, string[] groups)
        {
            List<int> arr = new List<int>(JSMMedicine.Infos.Count);
            for (int i = 0; i < JSMMedicine.Infos.Count; i++)
                if (groups.Contains(JSMMedicine.Infos[i].GroupId)) arr.Add(i);
            return new JSMBase() { Arrays = this.Arrays.Intersect((other as JSMBase).Arrays, arr.ToArray()) };
        }

        public override JSMDataBase<JSMMedicine> Sum(JSMDataBase<JSMMedicine> other)
        {
            if (other == null)
                return new JSMBase() { Arrays = this.Arrays };
            return new JSMBase() { Arrays = this.Arrays.Sum((other as JSMBase).Arrays) };
        }
    }
}
