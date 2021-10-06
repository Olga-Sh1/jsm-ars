using System;
using System.Collections;
using System.Threading.Tasks;

namespace JSMBaseC.Services.RW.v2
{
    public interface IDataService
    {
        Boolean IsReadOnly { get; }
        IList Data { get; }
        Type DataType { get; }
        Type JSMWrapperDataType { get; }
        Type FilterType { get; }

        Task<IList> Open(string path);
    }
}