using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseElements
{
    public class ProgressReport
    {
        /// <summary>current progress</summary>
        public int CurrentProgressAmount { get; set; }
        /// <summary>total progress</summary>
        public int TotalProgressAmount { get; set; }
        /// <summary>some message to pass to the UI of current progress</summary>
        public string CurrentProgressMessage { get; set; }
    }
}
