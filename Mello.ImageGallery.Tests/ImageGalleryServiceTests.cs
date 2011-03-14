﻿using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using ImageGallery.Services;
using Mello.ImageGallery.Models;
using Mello.ImageGallery.Services;
using Moq;
using NUnit.Framework;
using Orchard.Data;
using Orchard.Media.Services;
using Orchard.Media.Models;
using System.Linq.Expressions;

namespace Mello.ImageGallery.Tests {
    [TestFixture]
    public class ImageGalleryServiceTests {
        private IContainer _container;
        private IImageGalleryService _imageGalleryService;
        private Mock<IMediaService> _mediaServiceMock;
        private Mock<IRepository<ImageGallerySettingsRecord>> _repositoryMock;
        private Mock<IRepository<ImageGalleryImageSettingsRecord>> _imageRepositoryMock;
        private Mock<IThumbnailService> _thumbnailServiceMock;
        private const string ImageGalleryFolderName = "ImageGalleries";

        [SetUp]
        public void Init() {
            _mediaServiceMock = new Mock<IMediaService>();
            _repositoryMock = new Mock<IRepository<ImageGallerySettingsRecord>>();
            _imageRepositoryMock = new Mock<IRepository<ImageGalleryImageSettingsRecord>>();
            _thumbnailServiceMock = new Mock<IThumbnailService>();
            _repositoryMock.Setup(
                repository => repository.Get(It.IsAny<Expression<Func<ImageGallerySettingsRecord, bool>>>())).Returns(
                    (TestUtils.GetImageGallerySettingsRecord()));
            IRepository<ImageGalleryRecord> partRepository = new Mock<IRepository<ImageGalleryRecord>>().Object;

            var builder = new ContainerBuilder();
            builder.RegisterType<ImageGalleryService>().As<IImageGalleryService>();
            builder.RegisterInstance(_mediaServiceMock.Object).As<IMediaService>();
            builder.RegisterInstance(_repositoryMock.Object).As<IRepository<ImageGallerySettingsRecord>>();
            builder.RegisterInstance(_imageRepositoryMock.Object).As<IRepository<ImageGalleryImageSettingsRecord>>();
            builder.RegisterInstance(_thumbnailServiceMock.Object).As<IThumbnailService>();
            builder.RegisterInstance(partRepository).As<IRepository<ImageGalleryRecord>>();

            _container = builder.Build();

            _imageGalleryService = _container.Resolve<IImageGalleryService>();
        }

        [Test]
        public void Galleries_Folder_Should_Be_Created_On_Instantiation() {
            // Arrange
            _mediaServiceMock.Setup(
                mediaService => mediaService.CreateFolder(ImageGalleryFolderName, It.IsAny<string>())).Verifiable();

            // Assert
            _mediaServiceMock.Verify(o => o.CreateFolder(string.Empty, ImageGalleryFolderName));
        }

        [Test]
        public void Can_Get_ImageGalleries() {
            _mediaServiceMock.Setup(mediaService => mediaService.GetMediaFolders(It.IsAny<string>()))
                .Returns(() => TestUtils.GetMediaFolders(1)).Verifiable();

            // Act
            var imageGalleries = _imageGalleryService.GetImageGalleries();

            // Assert
            _mediaServiceMock.Verify();
            Assert.AreEqual(1, imageGalleries.Count());
        }

        [Test]
        public void Can_Get_ImageGallery() {
            // Arrange
            _mediaServiceMock.Setup(mediaService => mediaService.GetMediaFolders(It.IsAny<string>())).Returns(
                TestUtils.GetMediaFolders(5));

            // Act
            Models.ImageGallery imageGallery = _imageGalleryService.GetImageGallery("1");

            // Assert
            Assert.IsNotNull(imageGallery);
            Assert.IsNotNullOrEmpty(imageGallery.Name);
            Assert.AreEqual("1", imageGallery.Name);
        }

