using Orchard.ContentManagement.Records;
using System.ComponentModel.DataAnnotations;

namespace Mello.ImageGallery.Models {
    public class ImageGalleryRecord : ContentPartRecord {
        public virtual string MediaPath { get; set; }

        public virtual byte SelectedPlugin { get; set; }
    }
}