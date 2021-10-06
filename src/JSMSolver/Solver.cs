using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JSMBase.Medicine;
using System.Collections.ObjectModel;
using JSMBase;
using System.Threading.Tasks;
using JSMBase.Algorithm;
using JSMSolver.Predicates;
using System.Diagnostics;
using System.Threading;
using JSMBaseC;
using JSMBaseC.Services.Diagram;
//using JSM.Services.Diagram;

namespace JSMSolver
{
    /// <summary>Решатель</summary>
    public class Solver<T, TData> : SolverBase<T, TData>, ISolver where TData : JSMDataBase<T>
    {
        /// <summary>Входные данные</summary>
        public Dictionary<Signs, ReadOnlyCollection<TData>> Data { get; private set; }
        /// <summary>Гипотезы</summary>
        public Dictionary<Signs, List<Hypothesis<T, TData>>> Hypotheses { get; private set; }
        public Dictionary<Signs, int> HypothesesCount { get; private set; }
       
        
        /// <summary>Доопределения</summary>
        public Dictionary<Signs, List<TData>> Enclosures { get; set; }
        public Dictionary<Signs, int> EnclosuresCount { get; set; }
       
        /// <summary>Обязательные объекты</summary>
        public ReadOnlyCollection<TData> ObligatoryObjects { get; set; }
        
        public SolverMessages Messages { get; private set; }
        /// <summary>Тормоза для (+)-гипотез</summary>
        public Dictionary<Hypothesis<T, TData>, Hypothesis<T, TData>[]> BreaksPlus { get; private set; }
        /// <summary>Тормоза для (-)-гипотез</summary>
        public Dictionary<Hypothesis<T, TData>, Hypothesis<T, TData>[]> BreaksMinus { get; private set; }
        /// <summary>Доопределения</summary>
        public Dictionary<JSMDataBase<T>, IEnumerable<Hypothesis<T, TData>>> Dependencies { get; private set; }
        /// <summary>Контекст для стратегий</summary>
        protected StrContext<T, TData> _context;
        public Solver()
        {
            //исходные данные
            this.Data = new Dictionary<Signs, ReadOnlyCollection<TData>>();
            //гипотезы
            this.Hypotheses = new Dictionary<Signs, List<Hypothesis<T, TData>>>();
            this.Hypotheses.Add(Signs.Plus, new List<Hypothesis<T, TData>>());
            this.Hypotheses.Add(Signs.Minus, new List<Hypothesis<T, TData>>());
            this.Hypotheses.Add(Signs.Null, new List<Hypothesis<T, TData>>());
            this.HypothesesCount = new Dictionary<Signs, int>();
            this.HypothesesCount.Add(Signs.Plus, 0);
            this.HypothesesCount.Add(Signs.Minus, 0);
           
            //доопределения
            this.Enclosures = new Dictionary<Signs, List<TData>>();
            this.Enclosures.Add(Signs.Plus, new List<TData>());
            this.Enclosures.Add(Signs.Minus, new List<TData>());
            this.Enclosures.Add(Signs.Null, new List<TData>());
            this.EnclosuresCount = new Dictionary<Signs, int>();
            this.EnclosuresCount.Add(Signs.Plus, 0);
            this.EnclosuresCount.Add(Signs.Minus, 0);
            this.EnclosuresCount.Add(Signs.Null, 0);
            
            //контекст для стратегий
            _context = new StrContext<T, TData>(this);
            //if (l != null)
            //    ObligatoryObjects = new ReadOnlyCollection<JSMDataBase<T>>(l);
            //EconomyMode = ec;
            this.Dependencies = new Dictionary<JSMDataBase<T>, IEnumerable<Hypothesis<T, TData>>>();
        }


        public override void Init(CompareData npl, CompareData nmn, CompareData nppl, CompareData npmn, Addings plus, Addings minus, Dictionary<Signs, IEnumerable<IReasoningType>> types, Boolean ec, CandidateGroupData[] groups)
        {
            base.Init(npl, nmn, nppl, npmn, plus, minus, types, ec, groups);
        }

