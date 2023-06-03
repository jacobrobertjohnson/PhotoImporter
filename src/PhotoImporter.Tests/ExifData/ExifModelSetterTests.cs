using PhotoImporter.ExifData;

namespace PhotoImporter.Tests.ExifData;

[TestClass]
public class ExifModelSetterTests : _TestBase {
    const string STORAGE_PATH = "/fakepath/";
    ISetup<ILibraryManager, List<PhotoWithoutThumbnail>> _getImagesWithoutExifModel;
    ISetup<ILibraryManager> _setExifModel;
    ISetup<IFilesystem, string> _getExifModel;

    IExifModelSetter _exif;

    [TestInitialize]
    public void Setup() {
        _configReader.Setup(x => x.AppConfig).Returns(new AppConfig() {
            StoragePath = STORAGE_PATH
        });

        _getImagesWithoutExifModel = _libraryManager.Setup(x => x.GetImagesWithoutExifModel());
        _setExifModel = _libraryManager.Setup(x => x.SetExifModel(It.IsAny<string>(), It.IsAny<string>()));
        _getExifModel = _filesystem.Setup(x => x.GetExifModel(It.IsAny<string>()));

        _exif = new ExifModelSetter(_dependencies.Object);
    }

    [TestMethod]
    public void SetModel_PhotosGotten() {
        _getImagesWithoutExifModel.Returns(new List<PhotoWithoutThumbnail>()).Verifiable();

        setModel();

        _libraryManager.Verify(x => x.GetImagesWithoutExifModel(), Times.Once);
    }

    [TestMethod]
    public void SetModel_ExifModelReadForEachImage() {
        _getImagesWithoutExifModel.Returns(new List<PhotoWithoutThumbnail>() {
            new PhotoWithoutThumbnail() {
                Id = "Id1",
                DateTaken = DateTime.Parse("2023-01-01"),
                Extension = ".JPG"
            },
            new PhotoWithoutThumbnail() {
                Id = "Id2",
                DateTaken = DateTime.Parse("2023-02-02"),
                Extension = ".PNG"
            },
        });
        _getExifModel.Verifiable();

        setModel();

        _filesystem.Verify(x => x.GetExifModel(It.IsAny<string>()), Times.Exactly(2));
        _filesystem.Verify(x => x.GetExifModel(Path.Combine(STORAGE_PATH, "2023-01", "2023-01-01_Id1.JPG")), Times.Once);
        _filesystem.Verify(x => x.GetExifModel(Path.Combine(STORAGE_PATH, "2023-02", "2023-02-02_Id2.PNG")), Times.Once);
    }

    [TestMethod]
    public void SetModel_ExifModelSetForEachImage() {
        _getImagesWithoutExifModel.Returns(new List<PhotoWithoutThumbnail>() {
            new PhotoWithoutThumbnail() { Id = "Id1" },
            new PhotoWithoutThumbnail() { Id = "Id2" }
        });
        _getExifModel.Returns((string path) => {
            if (path.Contains("Id1")) return "Model1";
            else return "Model2";
        });
        _setExifModel.Verifiable();

        setModel();

        _libraryManager.Verify(x => x.SetExifModel(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
        _libraryManager.Verify(x => x.SetExifModel("Id1", "Model1"), Times.Once);
        _libraryManager.Verify(x => x.SetExifModel("Id2", "Model2"), Times.Once);
    }

    void setModel() => _exif.SetModel();
}