namespace Mello.ImageGallery.Models.Plugins {
    public abstract class ImageGalleryPlugin {
        public abstract string ToString(string cssSelector);

        public virtual string AdditionalHrefMarkup {
            get { return string.Empty; }
        }
    }
}