using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.RNK
{
    /// <summary>Информация о значении признака</summary>
    public interface IPropValue
    {
        /// <summary>Информация</summary>
        IPropInfo PropInfo { get; }
        /// <summary>Найти сходство</summary><param name="other">Пара для сходства</param><returns>Сходство</returns>
        IPropValue FindSimilarity(IPropValue other);
        /// <summary>Значение</summary>
        Object Value { get; }
        /// <summary>Пустой</summary>
        bool IsEmpty { get; }
        /// <summary>Равенство</summary><param name="other">Другой</param><returns></returns>
        bool IsEquals(IPropValue other);
        /// <summary>Проверка включения</summary><param name="other">Другой</param><returns></returns>
        bool IsEnclosed(IPropValue other);
        /// <summary>Найти разность</summary><param name="other">Пара для разности</param><returns>Разность</returns>
        IPropValue FindDifference(IPropValue other);
    }
}