        [Test]
        public void Can_Get_ImageGallery_Images() {
            // Arrange
            List<MediaFile> mediaFiles = new List<MediaFile>(new[] {new MediaFile {FolderName = "any", Name = "any"}});
            _mediaServiceMock.Setup(mediaService => mediaService.GetMediaFolders(It.IsAny<string>())).Returns(
                TestUtils.GetMediaFolders(5));
            _mediaServiceMock.Setup(mediaService => mediaService.GetMediaFiles(It.IsAny<string>())).Returns(mediaFiles);

            // Act
            Models.ImageGallery imageGallery = _imageGalleryService.GetImageGallery("gallery");

            // Assert
            Assert.IsNotNull(imageGallery);
            Assert.AreEqual(1, imageGallery.Images.Count());
        }

        [Test]
        public void Can_Create_ImageGallery() {
            // Arrange
            _mediaServiceMock.Setup(mediaService => mediaService.CreateFolder(ImageGalleryFolderName, "TestGallery")).
                Verifiable();

            // Act
            _imageGalleryService.CreateImageGallery("TestGallery");

            // Assert
            _mediaServiceMock.Verify();
        }

        [Test]
        public void Can_Delete_ImageGallery() {
            // Arrange
            _mediaServiceMock.Setup(mediaService => mediaService.GetMediaFolders(It.IsAny<string>())).Returns(
                TestUtils.GetMediaFolders(5));
            _mediaServiceMock.Setup(mediaService => mediaService.DeleteFolder(ImageGalleryFolderName + "\\" + "gallery"))
                .Verifiable();
            _repositoryMock.Setup(o => o.Delete(It.IsAny<ImageGallerySettingsRecord>())).Verifiable();

            // Act
            _imageGalleryService.DeleteImageGallery("gallery");

            // Assert
            _mediaServiceMock.Verify();
            _repositoryMock.Verify();
        }

        [Test]
        public void Can_Update_ImageGallery_Creating_Properties() {
            // Arrange
            _repositoryMock.Setup(o => o.Get(It.IsAny<Expression<Func<ImageGallerySettingsRecord, bool>>>())).Returns(
                (ImageGallerySettingsRecord) null);
            _repositoryMock.Setup(
                o =>
                o.Create(
                    It.Is<ImageGallerySettingsRecord>(
                        record => record.ThumbnailHeight == 200 && record.ThumbnailWidth == 300))).Verifiable();
            _mediaServiceMock.Setup(mediaService => mediaService.GetMediaFolders(It.IsAny<string>())).Returns(
                TestUtils.GetMediaFolders(5));

            // Act
            _imageGalleryService.UpdateImageGalleryProperties("4", 200, 300);

            // Assert
            _mediaServiceMock.Verify();
            _repositoryMock.Verify();
        }

        [Test]
        public void Can_Update_ImageGallery_Updating_Properties() {
            // Arrange          
            _repositoryMock.Setup(
                o =>
                o.Update(
                    It.Is<ImageGallerySettingsRecord>(
                        record => record.ThumbnailHeight == 200 && record.ThumbnailWidth == 300))).Verifiable();
            _mediaServiceMock.Setup(mediaService => mediaService.GetMediaFolders(It.IsAny<string>())).Returns(
                TestUtils.GetMediaFolders(5));

            // Act
            _imageGalleryService.UpdateImageGalleryProperties("4", 200, 300);

            // Assert
            _mediaServiceMock.Verify();
            _repositoryMock.Verify();
        }

        [Test]
        public void Can_Add_Image_To_ImageGallery() {
            // Arrange
            var fileMock = new Mock<System.Web.HttpPostedFileBase>().Object;
            _mediaServiceMock.Setup(
                mediaService => mediaService.UploadMediaFile(ImageGalleryFolderName + "\\gallery", fileMock, false)).
                Verifiable();

            // Act
            _imageGalleryService.AddImage("gallery", fileMock);

            // Assert
            _mediaServiceMock.Verify();
        }

