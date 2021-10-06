using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.Medicine
{
    public class MedicineContext : IContext
    {
        public MedicineContext(JSMStructure infs)
        {
            Infos = infs;
        }
        public JSMStructure Infos { get; private set; }

        public PropInfo[] GetRealProps()
        {
            return RealIndices.Select(ri => Infos[ri]).ToArray();
        }
        private int[] RealIndices;
        public int[] GetRealIndices()
        {
            return RealIndices;
        }
        private int[] _IndicesUsed;
        public int[] IndicesUsed
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
        
        public int GetRealIndex(PropInfo pi)
        {
            if (_IndicesUsed == null)
                return Infos.IndexOf(pi);
            return Array.IndexOf(_IndicesUsed, pi.Index);
        }

        public int GetRealIndex(int pi)
        {
            if (_IndicesUsed == null)
                return Infos.FindIndex(ind => ind.Index == pi);
            return Array.IndexOf(_IndicesUsed, pi);
        }
    }
}
