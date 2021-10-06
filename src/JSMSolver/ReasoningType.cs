using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMSolver
{
    /// <summary>Тип рассуждения</summary>
    public enum ReasoningType
    {
        [Description("Нет")]
        None,
        [Description("Обобщенный")]
        General
    }

    public interface IReasoningType
    {
    }

    public class GeneralReasoning : IReasoningType
    {
        public GeneralReasoning(int _k)
        {
            this.k = _k;
        }
        /// <summary>Порог для тормозов</summary>
        public int k { get; private set; }
    }
}
