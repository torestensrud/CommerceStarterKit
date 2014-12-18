using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OxxCommerceStarterKit.Web.Models.ViewModels.Email;

namespace OxxCommerceStarterKit.Web.Services.Email
{
    public interface INotificationSettingsRepository
    {
        NotificationSettings GetNotificationSettings(string language = null);
    }
}
