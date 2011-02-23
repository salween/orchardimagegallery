using System.Text;

namespace Mello.ImageGallery.Models {
    public class LightBoxSettings {
        public LightBoxSettings() {
            ButtonCloseImage = "Modules/ImageGallery/Content/Images/lightbox-btn-close.gif";
            ButtonPreviousImage = "Modules/ImageGallery/Content/Images/lightbox-btn-prev.gif";
            ButtonNextImage = "Modules/ImageGallery/Content/Images/lightbox-btn-next.gif";
            LoadingIcon = "Modules/ImageGallery/Content/Images/lightbox-ico-loading.gif";
            ImageBlank = "Modules/ImageGallery/Content/Images/lightbox-blank.gif";
        }

        public string ButtonCloseImage { get; set; }

        public string ButtonPreviousImage { get; set; }

        public string ButtonNextImage { get; set; }

        public string LoadingIcon { get; set; }

        public string ImageBlank { get; set; }

        public override string ToString() {
            StringBuilder stringBuilder = new StringBuilder("{");
            stringBuilder.AppendLine(string.Concat("imageBlank : '", ImageBlank, "' ,"));
            stringBuilder.AppendLine(string.Concat("imageBtnClose: '", ButtonCloseImage, "' ,"));
            stringBuilder.AppendLine(string.Concat("imageBtnPrev: '", ButtonPreviousImage, "' ,"));
            stringBuilder.AppendLine(string.Concat("imageBtnNext: '", ButtonNextImage, "' ,"));
            stringBuilder.AppendLine(string.Concat("imageLoading: '", LoadingIcon, "'"));
            stringBuilder.AppendLine("}");


            return stringBuilder.ToString();
        }
    }
}