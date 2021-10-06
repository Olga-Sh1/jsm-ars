using JSMBase;
using JSMBase.Medicine;
using JSMBase.Medicine.Models;
using JSMBase.Medicine.Models.BoF;
using JSMBase.Medicine.RWServices;
using JSMBaseC.Medicine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.Medicine.Models
{
    public class HypBatchFull : HypBatchBase<JSMMedicine, global::JSMBase.JSMBase>
    {
        public String DataPath { get { return data_path; } }
        String data_path;
        PartitionBaseData pbd;
        public PartitionRules CurrentRules
        {
            get { return pbd != null ? pbd.Partitions.LastOrDefault() : null; }
        }
        public HypBatchFull(String data_path)
        {
            this.data_path = data_path;
        }
        public override async Task Read(string path)
        {
            inner = new Dictionary<HypCollectionType, Dictionary<Signs, IEnumerable<Hypothesis<JSMMedicine, global::JSMBase.JSMBase>>>>();
            //данные
            string ext = System.IO.Path.GetExtension(data_path);
            IReaderService serv = RWServiceFactory.Instance.GetServiceByPath(data_path);
            Collection = serv.Read(data_path);
            pbd = serv.ReadSettings(data_path);
            BaseOfFacts<JSMMedicine> bof = new BaseOfFacts<JSMMedicine>(Collection, CurrentRules);
            bof.Divide();
            var dict = bof.List;
            WrappedCollection = dict.Select(c =>
            {
                global::JSMBase.JSMBase jb = new global::JSMBase.JSMBase(c.Key);
                jb.Sign = c.Value;
                jb.ConvertToBitArray();
                return jb;
            }).ToArray();

            var data = await Parser.ReadHypotheses(path, WrappedCollection);

            foreach (var pair in data)
                foreach (var h in pair.Value)
                {
                    (h.Body as global::JSMBase.JSMBase).ConvertToBitArray();
                    h.IsRealHyp = true;
                }

            inner.Add(HypCollectionType.Reason, data);

            if (String.IsNullOrEmpty(Comment)) Comment = Path.GetFileName(path);
        }
        public override async Task Write(string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
                foreach (var t in inner)
                {
                    foreach (var tt in t.Value)
                        foreach (var h in tt.Value)
                            await Parser.WriteHypothesis(h, tt.Key, t.Key, sw);
                }
        }
    }
}
