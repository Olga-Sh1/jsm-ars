using JSMBase;
using JSMBaseC.Medicine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.Medicine.Models
{
    public class BaseOfFacts<T> where T : class, IID
    {
        private readonly IEnumerable<T> inner;
        Dictionary<Signs, List<T>> innerDict;
        public Dictionary<T, Signs> List { get; private set; }
        /// <summary>Правила разбиения по знакам</summary>
        public PartitionRules Rules { get; private set; }
        /// <summary>Коллекция элементов знака</summary><param name="sg">Знак</param><returns>Коллекция элементов знака</returns>
        public IEnumerable<T> this[Signs sg]
        {
            get { return innerDict[sg]; }
        }
        public BaseOfFacts(IEnumerable<T> array, PartitionRules rules)
        {
            Rules = rules;
            inner = array;
            innerDict = new Dictionary<Signs, List<T>>();
            List = new Dictionary<T, Signs>();
        }

        public void Divide()
        {
            innerDict.Clear();
            List.Clear();
            var arr = Enum.GetValues(typeof(Signs));
            foreach(Signs sg in arr)
                innerDict.Add(sg, new List<T>());
            if (Rules.CustomPartition)
            {
                foreach (var pair in Rules.Partition)
                {
                    var ob = inner.FirstOrDefault(o => o.ID == pair.Key);
                    if (ob != null)
                    {
                        innerDict[pair.Value].Add(ob);
                        List.Add(ob, pair.Value);
                    }
                }
            }
            else
            {
                var dict = Rules.PartitionDesc.GetSign(inner);
                foreach (var pair in dict)
                {
                    innerDict[pair.Key].AddRange(pair.Value.OfType<T>());
                    foreach (var ob in pair.Value)
                        List.Add(ob as T, pair.Key);
                }
            }

        }

        public IEnumerable<T> GetInnerArray()
        {
            return inner;
        }
    }
}
