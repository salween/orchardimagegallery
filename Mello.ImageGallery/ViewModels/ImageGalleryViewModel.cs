using System.Collections.Generic;
using Mello.ImageGallery.Models;

namespace Mello.ImageGallery.ViewModels {
    public class ImageGalleryViewModel {
        public IEnumerable<ImageGalleryImage> Images { get; set; }

        public LightBoxSettings PluginSettings { get; set; }
    }
}