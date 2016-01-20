using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sannsyn.Episerver.Commerce.Models
{
    public class ScoredItemsModel
    {
        public List<ScoredItems> result { get; set; }
    }

    public class ScoredItems
    {

        public double w { get; set; }
        public string id { get; set; }
    }
}
