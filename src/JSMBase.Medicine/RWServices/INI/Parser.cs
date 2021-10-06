using IniParser.Model;
using JSMBase.Medicine.RWServices.CSV;
using JSMBase.RNK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JSMBase.Medicine.RWServices.INI
{
    public static class Parser
    {
        private static Regex colHeader = new Regex(@"Col(\d+)");

        public static IEnumerable<IPropInfo> Parse(String schName, String fileName = null)
        {
            //Считываение схемы 
            IniParser.FileIniDataParser parser = new IniParser.FileIniDataParser();
            var model = parser.ReadFile(schName, Encoding.Default);
            return parseIniData(model, fileName);
        }

        public static IEnumerable<IPropInfo> Parse(StreamReader stream, String fileName = null)
        {
            //Считываение схемы 
            IniParser.FileIniDataParser parser = new IniParser.FileIniDataParser();
            var model = parser.ReadData(stream);
            return parseIniData(model, fileName);
        }

        private static IEnumerable<IPropInfo> parseIniData(IniData model, String fileName )
        {
            KeyDataCollection section = null;
            if (String.IsNullOrEmpty(fileName))
                section = model.Global;
            else
                section = model.Sections[fileName];
            List<IPropInfo> prinfos = new List<IPropInfo>();
            Dictionary<int, List<string>> lazyData = new Dictionary<int, List<string>>();

            foreach (var pair in section)
            {
                var m = colHeader.Match(pair.KeyName);
                if (m.Success)
                {
                    int index = int.Parse(m.Groups[1].Value);
                    string[] values = pair.Value.Split(' ');

                    IPropInfo pr = null;
                    if (values[0] == "ID")
                        pr = new PropInfoId() { Description = values[0], Name = values[0], Index = index };
                    else
                    {
                        JetDataTypes dt = JetDataTypes.Text;
                        if (Enum.TryParse<JetDataTypes>(values[1], out dt))
                        {
                            switch (dt)
                            {
                                case JetDataTypes.Double:
                                case JetDataTypes.Byte:
                                case JetDataTypes.Integer:
                                    PropInfoNumeric n = new PropInfoNumeric();
                                    n.Name = n.Description = ConvertName(values[0]);
                                    n.Index = index;
                                    if (values.Length > 2)
                                        n.Delta = double.Parse(values[2]);
                                    pr = n;
                                    break;
                                case JetDataTypes.Text:
                                    PropInfoList l = new PropInfoList();
                                    l.Index = index;
                                    l.Name = l.Description = ConvertName(values[0]);
                                    pr = l;
                                    lazyData.Add(index, new List<string>());
                                    break;
                                case JetDataTypes.Bit:
                                    PropInfoBit b = new PropInfoBit();
                                    b.Index = index;
                                    b.Name = b.Description = ConvertName(values[0]);
                                    pr = b;
                                    break;
                                case JetDataTypes.LongText:
                                    PropInfoString str = new PropInfoString();
                                    str.Index = index;
                                    str.Name = str.Description = ConvertName(values[0]);
                                    pr = str;
                                    break;
                                case JetDataTypes.DateTime:
                                    PropInfoDate d = new PropInfoDate();
                                    d.Index = index;
                                    d.Name = d.Description = ConvertName(values[0]);
                                    pr = d;
                                    break;

                            }

                        }
                    }
                    if (pr != null) prinfos.Add(pr);
                }
            }
            return prinfos;
        }

        private static string ConvertName(String val)
        {
            return val.Replace("%20", " ");
        }
    }
}
