using System.Web.Mvc;
using Mello.ImageGallery.Models;
using Mello.ImageGallery.Models.Plugins;

namespace Mello.ImageGallery.Helpers {
    public static class ImageGalleryExtensions {
        public static MvcHtmlString ImageGalleryScript(this HtmlHelper helper, string cssSelector, ImageGalleryPlugin imageGalleryPlugin) {
          return MvcHtmlString.Create(imageGalleryPlugin.ToString(cssSelector));          
        }

        //public static MvcHtmlString ImageGallery(this HtmlHelper helper, string cssSelector, ImageGalleryPlugin imageGalleryPlugin)
        //{
        //  //return MvcHtmlString.Create(imageGalleryPlugin.ToString(cssSelector));
          
        //}
      public static MvcHtmlString ImageGalleryAdditionalAttributes(this HtmlHelper helper, ImageGalleryPlugin imageGalleryPlugin) {
        return MvcHtmlString.Create(imageGalleryPlugin.AdditionalHrefMarkup);
      }

    }
}