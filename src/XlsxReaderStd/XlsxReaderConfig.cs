using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XlsxReader4_6
{
    public sealed class XlsxReaderConfig
    {
        public XlsxReaderConfig()
        {
            Headers = new Dictionary<string, int>();
        }
        /// <summary>Количество заголовков по листам</summary>
        public Dictionary<String, int> Headers { get; private set; }
    }
}
