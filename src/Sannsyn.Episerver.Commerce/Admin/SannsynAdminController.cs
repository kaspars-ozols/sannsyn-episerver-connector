using EPiServer.ServiceLocation;
using EPiServer.Shell;
using EPiServer.Shell.Navigation;
using Mediachase.Commerce.Orders;
using Mediachase.Commerce.Orders.Search;
using Sannsyn.Episerver.Commerce.Models.ViewModels;
using Sannsyn.Episerver.Commerce.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

namespace Sannsyn.Episerver.Commerce.Admin
{
  
  
    //[System.Web.Mvc.Authorize(Roles = "Administrators, WebAdmins, WebEditors")]
    public class SannsynAdminController : Controller
    {


        [MenuItem("/global/sannsyn/admin", Text = "Admin")]
        public ActionResult Index()
        {
            SannsynAdminViewModel viewModel = new SannsynAdminViewModel();
            return View(string.Format("{0}{1}/Views/SannsynAdmin/Index.cshtml", Paths.ProtectedRootPath, "Sannsyn"), viewModel);
        }

        public ActionResult RunIndexingTool()
        {
            SannsynAdminViewModel viewModel = new SannsynAdminViewModel();
            ISannsynOrderIndexerService sannsynContentIndexer = ServiceLocator.Current.GetInstance<ISannsynOrderIndexerService>();
            List<PurchaseOrder> allOrders = GetOrders(string.Empty, string.Empty, int.MaxValue);
            foreach (var order in allOrders)
            {
                sannsynContentIndexer.AddLineItemsToSannsyn(order);
            }
            viewModel.StatusMessage = string.Format("Sent {0} orders for to sannsyn",allOrders.Count);
            return View(string.Format("{0}{1}/Views/SannsynAdmin/Index.cshtml", Paths.ProtectedRootPath, "Sannsyn"),viewModel);
        }

        public ActionResult StopAndStartSannsynService()
        {
            //stop and start Sannsyn Service

            return View(string.Format("{0}{1}/Views/SannsynAdmin/Index.cshtml", Paths.ProtectedRootPath, "Sannsyn"));
        }



        private List<PurchaseOrder> GetOrders(string sqlWhereClause, string sqlMetaWhereClause, int recordCount = int.MaxValue)
        {
            var orderSearchParameters = new OrderSearchParameters();
            if (!string.IsNullOrEmpty(sqlWhereClause))
            {
                orderSearchParameters.SqlWhereClause = sqlWhereClause;
            }

            if (!string.IsNullOrEmpty(sqlMetaWhereClause))
            {
                orderSearchParameters.SqlMetaWhereClause = sqlMetaWhereClause;
            }

            var orderSearchOptions = new OrderSearchOptions();
            orderSearchOptions.Namespace = "Mediachase.Commerce.Orders";
            orderSearchOptions.Classes.Add("PurchaseOrder");
            orderSearchOptions.Classes.Add("Shipment");
            orderSearchOptions.CacheResults = false;
            orderSearchOptions.RecordsToRetrieve = recordCount;

            return OrderContext.Current.FindPurchaseOrders(orderSearchParameters, orderSearchOptions).ToList();
        }


    }
}
