using JSMBase.Medicine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSMBase;
using Antlr4.Runtime;
using JSMSolver.Filter.ANTLR;
using System.Linq.Expressions;
using HT = JSMBase.Hypothesis<JSMBase.Medicine.JSMMedicine, JSMBase.JSMBase>;
using System.Reflection;

namespace JSMSolver.Filter
{
    public sealed class FilterMedStr : BaseFilterStr<JSMMedicine, JSMBase.JSMBase>
    {
        AntlrGirdleTree serv = new AntlrGirdleTree();
        private void fillError(RecognitionException ex)
        {
            this.Error = ex.Message;
            this.Position = ex.OffendingToken.Column;
        }
        protected override Func<HT, bool> ParseFString(string f)
        {
            try
            {
                Func<HT, bool> ff = serv.ParseFString(f);
                return ff;
            }
            catch(RecognitionException ex)
            {
                fillError(ex);
            }
            catch
            {
                throw;
            }
            return null;
        }

       

        private Func<HT, bool> toLambda(FilterSyntaxParser.T3Context tt)
        {
            ParameterExpression p = Expression.Parameter(typeof(Hypothesis<JSMMedicine, JSMBase.JSMBase>));
            Expression ex = toExpression(tt, p);
            Expression<Func<HT, bool>> lambda = Expression.Lambda<Func<HT, bool>>(ex, p);
            return lambda.Compile();
        }

        private Expression toExpression(FilterSyntaxParser.T3Context tt, ParameterExpression p)
        {
            if (tt.exception != null)
            {
                fillError(tt.exception);
                throw new Exception();
            }
            var t2 = tt.t2();
            if (t2 != null) return toExpression(t2, p);
            var arr = tt.t22();
            Expression ex1 = toExpression(arr[0].t2(), p);
            for (int i = 1; i < arr.Length; i++)
            {
                Expression ex2 = toExpression(arr[i].t2(), p);
                ex1 = Expression.OrElse(ex1, ex2);
            }
            return ex1;
        }

        private Expression toExpression(FilterSyntaxParser.T2Context t2, ParameterExpression p)
        {
            if (t2.exception != null)
            {
                fillError(t2.exception);
                throw new Exception();
            }
            FilterSyntaxParser.T1Context[] arr = t2.t1();
            Expression ex1 = toExpression(arr[0], p);
            for(int i = 1; i < arr.Length; i++)
            {
                Expression ex2 = toExpression(arr[i], p);
                ex1 = Expression.AndAlso(ex1, ex2);
            }
            return ex1;
        }

        private Expression toExpression(FilterSyntaxParser.T1Context t1, ParameterExpression p)
        {
            if (t1.exception != null)
            {
                fillError(t1.exception);
                throw new Exception();
            }
            //Моделируем выражения: m.Body.IsEmptyProp(JSMMedicine.GetRealIndex(pi)
            MemberExpression propertyExp = Expression.Property(p, "Body");
            var nms = t1.NUM();
            String n = nms[0].GetText();
            int pn = int.Parse(n);
            bool eq = t1.CONN().GetText() == "=";
            var nl = t1.NULL();
            if (nl != null)
                return toNulExp(pn, p, eq);
            String n2 = nms[1].GetText();
            int pv = int.Parse(n);
            return toValExp(pn, p, eq, pv);
            //m.Body
        }

        private Expression toNulExp(int pn, ParameterExpression p, bool eq)
        {
            //m.Body
            //MemberExpression propertyExp = Expression.Property(p, "Body");

            //JSMMedicine.GetRealIndex(pi)
            ConstantExpression cex = Expression.Constant(pn);
            MethodInfo miContain = typeof(JSMMedicine).GetMethod("GetRealIndex", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(int) }, null);
            var strReal = Expression.Call(miContain, cex);

            //m.Body.IsEmptyProp
            MethodInfo miIsEmp = p.Type.GetMethod("IsEmptyProp");
            MethodCallExpression mcex = Expression.Call(p, miIsEmp, strReal);
            if (eq) return mcex;
            return Expression.Not(mcex);
        }

        private Expression toValExp(int pn, ParameterExpression p, bool eq, int val)
        {
            var propData = Expression.Property(p, "Data");
            Expression ex = checkInArray(pn, propData, val);
            if (eq) return ex;
            return Expression.Not(ex);
        }

        private MethodCallExpression checkInArray(int i, MemberExpression propData, int ic)
        {
            var index = Expression.ArrayIndex(propData, Expression.Constant(i));
            var pls = Expression.Property(index, "PlusValuesIndeces");
            var ms = typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name == "Contains" && m.GetParameters().Length == 2)
                .First();
            ms = ms.MakeGenericMethod(typeof(int));

            return Expression.Call(ms, pls, Expression.Constant(ic));
        }
    }
}
