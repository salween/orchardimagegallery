using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ImageGallery.Services;
using Mello.ImageGallery.Models;
using Orchard.Data;
using Orchard.Media.Models;
using Orchard.Media.Services;

namespace Mello.ImageGallery.Services {
    public class ImageGalleryService : IImageGalleryService {
        private const string ImageGalleriesMediaFolder = "ImageGalleries";
        private const int ThumbnailDefaultSize = 100;

        private readonly IMediaService _mediaService;
        private readonly IThumbnailService _thumbnailService;
        private readonly IRepository<ImageGallerySettingsRecord> _repository;
        private readonly IRepository<ImageGalleryImageSettingsRecord> _imageRepository;
        private readonly IRepository<ImageGalleryRecord> _imageGalleryPartRepository;

        //TODO: Remove Image repository as soon as it can cascade the saving
        public ImageGalleryService(IMediaService mediaService, IRepository<ImageGallerySettingsRecord> repository,
                                   IRepository<ImageGalleryImageSettingsRecord> imageRepository, IThumbnailService thumbnailService,
                                   IRepository<ImageGalleryRecord> imageGalleryPartRepository) {
            _imageGalleryPartRepository = imageGalleryPartRepository;
            _repository = repository;
            _mediaService = mediaService;
            _imageRepository = imageRepository;
            _thumbnailService = thumbnailService;

            if (!_mediaService.GetMediaFolders(string.Empty).Any(o => o.Name == ImageGalleriesMediaFolder)) {
                _mediaService.CreateFolder(string.Empty, ImageGalleriesMediaFolder);
            }
        }

        public IEnumerable<Models.ImageGallery> GetImageGalleries() {
            return _mediaService.GetMediaFolders(ImageGalleriesMediaFolder).Select(CreateImageGalleryFromMediaFolder);
        }

        public void CreateImageGallery(string name) {
            _mediaService.CreateFolder(ImageGalleriesMediaFolder, name);
        }

        public void DeleteImageGallery(string name) {
            var gallerySettings = GetImageGallerySettings(GetMediaPath(name));

            foreach (ImageGalleryImage image in GetImageGallery(name).Images) {
                DeleteImage(name, image.Name, GetImageSettings(gallerySettings, image.Name));
            }

            if (gallerySettings != null)
                _repository.Delete(gallerySettings);
            _mediaService.DeleteFolder(GetMediaPath(name));
        }

        public void RenameImageGallery(string imageGalleryName, string newName) {
            string mediaPath = GetMediaPath(imageGalleryName);
            _mediaService.RenameFolder(mediaPath, newName);

            ImageGallerySettingsRecord settings = GetImageGallerySettings(imageGalleryName);
            if (settings != null) {
                settings.ImageGalleryName = newName;
                _repository.Update(settings);
            }

            IEnumerable<ImageGalleryRecord> records = _imageGalleryPartRepository.Fetch(partRecord => partRecord.ImageGalleryName == imageGalleryName);

            foreach (ImageGalleryRecord imageGalleryRecord in records) {
                imageGalleryRecord.ImageGalleryName = newName;
                _imageGalleryPartRepository.Update(imageGalleryRecord);
            }
        }

        public void UpdateImageGalleryProperties(string imageGalleryName, int thumbnailHeight, int thumbnailWidth) {
            var imageGallery = GetImageGallery(imageGalleryName);
            var imageGallerySettings = GetImageGallerySettings(imageGallery.MediaPath);

            if (imageGallerySettings == null) {
                CreateImageGallerySettings(imageGallery.MediaPath, thumbnailHeight, thumbnailWidth);
            }
            else {
                imageGallerySettings.ThumbnailHeight = thumbnailHeight;
                imageGallerySettings.ThumbnailWidth = thumbnailWidth;

                _repository.Update(imageGallerySettings);
            }
        }

        public ImageGalleryImage GetImage(string imageGalleryName, string imageName) {
            string imageGalleryMediaPath = GetMediaPath(imageGalleryName);
            ImageGallerySettingsRecord imageGallerySettings = GetImageGallerySettings(imageGalleryMediaPath);

            MediaFile file = _mediaService.GetMediaFiles(imageGalleryMediaPath)
                .SingleOrDefault(mediaFile => mediaFile.Name == imageName);

            if (file == null) {
                return null;
            }

            return CreateImageFromMediaFile(file, imageGallerySettings);
        }

