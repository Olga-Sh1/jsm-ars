using JSMBase;
using JSMBaseC;
using JSMSolver.Predicates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMSolver
{
    public class SolverBase<T, TData> where TData : JSMDataBase<T>
    {
        /// <summary>Добавки</summary>
        public Dictionary<Signs, PredAllExpression<T, TData>> Addings { get; private set; }
        public Dictionary<Signs, IEnumerable<IReasoningType>> Types { get; private set; }
        /// <summary>Органичения по количеству родителей</summary>
        public Dictionary<Signs, CompareData> NumParents { get; private set; }
        /// <summary>Органичения по количеству признаков</summary>
        public Dictionary<Signs, CompareData> NumProps { get; private set; }
        /// <summary>Экономный режим</summary>
        public Boolean EconomyMode { get; set; }
        /// <summary>Фильтры</summary>
        public Dictionary<Signs, Func<Hypothesis<T, TData>, Boolean>> Filters { get; set; }
        /// <summary>Для исчерпываемости</summary>
        public int[] PropsF { get; set; }

        protected CandidateGroupData[] groups;
        public SolverBase()
        {
            
        }

        public virtual void Init(CompareData npl, CompareData nmn, CompareData nppl, CompareData npmn, Addings plus, Addings minus, Dictionary<Signs, IEnumerable<IReasoningType>> types, bool ec, CandidateGroupData[] groups)
        {
            //добавки
            this.Addings = new Dictionary<Signs, PredAllExpression<T, TData>>();
            PredAllExpression<T, TData> pr1 = new PredAllExpression<T, TData>();
            pr1.MainPredicate = createPred(plus);
            this.Addings.Add(Signs.Plus, pr1);
            PredAllExpression<T, TData> pr2 = new PredAllExpression<T, TData>();
            pr2.MainPredicate = createPred(minus);
            this.Addings.Add(Signs.Minus, pr2);
            this.Types = types;
            this.NumParents = new Dictionary<Signs, CompareData>();
            this.NumParents.Add(Signs.Plus, npl);
            this.NumParents.Add(Signs.Minus, nmn);
            this.NumProps = new Dictionary<Signs, CompareData>();
            this.NumProps.Add(Signs.Plus, nppl);
            this.NumProps.Add(Signs.Minus, npmn);
            this.EconomyMode = ec;
            this.groups = groups;
        }

        private IPredAbstract<T, TData> createPred(Addings a)
        {
            if (a == 0) return new PredLeaf<T, TData>(a);
            List<Addings> args = new List<Addings>();
            Addings[] adds = Enum.GetValues(typeof(Addings)).Cast<Addings>().ToArray();
            foreach (var add in adds)
                if (add != 0 && (add & a) == add)
                    args.Add(add);
            IPredAbstract<T, TData> st = new PredLeaf<T, TData>(args[0]);
            for (int i = 1; i < args.Count; i++)
            {
                PredLeaf<T, TData> arg2 = new PredLeaf<T, TData>(args[i]);
                PredExpr<T, TData> expr = new PredExpr<T, TData>(Union.Conj, st, arg2);
                st = expr;
            }
            return st;
        }

        public GeneralReasoning IsGen(Signs s)
        {
            if (this.Types == null) return null;
            if (!this.Types.ContainsKey(s)) return null;
            return this.Types[s].OfType<GeneralReasoning>().FirstOrDefault();
        }
    }
}
