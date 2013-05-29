using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Funnel.Tests.Models
{
    public class StarcraftDataItem
    {
        public string Unit { get; set; }
        public int? Mineral { get; set; }
        public int Gas { get; set; }
        public string Supply { get; set; }
        public string Time { get; set; }
        public string Type { get; set; }
        public string Bonus { get; set; }
        public string Race { get; set; }
        public string Other { get; set; }
    }
}
