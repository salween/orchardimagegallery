using System.Collections.Generic;

namespace Mello.ImageGallery.ViewModels {
    public class ImageGalleryIndexViewModel {
        public ImageGalleryIndexViewModel() {
            ImageGalleries = new List<Mello.ImageGallery.Models.ImageGallery>();
        }

        public IEnumerable<Mello.ImageGallery.Models.ImageGallery> ImageGalleries { get; set; }
    }
}