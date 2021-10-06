using JSMBase;
using JSMBaseC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JSMSolver
{
    /// <summary>Общий интерфейс для решателя</summary>
    public interface ISolver
    {
        void Init(CompareData npl, CompareData nmn, CompareData nppl, CompareData npmn, Addings plus, Addings minus, Dictionary<Signs, IEnumerable<IReasoningType>> types, bool ec, CandidateGroupData[] groups);
        Task JSMSimple(CancellationToken ct, IProgress<int> pr, bool step = false);
        Task JSMNonAthomic(CancellationToken ct, IProgress<Tuple<Signs, String>> pr);

        Dictionary<Signs, IList> Hypotheses { get; }

        Dictionary<Signs, IList> Predictions { get; }

        Dictionary<Signs, IList> Data { get; }

    }
}
