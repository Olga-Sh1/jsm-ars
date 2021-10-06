using Ionic.Zip;
using JSMBase.Medicine;
using JSMBase.Medicine.Models.BoF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace JSMBase.Medicine.RWServices
{
    /// <summary>Чтение из архивированного DMODEL</summary>
    public sealed class JsmxReaderService : IReaderService
    {
        /// <summary>Считать данные</summary>
        /// <param name="path">Путь</param>
        /// <returns></returns>
        public IEnumerable<JSMMedicine> Read(String path)
        {
            using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile(path, Encoding.Default))
            {
                var data = zip.FirstOrDefault(t => Path.GetExtension(t.FileName).Equals(".txt", StringComparison.InvariantCultureIgnoreCase));
                var descr = zip.FirstOrDefault(t => Path.GetExtension(t.FileName).Equals(".jdm", StringComparison.InvariantCultureIgnoreCase));
                if (data == null || descr == null) throw new Exception("Данные поврежедны");

                List<JSMMedicine> _lst = new List<JSMMedicine>();
                JSMStructure str = null;
                try
                {
                    using(MemoryStream ms = new MemoryStream())
                    {
                        descr.Extract(ms);
                        ms.Seek(0, SeekOrigin.Begin);
                        using (StreamReader sr = new StreamReader(ms, Encoding.Default))
                            str = Parser.ReadFromJDMStream(sr);
                    }
                }
                catch (Exception ex)
                {
                    throw new System.Exception("Ошибка при загрузке структуры", ex);
                }
                JSMMedicine.Infos = str;
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        data.Extract(ms);
                        ms.Seek(0, SeekOrigin.Begin);
                        using (StreamReader sr = new StreamReader(ms, Encoding.Default))
                            Parser.ReadPatientsFromStream(sr, _lst);
                    }
                }
                catch (Exception ex)
                {
                    throw new System.Exception("Ошибка при загрузке данных", ex);
                }
                return _lst;
            }
        }
        /// <summary>Считать настройки БФ</summary>
        /// <param name="path">Путь</param>
        /// <returns></returns>
        public PartitionBaseData ReadSettings(String path)
        {
            using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile(path, Encoding.Default))
            {
                var setts = zip.FirstOrDefault(t => Path.GetExtension(t.FileName).Equals(".xml", StringComparison.InvariantCultureIgnoreCase));
                if (setts == null) return null;
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        setts.Extract(ms);
                        ms.Seek(0, SeekOrigin.Begin);
                        return new XmlSerializer(typeof(PartitionBaseData)).Deserialize(ms) as PartitionBaseData;
                        //using (StreamReader sr = new StreamReader(ms, Encoding.Default))
                        //    str = Parser.ReadFromJDMStream(sr);
                    }
                }
                catch (Exception ex)
                {
                    throw new System.Exception("Ошибка при загрузке настроек", ex);
                }
            }
        }

        private static readonly string _setName = "BoF.xml";
        public void WriteSettings(String path, PartitionBaseData setts)
        {
            using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile(path, Encoding.Default))
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        new XmlSerializer(typeof(PartitionBaseData))
                            .Serialize(ms, setts);
                        ms.Seek(0, SeekOrigin.Begin);
                        zip.UpdateEntry(_setName, ms);
                        zip.Save();
                    }
                }
                catch (Exception ex)
                {
                    throw new System.Exception("Ошибка при сохранении настроек", ex);
                }
            }
        }

        public string GetComment(string path)
        {
            using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile(path, Encoding.Default))
            {
                return zip.Comment;
            }
        }
    }
}