        /// <summary>Найти пересечения с другими объектами в массиве</summary>
        /// <param name="main">Выбранный объект</param>
        public void FindSimilarItems(TData main)
        {
            ClearCache();
            uint index = 1;
            Hypothesis<T, TData> mainHyp = new Hypothesis<T, TData>(main);
            foreach (var pair in Data)
            {
                if (pair.Key == Signs.Tau) continue;
                foreach (var ex in pair.Value)
                {
                    if (ex.ID != main.ID)
                    {
                        Hypothesis<T, TData> res = null;
                        var inter = mainHyp.Intersect(new Hypothesis<T, TData>(ex), out res);
                        if ((inter & IntersectResult.CreateNew) == IntersectResult.CreateNew)
                        {
                            res.IsRealHyp = true;
                            //res.Index = index++;
                            Hypotheses[pair.Key].Add(res);
                        }
                    }
                }
            }
        }

        const double genThresh = 0.5;

        public async Task JSMNonAthomic(CancellationToken ct, IProgress<Tuple<Signs, String>> pr)
        {
            ClearCache();
            Messages = SolverMessages.None;
            await Induction(groups, ct, pr);
            CheckForNulls();
            Analogy();
            Explaination();
            foreach (var pair in Hypotheses)
                HypothesesCount[pair.Key] = pair.Value.Count;
            foreach (var pair in Enclosures)
                EnclosuresCount[pair.Key] = pair.Value.Count;
        }
        public async Task JSMSimple(CancellationToken ct, IProgress<int> pr, bool step = false)
        {
            if (pr != null) pr.Report(0);
            ClearCache();
            Messages = SolverMessages.None;
            if (step)
                await InductionNext(ct);
            else
            {
                var t = PrepareObligatory();
                _cache.Add("hPlusSt", t.Item3);
                _cache.Add("hMinSt", t.Item4);
                await Induction(t.Item1, t.Item2, t.Item3, t.Item4, ct, pr);
            }
            #region Обобщенный метод
            bool auto_threshold = false;
            var g1 = IsGen(Signs.Plus);
            var g2 = IsGen(Signs.Minus);
            if (g1 != null)
            {
                this.BreaksPlus = await GetBreaks(Signs.Plus, ct, g1.k);
                if (ct.IsCancellationRequested) return;
            }
            if (g2 != null)
            {
                this.BreaksMinus = await GetBreaks(Signs.Minus, ct, g2.k);
                if (ct.IsCancellationRequested) return;
            }
            if (g1 != null)
            {
                foreach (var pair in this.BreaksPlus)
                    foreach (var br in pair.Value)
                    {
                        foreach (JSMDataBase<T> par in pair.Key.ParentList)
                        {
                            if (br.Body.IsEnclosed(par))
                            {
                                pair.Key.IsRealHyp = false;
                                break;
                            }
                        }
                        //if (!isEncPar) 
                    }
            }
            if (g2 != null)
            {
                foreach (var pair in this.BreaksMinus)
                    foreach (var br in pair.Value)
                        foreach (JSMDataBase<T> par in pair.Key.ParentList)
                        {
                            if (br.Body.IsEnclosed(par))
                            {
                                pair.Key.IsRealHyp = false;
                                break;
                            }
                        }
            }
            #endregion
            CheckForNulls();
            CheckFull();
            Analogy();
            Explaination();
            //ClosureAnalyse(Signs.Plus);
            //ClosureAnalyse(Signs.Minus);
            foreach (var pair in Hypotheses)
                HypothesesCount[pair.Key] = pair.Value.Count;
            foreach (var pair in Enclosures)
                EnclosuresCount[pair.Key] = pair.Value.Count;

            
        }

        private void CheckFull()
        {
            if (PropsF != null && PropsF.Length > 0 && typeof(TData).GetInterfaces().Contains((typeof(IPropertyEqual))))
            {
                checkFullSign(Hypotheses[Signs.Plus]);
                checkFullSign(Hypotheses[Signs.Minus]);
            }
        }

        private void checkFullSign(IList<Hypothesis<T, TData>> hyps)
        {
            DiagramGirdleService<T, TData> service = new DiagramGirdleService<T, TData>();
            List<Hypothesis<T, TData>> current_list = hyps
                .Where(h => h.IsRealHyp).ToList();
            if (current_list.Count == 0) return;
            do
            {
                IPropertyEqual current = current_list[0].Body as IPropertyEqual;
                Hypothesis<T, TData>[] arr = current_list.Where(c => (c.Body as IPropertyEqual).IsEqualProp(current, PropsF)).ToArray();
                foreach (Hypothesis<T, TData> ob in arr)
                {
                    current_list.Remove(ob);
                }
                var mins = service.FindMins(arr);
                foreach (Hypothesis<T, TData> ob in arr)
                    ob.IsRealHyp = mins.Contains(ob);
            }
            while (current_list.Count > 0);
        }