        [Test]
        public void Can_Get_Image() {
            // Arrange
            List<MediaFile> mediaFiles =
                new List<MediaFile>(new[] {new MediaFile {Name = "image1", FolderName = "gallery"}});

            _mediaServiceMock.Setup(mediaService => mediaService.GetMediaFiles(ImageGalleryFolderName + "\\gallery")).
                Returns(mediaFiles).Verifiable();

            // Act
            ImageGalleryImage result = _imageGalleryService.GetImage("gallery", "image1");

            // Assert
            _mediaServiceMock.Verify();
            Assert.AreEqual("image1", result.Name);
        }

        [Test]
        public void Can_Update_Image_Properties() {
            // Arrange
            _imageRepositoryMock.Setup(
                o =>
                o.Create(
                    It.Is<ImageGalleryImageSettingsRecord>(
                        record => record.Name == "image3" && record.Caption == "caption"))).Verifiable();
            _mediaServiceMock.Setup(mediaService => mediaService.GetMediaFiles(It.IsAny<string>())).Returns(
                TestUtils.GetMediaFiles(5));
            _mediaServiceMock.Setup(mediaService => mediaService.GetMediaFolders(It.IsAny<string>())).Returns(
                TestUtils.GetMediaFolders(5));

            // Act
            _imageGalleryService.UpdateImageProperties("gallery", "image3", "title", "caption");

            // Assert
            _imageRepositoryMock.Verify();
            _mediaServiceMock.Verify();
        }

        [Test]
        public void Can_Delete_Image() {
            // Arrange
            _imageRepositoryMock.Setup(o => o.Delete(It.IsAny<ImageGalleryImageSettingsRecord>())).Verifiable();
            _mediaServiceMock.Setup(o => o.DeleteFile("image1", ImageGalleryFolderName + "\\gallery")).Verifiable();

            // Act
            _imageGalleryService.DeleteImage("gallery", "image1");

            // Assert
            _imageRepositoryMock.Verify();
            _mediaServiceMock.Verify();
        }

        [Test]
        public void Can_Get_Public_Url() {
            // Arrange
            _mediaServiceMock.Setup(mediaService => mediaService.GetPublicUrl("any")).Returns("ok").Verifiable();

            // Act
            string result = _imageGalleryService.GetPublicUrl("any");

            // Assert
            _mediaServiceMock.Verify();
            Assert.AreEqual("ok", result);
        }

        [Test]
        public void Can_Reorder_Images() {
            // Arrange
            List<string> images = new List<string>();
            images.Add("image1");
            images.Add("image2");
            images.Add("image3");

            _imageRepositoryMock.Setup(o => o.Create(It.IsAny<ImageGalleryImageSettingsRecord>())).Verifiable();
            _mediaServiceMock.Setup(mediaService => mediaService.GetMediaFiles(It.IsAny<string>())).Returns(
                TestUtils.GetMediaFiles(5));
            _mediaServiceMock.Setup(mediaService => mediaService.GetMediaFolders(It.IsAny<string>())).Returns(
                TestUtils.GetMediaFolders(5));

            // Act
            _imageGalleryService.ReorderImages("gallery", images);

            // Assert
            _imageRepositoryMock.Verify();
            _mediaServiceMock.Verify();
        }

        [Test]
        public void Can_Rename_Image_Gallery() {
            // Arrange
            _mediaServiceMock.Setup(mediaService => mediaService.GetMediaFolders(It.IsAny<string>())).Returns(
                TestUtils.GetMediaFolders(5));
            _mediaServiceMock.Setup(
                mediaService => mediaService.RenameFolder(ImageGalleryFolderName + "\\" + "gallery", "newGallery")).
                Verifiable();

            // Act
            _imageGalleryService.RenameImageGallery("gallery", "newGallery");

            // Assert
            _mediaServiceMock.Verify();
        }
    }
}