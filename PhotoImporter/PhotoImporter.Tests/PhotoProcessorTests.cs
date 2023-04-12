using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Language.Flow;

namespace PhotoImporter.Tests;

[TestClass]
public class PhotoProcessorTests : _TestBase {
    const string FILE_PATH = "/fakepath/file.jpg";

    ISetup<IDuplicateManager, bool> _fileAlreadyAdded;
    ISetup<IDuplicateManager> _addFile;

    IPhotoProcessor _processor;

    [TestInitialize]
    public void Setup() {
        _fileAlreadyAdded = _duplicateManager.Setup(x => x.FileAlreadyAdded(It.IsAny<string>()));
        _addFile = _duplicateManager.Setup(x => x.AddFile(It.IsAny<string>()));

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

        _duplicateManager.Verify(x => x.AddFile(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public void ProcessFile_FileNotAlreadyAdded_AddedToAvoidFutureDuplicates() {
        _fileAlreadyAdded.Returns(false);
        _addFile.Verifiable();

        processFile();

        _duplicateManager.Verify(x => x.AddFile(FILE_PATH), Times.Once);
    }

    void processFile() => _processor.ProcessFile(FILE_PATH);
}