        Dictionary<String, Object> _cache = new Dictionary<string,object>();
        private void ClearCache()
        {
            _cache.Clear();
            this.Dependencies.Clear();
        }
        #region Индукция
        private async Task Induction(CandidateGroupData[] groups, CancellationToken ct, IProgress<Tuple<Signs, String>> pr)
        {
            //DoInduction(Signs.Plus, groups, pr);
            Task tPl = Task.Run(() =>  { DoInduction(Signs.Plus, groups, pr, ct); }, ct);
            Task tMn = Task.Run(() => { DoInduction(Signs.Minus, groups, pr, ct); }, ct);
            try
            {
                await Task.WhenAll(new Task[] { tPl, tMn });
            }
            catch (AggregateException ex)
            {
                throw ex.InnerException;
            }
            catch (OperationCanceledException ex)
            {
                return;
            }

            if (ct.IsCancellationRequested) return;
            InductionEnd();
        }

        private void DoInduction(Signs s, CandidateGroupData[] groups, IProgress<Tuple<Signs, String>> pr, CancellationToken ct)
        {
            Candidate<T, TData> start = new Candidate<T, TData>();
            start.InitParents(Data[s]);
            List<Candidate<T, TData>> lst = new List<Candidate<T, TData>>();
            lst.Add(start);
            for(int i = 0; i < groups.Length - 1; i++)
            {
                ct.ThrowIfCancellationRequested();
                var group = groups[i];
                if (pr != null) pr.Report(Tuple.Create<Signs, String>(s, String.Join(",", group)));
                List<Candidate<T, TData>> lstbf = new List<Candidate<T, TData>>();
                foreach (Candidate<T, TData> c in lst)
                {
                    lstbf.AddRange(c.DoStep(group));
                }
                lst = lstbf;
            }
            CandidateGroupData gr_lst = groups.Last();
            List<Candidate<T, TData>> res = new List<Candidate<T, TData>>();
            foreach (var c in lst)
            {
                ct.ThrowIfCancellationRequested();
                var r = c.DoLastStep(gr_lst);
                if (r != null)
                    res.Add(r);
            }

            Hypotheses[s].AddRange(res.Select((l, i) =>
            {
                var hh = l.ToHypothesis(i + 1);
                hh.IsRealHyp = true;
                return hh;
            }));
        }

