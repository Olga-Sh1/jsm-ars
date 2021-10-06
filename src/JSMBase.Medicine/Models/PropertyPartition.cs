using JSMBaseC.Medicine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace JSMBase.Medicine.Models
{
    /// <summary>Разделение относительно свойства</summary>
    public class PropertyPartition : IPartitionDescription
    {
        public Int32 Prop { get; private set; }
        public IndexDataPart[] Data { get; private set; }
        public PropertyPartition()
        {
        }
        public PropertyPartition(int pr, IEnumerable<IndexDataPart> data)
        {
            Prop = pr;
            Data = data.ToArray();
        }

        public Dictionary<Signs, IEnumerable<IID>> GetSign(IEnumerable<IID> obs)
        {
            var ordered = Data.OrderBy(d => d.Type).ToArray();

            List<JSMMedicine> meds = obs.OfType<JSMMedicine>().ToList();
            Dictionary<Signs, List<JSMMedicine>> res = new Dictionary<Signs, List<JSMMedicine>>()
            {
                { Signs.Plus, new List<JSMMedicine>() },
                { Signs.Minus, new List<JSMMedicine>() },
                { Signs.Null, new List<JSMMedicine>() },
                { Signs.Tau, new List<JSMMedicine>() },
                { Signs.None, new List<JSMMedicine>() }
            };
            int j = JSMMedicine.GetRealIndex(JSMMedicine.Infos.First(f => f.Index == Prop));
            for (int i = 0; i < ordered.Length; i++)
            {
                switch (ordered[i].Type)
                {
                    case DiffPart.None:
                        continue;
                    case DiffPart.Exact:
                        List<JSMMedicine> spare = new List<JSMMedicine>();
                        foreach (JSMMedicine med in meds)
                        {
                            PropData pd = med.Data[j];
                            for(int k = 0; k < ordered[i].PropIndex.Length; k++)
                            {
                                var opi = ordered[i].PropIndex[k];
                                if (pd.PlusValuesIndeces.Contains(opi))
                                {
                                    res[ordered[i].Sign].Add(med);
                                    spare.Add(med);
                                }
                            }
                        }
                        foreach (var sp in spare)
                            meds.Remove(sp);
                        break;
                    case DiffPart.ExactWithNull:
                        List<JSMMedicine> spare4 = new List<JSMMedicine>();
                        foreach (JSMMedicine med in meds)
                        {
                            PropData pd = med.Data[j];
                            for (int k = 0; k < ordered[i].PropIndex.Length; k++)
                            {
                                var opi = ordered[i].PropIndex[k];
                                if (pd.PlusValuesIndeces.Length == 0 || pd.PlusValuesIndeces.Contains(opi))
                                {
                                    res[ordered[i].Sign].Add(med);
                                    spare4.Add(med);
                                }
                            }
                        }
                        foreach (var sp in spare4)
                            meds.Remove(sp);
                        break;
                    case DiffPart.AllExceptNull:
                        List<JSMMedicine> spare2 = new List<JSMMedicine>();
                        foreach (JSMMedicine med in meds)
                        {
                            
                            PropData pd = med.Data[j];
                            if (pd.MinusValuesIndeces.Length > 0 || pd.PlusValuesIndeces.Length > 0)
                            {
                                res[ordered[i].Sign].Add(med);
                                spare2.Add(med);
                            }
                        }
                        foreach (var sp in spare2)
                            meds.Remove(sp);
                        break;
                    case DiffPart.AllExeptNotNull:
                        List<JSMMedicine> spare3 = new List<JSMMedicine>();
                        foreach (JSMMedicine med in meds)
                        {
                            PropData pd = med.Data[j];
                            if (pd.MinusValuesIndeces.Length == 0 && pd.PlusValuesIndeces.Length == 0)
                            {
                                res[ordered[i].Sign].Add(med);
                                spare3.Add(med);
                            }
                        }
                        foreach (var sp in spare3)
                            meds.Remove(sp);
                        break;
                }
            }
            return res.ToDictionary(k => k.Key, v => v.Value.Cast<IID>());
        }

        #region IXmlSerializable
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            Prop = reader.ReadElementContentAsInt();
            Data = new XmlSerializer(typeof(IndexDataPart[])).Deserialize(reader) as IndexDataPart[];
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("Prop");
            writer.WriteValue(Prop);
            writer.WriteEndElement();
            new XmlSerializer(Data.GetType()).Serialize(writer, Data);
        }
        #endregion
    }

    /// <summary>Данные о разделении по знакам</summary>
    public class IndexDataPart
    {
        /// <summary>Знак</summary>
        public Signs Sign { get; set; }
        /// <summary>Порядковый номер</summary>
        public Int32[] PropIndex { get; set; }
        /// <summary>Тип</summary>
        public DiffPart Type { get; set; }
    }

    public enum DiffPart
    {
        None,
        Exact,
        ExactWithNull,
        AllExeptNotNull,
        AllExceptNull
    }
}
