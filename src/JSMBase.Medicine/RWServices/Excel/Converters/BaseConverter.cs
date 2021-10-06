using BFSettings;
using JSMBase.RNK;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.Medicine.RWServices.Excel.Converters
{
    public abstract class BaseConverter<T> : IConverter where T : PropSetting
    {
        protected T inner;
        public BaseConverter(T ob)
        {
            this.inner = ob;
        }
        public virtual Boolean CheckTable(DataTable dt)
        {
            return dt.TableName == inner.ExcelSheetName;
        }
        public abstract IPropInfo CreateInfo();
        public abstract IPropValue CreateValue(DataRow dr);
        protected T1 TryConvert<T1>(Object ob)
        {
            return (T1)TryConvert(ob, typeof(T1));
        }

        protected Object TryConvert(Object ob, Type t)
        {
            if (ob == null) return ob;

            if (Convert.IsDBNull(ob))
            {
                if (!t.IsValueType) return null;
                if (t.Equals(typeof(double))) return double.NaN;
                return Activator.CreateInstance(t);
            }

            if (ob.GetType().Equals(t)) return ob;
            if (ob.GetType().Equals(typeof(String)) && t.Equals(typeof(Double)))
            {
                double res = double.NaN;
                if (!Double.TryParse(ob as string, NumberStyles.Number, CultureInfo.CurrentCulture, out res))
                    try
                    {
                        res = Double.Parse(ob as string, NumberStyles.Number, CultureInfo.InvariantCulture);
                    }
                    catch(Exception ex)
                    {
                        String err = String.Format("Строка \"{0}\" не распознана как дейстивтельное значение\nExcelSheetName: {1}, PropName: {2}", ob, inner.ExcelSheetName, inner.Name);
                        throw new Exception(err, ex);
                    }
                    
                //return Double.Parse(ob as string, NumberStyles.Number, CultureInfo.CurrentCulture);
                return res;
            }

            return Convert.ChangeType(ob, t);
        }
    }
}
