using JSMBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Collections;

namespace JSMBaseC.Services.RW.v2.Serialization
{
    [Serializable]
    public class SerializeWrapper<T, TData> : IRestorable where TData : JSMDataBase<T>, new()
    {
        public uint Index { get; set; }
        public String ID { get; set; }
        public string[] Parents { get; set; }
        public Signs Sign { get; set; }
        public T Object { get; set; }

        public SerializeWrapper() { }
        public SerializeWrapper(TData d)
        {
            this.ID = d.ID;
            this.Object = d.Inner;
            this.Sign = d.Sign;
        }
        public SerializeWrapper(Hypothesis<T, TData> h)
        {
            this.Index = h.Index;
            this.Parents = h.ParentList.Select(p => p.ID).ToArray();
            this.Sign = h.Body.Sign;
            this.Object = h.Body.Inner;
        }

        public SerializeWrapper(Prediction<T, TData> p)
        {
            this.Parents = p.ParentList.Select(pp => pp.Index.ToString()).ToArray();
            this.Sign = p.Body.Sign;
            this.Object = p.Body.Inner;
            this.ID = p.Body.ID;
        }

        public Hypothesis<T, TData> Restore(IList<TData> all)
        {
            TData td = new TData();
            td.Inner = this.Object;

            Hypothesis<T, TData> h = new Hypothesis<T, TData>(this.Index, "");
            h.Body = td;
            foreach(String idP in this.Parents)
                h.ParentList.Add(all.First(a => a.ID == idP));
            return h;
        }

        public Prediction<T, TData> Restore(IList<Hypothesis<T, TData>> all)
        {
            TData td = new TData();
            td.ID = this.ID;
            td.Inner = this.Object;
           

            var ps = all.Where(a => this.Parents.Contains(a.Index.ToString())).Cast<IHypothesis<T, TData>>().ToList();

            Prediction<T, TData> p = new Prediction<T, TData>(td, ps);
            return p;
        }

        public TData Restore()
        {
            TData td = new TData();
            td.Inner = this.Object;
            td.Sign = this.Sign;
            td.ID = this.ID;
            td.Complete();
            return td;
        }

        object IRestorable.Restore(IList all)
        {
            return this.Restore(all.Cast<TData>().ToArray());
        }

        object IRestorable.Restore()
        {
            return this.Restore();
        }

        object IRestorable.RestorePred(IList hyp)
        {
            return this.Restore(hyp.Cast<Hypothesis<T, TData>>().ToArray());
        }
    }
}