        /// <summary>Индукция на первом шаге</summary><returns></returns>
        private async Task Induction(IEnumerable<TData> oblsPlus, IEnumerable<TData> oblsMinus, Hypothesis<T, TData> hPlusStart, Hypothesis<T, TData> hMinusStart, CancellationToken ct, IProgress<int> pr)
        {
            CustomProgress cp = new CustomProgress(pr);
            
            Task tPl = Task.Run(() =>
            {
                if (NeedStart(Signs.Plus))
                {
                    SaveMode sm = EconomyMode ? new SaveMode(8, NumParents[Signs.Plus], NumProps[Signs.Plus]) : SaveMode.None;
                    JSMBase.Algorithm.BaseAlgorithms.AlgorithmNorris<T, TData>(Data[Signs.Plus].Except(oblsPlus).ToArray(), Hypotheses[Signs.Plus], hPlusStart, sm, ct, Filters == null ? null : Filters[Signs.Plus], cp);
                }
            }, ct);
            Task tMn = Task.Run(() =>
            {
                if (NeedStart(Signs.Minus))
                {
                    SaveMode sm = EconomyMode ? new SaveMode(8, NumParents[Signs.Minus], NumProps[Signs.Minus]) : SaveMode.None;
                    JSMBase.Algorithm.BaseAlgorithms.AlgorithmNorris<T, TData>(Data[Signs.Minus].Except(oblsMinus).ToArray(), Hypotheses[Signs.Minus], hMinusStart, sm, ct, Filters == null ? null : Filters[Signs.Minus], cp);
                }
            }, ct);

            /*
        if (NeedStart(Signs.Plus))
        {
            SaveMode sm = EconomyMode ? new SaveMode(8, NumParents[Signs.Plus], NumProps[Signs.Plus]) : SaveMode.None;
                await Task.Run(() =>
                    JSMBase.Algorithm.BaseAlgorithms.AlgorithmNorris<T, TData>(Data[Signs.Plus].Except(oblsPlus).ToArray(), Hypotheses[Signs.Plus], hPlusStart, sm, ct, Filters == null ? null : Filters[Signs.Plus]), ct);
        }
        if (ct.IsCancellationRequested) return;
        if (NeedStart(Signs.Minus))
        {
                SaveMode sm = EconomyMode ? new SaveMode(8, NumParents[Signs.Minus], NumProps[Signs.Minus]) : SaveMode.None;
                await Task.Run(() =>
                    JSMBase.Algorithm.BaseAlgorithms.AlgorithmNorris<T, TData>(Data[Signs.Minus].Except(oblsMinus).ToArray(), Hypotheses[Signs.Minus], hMinusStart, sm, ct, Filters == null ? null : Filters[Signs.Minus]), ct);
        }
        */
            try
            {
                await Task.WhenAll(new Task[] { tPl, tMn });
            }
            catch(AggregateException ex)
            {
                throw ex.InnerException;
            }
            catch(OperationCanceledException ex)
            {
                return;
            }
            
            if (ct.IsCancellationRequested) return;
            InductionEnd();
        }
        /// <summary>Индукция на последующем после первого шагах</summary><returns></returns>
        private async Task InductionNext(CancellationToken ct)
        {
            
            await Task.Run(() =>
                JSMBase.Algorithm.BaseAlgorithms.AlgorithmNorris<T, TData>(Enclosures[Signs.Plus].Skip(EnclosuresCount[Signs.Plus]).ToArray(), Hypotheses[Signs.Plus], _cache["hPlusSt"] as Hypothesis<T, TData>, SaveMode.None, ct,  Filters[Signs.Plus]));
            await Task.Run(() =>
                JSMBase.Algorithm.BaseAlgorithms.AlgorithmNorris<T, TData>(Enclosures[Signs.Minus].Skip(EnclosuresCount[Signs.Minus]), Hypotheses[Signs.Minus], _cache["hMinSt"] as Hypothesis<T, TData>, SaveMode.None, ct, Filters[Signs.Minus]));
            InductionEnd();
        }
        private void InductionEnd()
        {
            CheckForList(NumParents[Signs.Plus], NumProps[Signs.Plus], Hypotheses[Signs.Plus]);
            CheckForList(NumParents[Signs.Minus], NumProps[Signs.Minus], Hypotheses[Signs.Minus]);

            _context.ThroughAddings2(Addings);

        }

        private void CheckForList(CompareData cdParent, CompareData cdProps, List<Hypothesis<T, TData>> lst)
        {
            if (cdParent != null)
                switch (cdParent.Operator)
                {
                    case CompareOperator.LessOrEquals:
                        foreach (var h in lst)
                            h.IsRealHyp = h.ParentList.Count <= cdParent.Value && h.ParentList.Count > 1;
                        break;
                    case CompareOperator.MoreOrEquals:
                        foreach (var h in lst)
                            h.IsRealHyp = h.ParentList.Count >= cdParent.Value;
                        break;
                }

            if (cdProps != null)
                switch(cdProps.Operator)
                {
                    case CompareOperator.LessOrEquals:
                        foreach (var h in lst)
                            h.IsRealHyp = (cdParent == null ? true : h.IsRealHyp) &&  h.Body.CountNonEmptyProps <= cdParent.Value;
                        break;
                    case CompareOperator.MoreOrEquals:
                        foreach(var h in lst)
                            h.IsRealHyp = (cdParent == null ? true : h.IsRealHyp) && h.Body.CountNonEmptyProps >= cdParent.Value;
                        break;
                }
        }

