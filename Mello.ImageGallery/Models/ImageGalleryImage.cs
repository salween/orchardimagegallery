using System;

namespace Mello.ImageGallery.Models {
    public class ImageGalleryImage {
        public string Name { get; set; }

        public string Caption { get; set; }

        public long Size { get; set; }

        public string User { get; set; }

        public DateTime LastUpdated { get; set; }

        public string PublicUrl { get; set; }

        public string ThumbnailPublicUrl { get; set; }
    }
}