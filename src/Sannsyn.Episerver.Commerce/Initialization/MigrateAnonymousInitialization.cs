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
using Sannsyn.Episerver.Commerce.Configuration;
using Sannsyn.Episerver.Commerce.Services;

namespace Sannsyn.Episerver.Commerce.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Commerce.Initialization.InitializationModule))]
    public class MigrateAnonymousInitialization : IInitializableHttpModule
    {
        private ILogger _log;
        private ICustomerService _customerService;

        public MigrateAnonymousInitialization()
        {
            _log = EPiServer.Logging.LogManager.GetLogger();
        }

        public void Initialize(InitializationEngine context)
        {
            _customerService = ServiceLocator.Current.GetInstance<ICustomerService>();
        }

        public void Uninitialize(InitializationEngine context)
        {
            //Add uninitialization logic
        }

        public void InitializeHttpEvents(HttpApplication application)
        {
            SannsynConfiguration sannsynConfiguration = ServiceLocator.Current.GetInstance<SannsynConfiguration>();
            if (sannsynConfiguration.ModuleEnabled)
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
        }

        private void OnMigrateAnonymous(object sender, ProfileMigrateEventArgs args)
        {
            string newId = _customerService.GetCurrentUserId();
            try
            {
                _customerService.MigrateUser(args.AnonymousID, newId);
            }
            catch (Exception e)
            {
                _log.Error("Cannot migrate Sannsyn user", e);
            }
            _log.Debug("Migrating from: {0} to: {1}", args.AnonymousID, newId);
        }

    }
}