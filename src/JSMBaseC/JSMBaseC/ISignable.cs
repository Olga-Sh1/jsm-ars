using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase
{
    /// <summary>Сущность, которой присвоена оценка</summary>
    public interface ISignable
    {
        /// <summary>Знак</summary>
        Signs Sign { get; }
    }
}
