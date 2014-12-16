using System;
using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.SpecializedProperties;

namespace OxxCommerceStarterKit.Web.Models.PageTypes.Payment
{
    [ContentType(DisplayName = "GenericPaymentPage", GUID = "d57314a3-2331-41a9-a3e8-339c70c4a21e", Description = "")]
    public class GenericPaymentPage : BasePaymentPage
    {
        
                [CultureSpecific]
                [Display(
                    Name = "Main body",
                    Description = "The main body will be shown in the main content area of the page, using the XHTML-editor you can insert for example text, images and tables.",
                    GroupName = SystemTabNames.Content,
                    Order = 1)]
                public virtual XhtmlString MainBody { get; set; }
         
    }
}