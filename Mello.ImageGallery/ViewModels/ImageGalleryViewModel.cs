﻿using System.Collections.Generic;
using Mello.ImageGallery.Models;
using Mello.ImageGallery.Models.Plugins.LightBox;
using Mello.ImageGallery.Models.Plugins;

namespace Mello.ImageGallery.ViewModels {
    public class ImageGalleryViewModel {
        public IEnumerable<ImageGalleryImage> Images { get; set; }

        //public LightBoxSettings PluginSettings { get; set; }
        public ImageGalleryPlugin ImageGalleryPlugin { get; set; }
    }
}