        public Models.ImageGallery GetImageGallery(string imageGalleryName) {
            if (imageGalleryName.Contains("\\"))
                imageGalleryName = GetName(imageGalleryName);

            var mediaFolder = _mediaService.GetMediaFolders(ImageGalleriesMediaFolder).SingleOrDefault(m => m.Name == imageGalleryName);

            if (mediaFolder != null) {
                return CreateImageGalleryFromMediaFolder(mediaFolder);
            }
            return null;
        }

        public void AddImage(string imageGalleryName, HttpPostedFileBase imageFile) {
            _mediaService.UploadMediaFile(GetMediaPath(imageGalleryName), imageFile, false);
        }

        public void UpdateImageProperties(string imageGalleryName, string imageName, string imageTitle, string imageCaption) {
            UpdateImageProperties(imageGalleryName, imageName, imageTitle, imageCaption, null);
        }

        private void UpdateImageProperties(string imageGalleryName, string imageName, string imageTitle, string imageCaption, int? position) {
            var image = GetImage(imageGalleryName, imageName);
            var imageGallery = GetImageGallery(imageGalleryName);

            var imageGallerySettings = GetImageGallerySettings(imageGallery.MediaPath);

            if (imageGallerySettings.ImageSettings.Any(o => o.Name == image.Name)) {
                var imageSetting = imageGallerySettings.ImageSettings.Single(o => o.Name == image.Name);
                imageSetting.Caption = imageCaption;
                imageSetting.Title = imageTitle;
                if (position.HasValue)
                    imageSetting.Position = position.Value;
                _imageRepository.Update(imageSetting); // TODO: Remove when cascade is fixed
            }
            else {
                var imageSetting = new ImageGalleryImageSettingsRecord {Caption = imageCaption, Name = image.Name, Title = imageTitle};
                if (position.HasValue)
                    imageSetting.Position = position.Value;
                imageGallerySettings.ImageSettings.Add(imageSetting);
                _imageRepository.Create(imageSetting); // TODO: Remove when cascade is fixed
            }

            // TODO: See how to cascade changes          
            _repository.Update(imageGallerySettings);
        }

        private ImageGallerySettingsRecord GetImageGallerySettings(string imageGalleryName) {
            if (imageGalleryName.Contains("\\"))
                imageGalleryName = GetName(imageGalleryName);
            return _repository.Get(o => o.ImageGalleryName == imageGalleryName);
        }

        private ImageGalleryImageSettingsRecord GetImageSettings(ImageGallerySettingsRecord imageGallerySettings, string imageName) {
            if (imageGallerySettings == null)
                return null;
            return imageGallerySettings.ImageSettings.SingleOrDefault(o => o.Name == imageName);
        }

        private ImageGalleryImageSettingsRecord GetImageSettings(string imageGalleryName, string imageName) {
            var imageGallerySettings = GetImageGallerySettings(GetMediaPath(imageGalleryName));

            return imageGallerySettings.ImageSettings.SingleOrDefault(o => o.Name == imageName);
        }

        private Models.ImageGallery CreateImageGalleryFromMediaFolder(MediaFolder mediaFolder) {
            var images = _mediaService.GetMediaFiles(mediaFolder.MediaPath);
            ImageGallerySettingsRecord imageGallerySettings = GetImageGallerySettings(GetName(mediaFolder.MediaPath)) ??
                                                              CreateImageGallerySettings(mediaFolder.MediaPath, ThumbnailDefaultSize,
                                                                                         ThumbnailDefaultSize);

            return new Models.ImageGallery
                   {
                       Id = imageGallerySettings.Id,
                       LastUpdated = mediaFolder.LastUpdated,
                       MediaPath = mediaFolder.MediaPath,
                       Name = mediaFolder.Name,
                       Size = mediaFolder.Size,
                       User = mediaFolder.User,
                       ThumbnailHeight = imageGallerySettings.ThumbnailHeight,
                       ThumbnailWidth = imageGallerySettings.ThumbnailWidth,
                       Images = images.Select(image => CreateImageFromMediaFile(image, imageGallerySettings)).OrderBy(image => image.Position)
                   };
        }