        private void CheckForNulls()
        {
            Hypothesis<T, TData>[] hyps1 = Hypotheses[Signs.Plus].Where(h => h.IsRealHyp).ToArray();
            Hypothesis<T, TData>[] hyps2 = Hypotheses[Signs.Minus].Where(h => h.IsRealHyp).ToArray();
            List<Tuple<Hypothesis<T, TData>, Hypothesis<T, TData>>> lst = new List<Tuple<Hypothesis<T, TData>, Hypothesis<T, TData>>>();
            foreach (var h1 in hyps1)
                foreach (var h2 in hyps2)
                {
                    if (h1.Body.IsJSMEqual(h2.Body))
                    {
                       
                        if (BreaksPlus != null && BreaksMinus != null)
                        {
                            #region Обобщенный метод
                            if (!BreaksPlus.ContainsKey(h1) && !BreaksMinus.ContainsKey(h2))
                                lst.Add(Tuple.Create(h1, h2));
                            else if (BreaksPlus.ContainsKey(h1) && BreaksMinus.ContainsKey(h2))
                            {
                                var br1 = BreaksPlus[h1];
                                var br2 = BreaksMinus[h2];
                                if (br1.Length == br2.Length)
                                {
                                    foreach (var bro1 in br1)
                                    {
                                        bool bc = false;
                                        foreach (var bro2 in br2)
                                        {
                                            if (bro1.Body.Equals(bro2.Body))
                                            {
                                                bc = true;
                                                break;
                                            }
                                        }
                                        if (!bc) break;
                                    }
                                    lst.Add(Tuple.Create(h1, h2));
                                }
                            }
                            #endregion
                        }
                        else
                            lst.Add(Tuple.Create(h1, h2));
                    }
                }
            foreach (var pair in lst)
            {
                Hypotheses[Signs.Plus].Remove(pair.Item1);
                Hypotheses[Signs.Minus].Remove(pair.Item2);
                //if (!Hypotheses[Signs.Null].Contains(pair.Item1))
                    Hypotheses[Signs.Null].Add(pair.Item1);
            }
        }
        /// <summary>Обработка обязательных гипотез</summary>
        private Tuple<IEnumerable<TData>, IEnumerable<TData>, Hypothesis<T, TData>, Hypothesis<T, TData>> PrepareObligatory()
        {
            if (ObligatoryObjects != null && ObligatoryObjects.Count > 0)
            {
                List<TData> lstPlusOb = new List<TData>();
                List<TData> lstMinusOb = new List<TData>();
                foreach (var med in ObligatoryObjects)
                {
                    if (Data[Signs.Plus].Contains(med))
                        lstPlusOb.Add(med);
                    else if (Data[Signs.Minus].Contains(med))
                        lstMinusOb.Add(med);
                }
                Hypothesis<T, TData > h = SetStartHypotheses(lstPlusOb, Signs.Plus);
                if (h != null) Hypotheses[Signs.Plus].Add(h);
                Hypothesis<T, TData> h2 = SetStartHypotheses(lstMinusOb, Signs.Minus);
                if (h2 != null) Hypotheses[Signs.Minus].Add(h2);
                return Tuple.Create<IEnumerable<TData>, IEnumerable<TData>, Hypothesis<T, TData>, Hypothesis<T, TData>>(lstPlusOb, lstMinusOb, h, h2);
            }
            return Tuple.Create<IEnumerable<TData>, IEnumerable<TData>, Hypothesis<T, TData>, Hypothesis<T, TData>>(new TData[0], new TData[0], null, null);
        }
        private Boolean NeedStart(Signs s)
        {
            if (Messages == SolverMessages.None) return true;
            if (s == Signs.Minus && Messages == SolverMessages.MinusOblNotCompatible) return false;
            if (s == Signs.Plus && Messages == SolverMessages.PlusOblNotCompatible) return false;
            return true;
        }
        private Hypothesis<T, TData> SetStartHypotheses(List<TData> lst, Signs s)
        {
            if (lst.Count == 0) return null;
            Hypothesis<T, TData> h = new Hypothesis<T, TData>(lst[0]);
            for (int i = 1; i < lst.Count;i++)
            {
                Hypothesis<T, TData> h1 = new Hypothesis<T, TData>(lst[i]);
                Hypothesis<T, TData> h2 = null;
                IntersectResult res;
                if ((res = h.Intersect(h1, out h2)) == IntersectResult.CreateNew)
                    h = h2;
                else if (res != IntersectResult.IsNull)
                    h.ParentList.Add(lst[i]);
                else if (s == Signs.Plus)
                    Messages &= SolverMessages.PlusOblNotCompatible;
                else
                    Messages &= SolverMessages.MinusOblNotCompatible;
            }
            return h;
        }
        #endregion

