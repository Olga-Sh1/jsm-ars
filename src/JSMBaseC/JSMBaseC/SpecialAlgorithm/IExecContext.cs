using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.SpecialAlgorithm
{
    public interface IExecContext<T> 
    {
        IContext Context { get; }
        BitArrayBase[] GetCompletenessGroups(BitArrayBase _base);
        BitArrayBase[] GetInCompletenessGroups(BitArrayBase _base);
        BitArrayBase GetOther(BitArrayBase _base);
        /// <summary>Проверка групп структуры на пустоту</summary><param name="data">Данные</param><returns>TRUE - пусто, FALSE - непусто</returns>
        Boolean CheckComplenessIsEmpty(BitArrayBase[] data);
        BitArrayBase Create(BitArrayBase[] compl, BitArrayBase[] incompl, BitArrayBase oth);
    }
}
