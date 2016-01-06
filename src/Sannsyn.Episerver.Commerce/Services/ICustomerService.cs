namespace Sannsyn.Episerver.Commerce.Services
{
    public interface ICustomerService
    {
        string GetCurrentUserId();
        void MigrateUser(string oldId, string newId);
    }
}