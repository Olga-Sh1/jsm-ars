using JSMBaseC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSMBase.Algorithm
{
    /// <summary>Данные по эвристикам</summary>
    public class SaveMode
    {
        public readonly static SaveMode None = new SaveMode();
        private SaveMode() { }
        public SaveMode(int st, CompareData thr, CompareData thrp)
        {
            this.Step = st;
            this.ThresholdParent = thr;
            this.ThresholdProps = thrp;
        }
        /// <summary>Частота проверки</summary>
        public int Step { get; private set; }
        /// <summary>Пороговые значения для родителей</summary>
        public CompareData ThresholdParent { get; private set; }
        /// <summary>Пороговые значения для признаков</summary>
        public CompareData ThresholdProps { get; private set; }
    }
}
