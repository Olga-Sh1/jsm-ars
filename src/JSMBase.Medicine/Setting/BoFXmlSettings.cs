using JSMBase.Medicine.Models.BoF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace JSMBase.Medicine.Setting
{
    /// <summary>Настройки баз фактов</summary>
    public class BoFXmlSettings
    {
        private static readonly string path = "BoF.xml";
        private readonly string cur_path = "BoF.xml";
        public PartitionBaseDataList Data { get; private set; }
        public static readonly BoFXmlSettings Instance = new BoFXmlSettings(); 
        private BoFXmlSettings()
        {
            cur_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
        }
        public PartitionBaseData this[string path]
        {
            get
            {
                if (Data == null) return null;
                return Data.FirstOrDefault(d => d.DBPath.Equals(path, StringComparison.InvariantCultureIgnoreCase));
            }
        }
        /// <summary>Загрузить</summary>
        public void Load()
        {
            if (File.Exists(cur_path))
            {
                using (StreamReader sr = new StreamReader(cur_path))
                {
                    Data = new XmlSerializer(typeof(PartitionBaseDataList)).Deserialize(sr) as PartitionBaseDataList;
                }
            }
            if (Data == null) Data = new PartitionBaseDataList();
        }
        /// <summary>Сохранить</summary>
        public void Save()
        {
            if (Data != null)
            {
                using (StreamWriter sw = new StreamWriter(cur_path))
                {
                    new XmlSerializer(typeof(PartitionBaseDataList)).Serialize(sw, Data);
                }
            }
        }
    }
}
