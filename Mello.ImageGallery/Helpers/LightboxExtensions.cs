using System.Web.Mvc;
using Mello.ImageGallery.Models;

namespace Mello.ImageGallery.Helpers {
    public static class LightboxExtensions {
        public static MvcHtmlString Lightbox(this HtmlHelper helper, string selector, LightBoxSettings settings) {
            return MvcHtmlString.Create(string.Format("$('{0}').lightBox({1});", selector, settings));
        }
    }
}