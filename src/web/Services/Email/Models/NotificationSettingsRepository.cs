using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using OxxCommerceStarterKit.Web.Models.ViewModels.Email;

namespace OxxCommerceStarterKit.Web.Services.Email.Models
{
    public class NotificationSettingsRepository : INotificationSettingsRepository
    {
        public NotificationSettings GetNotificationSettings(string language = null)
        {
            var contentTypeRepository = ServiceLocator.Current.GetInstance<EPiServer.DataAbstraction.IContentTypeRepository>();
            var pageCriteriaQueryService = ServiceLocator.Current.GetInstance<IPageCriteriaQueryService>();
            var criterias = new PropertyCriteriaCollection();
            var criteria = new PropertyCriteria();
            criteria.Condition = EPiServer.Filters.CompareCondition.Equal;
            criteria.Name = "PageTypeID";
            criteria.Type = PropertyDataType.PageType;
            criteria.Value = contentTypeRepository.Load("NotificationSettings").ID.ToString();
            criteria.Required = true;
            criterias.Add(criteria);
            var page = language == null ? pageCriteriaQueryService.FindPagesWithCriteria(PageReference.StartPage, criterias).FirstOrDefault() : pageCriteriaQueryService.FindPagesWithCriteria(PageReference.StartPage, criterias, language).FirstOrDefault();
            if (page is NotificationSettings)
            {
                return (NotificationSettings)page;
            }
            return null;
        }
    }
}