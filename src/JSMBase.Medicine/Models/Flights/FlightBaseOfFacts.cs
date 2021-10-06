using JSMBase.Flights;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSMBase;

namespace JSMBase.Medicine.Models.Flights
{
    /// <summary>БФ для данных по полетам</summary>
    public sealed class FlightBaseOfFacts : BaseOfFacts2<Flight>
    {
        public FlightBaseOfFacts(IEnumerable<Flight> inner) : base(inner)
        {

        }
        JSMDataFlight[] cache = null;
        public override IEnumerable<JSMDataBase<Flight>> Divide()
        {
            if (cache == null)
            {
                cache = Inner.Select(i => new JSMDataFlight(i)).ToArray();
            }
            return cache;
            
        }
    }
}
