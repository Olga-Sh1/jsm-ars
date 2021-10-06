using BaseElements;
using JSMBase.Flights;
using JSMBase.RNK;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.Medicine.RWServices.Excel
{
    /// <summary>Класс считывания данных из набора таблиц Excel</summary>
    public class Parser : ParserCommon
    {
        /// <summary>Получить данные</summary>
        /// <param name="path">Директория</param>
        /// <returns></returns>
        public override async Task<IEnumerable<Flight>> Parse(string path, IProgress<ProgressReport> pr)
        {
            List<Flight> results = new List<Flight>();
            var ini_file = Directory.GetFiles(path, "schema.ini");
            IEnumerable<IPropInfo> infos = INI.Parser.Parse(ini_file[0]);
            Flight.TableInfos = infos.ToArray();
            IEnumerable<IPropInfo> sinfos = INI.Parser.Parse(ini_file[0], ParserCommon.FILE_SUMMARY_NAME);
            Flight.Infos = sinfos.ToArray();

            var files = Directory.GetFiles(path, "*.xls");
            foreach(var file in files)
                if (Path.GetFileName(file) != ParserCommon.FILE_SUMMARY_NAME)
                    results.Add(ParseFlight(file));

            var summ_file = Directory.GetFiles(path, ParserCommon.FILE_SUMMARY_NAME);
            if (summ_file.Length > 0)
                AddInfo(summ_file[0], results);

            return results;
        }

       

       

       

       
    }
}
