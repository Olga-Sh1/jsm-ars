using JSMBase;
using JSMBase.Medicine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace JSMBaseC.Medicine
{
    public interface IPartitionDescription : IXmlSerializable
    {
        Dictionary<Signs, IEnumerable<IID>> GetSign(IEnumerable<IID> obs);
    }

    /// <summary>Правила БФ для разбиения</summary>
    public class PartitionRules : IXmlSerializable
    {
        /// <summary>Название</summary>
        public String Name { get; set; }
        /// <summary>Признак разделения</summary>
        public bool CustomPartition { get; set; }
        /// <summary>Перечень разделения</summary>
        public Dictionary<String, Signs> Partition { get; private set; }
        /// <summary>Описание разделения</summary>
        public IPartitionDescription PartitionDesc { get; private set; }
        public PartitionRules()
        {

        }
        public PartitionRules(IEnumerable<String> keys, Signs def_sg, IPartitionDescription part)
        {
            Partition = new Dictionary<string, Signs>();
            if (keys != null)
                foreach (String key in keys)
                    Partition.Add(key, def_sg);
            PartitionDesc = part;
        }
        public PartitionRules(IEnumerable<String> keys, Signs def_sg) : this(keys, def_sg, null)
        {
           
            CustomPartition = true;
        }
        public PartitionRules(IPartitionDescription part)
            : this(null, Signs.None, part)
        {
            
            CustomPartition = false;
        }

        #region IXmlSerializable
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            reader.Read();
            Name = reader.ReadElementContentAsString();
            CustomPartition = reader.ReadElementContentAsBoolean();
            if (CustomPartition)
            {
                //TODO:
            }
            else
            {
                string type = reader.GetAttribute("type");
                reader.Read();
                if (type == "null")
                    return;// leave T at default value
                this.PartitionDesc = Activator.CreateInstance(Type.GetType(type)) as IPartitionDescription;
                this.PartitionDesc.ReadXml(reader);
                reader.ReadEndElement();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("Name");
            writer.WriteValue(this.Name);
            writer.WriteEndElement();

            writer.WriteStartElement("CustomPartition");
            writer.WriteValue(this.CustomPartition);
            writer.WriteEndElement();

            if (CustomPartition)
            {
                writer.WriteStartElement("Partition");
                //TODO:
                writer.WriteEndElement();
            }
            else
            {
                writer.WriteStartElement("PartitionDesc");
                writer.WriteAttributeString("type", PartitionDesc.GetType().AssemblyQualifiedName); 
                PartitionDesc.WriteXml(writer);
                writer.WriteEndElement();
            }
        }
        #endregion

        public override string ToString()
        {
            return Name;
        }
    }

   
}
