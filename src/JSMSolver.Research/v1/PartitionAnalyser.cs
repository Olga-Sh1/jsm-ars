using JSMBase;
using JSMSolver.Research.v1.Data;
using JSMSolver.Research.v1.Partitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSMSolver.Research.v1
{
    public sealed class PartitionAnalyser<T, TData, TWr> where TData : JSMDataBase<T> where TWr : BaseWrapper<T, TData>
    {
        public PartitionInputData<T, TData>[][] BuildPartition(Dictionary<String, Signs> verified, params PartitionInputData<T, TData>[] arr)
        {
            if (arr.Length == 0) throw new ArgumentException("Zero length of array");
            BaseWrapper<T, TData>[] arr2 = arr.Select(a => new StrongInconsistencyWrapper<T, TData>(a, verified)).ToArray();
            //var res = CliqueLib.CliqueLib.AllCliques(arr2);
            //return res.Select(r => r.Select(rr => rr.Inner).ToArray()).ToArray();
            throw new NotImplementedException();
        }

        private Object create(PartitionInputData<T, TData>[] a, Dictionary<String, Signs> verified)
        {
            return Activator.CreateInstance(typeof(TWr), a, verified) as BaseWrapper<T, TData>;
        }
    }
    
}
