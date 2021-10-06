using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSMBase.Medicine
{
    [Serializable]
    public class Settings
    {
        public Settings()
        {
            NumPlusParent = 2;
            NumMinusParent = 2;
            IndecesProps = new int[0];
            NumToPlusMinus = 1;
            IDsAsTau = new String[0];
            IndicesToPlus = new int[0];
            IndicesToMinus = new int[0];
            IndicesToNull = new int[0];
        }
        /// <summary>Количество родителей плюс</summary>
        public int NumPlusParent { get; set; }
        /// <summary>Количество родителей минус</summary>
        public int NumMinusParent { get; set; }
        /// <summary>ID свойств, которые используем</summary>
        public int[] IndecesProps { get; set; }
        /// <summary>ID свойства, относительно которого делим на плюсы и минусы</summary>
        public int NumToPlusMinus { get; set; }
        /// <summary>Индекс значения, которое принимаем за "да"</summary>
        public int IndexToPlusMinusValue { get; set; }
        /// <summary>Идентификаторы объектов, которые закрываем</summary>
        public String[] IDsAsTau { get; set; }
        /// <summary>Индексы значения для (+)-примеров</summary>
        public int[] IndicesToPlus { get; set; }
        /// <summary>Индексы значения для (-)-примеров</summary>
        public int[] IndicesToMinus { get; set; }
        /// <summary>Индексы значения для (0)-примеров</summary>
        public int[] IndicesToNull { get; set; }
        /// <summary>Фильтр по строкам в исходных данных</summary>
        public String FilterString { get; set; }
    }
}
