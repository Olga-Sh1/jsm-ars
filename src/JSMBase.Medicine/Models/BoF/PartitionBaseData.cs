using JSMBaseC.Medicine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.Medicine.Models.BoF
{
    /// <summary>Данные для разделения базы фактов</summary>
    public class PartitionBaseData
    {
        public PartitionBaseData()
        {
            Partitions = new List<PartitionRules>();
        }
        /// <summary>Путь к БД</summary>
        public String DBPath { get; set; }
        /// <summary>Список разделений</summary>
        public List<PartitionRules> Partitions { get; set; }
    }

    /// <summary>Весь список БФ</summary>
    public class PartitionBaseDataList : List<PartitionBaseData>
    {
    }
}
