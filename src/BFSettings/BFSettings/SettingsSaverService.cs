using BFSettings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace BFSettings
{
    public class SettingsSaverService
    {
        public String Filter
        {
            get { return "*.xml|*.xml"; }
        }
        /// <summary>Сохранить</summary>
        /// <param name="set">Настройка</param>
        /// <param name="path">Путь для файла</param>
        public void Save(FBSetting set, string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
                new XmlSerializer(typeof(FBSetting), AllTypes()).Serialize(sw, set);
        }

        /// <summary>Прочитать из файла</summary>
        /// <param name="path">Путь</param>
        /// <returns></returns>
        public FBSetting Read(string path)
        {
            using (StreamReader sr = new StreamReader(path))
                return new XmlSerializer(typeof(FBSetting), AllTypes()).Deserialize(sr) as FBSetting;
        }

        Type[] tps;

        private Type[] AllTypes()
        {
            if (tps == null)
                tps = typeof(FBSetting).Assembly.GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(PropSetting)))
                    .ToArray();
            return tps;
        }
    }
}
