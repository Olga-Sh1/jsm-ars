using JSMBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMSolver.Filter
{
    public abstract class BaseFilterStr<T, TData> : IBaseFilter<T, TData> where TData : JSMDataBase<T>
    {
        /// <summary>Сообщение об ошибке</summary>
        public String Error { get; protected set; }
        /// <summary>Позиция в строке</summary>
        public Int32 Position { get; protected set; }

        protected String _FilterStrMin;
        public String FilterStrMin
        {
            get { return _FilterStrMin; }
            set
            {
                if (_FilterStrMin != value)
                {
                    _FilterStrMin = value;
                    fMn = ParseFString(_FilterStrMin);
                }
               
            }
        }

        protected String _FilterStrPl;
        public String FilterStrPl
        {
            get { return _FilterStrPl; }
            set
            {
                if (_FilterStrPl != value)
                {
                    _FilterStrPl = value;
                    fPl = ParseFString(_FilterStrPl);
                }

            }
        }

        public bool FilterMin(Hypothesis<T, TData> m)
        {
            return fMn == null || fMn(m);
        }

        public bool FilterPlus(Hypothesis<T, TData> m)
        {
            return fPl == null || fPl(m);
        }

        protected abstract Func<Hypothesis<T, TData>, bool> ParseFString(String f);

        private Func<Hypothesis<T, TData>, bool> fPl;
        private Func<Hypothesis<T, TData>, bool> fMn;
    }
}
