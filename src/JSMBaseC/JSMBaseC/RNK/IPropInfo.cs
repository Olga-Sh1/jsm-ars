using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.RNK
{
    /// <summary>Информация о признаке</summary>
    public interface IPropInfo
    {
        /// <summary>Порядковый номер</summary>
        Int32 Index { get; set; }
        /// <summary>Название</summary>
        String Name { get; }
        /// <summary>Описание</summary>
        String Description { get; }
        /// <summary>Группа (если есть)</summary>
        String GroupId { get; }
        /// <summary></summary>
        /// <param name="ob">Создать значение</param>
        /// <returns>Значение</returns>
        IPropValue CreateValue(Object ob);
    }
}
