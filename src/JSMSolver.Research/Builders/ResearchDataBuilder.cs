using JSMBase;
using JSMBase.Medicine;
using JSMBase.Medicine.Models;
using JSMBase.Medicine.RWServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSMSolver.Research.Builders
{
    public sealed class ResearchDataBuilder<T, TData> where TData : JSMDataBase<T>
   { 
        List<IEnumerable<TData>> bofs = new List<IEnumerable<TData>>();
        double threshold = double.NaN;
        public ResearchDataBuilder<T, TData> Add(IEnumerable<TData> bof)
        {
            bofs.Add(bof);
            return this;
        }

        public ResearchDataBuilder<T, TData> AddThreshold(double d)
        {
            threshold = d;
            return this;
        }
        public List<ResearchData<T, TData>> Build()
        {
            List<ResearchData<T, TData>> datas = new List<ResearchData<T, TData>>();
            foreach (var bof in bofs)
            {
                var dt = new ResearchData<T, TData>();
                dt.Data = bof;
                if (!double.IsNaN(threshold))
                {
                    dt.Restrictions = Defaults.CreateThresholdRestrict<T, TData>(threshold, dt.Data); ;
                }
                
                datas.Add(dt);
            }
           
            return datas;
        }
    }
}
