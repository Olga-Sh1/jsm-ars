
using JSMBase.Medicine;
using JSMBase.Medicine.Models;
using JSMBase.Medicine.Models.BoF;
using JSMBase.Medicine.Setting;
using JSMBaseC.Medicine;
//using SpssLib.DataReader;
//using SpssLib.SpssDataset;
using Curiosity.SPSS.DataReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Curiosity.SPSS.SpssDataset;
using System.Text.RegularExpressions;

namespace JSMBase.Medicine.RWServices
{
    /// <summary>Сервис чтения из SAV файла</summary>
    public sealed class SavReaderService : IReaderService
    {
        private Regex _GroupNameStart = new Regex("[a-zA-Z]+");
        private TempFolderService tempService = new TempFolderService();
        public IEnumerable<global::JSMBase.Medicine.JSMMedicine> Read(string path)
        {
            List<global::JSMBase.Medicine.JSMMedicine> res = new List<global::JSMBase.Medicine.JSMMedicine>();
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                using (SpssReader reader = new SpssReader(fs))
                {
                    #region Структура
                    JSMStructure str = new JSMStructure();
                    int index = 1;
                    var vars = reader.Variables.OrderBy(v => v.Name).ToArray();
                    foreach (Variable _var in vars)
                    {
                        PropInfo pi = new PropInfo();
                        pi.Name = createInfoName(_var);
                        pi.Index = index++;
                        var m = _GroupNameStart.Match(_var.Name);
                        if (m.Success) 
                            pi.GroupId = m.Value;
                        switch (_var.Type)
                        {
                            case DataType.Numeric:
                                pi.Type = PropTypes.Value;
                                break;
                            default:
                                index--;
                                continue;
                            //    throw new NotSupportedException(String.Format("Тип {0} SPSS переменной не поддерживается", _var.Type));
                        }
                        var kp = _var.ValueLabels;
                        foreach (var pair in kp)
                            pi.Values.Add(TupleEx.Create(0, pair.Value));
                        str.Add(pi);
                    }
                    JSMMedicine.Infos = str;
                    #endregion

                    #region Данные
                    int indexRecord = 0;
                    foreach (Record _case in reader.Records)
                    {
                        JSMMedicine ob = new JSMMedicine(indexRecord + 1, (indexRecord + 1).ToString(), false);
                        int i = 0;
                        foreach (Variable _var in reader.Variables)
                        {
                            if (_var.Type != DataType.Numeric) continue;
                            String varName = createInfoName(_var);
                            PropInfo piCurrent = JSMMedicine.Infos.FirstOrDefault(inf => inf.Name == varName);
                            if (piCurrent != null)
                            {
                                Object val = _case.GetValue(_var);
                                int[] pl_arr = null;
                                if (val == null || Convert.IsDBNull(val))
                                    pl_arr = new int[0];
                                else
                                {
                                    Double dval = (Double)val;
                                    int innerIndex = 0;
                                    foreach(var p in _var.ValueLabels)
                                    {
                                        if (p.Key == dval) break;
                                        innerIndex++;
                                    }
                                    if (innerIndex < _var.ValueLabels.Count)
                                        pl_arr = new int[] { innerIndex };
                                    else
                                        pl_arr = new int[0];
                                }
                                ob.Data[piCurrent.Index - 1] = new PropData(piCurrent, pl_arr, new int[0]);
                            }
                            i++;
                        }
                        res.Add(ob);
                        indexRecord++;
                    }
                    #endregion
                }
            }

