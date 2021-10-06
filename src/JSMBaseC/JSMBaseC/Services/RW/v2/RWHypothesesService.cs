using JSMBase;
using JSMBase.RNK;
using JSMBaseC.Services.RW.v2.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace JSMBaseC.Services.RW.v2
{
    public sealed class RWHypothesesService
    {
        IDataService serv;
        public RWHypothesesService(IDataService serv)
        {
            this.serv = serv;
        }
        public void Write(IList lpl, IList lmn, IList lnl, IList lplp, IList lmnp, IList lnlp, IList baseArray, String path)
        {
            Type t = null;
            if (serv.DataType != null)
            {
                t = typeof(SerializeWrapper<,>)
                .MakeGenericType(serv.DataType, serv.JSMWrapperDataType);
            }
           else
            {
                Type[] tgen = lpl.GetType().GetGenericArguments();
                Type[] tgen2 = tgen[0].GetGenericArguments();
                t = typeof(SerializeWrapper<,>).MakeGenericType(tgen2[0], tgen2[1]);
            }

            Type tl = typeof(List<>)
                .MakeGenericType(t);
            //Причины
            IList l1 = toList(lpl, tl, t);
            IList l2 = toList(lmn, tl, t);
            IList l3 = toList(lnl, tl, t);
            //Предсказания
            IList l1p = toList(lplp, tl, t);
            IList l2p = toList(lmnp, tl, t);
            IList l3p = toList(lnlp, tl, t);

            IList lb = toList(baseArray, tl, t);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (XmlWriter writer = XmlWriter.Create(path, settings))
            {
                XmlSerializer serializer = new XmlSerializer(l2.GetType());
                writer.WriteStartElement("HSD");
                writer.WriteAttributeString("bt", serv.DataType.AssemblyQualifiedName);
                writer.WriteAttributeString("jbt", serv.JSMWrapperDataType.AssemblyQualifiedName);
                writer.WriteStartElement("BaseArray");
                serializer.Serialize(writer, lb);
                writer.WriteEndElement();//BaseArray
                writer.WriteStartElement("Reason");
                writer.WriteStartElement(Signs.Plus.ToString());
                serializer.Serialize(writer, l1);
                writer.WriteEndElement();
                writer.WriteStartElement(Signs.Minus.ToString());
                serializer.Serialize(writer, l2);
                writer.WriteEndElement();
                writer.WriteStartElement(Signs.Null.ToString());
                serializer.Serialize(writer, l3);
                writer.WriteEndElement();
                writer.WriteEndElement();//Reason
                writer.WriteStartElement("Predictions");
                writer.WriteStartElement(Signs.Plus.ToString());
                serializer.Serialize(writer, l1p);
                writer.WriteEndElement();
                writer.WriteStartElement(Signs.Minus.ToString());
                serializer.Serialize(writer, l2p);
                writer.WriteEndElement();
                writer.WriteStartElement(Signs.Null.ToString());
                serializer.Serialize(writer, l3p);
                writer.WriteEndElement();
                writer.WriteEndElement();//Predictions
                writer.WriteEndElement();//HSD
            }
        }

        private IList toList(IList lbase, Type tl, Type t)
        {
            IList l2 = Activator.CreateInstance(tl, new object[] { lbase.Count }) as IList;
            foreach (var o1 in lbase)
            {
                Object o2 = Activator.CreateInstance(t, new object[] { o1 });
                l2.Add(o2);
            }
            return l2;
        }

        public Tuple<Dictionary<Signs, IList>, Dictionary<Signs, IList>, IList> Read(String path)
        {
            Dictionary<Signs, IList> res = new Dictionary<Signs, IList>();
            Dictionary<Signs, IList> preds = new Dictionary<Signs, IList>();
            IList data_in = null;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            using (XmlReader reader = XmlReader.Create(path, settings))
            {
                reader.Read();
                reader.Read();//HSB

                Type bt = Type.GetType(reader.GetAttribute("bt"));
                Type jbt = Type.GetType(reader.GetAttribute("jbt"));
                Type t = typeof(SerializeWrapper<,>)
                    .MakeGenericType(bt, jbt);

                Type tl = typeof(List<>)
                    .MakeGenericType(t);

                Type th = typeof(Hypothesis<,>)
                   .MakeGenericType(bt, jbt);

                Type tp = typeof(Prediction<,>)
                   .MakeGenericType(bt, jbt);

                Type tlh = typeof(List<>)
                    .MakeGenericType(th);

                Type tlp = typeof(List<>)
                    .MakeGenericType(tp);

                Type tlb = typeof(List<>)
                    .MakeGenericType(jbt);

                XmlSerializer serializer = new XmlSerializer(tl);
                reader.ReadStartElement();//BaseArray
                reader.ReadStartElement();//Array
                IList lb = serializer.Deserialize(reader) as IList;
                IList lb1 = toDataList(lb, tlb);
                data_in = lb1;
                reader.ReadEndElement();//BaseArray
                reader.ReadStartElement();//Reason
                Signs s1 = readerToSign(reader);
                reader.ReadStartElement();//Plus
                IList l1 = serializer.Deserialize(reader) as IList;
                res.Add(s1, toHypList(l1, lb1, tlh));
                reader.ReadEndElement();//Plus
                Signs s2 = readerToSign(reader);
                reader.ReadStartElement();//Minus
                IList l2 = serializer.Deserialize(reader) as IList;
                res.Add(s2, toHypList(l2, lb1, tlh));
                reader.ReadEndElement();//Minus
                Signs s3 = readerToSign(reader);
                reader.ReadStartElement();//Null
                IList l3 = serializer.Deserialize(reader) as IList;
                res.Add(s3, toHypList(l3, lb1, tlh));
                reader.ReadEndElement();//Null
                reader.ReadEndElement();//Reason
                reader.ReadStartElement();//Predictions
                Signs s1p = readerToSign(reader);
                reader.ReadStartElement();//Plus
                IList l1p = serializer.Deserialize(reader) as IList;
                preds.Add(s1p, toPredList(l1p, res[s1p], tlp));
                reader.ReadEndElement();//Plus
                Signs s2p = readerToSign(reader);
                reader.ReadStartElement();//Minus
                IList l2p = serializer.Deserialize(reader) as IList;
                preds.Add(s2p, toPredList(l2p, res[s2p], tlp));
                reader.ReadEndElement();//Minus
                Signs s3p = readerToSign(reader);
                reader.ReadStartElement();//Null
                IList l3p = serializer.Deserialize(reader) as IList;
                preds.Add(s3, toPredList(l3p, res[s3p], tlp));
                reader.ReadEndElement();//Null
                reader.ReadEndElement();//Predictions
            }
            return new Tuple<Dictionary<Signs, IList>, Dictionary<Signs, IList>, IList>(res, preds, data_in);
        }

        public IList ReadBase(String path)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            using (XmlReader reader = XmlReader.Create(path, settings))
            {
                reader.Read();
                reader.Read();//HSB

                Type bt = Type.GetType(reader.GetAttribute("bt"));
                Type jbt = Type.GetType(reader.GetAttribute("jbt"));
               
                Type t = typeof(SerializeWrapper<,>)
                    .MakeGenericType(bt, jbt);

                Type tl = typeof(List<>)
                    .MakeGenericType(t);
                /*
               Type th = typeof(Hypothesis<,>)
                  .MakeGenericType(bt, jbt);

               Type tp = typeof(Prediction<,>)
                  .MakeGenericType(bt, jbt);

               Type tlh = typeof(List<>)
                   .MakeGenericType(th);

               Type tlp = typeof(List<>)
                   .MakeGenericType(tp);
                */
                Type tlb = typeof(List<>)
                   .MakeGenericType(jbt);
               

                XmlSerializer serializer = new XmlSerializer(tl);
                reader.ReadStartElement();//BaseArray
                reader.ReadStartElement();//Array
                IList lb = serializer.Deserialize(reader) as IList;
                IList lb1 = toDataList(lb, tlb);
                reader.ReadEndElement();//BaseArray
                return lb1;
            }
        }

        private Signs readerToSign(XmlReader reader)
        {
            return (Signs)Enum.Parse(typeof(Signs), reader.Name);
        }

        private IList toDataList(IList lbase, Type thl)
        {
            IList l = Activator.CreateInstance(thl, new object[] { lbase.Count }) as IList;
            foreach (var o in lbase)
            {
                Object o2 = (o as IRestorable).Restore();
                l.Add(o2);
            }
            return l;
        }

        private IList toHypList(IList lbase, IList ldata, Type thl)
        {
            IList l = Activator.CreateInstance(thl, new object[] { lbase.Count }) as IList;
            foreach(var o in lbase)
            {
                Object o2 = (o as IRestorable).Restore(ldata);
                l.Add(o2);
            }
            return l;
        }

        private IList toPredList(IList lbase, IList lhyp, Type thl)
        {
            IList l = Activator.CreateInstance(thl, new object[] { lbase.Count }) as IList;
            foreach (var o in lbase)
            {
                Object o2 = (o as IRestorable).RestorePred(lhyp);
                l.Add(o2);
            }
            return l;
        }
    }
}
