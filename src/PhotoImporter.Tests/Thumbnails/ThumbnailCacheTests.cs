using System.IO;

namespace PhotoImporter.Tests.Thumbnails;

[TestClass]
public class ThumbnailCacheTests : _TestBase {
    const string THUMBNAIL_PATH = "/fakepath/thumbnails",
        LIBRARY_PATH = "/fakepath/library",
        PHOTO_ID = "PhotoId123",
        PHOTO_EXTENSION = ".jpg";

    static readonly DateTime DATE_TAKEN = DateTime.Parse("2021-02-03");

    static readonly PhotoWithoutThumbnail PHOTO = new PhotoWithoutThumbnail() {
        Id = PHOTO_ID,
        Extension = PHOTO_EXTENSION,
        DateTaken = DATE_TAKEN,
        OriginalFilename = "OriginalFile" + PHOTO_EXTENSION
    };

    ISetup<IFilesystem, bool> _fileExists,
        _directoryExists;
    
    ISetup<IFilesystem> _createDirectory;

    ISetup<IThumbnailMaker> _makeThumbnail;

    IThumbnailCache _thumbCache;

    [TestInitialize]
    public void Setup() {
        _configReader.Setup(x => x.AppConfig).Returns(new AppConfig() {
            ThumbnailPath = THUMBNAIL_PATH,
            StoragePath = LIBRARY_PATH,
        });

        _fileExists = _filesystem.Setup(x => x.FileExists(It.IsAny<string>()));
        _directoryExists = _filesystem.Setup(x => x.DirectoryExists(It.IsAny<string>()));
        _createDirectory = _filesystem.Setup(x => x.CreateDirectory(It.IsAny<string>()));
        _makeThumbnail = _thumbnailMaker.Setup(x => x.MakeThumbnail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));

        _thumbCache = new ThumbnailCache(_dependencies.Object);
    }

    [TestMethod]
    public void CacheThumbnails_ThumbnailsCheckedForExistence() {
        _fileExists.Verifiable();

        cacheThumbnails();

        _filesystem.Verify(x => x.FileExists(It.IsAny<string>()), Times.Exactly(3));

        _filesystem.Verify(x => x.FileExists(makeThumbnailPath(100)), Times.Once);
        _filesystem.Verify(x => x.FileExists(makeThumbnailPath(200)), Times.Once);
        _filesystem.Verify(x => x.FileExists(makeThumbnailPath(300)), Times.Once);
    }

    [TestMethod]
    public void CacheThumbnails_DirectoryCheckedIfThumbnailDoesNotExist() {
        _fileExists.Returns((string path) => {
            if (path.Contains(Path.DirectorySeparatorChar + "100" + Path.DirectorySeparatorChar))
                return false;
            else
                return true;
        });
        _directoryExists.Verifiable();

        cacheThumbnails();

        _filesystem.Verify(x => x.DirectoryExists(It.IsAny<string>()), Times.Once);

        _filesystem.Verify(x => x.DirectoryExists(makeThumbnailDirectory(100)), Times.Once);
        _filesystem.Verify(x => x.DirectoryExists(makeThumbnailDirectory(200)), Times.Never);
        _filesystem.Verify(x => x.DirectoryExists(makeThumbnailDirectory(300)), Times.Never);
    }

    [TestMethod]
    public void CacheThumbnails_DirectoryCreatedIfNotAlreadyExisting() {
        _fileExists.Returns(false);
        _directoryExists.Returns((string path) => {
            if (path.Contains(Path.DirectorySeparatorChar + "100" + Path.DirectorySeparatorChar))
                return false;
            else
                return true;
        });
        _createDirectory.Verifiable();

        cacheThumbnails();

        _filesystem.Verify(x => x.CreateDirectory(It.IsAny<string>()), Times.Once);

        _filesystem.Verify(x => x.CreateDirectory(makeThumbnailDirectory(100)), Times.Once);
        _filesystem.Verify(x => x.CreateDirectory(makeThumbnailDirectory(200)), Times.Never);
        _filesystem.Verify(x => x.CreateDirectory(makeThumbnailDirectory(300)), Times.Never);
    }

    [TestMethod]
    public void CacheThumbnails_ThumbnailMadeIfNotAlreadyExisting() {
        _fileExists.Returns((string path) => {
            if (path.Contains(Path.DirectorySeparatorChar + "100" + Path.DirectorySeparatorChar))
                return false;
            else
                return true;
        });
        _makeThumbnail.Verifiable();

        cacheThumbnails();

        _thumbnailMaker.Verify(x => x.MakeThumbnail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);

        _thumbnailMaker.Verify(x => x.MakeThumbnail(makeFullSizePath(100), makeThumbnailPath(100), 100), Times.Once);
        _thumbnailMaker.Verify(x => x.MakeThumbnail(makeFullSizePath(200), makeThumbnailPath(200), 200), Times.Never);
        _thumbnailMaker.Verify(x => x.MakeThumbnail(makeFullSizePath(300), makeThumbnailPath(300), 300), Times.Never);
    }

    string makeThumbnailDirectory(int size) {
        return Path.GetDirectoryName(makeThumbnailPath(size));
    }

    string makeThumbnailPath(int size) {
        return Path.Combine(
            THUMBNAIL_PATH,
            $"{size}",
            $"{PHOTO.DateTaken:yyyy-MM}",
            $"{PHOTO.DateTaken:yyyy-MM-dd}_{PHOTO.Id}{PHOTO.Extension}"
        );
    }

    string makeFullSizePath(int size) {
        return Path.Combine(
            LIBRARY_PATH,
            $"{PHOTO.DateTaken:yyyy-MM}",
            $"{PHOTO.DateTaken:yyyy-MM-dd}_{PHOTO.Id}{PHOTO.Extension}"
        );
    }

    void cacheThumbnails() => _thumbCache.CacheThumbnails(PHOTO);
}