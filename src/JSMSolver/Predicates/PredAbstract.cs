using JSMBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace JSMSolver.Predicates
{
    public interface IPredAbstract<T, TData> : IXmlSerializable where TData : JSMDataBase<T>
    {
        Expression GetExpression(StrContext<T, TData> ctxt, Expression ctxt_instance, params ParameterExpression[] pars);
    }

    public sealed class PredLeaf<T, TData> : IPredAbstract<T, TData> where TData : JSMDataBase<T>
    {
        public PredLeaf() { }
        public PredLeaf(Addings add) { Adding = add; }
        public Addings Adding { get; private set; }

        public Expression GetExpression(StrContext<T, TData> ctxt, Expression ctxt_instance, params ParameterExpression[] pars)
        {
            if (this.Adding == Addings.Simple)
                return Expression.Constant(true);
            var mis = ctxt.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .FirstOrDefault(attr => 
                    {
                        AddingInfoAttribute att = Attribute.GetCustomAttribute(attr, typeof(AddingInfoAttribute)) as AddingInfoAttribute;
                        return att != null && att.Adding == this.Adding;
                    });
            return Expression.Call(ctxt_instance, mis, pars);
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadStartElement();
            Adding = (Addings)reader.ReadContentAsInt();
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("Adding");
            writer.WriteValue(((int)Adding).ToString());
            writer.WriteEndElement();
        }
    }

    public sealed class PredExpr<T, TData> : IPredAbstract<T, TData> where TData : JSMDataBase<T>
    {
        public PredExpr() { }
        public PredExpr(Union un, IPredAbstract<T, TData> arg1, IPredAbstract<T, TData> arg2)
        {
            this.Union = un;
            this.Argument1 = arg1;
            this.Argument2 = arg2;
        }
        public Union Union { get; private set; }
        public IPredAbstract<T, TData> Argument1 { get; private set; }
        public IPredAbstract<T, TData> Argument2 { get; private set; }

        public Expression GetExpression(StrContext<T, TData> ctxt, Expression ctxt_instance, params ParameterExpression[] pars)
        {
            switch (Union)
            {
                case Predicates.Union.Conj:
                    return Expression.And(Argument1.GetExpression(ctxt, ctxt_instance, pars), Argument2.GetExpression(ctxt, ctxt_instance, pars));
                case Predicates.Union.Dizj:
                    return Expression.Or(Argument1.GetExpression(ctxt, ctxt_instance, pars), Argument2.GetExpression(ctxt, ctxt_instance, pars));
            }
            throw new NotSupportedException("Неизвестный тип союза "+Union);
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            Union un = (Predicates.Union)reader.ReadElementContentAsInt();
            Argument1 = new XmlSerializer(typeof(IPredAbstract<T, TData>)).Deserialize(reader) as IPredAbstract<T, TData>;
            Argument2 = new XmlSerializer(typeof(IPredAbstract<T, TData>)).Deserialize(reader) as IPredAbstract<T, TData>;
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("Union");
            writer.WriteValue((int)Union);
            writer.WriteEndElement();

            writer.WriteStartElement("Param1");
            new XmlSerializer(typeof(IPredAbstract<T, TData>)).Serialize(writer, Argument1);
            writer.WriteValue(Argument1);
            writer.WriteEndElement();

            writer.WriteStartElement("Param2");
            new XmlSerializer(typeof(IPredAbstract<T, TData>)).Serialize(writer, Argument2);
            writer.WriteEndElement();
        }
    }

    public sealed class PredAllExpression<T, TData> where TData : JSMDataBase<T>
    {
        public IPredAbstract<T, TData> MainPredicate { get; set; }
        public Func<Hypothesis<T, TData>, IEnumerable<TData>, IEnumerable<TData>, Boolean> GetExpression(Solver<T, TData> solv)
        {
            ParameterExpression par1 = Expression.Parameter(typeof(Hypothesis<T, TData>));
            ParameterExpression par2 = Expression.Parameter(typeof(IEnumerable<TData>));
            ParameterExpression par3 = Expression.Parameter(typeof(IEnumerable<TData>));
            StrContext<T, TData> ctxt = new StrContext<T, TData>(solv);
            Expression inst = Expression.Constant(ctxt);
            Expression exp = MainPredicate.GetExpression(ctxt, inst, par1, par2, par3);
            return Expression.Lambda<Func<Hypothesis<T, TData>, IEnumerable<TData>, IEnumerable<TData>, Boolean>>(exp, par1, par2, par3).Compile();
        }
    }
}
