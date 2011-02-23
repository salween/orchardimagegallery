using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageGallery.ViewModels {
    public class ImageAddViewModel {
        public string ImageGalleryName { get; set; }

        public HttpPostedFileBase ImageFile { get; set; }
    }
}