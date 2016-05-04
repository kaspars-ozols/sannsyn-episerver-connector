# Sannsyn Connector for Episerver Commerce
Real-time intelligent product recommendations for the Episerver Commerce platform.

# Installation
Open the Package Manager Console and write:
```
Install-Package sannsyn.episerver.commerce
```  
This will install the Sannsyn .NET API for Episerver Commerce to your solution

> You need to add the Episerver Nuget Feed to your development environment (see feed link on http://nuget.episerver.com/)

## Requirements
* Episerver Commerce 9.0 or newer.
* A configured Sannsyn Recommender Service

## Configuration
Add the connection string for the Sannsyn Recommender Service to your connectionStrings in web.config. You will receive the following data from Sannsyn:

* Url to API
* Name of service
* Username for API usage
* Password for API usage

### Connection String Format
The connection string for Sannsyn looks like this:
```
Service Url=<url to sannsyn service>;Service=<service name>;User Name=<username>;Password=<password>
```

Add the connection string to the `<connectionStrings>` section in web.config along with the one for CMS and Commerce:
```xml
<connectionStrings>
	...
  	<add name="SannsynConnection"
		 connectionString="Service Url=http://customername.sannsyn.com/;Service=servicename;User Name=username;Password=password"
		 providerName="Custom" />
	...
</connectionStrings>
```
# Getting Started
 1. Install the module from the package manager console: `Install-Package sannsyn.episerver.commerce`
 1. Configure the connection string as shown above
 1. Open the site, go to edit mode and look for the Sannsyn menu in the top navigation:
 ![Sannsyn Plugin Overview](https://raw.githubusercontent.com/BVNetwork/sannsyn-episerver-connector/master/doc/img/screenshot-overview.png?token=AGIDqHr9tWw59RMkY_wYG4TeZQUFIMwVks5XF3lfwA%3D%3D)
 1. See the Admin menu for tools to index the catalog and existing orders:
 !![Sannsyn Plugin Admin](https://raw.githubusercontent.com/BVNetwork/sannsyn-episerver-connector/master/doc/img/screenshot-admin.png?token=AGIDqL_S-lPVMJg3odUQii3EjyAwZCA3ks5XF3m9wA%3D%3D)
 1. Visit one of your product pages. You should see a reference to Sannsyn javascript tracking url.
 1. Start using the recommendation services in the connector

# Services
The connector wraps several of the availble recommendation services in the Sannsyn Recommender API to ease development. You can find these services in the `Sannsyn.Episerver.Commerce.Services` namespace:

Use the Episerver Service Locator to get the implementation of these services, or register your own override in order to customize the behaviour.

 1. `IRecommendationService` - the main service to use for product recommendations
 1. `ICustomerService` - Availble for customization. Used to get the customer id and to migrate from anonymous to known users
 1. `ISannsynAdminService` - Used by the admin tools.
 2. `ISannsynCatalogIndexService` - The default implementation of the catalog indexer. Can be customized.
 3. `ISannsynOrderIndexerService` - The default implementation of the order indexer. You might want to change the GetOrders method in order to control which orders are sent to Sannsyn.
 4. `ISannsynUpdateService` - Service being used to send data to Sannsyn. Can be customized.

##  Admin
There are two important commands in the Sannsyn admin:

### Index Orders
If you click the "Index Orders" button under the Sannsyn Admin menu, it will load all purchase orders in the Commerce database and index each line item with the corresponding customer id.

You can create your own implementation by registering your own class in the service locator. It needs to implement `ISannsynOrderIndexerService`.

### Index Products
If you click the Index Products button under the Sannsyn Admin menu, it will load all products from the first catalog in the catalog system along with the corresponding categories. The indexer supports products that belongs to multiple categories.

The indexer will prefer to index products before variations. If there are no product to variation relation, the variation will be indexed.

You can create your own implementation by registering your own class in the service locator. It needs to implement `ISannsynCatalogIndexService`. The base implementation is in the `Sannsyn.Episerver.Commerce.Services.SannsynCatalogIndexService` class. You can inherit from this in your own implementation in order to change the default indexer.

## Examples
See the example project for the [Epic Photo demo site](https://github.com/BVNetwork/CommerceStarterKit/tree/sannsyn/src/CommerceStarterKit.Sannsyn) for uses of the API and how to convert the results from the API to Episerver Commerce products.

Example on how to use the RecommendationService:
```C#
public class SannsynRecommendedProductsService
{
    private readonly IRecommendationService _recommendationService;
    private readonly ReferenceConverter _referenceConverter;
    private readonly IContentRepository _contentRepository;

	public SannsynRecommendedProductsService(IRecommendationService recommendationService, ReferenceConverter referenceConverter, IContentRepository contentRepository)
	{
	    _recommendationService = recommendationService;
	    _referenceConverter = referenceConverter;
	    _contentRepository = contentRepository;
	}

	public IEnumerable<IContent> GetRecommendedProducts(EntryContentBase catalogEntry, int maxCount)
	{
		// When looking at one product, get recommendations for other products
		// Note, this does not take into account the current user
	    var recommendationsForProduct = _recommendationService.GetRecommendationsForProduct(catalogEntry.Code, maxCount);

	    List<ContentReference> links = new List<ContentReference>();
	    foreach (string code in recommendationsForProduct)
	    {
	        links.Add(_referenceConverter.GetContentLink(code, CatalogContentType.CatalogEntry));
	    }

	    return _contentRepository.GetItems(links, catalogEntry.Language);
	}
}

```


----------
(C) 2016 Sannsyn AS
http://www.sannsyn.com
