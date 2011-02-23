using System.Linq;
using ImageGallery.Services;
using Mello.ImageGallery.Models;
using Mello.ImageGallery.Services;
using Mello.ImageGallery.Utils;
using Mello.ImageGallery.ViewModels;
using Orchard;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement;
using Orchard.UI.Resources;
using System.Web.Mvc;

namespace Mello.ImageGallery.Drivers {
    public class ImageGalleryDriver : ContentPartDriver<ImageGalleryPart> {
        //TODO: Refactor this consts to plugin configs                
        private const string LightboxFile = "Content/Scripts/jquery.lightbox-0.5.min.js";
        private const string LightboxStyleFile = "Content/Styles/jquery.lightbox-0.5.css";
        private const string ImageGalleryStyleFile = "Content/Styles/image-gallery.css";

        private readonly IImageGalleryService _imageGalleryService;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IThumbnailService _thumbnailService;

        public ImageGalleryDriver(IImageGalleryService imageGalleryService, IWorkContextAccessor workContextAccessor, IThumbnailService thumbnailService) {
            _thumbnailService = thumbnailService;
            _workContextAccessor = workContextAccessor;
            _imageGalleryService = imageGalleryService;
        }

        private void RegisterStaticContent() {
            string context = "Modules/ImageGallery";

            string lightbox = JavaScriptHelper.AddScriptTag(string.Concat(context, "/", LightboxFile));
            LinkEntry lightboxStyle = LinkHelper.BuildStyleLink(string.Concat(context, "/", LightboxStyleFile));
            LinkEntry imageGalleryStyle = LinkHelper.BuildStyleLink(string.Concat(context, "/", ImageGalleryStyleFile));

            IResourceManager resourceManager = _workContextAccessor.GetContext().Resolve<IResourceManager>();
            resourceManager.Require("script", "jQuery").AtHead();
            resourceManager.RegisterHeadScript(lightbox);
            resourceManager.RegisterLink(lightboxStyle);
            resourceManager.RegisterLink(imageGalleryStyle);
        }

        protected override DriverResult Display(
            ImageGalleryPart part, string displayType, dynamic shapeHelper) {
            global::Mello.ImageGallery.Models.ImageGallery imageGallery = _imageGalleryService.GetImageGallery(part.MediaPath);

            if (displayType == "SummaryAdmin") {
                // Image gallery returns nothing if in Summary Admin
                return null;
            }

            RegisterStaticContent();

            ImageGalleryViewModel viewModel = new ImageGalleryViewModel {PluginSettings = new LightBoxSettings()};

            if (!string.IsNullOrEmpty(part.MediaPath)) {
                viewModel.Images = imageGallery.Images;
            }
            else {
                return null;
            }

            return ContentShape("Parts_ImageGallery",
                                () => shapeHelper.DisplayTemplate(
                                    TemplateName: "Parts/ImageGallery",
                                    Model: viewModel,
                                    Prefix: Prefix));
        }

        //GET
        protected override DriverResult Editor(
            ImageGalleryPart part, dynamic shapeHelper) {
            part.AvailableGalleries = _imageGalleryService.GetImageGalleries()
                .OrderBy(o => o.Name).Select(o => new SelectListItem {
                    Text = o.Name,
                    Value = o.Name
                });

            if (!string.IsNullOrEmpty(part.MediaPath)) {
                part.SelectedGallery = part.MediaPath;
            }
            else {
                part.SelectedGallery = part.AvailableGalleries.FirstOrDefault() == null
                                           ? string.Empty
                                           : part.AvailableGalleries.FirstOrDefault().Value;
            }

            return ContentShape("Parts_ImageGallery_Edit",
                                () => shapeHelper.EditorTemplate(
                                    TemplateName: "Parts/ImageGallery",
                                    Model: part,
                                    Prefix: Prefix));
        }

        //POST
        protected override DriverResult Editor(
            ImageGalleryPart part, IUpdateModel updater, dynamic shapeHelper) {
            updater.TryUpdateModel(part, Prefix, null, null);
            part.MediaPath = part.SelectedGallery;
            return Editor(part, shapeHelper);
        }
    }
}