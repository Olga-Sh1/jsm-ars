using JSMBase.Medicine.Models;
using JSMBase.Medicine.Models.BoF;
using JSMBase.Medicine.Setting;
using JSMBaseC.Medicine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.Medicine.RWServices
{
    /// <summary>Сервис чтения из папки</summary>
    class JDMReaderService : IReaderService
    {
        public IEnumerable<JSMMedicine> Read(string path)
        {
            return Parser.ReadAllObjects(path); ;
        }

        public string GetComment(string path)
        {
            return path;
        }

        public Models.BoF.PartitionBaseData ReadSettings(string path)
        {
            var setts = BoFXmlSettings.Instance[path];
            if (setts == null)
            {
                var set_path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path), "set");
                Settings s = System.IO.File.Exists(set_path) ? SettingsSaver.Load(set_path) : new global::JSMBase.Medicine.Settings();
                PartitionBaseData d = new PartitionBaseData();
                d.DBPath = path;
                d.Partitions = new List<JSMBaseC.Medicine.PartitionRules>();
                var prpart = new PropertyPartition(s.NumToPlusMinus,
                    new IndexDataPart[] 
                    { 
                        new IndexDataPart() { Sign = Signs.Plus, PropIndex = new [] { s.IndexToPlusMinusValue }, Type = DiffPart.Exact },
                        new IndexDataPart() { Sign = Signs.Minus, PropIndex = new [] { s.IndexToPlusMinusValue }, Type = DiffPart.AllExceptNull },
                        new IndexDataPart() { Sign = Signs.Tau, PropIndex = new [] { s.IndexToPlusMinusValue }, Type = DiffPart.AllExeptNotNull },
                    });
                d.Partitions.Add(new PartitionRules(prpart));
                //JSMMedicine.IndicesUsed = s.IndecesProps;
                return d;
            }
            return setts;
        }

        public void WriteSettings(string path, Models.BoF.PartitionBaseData setts)
        {
            throw new NotImplementedException();
        }
    }
}
