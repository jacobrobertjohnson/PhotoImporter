namespace PhotoImporter.Tests.Thumbnails;

[TestClass]
public class ThumbnailGeneratorTests : _TestBase {
    const string PHOTO_ID = "PhotoId123",
        PHOTO_ID_2 = "PhotoId234",
        PHOTO_ID_3 = "PhotoId345";

    AppConfig _config;

    ISetup<ILibraryManager, IEnumerable<PhotoWithoutThumbnail>> _getPhotosWithoutThumbnails;
    ISetup<ILibraryManager> _setThumbnailGenerated;
    ISetup<IThumbnailCache> _cacheThumbnails;

    IThumbnailGenerator _thumbGen;

    [TestInitialize]
    public void Setup() {
        _config = new AppConfig();

        _getPhotosWithoutThumbnails = _libraryManager.Setup(x => x.GetPhotosWithoutThumbnails());
        _setThumbnailGenerated = _libraryManager.Setup(x => x.SetThumbnailGenerated(It.IsAny<string>()));
        _cacheThumbnails = _thumbnailCache.Setup(x => x.CacheThumbnails(It.IsAny<PhotoWithoutThumbnail>()));

        _thumbGen = new ThumbnailGenerator(_dependencies.Object);
    }

    [TestMethod]
    public void MakeThumbnails_ThumbnailsGottenFromLibrary() {
        _getPhotosWithoutThumbnails.Verifiable();

        makeThumbnails();

        _libraryManager.Verify(x => x.GetPhotosWithoutThumbnails(), Times.Once);
    }

    [TestMethod]
    public void MakeThumbnails_CacheGeneratedForEachEntry() {
        var photo1 = new PhotoWithoutThumbnail();
        var photo2 = new PhotoWithoutThumbnail();
        var photo3 = new PhotoWithoutThumbnail();

        _getPhotosWithoutThumbnails.Returns(new List<PhotoWithoutThumbnail>() {
            photo1,
            photo2,
            photo3
        });

        _cacheThumbnails.Verifiable();

        makeThumbnails();

        _thumbnailCache.Verify(x => x.CacheThumbnails(It.IsAny<PhotoWithoutThumbnail>()), Times.Exactly(3));
        _thumbnailCache.Verify(x => x.CacheThumbnails(photo1), Times.Once);
        _thumbnailCache.Verify(x => x.CacheThumbnails(photo2), Times.Once);
        _thumbnailCache.Verify(x => x.CacheThumbnails(photo3), Times.Once);
    }

    [TestMethod]
    public void MakeThumbnails_ThumbnailGeneratedSetForEachEntry() {
        _getPhotosWithoutThumbnails.Returns(new List<PhotoWithoutThumbnail>() {
            new PhotoWithoutThumbnail() { Id = PHOTO_ID },
            new PhotoWithoutThumbnail() { Id = PHOTO_ID_2 },
            new PhotoWithoutThumbnail() { Id = PHOTO_ID_3 },
        });
        _setThumbnailGenerated.Verifiable();

        makeThumbnails();

        _libraryManager.Verify(x => x.SetThumbnailGenerated(It.IsAny<string>()), Times.Exactly(3));
        _libraryManager.Verify(x => x.SetThumbnailGenerated(PHOTO_ID), Times.Once);
        _libraryManager.Verify(x => x.SetThumbnailGenerated(PHOTO_ID_2), Times.Once);
        _libraryManager.Verify(x => x.SetThumbnailGenerated(PHOTO_ID_3), Times.Once);
    }

    [TestMethod]
    public void MakeThumbnails_ExceptionThrown_ItIsReported() {
         _getPhotosWithoutThumbnails.Returns(new List<PhotoWithoutThumbnail>() {
            new PhotoWithoutThumbnail() {
                Id = PHOTO_ID
            }
        });

        _cacheThumbnails.Callback((PhotoWithoutThumbnail photo) => {
            throw new ArithmeticException("Your math is wrong");
        });

        makeThumbnails();

        verifySingleMessageStartsWith($"An exception occurred while generating thumbnails for photo {PHOTO_ID}:\nSystem.ArithmeticException: Your math is wrong");
    }

    void makeThumbnails() => _thumbGen.MakeThumbnails();
}