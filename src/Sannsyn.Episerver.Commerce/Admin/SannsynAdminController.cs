using System;
using EPiServer.ServiceLocation;
using EPiServer.Shell;
using EPiServer.Shell.Navigation;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Search;
using Sannsyn.Episerver.Commerce.Models.ViewModels;
using Sannsyn.Episerver.Commerce.Services;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Sannsyn.Episerver.Commerce.Admin
{


    //[System.Web.Mvc.Authorize(Roles = "Administrators, WebAdmins, WebEditors")]
    [MenuItem("/global/sannsyn/admin", Text = "Admin", SortIndex = 20)]
    public class SannsynAdminController : Controller
    {

        private readonly ISannsynCatalogIndexService _sannsynCatalogIndexService;
        private readonly ISannsynOrderIndexerService _sannsynOrderIndexerService;
        private readonly ISannsynAdminService _sannsynAdminService;

        public SannsynAdminController(ISannsynCatalogIndexService sannsynCatalogIndexService,
            ISannsynOrderIndexerService sannsynOrderIndexerService,
            ISannsynAdminService sannsynAdminService)
        {
            _sannsynCatalogIndexService = sannsynCatalogIndexService;
            _sannsynOrderIndexerService = sannsynOrderIndexerService;
            _sannsynAdminService = sannsynAdminService;
        }


        
        public ActionResult Index()
        {
            SannsynAdminViewModel viewModel = new SannsynAdminViewModel();
            return View(string.Format("{0}{1}/Views/SannsynAdmin/Index.cshtml", Paths.ProtectedRootPath, "Sannsyn"), viewModel);
        }

        public ActionResult RunIndexingTool()
        {
            SannsynAdminViewModel viewModel = new SannsynAdminViewModel();
            
            List<PurchaseOrder> allOrders = _sannsynOrderIndexerService.GetOrders(string.Empty, string.Empty, int.MaxValue);
            foreach (var order in allOrders)
            {
                _sannsynOrderIndexerService.AddLineItemsToSannsyn(order);
            }
            viewModel.StatusMessage = string.Format("Sent {0} orders to sannsyn",allOrders.Count);
            return View(string.Format("{0}{1}/Views/SannsynAdmin/Index.cshtml", Paths.ProtectedRootPath, "Sannsyn"),viewModel);
        }

        public ActionResult StopAndStartSannsynService()
        {

            SannsynAdminViewModel viewModel = new SannsynAdminViewModel();

            _sannsynAdminService.StopService();
            var model = _sannsynAdminService.CreateService();
            if(string.Compare(model.status, "active", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                viewModel.StatusMessage = "Service Created";
            }
            else
            {
                viewModel.StatusMessage = "Service not created. Returned: " + model.status;
            }

            return View(string.Format("{0}{1}/Views/SannsynAdmin/Index.cshtml", Paths.ProtectedRootPath, "Sannsyn"), viewModel);
        }

        public ActionResult IndexProductsWithCategories()
        {
            SannsynAdminViewModel viewModel = new SannsynAdminViewModel();
            int numberOfProductsToSannsyn = _sannsynCatalogIndexService.IndexProductsWithCategories();
            viewModel.StatusMessage = string.Format("Sent {0} products with categories to sannsyn", numberOfProductsToSannsyn);
            return View(string.Format("{0}{1}/Views/SannsynAdmin/Index.cshtml", Paths.ProtectedRootPath, "Sannsyn"), viewModel);

        }



      


    }
}
