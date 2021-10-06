using JSMBase.RNK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.Flights
{
    /// <summary>Класс полетной информации</summary>
    public sealed class Flight
    {
        public Flight()
        {
            TablePropValues = new Dictionary<int, IPropValue[]>();
            PropValues = new IPropValue[Infos.Length];
        }
        /// <summary>Данные о признаках</summary>
        public static IPropInfo[] Infos { get; set; }
        /// <summary>Данные о признаках хода полета</summary>
        public static IPropInfo[] TableInfos { get; set; }

        /// <summary>Значения признаков</summary>
        public IPropValue[] PropValues { get; private set; }

        public Dictionary<int, IPropValue[]> TablePropValues { get; private set; }
        /// <summary>Идентификатор</summary>
        public string ID
        {
            get
            {
                String val1 = null;
                DateTime val2 = new DateTime();
                for (int i = 0; i < Infos.Length; i++)
                {
                    if (Infos[i].Name == "Борт")
                        val1 = PropValues[i].Value as string;
                    if (Infos[i].Name == "Дата полета")
                        val2 = (DateTime)PropValues[i].Value;
                }
                return String.Format("{0}_{1:ddMMyyyy}", val1, val2);
            }
        }
    }
}
