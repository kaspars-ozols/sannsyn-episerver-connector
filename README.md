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
Add the connection string for the Sannsyn Recommender Service to your connectionStrings

### Connection String Format
Example Connection String:
```
...
```

Add the connection string to the `<connectionStrings>` section in web.config:
```
...
``` 

----------
(C) 2016 Sannsyn AS
http://www.sannsyn.com



