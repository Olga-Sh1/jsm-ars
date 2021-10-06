using JSMBase.Medicine;
using JSMBase.Medicine.Models.BoF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.Medicine
{
    /// <summary>Сервис чтения данных</summary>
    public interface IReaderService
    {
        /// <summary>Считать в массив элементов</summary><param name="path">Путь к данным</param><returns></returns>
        IEnumerable<JSMMedicine> Read(String path);
        /// <summary>Получить наименование/идентификатор данных</summary><param name="path">Путь к данным</param><returns></returns>
        string GetComment(string path);
        /// <summary>Считать настройки для БФ</summary><param name="path">Путь</param><returns></returns>
        PartitionBaseData ReadSettings(String path);
        /// <summary>Записать настройки для БФ</summary><param name="path">Путь</param><param name="setts">Гастройки</param>
        void WriteSettings(String path, PartitionBaseData setts);
    }
}
