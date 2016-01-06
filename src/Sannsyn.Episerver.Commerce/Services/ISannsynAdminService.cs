using Sannsyn.Episerver.Commerce.Models;

namespace Sannsyn.Episerver.Commerce.Services
{
    public interface ISannsynAdminService
    {
        SannsynServiceStartStopModel CreateService();
        SannsynServiceStatusModel GetServiceStatus();
        bool StopService();
    }
}