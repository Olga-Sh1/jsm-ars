using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace JSMBase.Medicine
{
    public class JSMMedicine : IID
    {
        private bool _NeedFormat = true;
        public int Index { get; private set; }
        public string ID { get; private set; }
        public String Comment { get; private set; }
        public JSMMedicine():this (0, "", "") { }
        public JSMMedicine(int index, String id, bool nf) : this(index, id, "", nf) { }
        public JSMMedicine(int index, String id) : this(index, id, "") { }
        public JSMMedicine(int index, String id, String _comment, bool nf = true)
        {
            Comment = _comment;
            Data = new PropData[Infos.Count];
            ID = id;
            Index = index;
            _NeedFormat = nf;
        }
       
        public static JSMStructure Infos { get; set; }

        public static PropInfo[] GetRealProps()
        {
            return RealIndices.Select(ri => Infos[ri]).ToArray();
        }
        private static int[] _IndicesUsed;
        public static int[] IndicesUsed 
        {
            get { return _IndicesUsed; }
            set
            {
                if (value == null)
                {
                    RealIndices = new int[Infos.Count];
                    for (int i = 0; i < Infos.Count; i++)
                        RealIndices[i] = i;
                }
                else
                {
                    RealIndices = new int[value.Length];
                    for (int k = 0; k < value.Length; k++)
                    {
                        RealIndices[k] = Infos.FindIndex(inf => inf.Index == value[k]);
                    }
                }
                _IndicesUsed = value;
            }
        }
        public static int[] RealIndices { get; private set; }
        public static int GetRealIndex(PropInfo pi)
        {
            if (_IndicesUsed == null)
                return Infos.IndexOf(pi);
            return Array.IndexOf(_IndicesUsed, pi.Index);
            //int index = -1;
            //for (int i = 0; i < IndicesUsed.Length; i++)
            //    if (IndicesUsed[i] == pi.Index)
            //    {
            //        index = i;
            //        break;
            //    }
            //return index;
        }

        public static int GetRealIndex(int npi)
        {
            if (_IndicesUsed == null)
                return Infos.FindIndex(pi => pi.Index == npi);
            return Array.IndexOf(_IndicesUsed, npi);
        }

        public PropData[] Data { get; set; }


        public override string ToString()
        {
            if (_NeedFormat)
                return String.Format("Пациент {0} - {1}", Index, ID);
            return ID;
        }

        public JSMMedicine Clone()
        {
            JSMMedicine med = new JSMMedicine(Index, ID, Comment);
            for (int i = 0; i < Data.Length; i++)
                med.Data[i] = Data[i];
            return med;
        }

      
    }
}
