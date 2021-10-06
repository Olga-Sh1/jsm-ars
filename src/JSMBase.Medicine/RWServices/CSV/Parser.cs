using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using JSMBase.RNK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JSMBase.Medicine.RWServices.CSV
{
    public static class Parser
    {
        private static Regex colHeader = new Regex(@"Col(\d+)");
        public static IEnumerable<MedicineExample> Parse(string path)
        {
            DoubleCustom typeConv = new DoubleCustom();
            string schName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path), "schema.ini");
            string fileName = System.IO.Path.GetFileName(path);

            //Считываение схемы 
            Dictionary<int, List<string>> lazyData = new Dictionary<int, List<string>>();
            IEnumerable<IPropInfo> prinfos = RWServices.INI.Parser.Parse(schName, fileName);
            foreach(IPropInfo pr in prinfos)
            {
                PropInfoList l = pr as PropInfoList;
                if(l != null)
                {
                    lazyData.Add(l.Index, new List<string>());
                }
            }
            MedicineExample.Infos = prinfos.ToArray();

            List<MedicineExample> result = new List<MedicineExample>();
            using (StreamReader sr = new StreamReader(path))
            {
                using (var csv = new CsvReader(sr, new CsvConfiguration(System.Globalization.CultureInfo.CurrentCulture)))
                {
                    while (csv.Read())
                    {
                        IPropValue[] vals = new IPropValue[prinfos.Count()];
                        int index = 0;
                        foreach (var prinfo in prinfos)
                        {
                            PropInfoNumeric prn = prinfo as PropInfoNumeric;
                            if (prn != null)
                            {
                                var num = csv.GetField<double>(prn.Name, typeConv);
                                vals[index] = new PropValueNumeric(prn, num);
                            }
                            else
                            {
                                PropInfoList pil = prinfo as PropInfoList;
                                if (pil != null)
                                {
                                    var str = csv.GetField<string>(pil.Name);
                                    vals[index] = new PropValueList(pil, str);
                                    if (!lazyData[prinfo.Index].Contains(str))
                                        lazyData[prinfo.Index].Add(str);
                                }
                                else
                                {
                                    PropInfoId prid = prinfo as PropInfoId;
                                    if (prid != null)
                                    {
                                        var str = csv.GetField<string>(prinfo.Name);
                                        vals[index] = new PropValueId(prid, str);
                                    }
                                }
                            }
                            index++;
                        }
                        MedicineExample mex = new MedicineExample(vals);
                        result.Add(mex);
                    }
                    
                }

                foreach(var pair in lazyData)
                {
                    IPropInfo pr = prinfos.First(pr2 => pr2.Index == pair.Key);
                    PropInfoList pil = pr as PropInfoList;
                    pil.Values = pair.Value.ToArray();
                }

                foreach (var m in result)
                {
                    foreach(var pr in m.PropValues)
                    {
                        PropValueList pvl = pr as PropValueList;
                        if (pvl != null)
                            pvl.SetDataIndex();
                    }
                }
            }
            return result;
        }
    }

    public class DoubleCustom : CsvHelper.TypeConversion.ITypeConverter
    {
        public bool CanConvertFrom(Type type)
        {
            return type == typeof(double);
        }

        public bool CanConvertTo(Type type)
        {
            throw new NotImplementedException();
        }

        public object ConvertFromString(CsvHelper.TypeConversion.TypeConverterOptions options, string text)
        {
            if (String.IsNullOrEmpty(text)) return 0.0d;
            return double.Parse(text, System.Globalization.NumberStyles.Number);
        }

        public string ConvertToString(CsvHelper.TypeConversion.TypeConverterOptions options, object value)
        {
            throw new NotImplementedException();
        }

        object ITypeConverter.ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (String.IsNullOrEmpty(text)) return 0.0d;
            return double.Parse(text, System.Globalization.NumberStyles.Number);
        }

        string ITypeConverter.ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            throw new NotImplementedException();
        }
    }
}