        public void Analogy()
        {
            foreach (var data in Data[Signs.Tau])
            {
                //если ещё не доопределили
                if (!Enclosures[Signs.Null].Contains(data) && !Enclosures[Signs.Plus].Contains(data) && !Enclosures[Signs.Minus].Contains(data))
                {
                    Boolean b1 = false;
                    double max_pl = 0d;
                    List<Hypothesis<T, TData >> lst = new List<Hypothesis<T, TData>>();
                    foreach(var h in Hypotheses[Signs.Plus])
                        if (h.IsRealHyp && h.Body.IsEnclosed(data))
                        {
                            if (BreaksPlus != null)
                            {
                                 bool isEnc = true;
                                 if (BreaksPlus.ContainsKey(h))
                                 {
                                     var breaks = BreaksPlus[h];
                                     foreach (var opp in breaks)
                                     {
                                         if (opp.Body.IsEnclosed(data))
                                         {
                                             isEnc = false;
                                             break;
                                         }
                                     }
                                 }
                                if (isEnc) b1 = true;
                            }
                            else
                            b1 = true;
                            if (b1) lst.Add(h);
                            max_pl = Math.Max(max_pl, h.Weight);
                        }
                    Boolean b2 = false;
                    double max_mn = 0d;
                    foreach (var h in Hypotheses[Signs.Minus])
                        if (h.IsRealHyp && h.Body.IsEnclosed(data))
                        {
                            if (BreaksMinus != null)
                            {
                                bool isEnc = true;
                                if (BreaksMinus.ContainsKey(h))
                                {
                                    var breaks = BreaksMinus[h];
                                    foreach (var opp in breaks)
                                    {
                                        if (opp.Body.IsEnclosed(data))
                                        {
                                            isEnc = false;
                                            break;
                                        }
                                    }
                                }
                                if (isEnc) b2 = true;
                            }
                            else b2 = true;
                            if (b2) lst.Add(h);
                            max_mn = Math.Max(max_mn, h.Weight);
                        }
                    Boolean isStopped = false;
                    double max_nl = 0d;
                    
                    foreach (var h in Hypotheses[Signs.Null])
                    {
                        if (h.Body.IsEnclosed(data))
                        {
                            Enclosures[Signs.Null].Add(data);
                            isStopped = true;
                            break;
                        }
                    }
                    if (b1 || b2) Dependencies.Add(data, lst);
                     
                    if (isStopped) continue;
                    //if (max_mn == max_nl && max_mn == max_pl)
                    //{
                        if (b1 && b2)
                            Enclosures[Signs.Null].Add(data);
                        else
                        {
                            if (b1)
                                Enclosures[Signs.Plus].Add(data);
                            if (b2)
                                Enclosures[Signs.Minus].Add(data);
                        }
                    //}
                    /*
                    else
                    {
                        Dictionary<Signs, double> dict = new Dictionary<Signs, double>()
                        {
                            { Signs.Plus, max_pl },
                            { Signs.Minus, max_mn },
                            { Signs.Null, max_nl }
                        };
                        var max = dict.Max(p => p.Value);
                        var pair = dict.Where(p => p.Value == max);
                        if (pair.Count() > 1)
                            Enclosures[Signs.Null].Add(data);
                        else
                        {
                            Enclosures[pair.First().Key].Add(data);
                        }
                    }
                     */
                }
    }
        }
        private void Explaination()
        {
            foreach (JSMDataBase<T> d in Data[Signs.Plus])
                foreach (var h in Hypotheses[Signs.Plus])
                    if (h.IsRealHyp)
                        if (h.Body.IsEnclosed(d))
                            h.Type &= HypFeatures.Explaining;

            foreach (JSMDataBase<T> d in Data[Signs.Minus])
                foreach (var h in Hypotheses[Signs.Minus])
                    if (h.IsRealHyp)
                        if (h.Body.IsEnclosed(d))
                            h.Type &= HypFeatures.Explaining;
        }
        private void ClosureAnalyse(Signs s)
        {
            var arr = Hypotheses[s].Where(h => h.IsRealHyp).ToArray();
            foreach (var h1 in arr)
            {
                List<Hypothesis<T, TData>> lst = new List<Hypothesis<T, TData>>();
                foreach (var h2 in arr)
                {
                    if (h1 != h2)
                    {
                        TData res = h1.Body.Intersect(h2.Body) as TData;
                        if (!res.IsEmpty)
                            lst.Add(h2);
                    }
                }

                if (lst.Count > 1)
                {
                    var fin = lst[0].Body;
                    for (int i = 1; i < lst.Count; i++)
                    {
                        fin = fin.Intersect(lst[i].Body) as TData;
                        if (fin.IsEmpty) break;
                    }
                    h1.IsLeaf = fin.Equals(h1.Body);
                }
            }
        }
        internal static Signs GetOpposite(Signs s)
        {
            switch (s)
            {
                case Signs.Minus: return Signs.Plus;
                case Signs.Plus: return Signs.Minus;
                default: return s;
            }
        }
        #region Тормоза
        public async Task<IEnumerable<Hypothesis<T, TData>>> GetBreaks(Hypothesis<T, TData> hyp, CancellationToken ct, Int32 threshold = 2, Boolean canBeEnclosed = true)
        {
            if (threshold < 2) throw new ArgumentException("Порог для \"Тормозов\" не может быть меньше 2");
            Signs mySign = Hypotheses[Signs.Plus].Contains(hyp) ? Signs.Plus : Signs.Minus;
            Signs opp = GetOpposite(mySign);
            List<TData> lst = new List<TData>();
            foreach (TData hOpp in Data[opp])
            {
                if (hyp.Body.IsEnclosed(hOpp))
                    lst.Add(hOpp);
            }
            if (lst.Count >= threshold)
            {
                TData[] arr = lst.Select(l => l.Difference(hyp.Body) as TData).ToArray();
                List<Hypothesis<T, TData>> hyps = new List<Hypothesis<T, TData>>();
                await Task.Run(() =>
                    JSMBase.Algorithm.BaseAlgorithms.AlgorithmNorris<T, TData>(arr, hyps, null, SaveMode.None, ct));
                foreach (var h in hyps)
                    h.IsRealHyp = h.ParentList.Count >= threshold;
                if (!canBeEnclosed)
                {
                    List<Hypothesis<T, TData>> spares = new List<Hypothesis<T, TData>>();
                    foreach (Hypothesis<T, TData> hIn in hyps)
                    {
                        if (hIn.IsRealHyp)
                            foreach (JSMDataBase<T> hPl in Data[Signs.Plus])
                                if (hIn.Body.IsEnclosed(hPl))
                                    spares.Add(hIn);
                    }
                    foreach (Hypothesis<T, TData> hIn in spares)
                        hyps.Remove(hIn);
                }
                return hyps;
            }
            return new Hypothesis<T, TData>[0];
        }
        private Task<Dictionary<Hypothesis<T, TData>, Hypothesis<T, TData>[]>> GetBreaks(Signs s, CancellationToken ct, Int32 threshold = 2)
        {
            if (threshold < 2) throw new ArgumentException("Порог для \"Тормозов\" не может быть меньше 2");
            return Task.Run < Dictionary<Hypothesis<T, TData>, Hypothesis<T, TData>[]>>(() =>
                {
                    Dictionary<Hypothesis<T, TData>, Hypothesis<T, TData>[]> dict = new Dictionary<Hypothesis<T, TData>, Hypothesis<T, TData>[]>();
                    Signs oppSign = GetOpposite(s);
                    foreach (Hypothesis<T, TData> h in Hypotheses[s])
                        if (h.IsRealHyp)
                        {
                            List<TData> lst = new List<TData>();
                            foreach (TData hOpp in Data[oppSign])
                            {
                                if (h.Body.IsEnclosed(hOpp))
                                    lst.Add(hOpp);
                            }
                            
                            if (lst.Count >= threshold)
                            {
                                TData[] arr = lst.Select(l => 
                                    {
                                        JSMDataBase<T> diff = l.Difference(h.Body);
                                        diff.ID = "*" + l.ID;
                                        return diff as TData;
                                    }).ToArray();
                                List<Hypothesis<T, TData>> hyps = new List<Hypothesis<T, TData>>();
                                JSMBase.Algorithm.BaseAlgorithms.AlgorithmNorris<T, TData>(arr, hyps, null, new SaveMode(3, new CompareData(CompareOperator.MoreOrEquals, threshold), null), ct);
                                List<Hypothesis<T, TData>> spares = new List<Hypothesis<T, TData>>();
                                foreach (Hypothesis<T, TData> hIn in hyps)
                                {
                                    foreach (JSMDataBase<T> hPl in Data[s])
                                        if (hIn.Body.IsEnclosed(hPl))
                                        {
                                            spares.Add(hIn);
                                            break;
                                        }
                                }
                                foreach (Hypothesis<T, TData> hIn in spares)
                                    hyps.Remove(hIn);
                                Hypothesis<T, TData>[] vals = hyps.Where(hh => { hh.GainType = HypGain.Break; return hh.ParentList.Count >= threshold; }).ToArray();
                                if (vals.Length > 0)
                                    dict.Add(h, vals);
                            }
                             
                        }
                    return dict;
                });
        }

