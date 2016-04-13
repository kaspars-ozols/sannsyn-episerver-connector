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

----------
(C) 2016 Sannsyn AS
http://www.sannsyn.com



