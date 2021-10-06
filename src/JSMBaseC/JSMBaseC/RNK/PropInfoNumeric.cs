using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.RNK
{
    /// <summary>Информация о признаке, заданном числовым значением </summary>
    public sealed class PropInfoNumeric : IPropInfo
    {
        /// <summary>Порядковый номер</summary>
        public int Index { get; set; }
        /// <summary>Название</summary>
        public string Name { get; set; }
        /// <summary>Значение отклонения</summary>
        public double Delta { get; set; }
        /// <summary>Описание</summary>
        public string Description { get; set; }
        /// <summary>Группа</summary>
        public string GroupId { get; set; }
        /// <summary>Создать значение</summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public IPropValue CreateValue(object ob)
        {
            return new PropValueNumeric(this, (Double)CustomConverter.TryConvert(ob, typeof(Double)));
        }
    }
}