        private async Task<Dictionary<Hypothesis<T, TData>, Hypothesis<T, TData>[]>> GetBreaks(Signs s)
        {
            Signs oppSign = GetOpposite(s);
            Dictionary<Hypothesis<T, TData>, Hypothesis<T, TData>[]> dict = new Dictionary<Hypothesis<T, TData>, Hypothesis<T, TData>[]>();
            IEnumerable<Hypothesis<T, TData>> opps = GetRealHyps(oppSign);
            foreach (Hypothesis<T, TData> h in Hypotheses[s])
                if (h.IsRealHyp)
                {
                    List<JSMDataBase<T>> lst = new List<JSMDataBase<T>>();
                    foreach (JSMDataBase<T> hOpp in Data[oppSign])
                        if (h.Body.IsEnclosed(hOpp))
                            lst.Add(hOpp);

                    var ohs = opps.Where(oh => oh.ParentList.Any(p => lst.Contains(p))).ToArray();
                    List<Hypothesis<T, TData>> res = new List<Hypothesis<T, TData>>();
                    for (int i = 0; i < ohs.Length; i++)
                    {
                        bool needAdd = true;
                        for (int j = 0; j < h.ParentList.Count; j++)
                        {
                            if (ohs[i].Body.IsEnclosed(h.ParentList[j]))
                            {
                                needAdd = false;
                                break;
                            }
                        }
                        if (needAdd)
                            res.Add(ohs[i]);
                    }
                    dict.Add(h, res.ToArray());
                }
            return dict;
        }
        #endregion
        public IEnumerable<Hypothesis<T, TData>> GetRealHyps(Signs s)
        {
            return Hypotheses[s].Where(h => h.IsRealHyp).ToArray();
        }
        public Dictionary<Signs, JSMBase.SpecialAlgorithm.IExecContext<T>> Contexts { get; private set; }


