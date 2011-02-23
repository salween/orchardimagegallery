using System.Collections.Generic;
using System.Web;
using Mello.ImageGallery.Models;
using Orchard;

namespace Mello.ImageGallery.Services {
    public interface IImageGalleryService : IDependency {
        IEnumerable<Models.ImageGallery> GetImageGalleries();
        Models.ImageGallery GetImageGallery(string imageGalleryName);

        void CreateImageGallery(string imageGalleryName);
        void DeleteImageGallery(string imageGalleryName);
        void RenameImageGallery(string imageGalleryName, string newName);
        void UpdateImageGalleryProperties(string name, int thumbnailHeight, int thumbnailWidth);

        //IEnumerable<ImageGalleryImage> GetImages();
        ImageGalleryImage GetImage(string galleryName, string imageName);
        void AddImage(string imageGalleryName, System.Web.HttpPostedFileBase imageFile);
        void UpdateImageProperties(string imageGalleryName, string imageName, string imageCaption);
        void DeleteImage(string imageGalleryName, string imageName);

        bool IsFileAllowed(HttpPostedFileBase file);
    }
}