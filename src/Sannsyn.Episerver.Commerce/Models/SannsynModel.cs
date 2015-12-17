using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sannsyn.Episerver.Commerce.Models
{
    public class SannsynModel
    {
        public string Service { get; set; }
        public List<SannsynObjectModel> Updates { get; set; }
    }
}
