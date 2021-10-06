using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JSMBase.Medicine
{
    public static class Parser
    {
        /// <summary>Содержание в {}</summary>
        private static readonly Regex reContentSimple = new Regex("(?<=\\{)[^\\{\\}]+(?=\\})", RegexOptions.Compiled);
        /// <summary>Value=(...)</summary>
        private static readonly Regex reContentValue = new Regex("(?<=Value=\\()[^\\(\\)]+(?=\\))", RegexOptions.Compiled);


        private static readonly Regex reContentName = new Regex("\\{[^\\{\\}]+\\}");
        private static readonly Regex reContent = new Regex("\\{[\\s\\S]+[\\}\\)]$");
        private static readonly Char[] delims = new Char[] { ' ' };
        public static JSMStructure ReadFromJDM(string path)
        {
            using (StreamReader sr = new StreamReader(path, Encoding.Default))
            {
                return ReadFromJDMStream(sr);
            }
        }
        public static JSMStructure ReadFromJDMStream(StreamReader sr)
        {
            JSMStructure _str = new JSMStructure();
            while (!sr.EndOfStream)
            {
                string str = sr.ReadLine().Trim();
                if (str.StartsWith("\\S", true, System.Globalization.CultureInfo.CurrentCulture))
                    break;
                //закомментированные
                if (str.StartsWith("\\")) continue;
                Match m = reContentName.Match(str);
                if (m.Success)
                {
                    string[] arr = str.Remove(m.Index, m.Length).Split(delims, StringSplitOptions.RemoveEmptyEntries);
                    int num;
                    if (int.TryParse(arr[0], out num))
                    {
                        PropInfo pi = new PropInfo();
                        pi.Name = m.Value.Substring(1, m.Value.Length - 2);
                        pi.Index = num;
                        bool fl = false;
                        switch (arr[1])
                        {
                            case "Set":
                                if (string.Compare(arr[arr.Length - 1], cSingleSelect, true) == 0)
                                {
                                    pi.Type = PropTypes.Value;
                                    fl = true;
                                }
                                else if (string.Compare(arr[2], cTwoValues, true) == 0)
                                {
                                    pi.Type = PropTypes.Set;
                                    fl = true;
                                }
                                else if (string.Compare(arr[2], cThreeValues, true) == 0)
                                {
                                    pi.Type = PropTypes.SetT;
                                    fl = true;
                                }
                                if (fl)
                                    ReadJDMSet(pi, sr);
                                break;
                            case "GroupTree":
                                if (string.Compare(arr[2], cTwoValues, true) == 0)
                                {
                                    pi.Type = PropTypes.Tree;
                                    fl = true;
                                }
                                else if (string.Compare(arr[2], cThreeValues, true) == 0)
                                {
                                    pi.Type = PropTypes.TreeT;
                                    fl = true;
                                }
                                if (fl)
                                    ReadJDMTree(pi, sr);
                                break;
                            case "Interval":
                                pi.Type = PropTypes.Interval;
                                ReadJDMInterval(pi, sr);
                                break;
                        }
                        _str.Add(pi);
                    }
                }
            }
            return _str;
        }
        private static void ReadJDMTree(PropInfo st, StreamReader sr)
        {
            ReadTreeInner(st, sr, 0, _endgrouptree);
        }
        private static void ReadTreeInner(PropInfo st, StreamReader sr, Int32 level, string final_str)
        {
            while (!sr.EndOfStream)
            {
                String str = sr.ReadLine().Trim();
                if (string.Compare(str, final_str, true) == 0)
                    return;
                Match _content = reContent.Match(str);
                if (_content.Success)
                {
                    string[] arr = str.Remove(_content.Index, _content.Length).Split(delims, StringSplitOptions.RemoveEmptyEntries);
                    if (string.Compare(arr[0], _tree, true) == 0)
                    {
                        st.Values.Add(TupleEx.Create(level, _content.Value.Substring(1, _content.Value.Length - 2)));
                        ReadTreeInner(st, sr, level + 1, _endtree);
                    }
                    else if (string.Compare(arr[0], _itemtree, true) == 0)
                    {
                        st.Values.Add(TupleEx.Create(level, _content.Value.Substring(1, _content.Value.Length - 2)));
                    }
                }
            }
        }
       
        private static void ReadJDMSet(PropInfo st,StreamReader sr)
        {
            string str = sr.ReadLine().Trim();
            while (!(string.Compare(str, _endset, true) == 0))
            {
                if (!str.StartsWith("\\"))
                {
                    Match _content = reContent.Match(str);
                    if (_content.Success)
                    {
                        string[] arr = str.Remove(_content.Index, _content.Length).Split(delims, StringSplitOptions.RemoveEmptyEntries);
                        if (string.Compare(arr[0], _itemset) == 0)
                            st.Values.Add(TupleEx.Create(0, _content.Value.Substring(1, _content.Value.Length - 2)));
                    }
                }
                str = sr.ReadLine().Trim();
            }
        }
        private static void ReadJDMInterval(PropInfo st, StreamReader sr)
        {
            string str = sr.ReadLine().Trim();
            while (!(string.Compare(str, _endinterval, true) == 0))
            {
                Match _content = reContent.Match(str);
                if (_content.Success)
                {
                    string value = _content.Value.Substring(1, _content.Value.Length - 2);
                    string[] arr = str.Remove(_content.Index, _content.Length).Split(delims, StringSplitOptions.RemoveEmptyEntries);
                    if (string.Compare(arr[0], cBelowNorm) == 0)
                        st.Values.Add(TupleEx.Create(-1, value));
                    else if (string.Compare(arr[0], cNorm) == 0)
                        st.Values.Add(TupleEx.Create(0, value));
                    else if (string.Compare(arr[0], cAboveNorm) == 0)
                        st.Values.Add(TupleEx.Create(1, value));
                }
                str = sr.ReadLine().Trim();
            }
        }

        const string cTwoValues = "Value=(+-)";
        const string cThreeValues = "Value=(+-t)";

        const string cSingleSelect = "SingleSelect";
        const string cMultiSelect = "MultiSelect";

        const string cBelowNorm = "Dn";
        const string cNorm = "Norm";
        const string cAboveNorm = "Up";

        const string _endgrouptree = "endgrouptree";
        const string _itemtree = "itemtree";
        const string _itemset = "ItemSet";
        const string _tree = "tree";
        const string _endtree = "endtree";
        const string _endset = "endset";
        const string _endinterval = "EndInterval";

        #region Объекты

        public const string cEndPatient = "EndPacient";
        public static JSMMedicine CreateObject(StreamReader sr, int n)
        {
           
            //pacient name
            string name = sr.ReadLine();
            if (sr.EndOfStream) return null;
            Match m = reContent.Match(name);
            string _ID = /*n.ToString() + "-" +*/ m.Value.Substring(1, m.Value.Length - 2);
            //string _ID = n.ToString();

            //comment 
            Match m2 = reContent.Match(sr.ReadLine());
            String _Comment = String.IsNullOrEmpty(m2.Value) ? String.Empty : m2.Value.Substring(1, m2.Value.Length - 2);
            //props
            JSMMedicine ob = new JSMMedicine(n, _ID, _Comment);
            FillProps(sr, ob, cEndPatient);
            return ob;
        }
        private static void FillProps(StreamReader sr, JSMMedicine ob, String end_str)
        {
            string tmp = sr.ReadLine().Trim();
            int i = 0;
            while (string.Compare(tmp, end_str, true) != 0)
            {
                string[] arr = tmp.Split(' ');
                int index = Int32.Parse(arr[0]);
                PropInfo piCurrent = JSMMedicine.Infos.FirstOrDefault(inf => inf.Index == index);
                if (piCurrent != null)
                    ob.Data[i] = new PropData(piCurrent, arr[2].Substring(0, arr[2].Length - 1).Trim());
                tmp = sr.ReadLine().Trim();
                i++;
            }
        }
        /// <summary>Считать пациентов</summary><param name="path">Путь</param><param name="list">Список</param>
        public static void ReadPatientsFromTxt(String path, List<JSMMedicine> list)
        {
           
            using (StreamReader sr = new StreamReader(path, Encoding.GetEncoding(1251)))
            {
                ReadPatientsFromStream(sr, list);
            }
        }

        public static void ReadPatientsFromStream(StreamReader sr, List<JSMMedicine> list)
        {
            int ii = 1;
            while (!sr.EndOfStream)
            {
                JSMMedicine ob = CreateObject(sr, ii);
                if (ob != null)
                {
                    list.Add(ob);
                    ii++;
                }
            }
        }

        public static IEnumerable<JSMMedicine> ReadAllObjects(String path)
        {
            String Path = System.IO.Path.GetDirectoryName(path);

            String PathModel = "";
            String PathOnco = "";
            List<JSMMedicine> _lst = new List<JSMMedicine>();
            try
            {
                PathModel = Directory.GetFiles(Path, "DMODEL.JDM")[0];
                PathOnco = path;
            }
            catch (Exception ex)
            {
                throw new System.Exception("Ошибка при извлечении данных из папки", ex);
            }

            JSMStructure str = null;
            try
            {
                str = Parser.ReadFromJDM(PathModel);
            }
            catch (Exception ex)
            {
                throw new System.Exception("Ошибка при загрузке структуры", ex);
            }
            JSMMedicine.Infos = str;
            try
            {
                Parser.ReadPatientsFromTxt(PathOnco, _lst);
            }
            catch (Exception ex)
            {
                throw new System.Exception("Ошибка при загрузке данных", ex);
            }
            return _lst;
        }
        #endregion

        #region Гипотезы
        const string _hyps = "Hypothesis";
        const string _endhyps = "EndHypothesis";
        const string _hypformat = "{{ {0}}} Value=({1}) {2}";
        const string _endpatientlist = "EndPacientList";
        public static async Task WriteHypothesis(Hypothesis<JSMMedicine, JSMBase> meds, Signs s, HypCollectionType t, StreamWriter sw)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(_hyps);
            sb.AppendFormat(_hypformat, meds.ID, s.ToSymbolString(), t.ToString());
            await sw.WriteLineAsync(sb.ToString());

            foreach (var ob in meds.ParentList)
                await WritePatientHeader(ob.Inner, sw);
            await sw.WriteLineAsync(_endpatientlist);

            foreach (var pr in meds.Body.Inner.Data)
                if (pr != null)
                    await WriteProp(pr, sw);

            await sw.WriteLineAsync(_endhyps);
        }

        #region Записать данные
        /// <summary>Записать данные в файл</summary><param name="meds">Данные</param>
        /// <param name="sw"></param><returns></returns>
        public static async Task WriteData(IEnumerable<JSMMedicine> meds, StreamWriter sw)
        {
            foreach (JSMMedicine med in meds)
            {
                await WritePatientHeader(med, sw);
                await WritePatientComment(med, sw);
                foreach (var pr in med.Data)
                    await WriteProp(pr, sw);
                await WritePatientEnd(med, sw);
            }
        }

        static readonly string _prop_val = " {0} {1}{{ {2}}}  Value=({3})  {4}";
        static readonly string _prop_item = "{0}{{ {1}}}";
        /// <summary>Записать описание</summary><param name="strs">Описание</param><returns></returns>
        public static async Task WriteDescription(JSMStructure strs, StreamWriter sw)
        {
            foreach (var st in strs)
            {
                string end = GetSt1(st.Type);
                await sw.WriteLineAsync(String.Format(_prop_val, st.Index, end, st.Name, GetStVals(st.Type), GetStEnd(st.Type)));
                foreach (var it in st.Values)
                {
                    await sw.WriteLineAsync(String.Format(_prop_item, _itemset, it.Item2)); 
                }
                await sw.WriteLineAsync("End"+end);
            }
        }

        private static string GetStEnd(PropTypes pt)
        {
            switch (pt)
            {
                case PropTypes.Set:
                case PropTypes.SetT:
                    return "MultiSelect";
                case PropTypes.Value:
                    return "SingleSelect";
            }
            throw new NotSupportedException();
        }

        private static string GetSt1(PropTypes pt)
        {
            switch (pt)
            {
                case PropTypes.Set:
                case PropTypes.SetT:
                case PropTypes.Value:
                    return "Set";
                case PropTypes.Interval:
                    return "Interval";
                case PropTypes.Tree:
                case PropTypes.TreeT:
                    return "GroupTree";
            }
            throw new NotSupportedException();
        }

        private static string GetStVals(PropTypes pt)
        {
            switch (pt)
            {
                case PropTypes.Set:
                case PropTypes.Value:
                case PropTypes.Interval:
                    return "+-";
                case PropTypes.Tree:
                case PropTypes.TreeT:
                case PropTypes.SetT:
                    return "+-t";
            }
            throw new NotSupportedException();
        }
        
        const string _patformat = "Pacient{{ {0}}}";
        private static Task WritePatientHeader(JSMMedicine m, StreamWriter sw)
        {
            return sw.WriteLineAsync(String.Format(_patformat, m.ID));
        }
        const string _prformat = " {1} Prop{{ {0}}}";
        private static Task WriteProp(PropData pr, StreamWriter sw)
        {
            return sw.WriteLineAsync(String.Format(_prformat, pr.ToString(), pr.Info.Index));
        }
        const string _patend = "EndPacient";
        private static Task WritePatientEnd(JSMMedicine m, StreamWriter sw)
        {
            return sw.WriteLineAsync(_patend);
        }
        const string _patcomm = " PComment{{ {0}}}";
        private static Task WritePatientComment(JSMMedicine m, StreamWriter sw)
        {
            return sw.WriteLineAsync(String.Format(_patcomm, m.Comment));
        }
        #endregion
        private static async Task<Tuple<Signs, Hypothesis<JSMMedicine, JSMBase>>> ReadHypothesis(StreamReader sr, IEnumerable<JSMBase> array)
        {
            String line = null;
            do
            {
                line = await sr.ReadLineAsync();
            }
            while (line != null && !line.StartsWith(_hyps));
            //ID гипотезы
            String id = reContentSimple.Match(line).Value;
            
            uint ind = 0;
            try{
                string str = id.Split('№')[1].Split(' ')[0];
                ind = uint.Parse(str);
            }
            catch { }
            Hypothesis<JSMMedicine, JSMBase> h = new Hypothesis<JSMMedicine, JSMBase>(ind, id);
            h.Body = new JSMBase(new JSMMedicine());
            //Знак
            String sign_str = reContentValue.Match(line).Value;
            Signs sign = Helps.FromSymbolString(sign_str);
            //Тип
            String type_str = line.Split(delims, StringSplitOptions.RemoveEmptyEntries).Last();
            HypCollectionType t = HypCollectionType.Reason;
            Enum.TryParse<HypCollectionType>(type_str, out t);
            while (!sr.EndOfStream)
            {
                #region Список родителей
                line = await sr.ReadLineAsync();
                line = line.Trim();
                if (String.Compare(line, _endpatientlist, true) == 0)
                    break;
                String parID = reContentSimple.Match(line).Value.Trim();
                var parent = array.FirstOrDefault(a => String.Compare(a.ID.Trim(), parID, true) == 0);
                if (parent == null) 
                    throw new KeyNotFoundException("Не найден родитель в массиве "+parID);
                h.ParentList.Add(parent);
                #endregion

            };
            FillProps(sr, h.Body.Inner, _endhyps);
            return Tuple.Create<Signs, Hypothesis<JSMMedicine, JSMBase>>(sign, h);
        }
        public static async Task<Dictionary<Signs, IEnumerable<Hypothesis<JSMMedicine, JSMBase>>>> ReadHypotheses(String path, IEnumerable<JSMBase> array)
        {
            List<Hypothesis<JSMMedicine, JSMBase>> resPl = new List<Hypothesis<JSMMedicine, JSMBase>>();
            List<Hypothesis<JSMMedicine, JSMBase>> resMin = new List<Hypothesis<JSMMedicine, JSMBase>>();
            List<Hypothesis<JSMMedicine, JSMBase>> resNul = new List<Hypothesis<JSMMedicine, JSMBase>>();
            using (StreamReader sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                {
                    var hr = await ReadHypothesis(sr, array);
                    switch (hr.Item1)
                    {
                        case Signs.Plus:
                            resPl.Add(hr.Item2);
                            break;
                        case Signs.Minus:
                            resMin.Add(hr.Item2);
                            break;
                        case Signs.Null:
                            resNul.Add(hr.Item2);
                            break;
                    }
                    
                }
            }
            return new Dictionary<Signs, IEnumerable<Hypothesis<JSMMedicine, JSMBase>>>()
                {
                    { Signs.Plus, resPl },
                    { Signs.Minus, resMin },
                    { Signs.Null, resNul }
                };
        }
        #endregion
    }
}
