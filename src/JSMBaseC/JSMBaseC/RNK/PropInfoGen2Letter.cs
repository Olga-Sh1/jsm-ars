using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.RNK
{
    /// <summary>Информация о гене, заданном 2умя буквами</summary>
    public sealed class PropInfoGen2Letter : IPropInfo
    {
        /// <summary>Порядковый номер</summary>
        public int Index { get; set; }
        /// <summary>Название</summary>
        public string Name { get; set; }
        /// <summary>Описание</summary>
        public string Description { get; set; }
        /// <summary>Группа</summary>
        public string GroupId { get; set; }

        public IPropValue CreateValue(object ob)
        {
            return new PropValueGen2Letter(this, ob.ToString());
        }
    }
}
