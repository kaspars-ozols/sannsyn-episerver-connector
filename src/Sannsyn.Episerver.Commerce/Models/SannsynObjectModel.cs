using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sannsyn.Episerver.Commerce.Models
{
    public class SannsynObjectModel
    {
        public string Customer { get; set; }
        public List<string> Tags { get; set; }
        public List<string> EntityIDs { get; set; }
        public long Time { get; set; }
        public float Boost  { get; set; }
        public int Number { get; set; }
    }
}
