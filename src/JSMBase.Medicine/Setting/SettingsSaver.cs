using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.Medicine.Setting
{
    /// <summary>Вспомогательный класс для работы с настройками сохранения</summary>
    public static class SettingsSaver
    {
        /// <summary>Получить настройки</summary><param name="path">Путь</param><returns></returns>
        public static Settings Load(string path)
        {
            Settings s = null;
            using (StreamReader sw = new StreamReader(path))
            {
                s = new BinaryFormatter().Deserialize(sw.BaseStream) as Settings;
            }
            return s;
        }

        /// <summary>Сохранение настроек в файл</summary>
        /// <param name="s">Настройки</param>
        /// <param name="path">Путь файла</param>
        public static void Save(Settings s, string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                new BinaryFormatter().Serialize(sw.BaseStream, s);
            }
        }
    }
}
