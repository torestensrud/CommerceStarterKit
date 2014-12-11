using System;
using System.Web.Mvc;
using EPiServer.Core;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using OxxCommerceStarterKit.Web.Models.Files;
using OxxCommerceStarterKit.Web.Models.ViewModels;
using OxxCommerceStarterKit.Web.Extensions;

namespace OxxCommerceStarterKit.Web.Controllers
{
    /// <summary>
    /// Controller for the video file.
    /// </summary>
    public class VideoFileController : PartialContentController<VideoFile>
    {
        private readonly UrlResolver _urlResolver;

        public VideoFileController(UrlResolver urlResolver)
        {
            _urlResolver = urlResolver;
        }

        /// <summary>
        /// The index action for the video file. Creates the view model and renders the view.
        /// </summary>
        /// <param name="currentContent">The current video file.</param>
        public override ActionResult Index(VideoFile currentContent)
        {
            var model = new VideoViewModel
            {
                Url = _urlResolver.GetDefaultModeUrl(currentContent.ContentLink),
                CoverImageUrl = ContentReference.IsNullOrEmpty(currentContent.PreviewImage) ? String.Empty : _urlResolver.GetDefaultModeUrl(currentContent.PreviewImage),
            };
            
            // TODO: Over the top hacky
            return PartialView("/Views/Shared/DisplayTemplates/VideoFile.cshtml", model);
        }
    }
}
