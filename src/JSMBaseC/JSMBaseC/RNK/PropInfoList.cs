using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.RNK
{
    /// <summary>Информация о признаке, заданном элементом из списка</summary>
    public sealed class PropInfoList : IPropInfo
    {
        /// <summary>Порядковый номер</summary>
        public int Index { get; set; }
        /// <summary>Название</summary>
        public string Name { get; set; }
        /// <summary>Значения</summary>
        public string[] Values { get; set; }
        /// <summary>Описание</summary>
        public string Description { get; set; }
        /// <summary>Группа</summary>
        public string GroupId { get; set; }

        /// <summary>Создать значение</summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public IPropValue CreateValue(object ob)
        {
            return new PropValueList(this, ob.ToString());
        }
    }
}
