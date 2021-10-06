using Ninject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.Medicine.DAL
{
    /// <summary>Фабрика классов для чтения данных</summary>
    public class ReaderFactory
    {
        IKernel k = new StandardKernel(new SimpleModule());
        static ReaderFactory()
        {
            Instance = new ReaderFactory();
        }
        /// <summary>Экземпляр</summary>
        public static ReaderFactory Instance { get; private set; }
        private ReaderFactory() { }
        /// <summary>Считыватель данных по пути файла</summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns></returns>
        public IReader Create(String path)
        {
            String ext = Path.GetExtension(path);
            if (String.Compare(ext, ".csv", true) == 0)
                return k.Get<CsvReader>();
            if (String.Compare(ext, ".xlsx", true) == 0)
                return k.Get<ExcelReader>();
            return null;
        }
    }
}