        Dictionary<Signs, System.Collections.IList> ISolver.Hypotheses
        {
            get 
            {
                Dictionary<Signs, System.Collections.IList> res = new Dictionary<Signs, System.Collections.IList>();
                foreach (var pair in this.Hypotheses)
                    res.Add(pair.Key, pair.Value.Where(p => p.IsRealHyp).ToArray());
                
                return res;
            }
        }

        Dictionary<Signs, System.Collections.IList> ISolver.Predictions
        {
            get
            {
                Dictionary<Signs, System.Collections.IList> res = new Dictionary<Signs, System.Collections.IList>();
                var pls = this.Hypotheses[Signs.Plus].Where(p => p.IsRealHyp).ToArray();
                var mns = this.Hypotheses[Signs.Minus].Where(p => p.IsRealHyp).ToArray();

                foreach (var pair in this.Enclosures)
                {
                    res.Add(pair.Key, new List<Prediction<T, TData>>(pair.Value.Count));
                    foreach(var enc in pair.Value)
                    {
                        List<IHypothesis<T, TData>> hyps = new List<IHypothesis<T, TData>>();
                        foreach (var h1 in pls)
                            if (h1.Body.IsEnclosed(enc))
                                hyps.Add(h1);

                        foreach (var h1 in mns)
                            if (h1.Body.IsEnclosed(enc))
                                hyps.Add(h1);

                        res[pair.Key].Add(new Prediction<T, TData>(enc, hyps));
                    }
                }
                    
                CheckOrAdd(Signs.Plus, res);
                CheckOrAdd(Signs.Minus, res);
                CheckOrAdd(Signs.Null, res);
                return res;
            }
        }

        Dictionary<Signs, System.Collections.IList> ISolver.Data
        {
            get
            {
                Dictionary<Signs, System.Collections.IList> res = new Dictionary<Signs, System.Collections.IList>();
                foreach (var pair in this.Data)
                    res.Add(pair.Key, pair.Value);

                return res;
            }
        }

        private void CheckOrAdd(Signs s, Dictionary<Signs, System.Collections.IList> res)
        {
            if (!res.ContainsKey(s))
                res.Add(s, new List<Prediction<T, TData>>());
        }
    }
}
