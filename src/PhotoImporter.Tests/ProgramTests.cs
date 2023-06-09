using PhotoImporter.ExifData;

namespace PhotoImporter.Tests;

[TestClass]
public class MainTests : _TestBase {
    const string CONFIG_PATH = "/fake/config/path.json";
    
    ISetup<IConfigReader> _readConfig;
    ISetup<IConfigReader, bool> _configIsValid;
    ISetup<IConfigReader, AppConfig> _appConfig;
    ISetup<IFilesystem, bool> _fileExists;
    ISetup<IPhotoImporter> _runJob;
    ISetup<IThumbnailGenerator> _makeThumbnails;
    ISetup<IExifModelSetter> _setExifModel;

    [TestInitialize]
    public void Setup() {  
        _readConfig = _configReader.Setup(x => x.ReadConfig(It.IsAny<string>()));
        _configIsValid = _configReader.Setup(x => x.ConfigIsValid);
        _appConfig = _configReader.Setup(x => x.AppConfig);

        _fileExists = _filesystem.Setup(x => x.FileExists(It.IsAny<string>()));

        _runJob = _photoImporter.Setup(x => x.RunJob(It.IsAny<AppConfig>()));
        _makeThumbnails = _thumbnailGenerator.Setup(x => x.MakeThumbnails());
        _setExifModel = _exifModel.Setup(x => x.SetModel());

        Program.DependencyFactory = _dependencies.Object;
    }

    [TestMethod]
    public void Main_NoArgs_HelpPrinted() {
        testBadArguments();
    }

    [TestMethod]
    public void Main_WrongFlagPresent_HelpPrinted() {
        testBadArguments("--badFlag");
    }

    [TestMethod]
    public void Main_ConfigFileFlagPresentButValueMissing_HelpPrinted() {
        testBadArguments("--configFile");
    }

    [TestMethod]
    public void Main_FileDoesntExist_MessageWritten() {
        _fileExists.Returns(false);

        main("--configFile", CONFIG_PATH);

        verifySingleMessage("Config file does not exist.");
    }

    [TestMethod]
    public void Main_FileExists_ConfigRead() {
        _fileExists.Returns(true);
        _readConfig.Verifiable();

        main("--configFile", CONFIG_PATH);

        _configReader.Verify(x => x.ReadConfig(CONFIG_PATH), Times.Once);
    }

    [TestMethod]
    public void Main_BadConfig_MessageWritten() {
        _fileExists.Returns(true);
        _configIsValid.Returns(false);

        main("--configFile", CONFIG_PATH);

        verifySingleMessage("Config file is not valid.");
    }

    [TestMethod]
    public void Main_GoodConfig_PhotoJobStarted() {
        var config = new AppConfig();

        _fileExists.Returns(true);
        _configIsValid.Returns(true);
        _appConfig.Returns(config);
        _runJob.Verifiable();

        main("--configFile", CONFIG_PATH);

        _photoImporter.Verify(x => x.RunJob(config), Times.Once);
    }

    [TestMethod]
    public void Main_GoodConfig_ThumbnailJobStarted() {
        _fileExists.Returns(true);
        _configIsValid.Returns(true);
        _makeThumbnails.Verifiable();

        main("--configFile", CONFIG_PATH);

        _thumbnailGenerator.Verify(x => x.MakeThumbnails(), Times.Once);
    }

    [TestMethod]
    public void Main_GoodConfig_ExifModelJobStarted() {
        _fileExists.Returns(true);
        _configIsValid.Returns(true);
        _setExifModel.Verifiable();

        main("--configFile", CONFIG_PATH);

        _exifModel.Verify(x => x.SetModel(), Times.Once);
    }

    void testBadArguments(params string[] args)
    {
        _readConfig.Verifiable();

        main(args);
        testHelp();

        _configReader.Verify(x => x.ReadConfig(It.IsAny<string>()), Times.Never);
    }

    void testHelp() {
        Assert.AreEqual(4, _writeLineResults.Count);
        Assert.AreEqual("Usage: PhotoImporter --configFile [pathToConfig].json", _writeLineResults[_index++]);
        Assert.AreEqual("", _writeLineResults[_index++]);
        Assert.AreEqual("Options:", _writeLineResults[_index++]);
        Assert.AreEqual("  --configFile\tPath to the JSON file containing the configuration for this importer instance.", _writeLineResults[_index++]);
    }

    void main(params string[] args)
        => Program.Main(args);
}