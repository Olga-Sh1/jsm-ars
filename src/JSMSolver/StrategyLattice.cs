using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace JSMSolver
{
    public class StrategyLattice : IXmlSerializable
    {
        public LatticePoint[] Nodes { get; private set; }
        public LatticeEdge[] Edges { get; private set; }

        public StrategyLattice(LatticePoint[] nodes, LatticeEdge[] edges)
        {
            Nodes = nodes;
            Edges = edges;
        }
        public StrategyLattice() { }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            List<LatticePoint> nodes = new List<LatticePoint>();
            List<LatticeEdge> edges = new List<LatticeEdge>();
            while (!reader.EOF)
            {
                switch (reader.Name)
                {
                    case "Node":
                        reader.Read();
                        LatticePoint node = new LatticePoint();
                        node.Adds = (Addings)reader.ReadElementContentAsInt();
                        node.ID = reader.ReadElementContentAsLong();
                        nodes.Add(node);
                        reader.ReadEndElement();
                        break;
                    case "Edge":
                        reader.Read();
                        long id1 = reader.ReadElementContentAsLong();
                        long id2 = reader.ReadElementContentAsLong();
                        LatticeEdge edge = new LatticeEdge(nodes.First(n => n.ID == id1), nodes.First(n => n.ID == id2));
                        edges.Add(edge);
                        reader.ReadEndElement();
                        break;
                    default: reader.Read(); break;
                }
            }
            Nodes = nodes.ToArray();
            Edges = edges.ToArray();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            var tt = DateTime.Now.Ticks;
            foreach (var node in Nodes)
                node.ID = tt++;
            foreach (var node in Nodes)
            {
                writer.WriteStartElement("Node");
                writer.WriteStartElement("Value");
                writer.WriteValue(node.Adds.ToString("D"));
                writer.WriteEndElement();
                writer.WriteStartElement("ID");
                writer.WriteValue(node.ID);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            foreach (var edge in Edges)
            {
                writer.WriteStartElement("Edge");
                writer.WriteStartElement("Start");
                writer.WriteValue(edge.StartPoint.ID);
                writer.WriteEndElement();
                writer.WriteStartElement("End");
                writer.WriteValue(edge.EndPoint.ID);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }
    }
    public class LatticeEdge
    {
        public LatticePoint StartPoint {get; private set;}
        public LatticePoint EndPoint {get; private set;}
        public LatticeEdge (LatticePoint st, LatticePoint end) 
        {
            StartPoint = st;
            EndPoint = end;
        }
    }

    public class LatticePoint
    {
        internal long ID;
        public Addings Adds { get; set; }
    }
}
