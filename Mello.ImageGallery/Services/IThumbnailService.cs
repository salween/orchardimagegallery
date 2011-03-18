using Orchard;

namespace Mello.ImageGallery.Services {
    public interface IThumbnailService : IDependency {
        /// <summary>
        /// Gets a thumbnail for an image.
        /// </summary>
        /// <param name="image">The image full path on the media storage.</param>
        /// <param name="thumbnailWidth">The thumbnail width in pixels.</param>
        /// <param name="thumbnailHeight">The thumbnail height in pixels.</param>
        /// <param name="keepAspectRatio">Indicates whether to keep the original image aspect ratio</param>
        /// <returns>The thumbnail full path on the media storage.</returns>
        string GetThumbnail(string image, int thumbnailWidth, int thumbnailHeight, bool keepAspectRatio);
    }
}