using System.Linq;
using Mello.ImageGallery.Models;
using Mello.ImageGallery.Models.Plugins;
using Mello.ImageGallery.Services;
using Mello.ImageGallery.ViewModels;
using Orchard;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement;
using Orchard.UI.Resources;
using System.Web.Mvc;
using System;

namespace Mello.ImageGallery.Drivers {
    public class ImageGalleryDriver : ContentPartDriver<ImageGalleryPart> {
        private readonly IImageGalleryService _imageGalleryService;
        private readonly IWorkContextAccessor _workContextAccessor;

        public ImageGalleryDriver(IImageGalleryService imageGalleryService, IWorkContextAccessor workContextAccessor) {
            _workContextAccessor = workContextAccessor;
            _imageGalleryService = imageGalleryService;
        }

        private void RegisterStaticContent(PluginResourceDescriptor pluginResourceDescriptor) {
            IResourceManager resourceManager = _workContextAccessor.GetContext().Resolve<IResourceManager>();

            foreach (string script in pluginResourceDescriptor.Scripts) {
                resourceManager.RegisterHeadScript(script);
            }

            foreach (LinkEntry style in pluginResourceDescriptor.Styles) {
                resourceManager.RegisterLink(style);
            }

            resourceManager.Require("script", "jQuery").AtHead();
        }

        protected override DriverResult Display(ImageGalleryPart part, string displayType, dynamic shapeHelper) {
            PluginFactory pluginFactory = PluginFactory.GetFactory(part.SelectedPlugin);
            Models.ImageGallery imageGallery = _imageGalleryService.GetImageGallery(part.ImageGalleryName);

            if (displayType == "SummaryAdmin") {
                // Image gallery returns nothing if in Summary Admin
                return null;
            }

            if (string.IsNullOrWhiteSpace(part.ImageGalleryName))
                return null;

            RegisterStaticContent(pluginFactory.PluginResourceDescriptor);

            ImageGalleryViewModel viewModel = new ImageGalleryViewModel {ImageGalleryPlugin = pluginFactory.Plugin};
            viewModel.Images = imageGallery.Images;

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
                .OrderBy(o => o.Name).Select(o => new SelectListItem
                                                  {
                                                      Text = o.Name,
                                                      Value = o.Name
                                                  });

            if (!string.IsNullOrWhiteSpace(part.ImageGalleryName)) {
                part.SelectedGallery = part.ImageGalleryName;
            }
            else {
                part.SelectedGallery = part.AvailableGalleries.FirstOrDefault() == null
                                           ? string.Empty
                                           : part.AvailableGalleries.FirstOrDefault().Value;
            }

            part.AvailablePlugins = Enum.GetNames(typeof (Plugin))
                .Select(o => new SelectListItem
                             {
                                 Text = o,
                                 Value = o
                             });

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
            part.ImageGalleryName = part.SelectedGallery;
            return Editor(part, shapeHelper);
        }
    }
}