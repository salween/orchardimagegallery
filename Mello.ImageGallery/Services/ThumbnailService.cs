using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using Orchard.FileSystems.Media;
using Orchard.Media.Services;

namespace ImageGallery.Services {
    public class ThumbnailService : IThumbnailService {
        private const string ThumbnailFolder = "Thumbnails";

        private readonly ImageFormat _thumbnailImageFormat = ImageFormat.Jpeg;
        private readonly IMediaService _mediaService;
        private readonly IStorageProvider _storageProvider;

        public ThumbnailService(IMediaService mediaService, IStorageProvider storageProvider) {
            _storageProvider = storageProvider;
            _mediaService = mediaService;
        }

        private string GetMediaName(string fullName) {
            return fullName.Split('/').LastOrDefault();
        }

        protected string GetThumbnailFolder(string mediaPath) {
            // Creates a thumbnail folder if doesn't exists
            if (!_mediaService.GetMediaFolders(mediaPath).Select(o => o.Name).Contains(ThumbnailFolder)) {
                _mediaService.CreateFolder(mediaPath, ThumbnailFolder);
            }

            //return string.Concat(mediaPath, "/", ThumbnailFolder);
            return Path.Combine(mediaPath, ThumbnailFolder);
        }

        /// <summary>
        /// Creates an images thumbnail.
        /// </summary>
        /// <param name="image">The image full path on the media storage.</param>
        /// <param name="thumbnailFolderPath">The media path to thumbnails folder.</param>
        /// <param name="imageName">The image name.</param>
        /// <param name="thumbnailWidth">The thumbnail width in pixels.</param>
        /// <param name="thumbnailHeight">The thumbnail height in pixels.</param>
        /// <returns>The thumbnail file media path.</returns>
        protected string CreateThumbnail(string image, string thumbnailFolderPath, string imageName, int thumbnailWidth,
                                         int thumbnailHeight) {
            if (thumbnailWidth <= 0) {
                throw new ArgumentException("Thumbnail width must be greater than zero", "thumbnailWidth");
            }

            if (thumbnailHeight <= 0) {
                throw new ArgumentException("Thumbnail height must be greater than zero", "thumbnailHeight");
            }

            string thumbnailFilePath = string.Concat(thumbnailFolderPath, "/", imageName);

            IStorageFile imageFile = _storageProvider.GetFile(image);
            using (Stream imageStream = imageFile.OpenRead()) {
                Image drawingImage = Image.FromStream(imageStream);
                Image thumbDrawing = drawingImage.GetThumbnailImage(thumbnailWidth, thumbnailHeight, null, new IntPtr());
                if (_storageProvider.ListFiles(thumbnailFolderPath).Select(o => o.GetName()).Contains(imageName)) {
                    _storageProvider.DeleteFile(thumbnailFilePath);
                }
                IStorageFile thumbFile = _storageProvider.CreateFile(thumbnailFilePath);

                using (var thumbStream = thumbFile.OpenWrite()) {
                    thumbDrawing.Save(thumbStream, _thumbnailImageFormat);
                }
            }

            return thumbnailFilePath;
        }

        /// <summary>
        /// Gets a thumbnail for an image.
        /// </summary>
        /// <param name="image">The image full path on the media storage.</param>
        /// <param name="thumbnailWidth">The thumbnail width in pixels.</param>
        /// <param name="thumbnailHeight">The thumbnail height in pixels.</param>
        /// <returns>The thumbnail full path on the media storage.</returns>
        public string GetThumbnail(string image, int thumbnailWidth, int thumbnailHeight) {
            string imageName = Path.GetFileName(image);
            string mediaPath = image.Substring(0, image.Length - imageName.Length - 1);
            string thumbnailFolderPath = GetThumbnailFolder(mediaPath);

            var thumbnailName = _mediaService.GetMediaFiles(thumbnailFolderPath)
                .Select(o => o.Name).SingleOrDefault(o => o == imageName);

            if (string.IsNullOrEmpty(thumbnailName)) {
                thumbnailName =
                    GetMediaName(CreateThumbnail(image, thumbnailFolderPath, imageName, thumbnailWidth, thumbnailHeight));
            }
            else {
                bool isCorrectSize;

                // Verify if the existing thumbnail has the correct size
                //string thumbNailPath = string.Concat(thumbnailFolderPath, "/", thumbnailName);
                string thumbNailPath = Path.Combine(thumbnailFolderPath, thumbnailName);
                using (Stream imageStream = _storageProvider.GetFile(thumbNailPath).OpenRead()) {
                    Image imageDrawing = Image.FromStream(imageStream);

                    isCorrectSize = imageDrawing.Height == thumbnailHeight && imageDrawing.Width == thumbnailWidth;
                }

                if (!isCorrectSize) {
                    CreateThumbnail(image, thumbnailFolderPath, imageName, thumbnailWidth, thumbnailHeight);
                }
            }

            return string.Concat(_mediaService.GetPublicUrl(thumbnailFolderPath), "/", thumbnailName);
        }
    }
}