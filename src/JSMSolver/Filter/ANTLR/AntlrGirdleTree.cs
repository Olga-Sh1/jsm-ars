using Antlr4.Runtime;
using JSMBase;
using JSMBase.RNK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using HT = JSMBase.Hypothesis<JSMBase.Medicine.JSMMedicine, JSMBase.JSMBase>;

namespace JSMSolver.Filter.ANTLR
{
    public class AntlrGirdleTree
    {
        public Func<HT, bool> ParseFString(string f)
        {
            AntlrInputStream inputStream = new AntlrInputStream(f);
            FilterSyntaxLexer lexer = new FilterSyntaxLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            FilterSyntaxParser parser = new FilterSyntaxParser(commonTokenStream);
            FilterSyntaxParser.T3Context tt = parser.t3();
            Func<HT, bool> ff = toLambda(tt);
            return ff;
        }


        private Func<HT, bool> toLambda(FilterSyntaxParser.T3Context tt)
        {
            ParameterExpression p = Expression.Parameter(typeof(Hypothesis<MedicineExample, JSMDataMedRnk>));
            Expression ex = toExpression(tt, p);
            Expression<Func<HT, bool>> lambda = Expression.Lambda<Func<HT, bool>>(ex, p);
            return lambda.Compile();
        }

        private Expression toExpression(FilterSyntaxParser.T3Context tt, ParameterExpression p)
        {
            if (tt.exception != null)
            {
                throw tt.exception;
            }
            var t2 = tt.t2();
            if (t2 != null) return toExpression(t2, p);
            var arr = tt.t22();
            Expression ex1 = toExpression(arr[0].t2(), p);
            for (int i = 1; i < arr.Length; i++)
            {
                Expression ex2 = toExpression(arr[i].t2(), p);
                ex1 = Expression.AndAlso(ex1, ex2);
            }
            return ex1;
        }

        private Expression toExpression(FilterSyntaxParser.T2Context t2, ParameterExpression p)
        {
            if (t2.exception != null)
            {
                throw t2.exception;
            }
            FilterSyntaxParser.T1Context[] arr = t2.t1();
            Expression ex1 = toExpression(arr[0], p);
            for (int i = 1; i < arr.Length; i++)
            {
                Expression ex2 = toExpression(arr[i], p);
                ex1 = Expression.OrElse(ex1, ex2);
            }
            return ex1;
        }

        private Expression toExpression(FilterSyntaxParser.T1Context t1, ParameterExpression p)
        {
            if (t1.exception != null)
            {
                throw t1.exception;
            }
            //Моделируем выражения: m.Inner.PropValues[i].IsEmpty
            var nms = t1.NUM();
            String n = nms[0].GetText();
            int pn = int.Parse(n);
            int rpn = pn - 1;
            bool eq = t1.CONN().GetText() == "=";
            var nl = t1.NULL();

            //m.Body
            MemberExpression p2 = Expression.Property(p, "Body");

            //m.Body.Inner
            MemberExpression propertyExp = Expression.Property(p2, "Inner");
            //m.Body.Inner.PropValues
            MemberExpression propertyExp2 = Expression.Property(propertyExp, "PropValues");


            //m.Inner.PropValues[i]
            ConstantExpression cex = Expression.Constant(rpn);
            BinaryExpression indexex = Expression.ArrayIndex(propertyExp2, cex);

            //m.Inner.PropValues[i] == null
            ConstantExpression cnull = Expression.Constant(null, typeof(IPropValue));
            BinaryExpression bex = Expression.Equal(indexex, cnull);

            //m.Inner.PropValues[i].IsEmpty
            MemberExpression propertyExp3 = Expression.Property(indexex, "IsEmpty");

            //m.Inner.PropValues[i] == null OR m.Inner.PropValues[i].IsEmpty
            BinaryExpression res = Expression.OrElse(bex, propertyExp3);
            if (eq) return res;
            return Expression.Not(res);
        }
    }
}