        private ImageGallerySettingsRecord CreateImageGallerySettings(string imageGalleryMediaPath, int thumbnailHeight, int thumbnailWidth) {
            ImageGallerySettingsRecord imageGallerySettings = new ImageGallerySettingsRecord
                                                              {
                                                                  ImageGalleryName = GetName(imageGalleryMediaPath),
                                                                  ThumbnailHeight = thumbnailHeight,
                                                                  ThumbnailWidth = thumbnailWidth
                                                              };
            _repository.Create(imageGallerySettings);

            return imageGallerySettings;
        }

        private ImageGalleryImage CreateImageFromMediaFile(MediaFile mediaFile, ImageGallerySettingsRecord imageGallerySettings) {
            if (imageGallerySettings == null) {
                throw new ArgumentNullException("imageGallerySettings");
            }

            var imageSettings = GetImageSettings(imageGallerySettings, mediaFile.Name);
            bool isValidThumbnailSize = imageGallerySettings.ThumbnailWidth > 0 &&
                                        imageGallerySettings.ThumbnailHeight > 0;
            string thumbnailUrl = string.Empty;

            if (isValidThumbnailSize) {
                thumbnailUrl = _thumbnailService.GetThumbnail(mediaFile.FolderName + "\\" + mediaFile.Name,
                                                              imageGallerySettings.ThumbnailWidth,
                                                              imageGallerySettings.ThumbnailHeight);
            }

            return new ImageGalleryImage
                   {
                       PublicUrl = _mediaService.GetPublicUrl(System.IO.Path.Combine(mediaFile.FolderName, mediaFile.Name)),
                       Name = mediaFile.Name,
                       Size = mediaFile.Size,
                       User = mediaFile.User,
                       LastUpdated = mediaFile.LastUpdated,
                       Caption = imageSettings == null ? string.Empty : imageSettings.Caption,
                       ThumbnailPublicUrl = thumbnailUrl,
                       Title = imageSettings == null ? null : imageSettings.Title,
                       Position = imageSettings == null ? 0 : imageSettings.Position
                   };
        }

        private string GetMediaPath(string imageGalleryName) {
            return string.Concat(ImageGalleriesMediaFolder, "\\", imageGalleryName);
        }

        private string GetMediaPath(string imageGalleryName, string imageName) {
            return string.Concat(ImageGalleriesMediaFolder, "\\", imageGalleryName, "\\", imageName);
        }

        private string GetName(string mediaPath) {
            return mediaPath.Split('\\').Last();
        }

        public void DeleteImage(string imageGalleryName, string imageName) {
            var imageSettings = GetImageSettings(imageGalleryName, imageName);
            DeleteImage(imageGalleryName, imageName, imageSettings);
        }

        public string GetPublicUrl(string path) {
            return _mediaService.GetPublicUrl(path);
        }

        public bool IsFileAllowed(HttpPostedFileBase file) {
            return _mediaService.FileAllowed(file);
        }

        private void DeleteImage(string imageGalleryName, string imageName, ImageGalleryImageSettingsRecord imageSettings) {
            if (imageSettings != null) {
                _imageRepository.Delete(imageSettings);
            }
            _mediaService.DeleteFile(imageName, GetMediaPath(imageGalleryName));
        }


        public void ReorderImages(string imageGalleryName, IEnumerable<string> images) {
            Models.ImageGallery imageGallery = GetImageGallery(imageGalleryName);
            int position = 0;

            foreach (string image in images) {
                ImageGalleryImage imageGalleryImage = imageGallery.Images.Single(o => o.Name == image);
                imageGalleryImage.Position = position++;
                UpdateImageProperties(imageGalleryName, imageGalleryImage.Name, imageGalleryImage.Title, imageGalleryImage.Caption,
                                      imageGalleryImage.Position);
            }

            foreach (ImageGalleryImage imageGalleryImage in imageGallery.Images.Where(o => !images.Contains(o.Name))) {
                imageGalleryImage.Position = position++;
                UpdateImageProperties(imageGalleryName, imageGalleryImage.Name, imageGalleryImage.Title, imageGalleryImage.Caption,
                                      imageGalleryImage.Position);
            }
        }
    }
}