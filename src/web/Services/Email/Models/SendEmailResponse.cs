using System;

namespace OxxCommerceStarterKit.Web.Services.Email.Models
{
    public class SendEmailResponse
    {
        public bool Success { get; set; }
        public Exception Exception { get; set; }
    }
}