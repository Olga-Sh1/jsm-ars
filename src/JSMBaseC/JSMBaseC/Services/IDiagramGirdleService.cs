using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBaseC.Services
{
    public interface IDiagramGirdleService
    {
        IEnumerable FindMins(IEnumerable arr);
        IEnumerable FindSuper(IEnumerable alls, Object child);
    }
}
