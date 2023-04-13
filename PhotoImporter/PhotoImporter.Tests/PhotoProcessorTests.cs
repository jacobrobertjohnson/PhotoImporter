using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Language.Flow;
using PhotoImporter._Dependencies;

namespace PhotoImporter.Tests;

[TestClass]
public class PhotoProcessorTests : _TestBase {
    const string FILE_PATH = "/fakepath/file.jpg",
        FILE_HASH = "The MD5 Hash";

    readonly DateTime ORIGINAL_DATE = DateTime.Parse("2023-01-01"),
        CREATED_DATE = DateTime.Parse("2023-02-02");

    ISetup<ILibraryManager, bool> _fileAlreadyAdded;
    ISetup<ILibraryManager> _addFile;

    ISetup<IFilesystem, DateTime?> _getImageTakenDate;
    ISetup<IFilesystem, DateTime> _getFileCreatedDate;

    IPhotoProcessor _processor;

    [TestInitialize]
    public void Setup() {
        _fileAlreadyAdded = _libraryManager.Setup(x => x.FileAlreadyAdded(It.IsAny<string>()));
        _addFile = _libraryManager.Setup(x => x.AddFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()));
        
        _filesystem.Setup(x => x.GetFileHash(It.IsAny<string>())).Returns(FILE_HASH);
        
        _getImageTakenDate = _filesystem.Setup(x => x.GetImageTakenDate(It.IsAny<string>()));
        _getImageTakenDate.Returns(ORIGINAL_DATE);

        _getFileCreatedDate = _filesystem.Setup(x => x.GetFileCreatedDate(It.IsAny<string>()));
        _getFileCreatedDate.Returns(CREATED_DATE);

        _processor = new PhotoProcessor(_dependencies.Object);
    }

    [TestMethod]
    public void ProcessFile_FileAlreadyAdded_MessageWritten() {
        _fileAlreadyAdded.Returns(true);

        processFile();

        verifySingleMessage(FILE_PATH + " already exists in the photo library. It will not be added again.");
    }

    [TestMethod]
    public void ProcessFile_FileAlreadyAdded_NotAddedAgain() {
        _fileAlreadyAdded.Returns(true);
        _addFile.Verifiable();

        processFile();

        _libraryManager.Verify(x => x.AddFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
    }

    [TestMethod]
    public void ProcessFile_FileNotAlreadyAdded_AddedToAvoidFutureDuplicates() {
        _fileAlreadyAdded.Returns(false);
        _addFile.Verifiable();
        
        processFile();

        _libraryManager.Verify(x => x.AddFile(FILE_HASH, FILE_PATH, ORIGINAL_DATE), Times.Once);
    }

    [TestMethod]
    public void ProcessFile_OriginalDateFound_OriginalDatePassedIntoDb() {
        _fileAlreadyAdded.Returns(false);
        _addFile.Verifiable();

        processFile();

        _libraryManager.Verify(x => x.AddFile(FILE_HASH, FILE_PATH, ORIGINAL_DATE), Times.Once);
    }

    [TestMethod]
    public void ProcessFile_OriginalDateNotFound_CreatedDatePassedIntoDb() {
        _fileAlreadyAdded.Returns(false);
        _getImageTakenDate.Returns(null as DateTime?);
        _addFile.Verifiable();

        processFile();

        _libraryManager.Verify(x => x.AddFile(FILE_HASH, FILE_PATH, CREATED_DATE), Times.Once);
    }

    void processFile() => _processor.ProcessFile(FILE_PATH);
}