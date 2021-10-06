using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.Medicine.RWServices
{
    /// <summary>Фабрика для сервисов чтения/записи</summary>
    public class RWServiceFactory
    {
        private RWServiceFactory() { }

        private static readonly RWServiceFactory _Instance = new RWServiceFactory();

        public static  RWServiceFactory Instance { get { return _Instance; } }

        /// <summary>Сервис чтения-записи по пути к файлу</summary>
        /// <param name="path">Путь файла</param>
        /// <returns></returns>
        public IReaderService GetServiceByPath(string path)
        {
            string ext = System.IO.Path.GetExtension(path);
            string ext_raw = ext.Substring(1);
            var dtf = Models.BoF.DataFormats.old;
            if (!ext_raw.Equals("txt", StringComparison.OrdinalIgnoreCase))
                if (!Enum.TryParse<Models.BoF.DataFormats>(ext_raw, out dtf))
                    throw new ArgumentException("Нет обработчика для расширения " + ext_raw);

            IReaderService reader = null;
            switch (dtf)
            {
                case Models.BoF.DataFormats.old:
                    reader = new JDMReaderService();
                    break;
                case Models.BoF.DataFormats.jbof:
                    reader = new JsmxReaderService();
                    break;
                case Models.BoF.DataFormats.sav:
                    reader = new SavReaderService();
                    break;
            }
            return reader;
        }
    }
}
