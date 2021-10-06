using JSMBase.Medicine.Models.CSV;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.Medicine.DAL
{
    public sealed class CsvReader : IReader
    {
        public IList Read(string path)
        {
            var data = global::JSMBase.Medicine.RWServices.CSV.Parser.Parse(path);
            CSVBaseOfFacts bof = new CSVBaseOfFacts(data);
            var jsmdata = bof.Divide();
            return (IList)jsmdata;
        }
    }
}
