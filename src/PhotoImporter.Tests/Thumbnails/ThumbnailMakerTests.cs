namespace PhotoImporter.Tests.Thumbnails;

[TestClass]
public class ThumbnailMakerTests : _TestBase {
    const string SOURCE_PATH = "/fakepath/source.jpg",
        TARGET_PATH = "/fakepath/target.jpg";

    const int OLD_HEIGHT = 1000,
        OLD_WIDTH = 600,
        NEW_HEIGHT = 100,
        NEW_WIDTH = 60;

    IThumbnailMaker _thumbMaker;

    Mock<IImage> _image;

    ISetup<IFilesystem, Stream> _getFilestream;
    ISetup<IFilesystem, IImage> _loadImage;

    [TestInitialize]
    public void Setup() {
        _image = new Mock<IImage>();
        _image.Setup(x => x.Height).Returns(OLD_HEIGHT);
        _image.Setup(x => x.Width).Returns(OLD_WIDTH);

        _getFilestream = _filesystem.Setup(x => x.GetFileStream(It.IsAny<string>(), It.IsAny<FileMode>()));
        _loadImage = _filesystem.Setup(x => x.LoadImage(It.IsAny<Stream>()));
        _loadImage.Returns(_image.Object);

        _thumbMaker = new ThumbnailMaker(_dependencies.Object);
    }

    [TestMethod]
    public void MakeThumbnail_FileStreamLoaded() {
        _getFilestream.Verifiable();

        makeThumbnail();

        _filesystem.Verify(x => x.GetFileStream(SOURCE_PATH, FileMode.Open), Times.Once);
    }

    [TestMethod]
    public void MakeThumbnail_ImageLoaded() {
        using (var stream = new MemoryStream()) {
            _getFilestream.Returns(stream);
            _loadImage.Verifiable();

            makeThumbnail();

            _filesystem.Verify(x => x.LoadImage(stream), Times.Once);
        }
    }

    [TestMethod]
    public void MakeThumbnail_ImageMutated() {
        using (var stream = new MemoryStream()) {
            _getFilestream.Returns(stream);
            _image.Setup(x => x.Resize(It.IsAny<int>(), It.IsAny<int>())).Verifiable();

            makeThumbnail();

            _image.Verify(x => x.Resize(NEW_WIDTH, NEW_HEIGHT), Times.Once);
        }
    }

    [TestMethod]
    public void MakeThumbnail_ImageSaved() {
        using (var stream = new MemoryStream()) {
            _getFilestream.Returns(stream);
            _image.Setup(x => x.Save(It.IsAny<string>())).Verifiable();

            makeThumbnail();

            _image.Verify(x => x.Save(TARGET_PATH), Times.Once);
        }
    }

    void makeThumbnail() => _thumbMaker.MakeThumbnail(SOURCE_PATH, TARGET_PATH, NEW_HEIGHT);
}