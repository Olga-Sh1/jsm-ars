using BFSettings;
using JSMBase.Medicine.RWServices.Excel.Converters;
using JSMBase.RNK;
using Ninject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XlsxReader4_6;

namespace JSMBase.Medicine.RWServices.Excel
{
    public sealed class ExcelParser
    {
        Dictionary<Type, Type> factory = new Dictionary<Type, Type>()
        {
            { typeof(IdPropSetting), typeof(IdConverter) },
            { typeof(ListPropSetting), typeof(ListConverter) },
            { typeof(NumberPropSetting), typeof(NumberConverter) },
            { typeof(DoublePointInterval), typeof(DPointsIntervalConverter)},
            { typeof(LogicalPropSetting), typeof(LogicalConverter) },
            { typeof(LogicalTreePropSetting), typeof(LogicalThreeConverter)},
            { typeof(TwoLetterGenPropSetting), typeof(GenConverter)}
        };
        [Inject]
        public XlsxReader reader { get; set; }
       
        public IEnumerable<MedicineExample> Read(String path, FBSetting setts)
        {
            XlsxReaderConfig conf = createConfig(setts.Settings);
            DataSet ds = reader.Read(path, conf);
            List<MedicineExample> lst = new List<MedicineExample>();
            var convs = CreateConverterChain(setts.Array);
            var add = CheckIfNeedExtraProp(setts.Settings, ds);
            if (add != null) convs.Add(add);
            Dictionary<String, List<IPropValue>> preobjs = new Dictionary<string, List<IPropValue>>();
            foreach (DataTable dt in ds.Tables)
            {
                IConverter[] sel_convs = convs.Where(cc => cc.CheckTable(dt)).ToArray();
                if (sel_convs.Length == 0) continue;
                IdConverter id = sel_convs.OfType<IdConverter>().First();
                foreach(DataRow row in dt.Rows)
                {
                    PropValueId idval = (PropValueId)id.CreateValue(row);
                    if (idval.Value == null) continue;
                    if (!preobjs.ContainsKey(idval.Value))
                        preobjs.Add(idval.Value, new List<IPropValue>());
                    else
                    {

                    }
                    foreach (IConverter cnv in sel_convs)
                    {
                        if (cnv is IdConverter)
                        {
                            if (preobjs[idval.Value].OfType<PropValueId>().Count() > 0)
                                continue;
                        }
                        preobjs[idval.Value].Add(cnv.CreateValue(row));
                    }
                        
                }
                
            }

            if (preobjs.Count > 0)
            {
                MedicineExample.Infos = preobjs.First().Value.Select(p => p.PropInfo).ToArray();
                int index = 1;
                foreach(var pi in MedicineExample.Infos)
                {
                    pi.Index = index++;
                }
            }

            foreach (var pres in preobjs.Values)
            {
                lst.Add(new MedicineExample(pres.ToArray()));
            }
                

            return lst;
        }

        private XlsxReader4_6.XlsxReaderConfig createConfig(GeneralSettings sts)
        {
            XlsxReaderConfig cc = new XlsxReader4_6.XlsxReaderConfig();
            if (sts == null) return cc;
            foreach (var h in sts.Headers)
                cc.Headers.Add(h.Name, h.HeadersCount);
            return cc;
        }

        private List<IConverter> CreateConverterChain(IEnumerable<PropSetting> prs)
        {
            List<IConverter> res = new List<IConverter>(prs.Count());
            foreach (PropSetting pst in prs)
            {
                if (!factory.ContainsKey(pst.GetType()))
                {

                }
                Type t = factory[pst.GetType()];
                IConverter conv = Activator.CreateInstance(t, new object[] { pst }) as IConverter;
                res.Add(conv);
            }
            return res;
        }

        private IConverter CheckIfNeedExtraProp(GeneralSettings gs, DataSet ds)
        {
            if (!String.IsNullOrEmpty(gs.ExcelSheetName) &&
                !String.IsNullOrEmpty(gs.ExpressionMinus) &&
                !String.IsNullOrEmpty(gs.ExpressionPlus))
            {
                DataTable dt = ds.Tables.Cast<DataTable>().FirstOrDefault(ff => String.Compare(ff.TableName, gs.ExcelSheetName, true) == 0);
                if (dt == null) return null;
                String plName = "PlusValues";
                String mnName = "MinusValues";
                DataColumn dc1 = new DataColumn(plName);
                dc1.Expression = gs.ExpressionPlus;
                DataColumn dc2 = new DataColumn(mnName);
                dc2.Expression = gs.ExpressionMinus;
                dt.Columns.Add(dc1);
                dt.Columns.Add(dc2);
                return new ToPlusMinusConverter(plName, mnName, gs.ExcelSheetName);
            }
            return null;
        }
    }
}
