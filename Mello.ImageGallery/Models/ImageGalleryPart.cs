using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Mello.ImageGallery.ViewModels;
using Orchard.ContentManagement;

namespace Mello.ImageGallery.Models {
    public class ImageGalleryPart : ContentPart<ImageGalleryRecord> {

        public virtual string MediaPath {
            get { return Record.MediaPath; }
            set { Record.MediaPath = value; }
        }

        public bool HasAvailableGalleries {
            get { return AvailableGalleries != null && AvailableGalleries.Count() > 0; }
        }

        public string SelectedGallery { get; set; } // used on editor

        public IEnumerable<SelectListItem> AvailableGalleries { get; set; } // used on editor

        public Plugins.Plugin SelectedPlugin {
          get { return (Plugins.Plugin)Record.SelectedPlugin; }
          set { Record.SelectedPlugin = (byte)value; }
        }

        public IEnumerable<SelectListItem> AvailablePlugins { get; set; } // used on editor

        public virtual ImageGalleryViewModel ViewModel { get; set; } // used on display
    }
}