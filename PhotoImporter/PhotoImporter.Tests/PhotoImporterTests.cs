using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Language.Flow;
using PhotoImporter._Dependencies;

namespace PhotoImporter.Tests;

[TestClass]
public class PhotoImporterTests : _TestBase {
    AppConfig _config;

    PhotoImporter _photoImporter;

    ISetup<IFilesystem, bool> _directoryExists;

    [TestInitialize]
    public void Setup() {
        _directoryExists = _filesystem.Setup(x => x.DirectoryExists(It.IsAny<string>()));

        _config = new AppConfig();

        _photoImporter = new PhotoImporter(_filesystem.Object, new Messenger(_consoleWriter.Object));
    }

    [TestMethod]
    public void RunJob_SourceFolderDoesntExist_MessageReturned() {
        _directoryExists.Returns(false);

        runJob();

        verifySingleMessage("Source directory doesn't exist.");
    }

    void runJob() => _photoImporter.RunJob(_config);
}