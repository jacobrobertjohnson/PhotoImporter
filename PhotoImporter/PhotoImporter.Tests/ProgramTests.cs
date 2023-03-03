using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhotoImporter._Dependencies;
using Moq;
using Moq.Language.Flow;
using System.Collections.Generic;

namespace PhotoImporter.Tests;

[TestClass]
public class MainTests {
    const string CONFIG_PATH = "/fake/config/path.json";

    Mock<IConsoleWriter> _consoleWriter;
    Mock<IConfigReader> _configReader;
    Mock<IFilesystem> _filesystem;
    Mock<IPhotoImporter> _photoImporter;
    
    List<string> _writeLineResults;
    
    ISetup<IConsoleWriter> _writeLine;
    ISetup<IConfigReader> _readConfig;
    ISetup<IConfigReader, bool> _configIsValid;
    ISetup<IFilesystem, bool> _fileExists;
    ISetup<IPhotoImporter> _runJob;

    int _index;

    [TestInitialize]
    public void Setup() {
        _consoleWriter = new Mock<IConsoleWriter>();
        _writeLineResults = new List<string>();
        _writeLine = _consoleWriter
            .Setup(x => x.WriteLine(It.IsAny<string>()));
        _writeLine.Callback((string line) => _writeLineResults.Add(line));

        _configReader = new Mock<IConfigReader>();
        _readConfig = _configReader.Setup(x => x.ReadConfig(It.IsAny<string>()));
        _configIsValid = _configReader.Setup(x => x.ConfigIsValid);

        _filesystem = new Mock<IFilesystem>();
        _fileExists = _filesystem.Setup(x => x.FileExists(It.IsAny<string>()));

        _photoImporter = new Mock<IPhotoImporter>();
        _runJob = _photoImporter.Setup(x => x.RunJob());
        
        _index = 0;

        Program.InjectDependencies(
            _consoleWriter.Object,
            _configReader.Object,
            _filesystem.Object,
            _photoImporter.Object
        );
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
    public void Main_GoodConfig_JobStarted() {
        _fileExists.Returns(true);
        _configIsValid.Returns(true);
        _runJob.Verifiable();

        main("--configFile", CONFIG_PATH);

        _photoImporter.Verify(x => x.RunJob(), Times.Once);
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

    void verifySingleMessage(string message) {
        Assert.AreEqual(1, _writeLineResults.Count);
        Assert.AreEqual(message, _writeLineResults[_index++]);
    }

    void main(params string[] args)
        => Program.Main(args);
}