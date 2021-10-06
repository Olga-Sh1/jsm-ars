using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace JSMBase.RNK
{
    /// <summary>Пример медицинских данных</summary>
    [Serializable]
    public class MedicineExample : IXmlSerializable
    {
        public Object this [String prName]
        {
            get 
            {
                int index = -1;
                for (int i = 0; i < Infos.Length; i++)
                    if (Infos[i].Name == prName)
                    {
                        index = i;
                        break;
                    }
                if (index == -1) return null;
                return PropValues[index].Value;
            }
        }
        public MedicineExample()
        {

        }
        public MedicineExample(IPropValue[] _PropValues)
        {
            this.PropValues = _PropValues;
            this.ID = createId();
        }
        /// <summary>Данные о признаках</summary>
        public static IPropInfo[] Infos { get; set; }
        /// <summary>Значения признаков</summary>
        public IPropValue[] PropValues { get; set; }

        private String createId()
        {
            for (int i = 0; i < Infos.Length; i++)
            {
                if (Infos[i].GetType() == typeof(PropInfoId)) return PropValues[i] == null ? null : PropValues[i].Value as string;
            }
            return null;
        }

        #region IXmlSerializable
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement();//ID
            this.ID = reader.ReadElementContentAsString();
            List<IPropValue> lst = new List<IPropValue>();
            if (!reader.IsEmptyElement)
            {
                reader.ReadStartElement();//PropValues

                Dictionary<String, XmlSerializer> cache = new Dictionary<String, XmlSerializer>();
                while (reader.IsStartElement())
                {
                    String pvName = reader.Name;
                    if (!cache.ContainsKey(pvName))
                        cache.Add(pvName, new XmlSerializer(tryFindType(pvName)));
                    var ser = cache[pvName];
                    IPropValue pv = ser.Deserialize(reader) as IPropValue;
                    lst.Add(pv);
                }
                if (MedicineExample.Infos != null && lst.Count < MedicineExample.Infos.Length)
                {
                    IPropValue[] arrd = new IPropValue[MedicineExample.Infos.Length];
                    for(int i = 0; i < MedicineExample.Infos.Length; i++)
                    {
                        arrd[i] = lst.FirstOrDefault(l => l.PropInfo.Index == MedicineExample.Infos[i].Index);
                    }
                    this.PropValues = arrd;
                }
                else
                    this.PropValues = lst.ToArray();
                reader.ReadEndElement();//PropValues
            }
            else
                reader.Read();
            
            reader.ReadEndElement();
        }

        private Type tryFindType(String name)
        {
            return this.GetType().Assembly.GetTypes().FirstOrDefault(t => t.Name == name);
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("ID");
            writer.WriteString(this.ID);
            writer.WriteEndElement();
            writer.WriteStartElement("PropValues");
            Dictionary<Type, XmlSerializer> cache = new Dictionary<Type, XmlSerializer>();
            foreach(var pv in PropValues)
            {
                if (pv == null) continue;
                if (!cache.ContainsKey(pv.GetType()))
                    cache.Add(pv.GetType(),  new XmlSerializer(pv.GetType()));
                var ser = cache[pv.GetType()];
                ser.Serialize(writer, pv);
            }
            writer.WriteEndElement();
        }
        #endregion

        /// <summary>Идентификатор</summary>
        public string ID { get; set; }
    }
}
