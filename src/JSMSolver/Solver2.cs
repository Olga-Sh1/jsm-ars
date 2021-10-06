using JSMBase;
using JSMBase.Algorithm;
using JSMBase.SpecialAlgorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JSMSolver
{
    public class Solver2<T, TBody> where TBody : JSMDataBase<T> 
    {
        public Solver2()
        {
            Data = new Dictionary<Signs, IEnumerable<TBody>>();
            Addings = new Dictionary<Signs, JSMSolver.Addings>();
            Hypotheses = new Dictionary<Signs, List<Hypothesis<T, TBody>>>();
            Hypotheses.Add(Signs.Plus, new List<Hypothesis<T, TBody>>());
            Hypotheses.Add(Signs.Minus, new List<Hypothesis<T, TBody>>());
            Hypotheses.Add(Signs.Null, new List<Hypothesis<T, TBody>>());

        }
        #region Входные данные
        /// <summary>Данные</summary>
        public Dictionary<Signs, IEnumerable<TBody>> Data { get; private set; }
        /// <summary>Добавки</summary>
        public Dictionary<Signs, Addings> Addings { get; private set; }
       
        #endregion

        #region Выходные данные
        public Dictionary<Signs, List<Hypothesis<T, TBody>>> Hypotheses { get; private set; }
        #endregion

        public async Task Induction()
        {
            TaskFactory factory = new TaskFactory();
            await factory.StartNew(InductionInner, Signs.Plus);
            await factory.StartNew(InductionInner, Signs.Minus);
        }

        private void InductionInner(Object ob)
        {
            Signs s = (Signs)ob;
            var f = CreateFilter(s);
            BaseAlgorithms.AlgorithmNorris(Data[s], Hypotheses[s], null, new SaveMode(10, new JSMBaseC.CompareData(JSMBaseC.CompareOperator.MoreOrEquals, 10), null), CancellationToken.None,  f);
        }

        StrContext2<T, TBody> ctxt = new StrContext2<T, TBody>();
        private Func<Hypothesis<T, TBody>, Boolean> CreateFilter(Signs s)
        {
            if (Addings.ContainsKey(s) && (Addings[s] & JSMSolver.Addings.Contr) != 0)
            {
                var opp = Solver<T, TBody>.GetOpposite(s);
                return new Func<Hypothesis<T, TBody>, bool>(t => ctxt.ContrPredicate(t, Data[s], Data[opp]));
            }
            return null;
        }

        private void Clear()
        {
            foreach (var pair in Hypotheses)
                pair.Value.Clear();
        }
    }
}
