using System;
using System.Linq;
using System.Web;
using System.Web.Profile;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Logging;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Security;
using Sannsyn.Episerver.Commerce.Services;

namespace Sannsyn.Episerver.Commerce.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Commerce.Initialization.InitializationModule))]
    public class MigrateAnonymousInitialization : IInitializableHttpModule
    {
        private ILogger _log;
        private ICurrentCustomerService _currentCustomerService;

        public MigrateAnonymousInitialization()
        {
            _log = EPiServer.Logging.LogManager.GetLogger();
        }

        public void Initialize(InitializationEngine context)
        {
            _currentCustomerService = ServiceLocator.Current.GetInstance<ICurrentCustomerService>();
        }

        public void Uninitialize(InitializationEngine context)
        {
            //Add uninitialization logic
        }

        public void InitializeHttpEvents(HttpApplication application)
        {
            for (int i = 0; i < application.Modules.Count; i++)
            {
                ProfileModule module = application.Modules[i] as ProfileModule;
                if (module != null)
                {
                    module.MigrateAnonymous += OnMigrateAnonymous;
                    _log.Debug("Initializing Anonymous Migration Module");
                }
            }
        }

        private void OnMigrateAnonymous(object sender, ProfileMigrateEventArgs args)
        {
            string newId = _currentCustomerService.GetCurrentUserId();

            _log.Debug("Migrating from: {0} to: {1}", args.AnonymousID, newId);
        }

    }
}