            /*
            using (Spss.SpssDataDocument dd = Spss.SpssDataDocument.Open(path, SpssFileAccess.Read))
            {
                #region Структура
                JSMStructure str = new JSMStructure();
                int index = 1;
                foreach (SpssVariable _var in dd.Variables)
                {
                    PropInfo pi = new PropInfo();
                    pi.Name = _var.Label;
                    pi.Index = index++;
                    switch (_var.SpssType)
                    {
                        case 0:
                            pi.Type = PropTypes.Value;
                            break;
                        default:
                            throw new NotSupportedException(String.Format("Тип {0} SPSS переменной не поддерживается", _var.SpssType));
                    }
                    var kp = _var.GetValueLabels();
                    foreach (var pair in kp)
                        pi.Values.Add(TupleEx.Create(0, pair.Value));
                    str.Add(pi);
                }
                JSMMedicine.Infos = str;
                #endregion

                #region Данные
                foreach (SpssCase _case in dd.Cases)
                {
                    JSMMedicine ob = new JSMMedicine(_case.Position + 1, (_case.Position + 1).ToString());
                    int i = 0;
                    foreach (SpssVariable _var in dd.Variables)
                    {
                        int cur_index = i + 1;
                        PropInfo piCurrent = JSMMedicine.Infos.FirstOrDefault(inf => inf.Index == cur_index);
                        if (piCurrent != null)
                        {
                            var val = _case.GetDBValue(_var.Name);
                            int[] pl_arr = null;
                            if (Convert.IsDBNull(val))
                                pl_arr = new int[0];
                            else
                            {
                                var ind = _var.GetValueLabels().ToList().FindIndex(f => f.Key == val.ToString());
                                if (ind >= 0)
                                    pl_arr = new int[] { ind };
                                else
                                    pl_arr = new int[0];
                            }
                            ob.Data[i] = new PropData(piCurrent, pl_arr, new int[0]);
                        }
                        i++;
                    }
                    res.Add(ob);
                }
                #endregion  
            }
            */
            return res;
        }

        public string GetComment(string path)
        {
            return System.IO.Path.GetFileNameWithoutExtension(path);
        }

        static readonly string _d = "SavDataPart.xml";
        public Models.BoF.PartitionBaseData ReadSettings(string path)
        {
            string patht = System.IO.Path.Combine(tempService.TempDir, _d);
            if (!File.Exists(patht))
            {
                var set_path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path), "set");
                Settings s = System.IO.File.Exists(set_path) ? SettingsSaver.Load(set_path) : new global::JSMBase.Medicine.Settings();
                PartitionBaseData d = new PartitionBaseData();
                d.DBPath = path;
                d.Partitions = new List<JSMBaseC.Medicine.PartitionRules>();
                PropertyPartition prpart = null;
                if (s.IndicesToMinus == null || s.IndicesToMinus.Length == 0)
                    prpart = new PropertyPartition(s.NumToPlusMinus,
                        new IndexDataPart[]
                        {
                        new IndexDataPart() { Sign = Signs.Plus, PropIndex = new[] { s.IndexToPlusMinusValue }, Type = DiffPart.Exact },
                        new IndexDataPart() { Sign = Signs.Minus, PropIndex = new [] { s.IndexToPlusMinusValue }, Type = DiffPart.AllExceptNull },
                        new IndexDataPart() { Sign = Signs.Tau, PropIndex = new [] { s.IndexToPlusMinusValue }, Type = DiffPart.AllExeptNotNull },
                    });
                else
                {
                    PropInfo pi = JSMMedicine.Infos.FirstOrDefault(ff => ff.Index == s.NumToPlusMinus);
                    List<int> lst = new List<int>();
                    for(int k = 0; k < pi.Values.Count;k++)
                    {
                        if (!s.IndicesToPlus.Contains(k) && !s.IndicesToMinus.Contains(k))
                            lst.Add(k);
                    }
                    prpart = new PropertyPartition(s.NumToPlusMinus,
                      new IndexDataPart[]
                      {
                        new IndexDataPart() { Sign = Signs.Plus, PropIndex = s.IndicesToPlus, Type = DiffPart.Exact },
                        new IndexDataPart() { Sign = Signs.Minus, PropIndex = s.IndicesToMinus, Type = DiffPart.Exact },
                        new IndexDataPart() { Sign = Signs.Tau, PropIndex = lst.ToArray(), Type = lst.Count > 0 ? DiffPart.ExactWithNull : DiffPart.AllExeptNotNull },
                  });
                }
                d.Partitions.Add(new PartitionRules(prpart));
                return d;
               
            }
           
            using (FileStream fs = new FileStream(patht, FileMode.Open))
            {
                return new XmlSerializer(typeof(PartitionBaseData)).Deserialize(fs) as PartitionBaseData;
            }
        }

        public void WriteSettings(string path, PartitionBaseData setts)
        {
            string patht = System.IO.Path.Combine(tempService.TempDir, _d);
            using (FileStream fs = new FileStream(patht, FileMode.OpenOrCreate))
            {
                new XmlSerializer(typeof(PartitionBaseData)).Serialize(fs, setts);
            }
        }

        private static String createInfoName(Variable _var)
        {
            return _var.Name + ", " + _var.Label;
        }
    }
}
