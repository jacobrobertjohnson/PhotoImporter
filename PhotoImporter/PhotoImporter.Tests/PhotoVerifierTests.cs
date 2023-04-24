namespace PhotoImporter.Tests;

[TestClass]
public class PhotoVerifierTests : _TestBase {
    const string FILE_HASH = "MD5 Hash",
        BAD_FILE_HASH = "Bad Hash",
        DESTINATION_PATH = "/fakepath/source.jpg";

    ISetup<IFilesystem, string> _getFileHash;
    ISetup<IFilesystem, bool> _fileExists;
    ISetup<IFilesystem> _deleteFile;
    ISetup<ILibraryManager, bool> _fileAlreadyAdded;
    ISetup<ILibraryManager> _deleteDbRecord;

    IPhotoVerifier _verifier;

    [TestInitialize]
    public void Setup() {
        _fileExists = _filesystem.Setup(x => x.FileExists(It.IsAny<string>()));
        _deleteFile = _filesystem.Setup(x => x.DeleteFile(It.IsAny<string>()));
        
        _getFileHash = _filesystem.Setup(x => x.GetFileHash(It.IsAny<string>()));
        _getFileHash.Returns(FILE_HASH);
    
        _fileAlreadyAdded = _libraryManager.Setup(x => x.FileAlreadyAdded(It.IsAny<string>()));
        _deleteDbRecord = _libraryManager.Setup(x => x.DeleteFile(It.IsAny<string>()));

        _verifier = new PhotoVerifier(_dependencies.Object);
    }

    [TestMethod]
    [DataRow(true, true, FILE_HASH, true)]
    [DataRow(false, true, FILE_HASH, false)]
    [DataRow(true, false, FILE_HASH, false)]
    [DataRow(true, true, BAD_FILE_HASH, false)]
    [DataRow(false, false, FILE_HASH, false)]
    [DataRow(false, true, BAD_FILE_HASH, false)]
    [DataRow(true, false, BAD_FILE_HASH, false)]
    [DataRow(false, false, BAD_FILE_HASH, false)]
    public void PhotoWasDelivered_TrueReturnedIfFileExistsAndMatchesHashAndDbRecordExist(bool fileExists, bool dbExists, string hash, bool expectedResult) {
        _fileExists.Returns(fileExists);
        _getFileHash.Returns(hash);
        _fileAlreadyAdded.Returns(dbExists);

        Assert.AreEqual(expectedResult, photoWasDelivered());
    }

    [TestMethod]
    public void PhotoWasDelivered_FileAndDbExistAndHashIsGood_FileNotDeleted() {
        _fileExists.Returns(true);
        _fileAlreadyAdded.Returns(true);
        _deleteFile.Verifiable();

        photoWasDelivered();

        _filesystem.Verify(x => x.DeleteFile(DESTINATION_PATH), Times.Never);
    }

    [TestMethod]
    public void PhotoWasDelivered_DbExistsButFileDoesNot_FileNotDeleted() {
        _fileExists.Returns(false);
        _fileAlreadyAdded.Returns(true);
        _deleteFile.Verifiable();

        photoWasDelivered();

        _filesystem.Verify(x => x.DeleteFile(DESTINATION_PATH), Times.Never);
    }

    [TestMethod]
    public void PhotoWasDelivered_NeitherFileNorDbExist_FileNotDeleted() {
        _fileExists.Returns(false);
        _fileAlreadyAdded.Returns(false);
        _deleteFile.Verifiable();

        photoWasDelivered();

        _filesystem.Verify(x => x.DeleteFile(DESTINATION_PATH), Times.Never);
    }

    [TestMethod]
    public void PhotoWasDelivered_FileExistsButDbDoesNot_FileDeleted() {
        _fileExists.Returns(true);
        _fileAlreadyAdded.Returns(false);
        _deleteFile.Verifiable();

        photoWasDelivered();

        _filesystem.Verify(x => x.DeleteFile(DESTINATION_PATH), Times.Once);
    }

    [TestMethod]
    public void PhotoWasDelivered_FileAndDbExistButHashIsBad_FileDeleted() {
        _fileExists.Returns(true);
        _fileAlreadyAdded.Returns(true);
        _getFileHash.Returns(BAD_FILE_HASH);
        _deleteFile.Verifiable();

        photoWasDelivered();

        _filesystem.Verify(x => x.DeleteFile(DESTINATION_PATH), Times.Once);
    }

    [TestMethod]
    public void PhotoWasDelivered_DbNotExistsAndHashIsBad_FileDeleted() {
        _fileExists.Returns(true);
        _fileAlreadyAdded.Returns(false);
        _getFileHash.Returns(BAD_FILE_HASH);
        _deleteFile.Verifiable();

        photoWasDelivered();

        _filesystem.Verify(x => x.DeleteFile(DESTINATION_PATH), Times.Once);
    }

    [TestMethod]
    public void PhotoWasDelivered_FileAndDbExistAndHashIsGood_DbRecordNotDeleted() {
        _fileExists.Returns(true);
        _fileAlreadyAdded.Returns(true);
        _deleteDbRecord.Verifiable();

        photoWasDelivered();

        _libraryManager.Verify(x => x.DeleteFile(FILE_HASH), Times.Never);
    }

    [TestMethod]
    public void PhotoWasDelivered_NeitherFileNorDbExist_DbRecordNotDeleted() {
        _fileExists.Returns(false);
        _fileAlreadyAdded.Returns(false);
        _deleteDbRecord.Verifiable();

        photoWasDelivered();

        _libraryManager.Verify(x => x.DeleteFile(FILE_HASH), Times.Never);
    }

    [TestMethod]
    public void PhotoWasDelivered_DbExistsButFileDoesNot_DbRecordDeleted() {
        _fileExists.Returns(false);
        _fileAlreadyAdded.Returns(true);
        _deleteDbRecord.Verifiable();

        photoWasDelivered();

        _libraryManager.Verify(x => x.DeleteFile(FILE_HASH), Times.Once);
    }

    [TestMethod]
    public void PhotoWasDelivered_FileAndDbExistButHashIsBad_DbRecordDeleted() {
        _fileExists.Returns(true);
        _fileAlreadyAdded.Returns(true);
        _getFileHash.Returns(BAD_FILE_HASH);
        _deleteDbRecord.Verifiable();

        photoWasDelivered();

        _libraryManager.Verify(x => x.DeleteFile(FILE_HASH), Times.Once);
    }

    bool photoWasDelivered() => _verifier.PhotoWasDelivered(FILE_HASH, DESTINATION_PATH);
}