using Orchard.ContentManagement.Records;
using System.ComponentModel.DataAnnotations;

namespace Mello.ImageGallery.Models {
    public class ImageGalleryRecord : ContentPartRecord {
        [Required]
        public virtual string MediaPath { get; set; }
    }
}