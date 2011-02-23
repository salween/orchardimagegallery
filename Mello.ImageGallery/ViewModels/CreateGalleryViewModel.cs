using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ImageGallery.ViewModels {
    public class CreateGalleryViewModel {
        [Required]
        public string GalleryName { get; set; }
    }
}