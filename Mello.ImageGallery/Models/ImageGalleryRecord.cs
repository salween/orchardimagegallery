using Orchard.ContentManagement.Records;

namespace Mello.ImageGallery.Models {
    public class ImageGalleryRecord : ContentPartRecord {
        public virtual string ImageGalleryName { get; set; }

        public virtual byte SelectedPlugin { get; set; }
    }
}