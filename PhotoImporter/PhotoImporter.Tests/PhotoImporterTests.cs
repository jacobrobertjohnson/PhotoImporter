using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Language.Flow;
using PhotoImporter._Dependencies;

namespace PhotoImporter.Tests;

[TestClass]
public class PhotoImporterTests : _TestBase {
    AppConfig _config;

    PhotoImporter _importer;

    ISetup<IFilesystem, bool> _directoryExists;
    ISetup<IFilesystem, string[]> _getFiles;
    ISetup<IPhotoProcessor> _processFile;

    [TestInitialize]
    public void Setup() {
        _directoryExists = _filesystem.Setup(x => x.DirectoryExists(It.IsAny<string>()));
        _getFiles = _filesystem.Setup(x => x.GetFiles(It.IsAny<string>(), It.IsAny<string>()));
        _processFile = _photoProcessor.Setup(x => x.ProcessFile(It.IsAny<string>()));

        _config = new AppConfig()
        {
            SourceDir = "/fakepath/imagesource",
            SourceFilePattern = "*.rar"
        };

        _importer = new PhotoImporter(_dependencies.Object);
    }

    [TestMethod]
    public void RunJob_SourceFolderDoesntExist_MessageReturned() {
        _directoryExists.Returns(false);

        runJob();

        verifySingleMessage("Source directory doesn't exist.");
    }

    [TestMethod]
    public void RunJob_SourceFolderDoesntExist_FilesNotRead() {
        _directoryExists.Returns(false);
        _getFiles.Verifiable();

        runJob();

        _filesystem.Verify(x => x.GetFiles(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public void RunJob_SourceFolderExists_FilesRead() {
        _directoryExists.Returns(true);
        _getFiles.Verifiable();

        runJob();

        _filesystem.Verify(x => x.GetFiles(_config.SourceDir, _config.SourceFilePattern), Times.Once);
    }

    [TestMethod]
    public void RunJob_EachFileGetsProcessed() {
        _directoryExists.Returns(true);
        _getFiles.Returns(new [] {
            "/fakepath/file1.jpg",
            "/fakepath/file2.jpg",
        });
        _processFile.Verifiable();

        runJob();

        _photoProcessor.Verify(x => x.ProcessFile(It.IsAny<string>()), Times.Exactly(2));
        _photoProcessor.Verify(x => x.ProcessFile("/fakepath/file1.jpg"), Times.Once);
        _photoProcessor.Verify(x => x.ProcessFile("/fakepath/file2.jpg"), Times.Once);
    }

    void runJob() => _importer.RunJob(_config);
} 