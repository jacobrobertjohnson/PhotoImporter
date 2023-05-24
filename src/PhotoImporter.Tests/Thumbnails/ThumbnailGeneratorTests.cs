namespace PhotoImporter.Tests.Thumbnails;

[TestClass]
public class ThumbnailGeneratorTests : _TestBase {
    AppConfig _config;

    ISetup<ILibraryManager, IEnumerable<PhotoWithoutThumbnail>> _getPhotosWithoutThumbnails;

    IThumbnailGenerator _thumbGen;

    [TestInitialize]
    public void Setup() {
        _config = new AppConfig();

        _getPhotosWithoutThumbnails = _libraryManager.Setup(x => x.GetPhotosWithoutThumbnails());

        _thumbGen = new ThumbnailGenerator(_dependencies.Object);
    }

    [TestMethod]
    public void MakeThumbnails_ThumbnailsGottenFromLibrary() {
        _getPhotosWithoutThumbnails.Verifiable();

        makeThumbnails();

        _libraryManager.Verify(x => x.GetPhotosWithoutThumbnails(), Times.Once);
    }

    void makeThumbnails() => _thumbGen.MakeThumbnails(_config);
}