using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Sannsyn.Episerver.Commerce.Models;

namespace Sannsyn.Episerver.Commerce.Services
{
    public interface ISannsynUpdateService
    {
        HttpResponseMessage SendToSannsyn(SannsynUpdateModel sannsynModel);
    }
}
