# Commerce Starterkit
From the Wiki: [About the Starter Kit](https://github.com/OXXAS/CommerceStarterKit/wiki) | [Feature List](https://github.com/OXXAS/CommerceStarterKit/wiki/features) | [FAQ](https://github.com/OXXAS/CommerceStarterKit/wiki/FAQ)

![Starter Kit Logo](https://github.com/OXXAS/CommerceStarterKit/raw/master/doc/img/logo/logo-500px.png)

## Getting Started
1. Clone the repository
2. Download the databases from [CommerceStarterKit-Database](https://github.com/OXXAS/CommerceStarterKit-Database/tree/master/Databases) ([download zip file](https://github.com/OXXAS/CommerceStarterKit-Database/blob/master/Databases/CommerceStarterKit.zip?raw=true))
3. Unzip the databases to the /db/ folder
4. Open the solution in Visual Studio 2013
5. Run it (also see [How to Start Commerce Manager](#how-to-start-commerce-manager))
6. Default admin account for web and manager sites is: **admin \ store**
7. Configure dependencies for more features 

## Configuring Dependencies
The Commerce Starter Kit has these dependencies:

### EPiServer Find

EPiServer Find is used in most product lists, as the wine and fashion lists, and also by the configurable search block. The main search and the search-as-you-type function also uses Find.

You will have a hard time using the starter kit without Find.

**Configuration**

1. Go to [http://find.episerver.com](http://find.episerver.com), log in, and create a new developer index (with English and Norwegian languages).
2. Add the `<episerver.find ... >` configuration to web.config. Search for it, it is already in the web.config, but with invalid configuration settings. 
2. Go to [http://localhost:49883/episerver/CMS/Admin/Default.aspx](http://localhost:49883/episerver/CMS/Admin/Default.aspx "Admin mode"), find the "Index Product Catalog" Scheduled Job and run it.
3. Verify that the product list is showing correctly by visiting the Wine list (http://localhost:49883/en/wine/all-wines/)

### DIBS
The payment step of the checkout requires a DIBS demo account. If your company does not already have a demo account with DIBS, you need to sign up for one: [http://www.dibspayment.com/demo-signup](http://www.dibspayment.com/demo-signup) The starter kit is using the [DIBS D2 platform](http://tech.dibspayment.com/dibs_payment_window).

You need the following settings from your DIBS account:

* API UserId (this is your merchant ID)
* HMAC key for MAC calculation (K)

Configure payment settings:

1. Log in to Commerce Manager
2. Open the Administration menu
3. Expand Order System / Payments / English (repeat for "norsk")
4. Select Parameters tab
5. Configure the settings according to the values above

**NOTE!** You need to configure the DIBS account for both English and "norsk" in Commerce Manager. 

This can be found on the [DIBS administration web site](https://payment.architrade.com/login/login.action)

You can find a [list of test cards](http://tech.dibspayment.com/toolbox/test_information_cards) on the DIBS web site. Use these to test the checkout process.

### Postnord
For the Norwegian market, you can select a pickup location for the order. This is based on the shipping address. The availble pick up locations are retrieved by calling a public Postnord web service. In order to use this, you should have your 

[Register for Web Service Access](http://www.postnordlogistics.no/en/e-services/widgets-and-web-services/Pages/Register-as-webservice-widget-consumer.aspx) on the Postnord web site.

Add your consumerId to the `PostNord.ConsumerId` appSettings value in `web.config`:

    <appSettings>
    	<add key="PostNord.ConsumerId" value="your-consumer-id-here" />
    <appSettings>

Without the Postnord integration, you should still be able to do a checkout, but you will not be able to demonstrate the pickup location feature.

### Google Analytics
The starter kit has extended ecommerce tracking, which will start tracking when you configure the EPiServer Google Analytics Add-on.

Make sure your Google Analytics account is using Universal Analytics and has the Enhanced Ecommerce feature enabled.

By default, the following is tracked:

* Page views
* Add to cart
* View cart
* Checkout
* Payment
* Wine Configurable Search Block 

# Nice to Know
Various helpful stuff

## How to Start Commerce Manager
When you start the project in Visual Studio, it will only start the the default site (the designated Startup Project). This is typically the web site and not Commerce Manager.

To start Commerce Manager, right click the CommerceStarterKit.Commerce project in Visual Studio, View, View in Browser (or Ctrl+Shift+W)

## How to Upgrade the Databases
Now and then the CMS and Commerce databases need to be upgraded. This is typically done using the `update-epidatabase` command in the Package Manager Console in Visual Studio. 

The latest version of the code is synced with the downloadable databases. If you haven't changed any content (that you'd care to keep), just download new versions and replace the databases you've got.

If you update these databases yourself using the `update-epidatabase` command
from the Package Manager Console, you need to change the connectionstring to
a full path instead of the "magic" one using the DataDirectory setting:

	Data Source=(LocalDb)\v11.0;AttachDbFilename=|DataDirectory|CommerceStarterKit-Web.mdf; ... 

The `update-epidatabase` command will read the `connectionstrings.config` file and try to locate the databases in the `app_Data` directory below your web site. Since the DataDirectory is changed in memory during the startup of the web sites, they know how to find the databases. This is not the case for the `update-epidatabase` command.

Change both AttachDbFilename settings:

	AttachDbFilename=c:\path\to\your\CommerceStarterKit-Web.mdf and
	AttachDbFilename=c:\path\to\your\CommerceStarterKit-CM.mdf

After you have run the `update-epidatabase` you can revert the connectionstring changes.

