namespace PhotoImporter.Tests;

[TestClass]
public class PhotoProcessorTests : _TestBase {
    const string ORIGINAL_FILENAME = "file.jpg",
        SOURCE_PATH = "/fakepath/source",
        DIRECTORY_PATH = SOURCE_PATH + "/subdir1",
        FILE_PATH = DIRECTORY_PATH + "/" + ORIGINAL_FILENAME,
        RANDOM_GUID = "RandomGuid123",
        STORAGE_PATH = "/fakepath/dest/",
        FILE_HASH = "The MD5 Hash";


    static readonly DateTime ORIGINAL_DATE = DateTime.Parse("2023-01-01"),
        CREATED_DATE = DateTime.Parse("2023-02-02");

    static readonly string DESTINATION_PATH = $"{STORAGE_PATH}{ORIGINAL_DATE:yyyy-MM}/{ORIGINAL_DATE:yyyy-MM-dd}_{RANDOM_GUID}.jpg";

    ISetup<ILibraryManager, bool> _fileAlreadyAdded;
    ISetup<ILibraryManager> _addFile;

    ISetup<IFilesystem, DateTime?> _getImageTakenDate;
    ISetup<IFilesystem, DateTime> _getFileCreatedDate;
    ISetup<IFilesystem, bool> _directoryExists;
    ISetup<IFilesystem, string[]> _getFiles;
    ISetup<IFilesystem> _copyFile,
        _createDirectory,
        _deleteFile,
        _deleteDirectory;

    ISetup<IPhotoVerifier, bool> _photoWasDelivered;

    IPhotoProcessor _processor;

    [TestInitialize]
    public void Setup() {
        _fileAlreadyAdded = _libraryManager.Setup(x => x.FileAlreadyAdded(It.IsAny<string>()));
        _addFile = _libraryManager.Setup(x => x.AddFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<string>()));
        
        _configReader.Setup(x => x.AppConfig).Returns(new AppConfig() {
            StoragePath = STORAGE_PATH,
            SourceDir = SOURCE_PATH,
            VerboseOutput = true
        });

        _valueProvider.Setup(x => x.MakeGuid()).Returns(RANDOM_GUID);

        _filesystem.Setup(x => x.GetFileHash(It.IsAny<string>())).Returns(FILE_HASH);
        
        _getImageTakenDate = _filesystem.Setup(x => x.GetImageTakenDate(It.IsAny<string>()));
        _getImageTakenDate.Returns(ORIGINAL_DATE);

        _getFileCreatedDate = _filesystem.Setup(x => x.GetFileCreatedDate(It.IsAny<string>()));
        _getFileCreatedDate.Returns(CREATED_DATE);

        _directoryExists = _filesystem.Setup(x => x.DirectoryExists(It.IsAny<string>()));

        _createDirectory = _filesystem.Setup(x => x.CreateDirectory(It.IsAny<string>()));
        _deleteFile = _filesystem.Setup(x => x.DeleteFile(It.IsAny<string>()));
        _deleteDirectory = _filesystem.Setup(x => x.DeleteDirectory(It.IsAny<string>()));
        _getFiles = _filesystem.Setup(x => x.GetFiles(It.IsAny<string>(), It.IsAny<string>()));

        _copyFile = _filesystem.Setup(x => x.CopyFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()));

        _photoWasDelivered = _photoVerifier.Setup(x => x.PhotoWasDelivered(It.IsAny<string>(), It.IsAny<string>()));

        _processor = new PhotoProcessor(_dependencies.Object);
    }

    [TestMethod]
    public void ProcessFile_FileAlreadyAdded_ExceptionWritten() {
        _fileAlreadyAdded.Callback((string hash) => {
            throw new ArithmeticException("Test Exception");
        });

        processFile();

        verifySingleMessageStartsWith($"An exception occurred while processing {FILE_PATH}:\nSystem.ArithmeticException: Test Exception");
    }

    [TestMethod]
    public void ProcessFile_FileAlreadyAdded_MessageWritten() {
        _fileAlreadyAdded.Returns(true);

        processFile();

        verifySingleVerboseMessage(FILE_PATH + " already exists in the photo library. It will not be added again.");
    }

    [TestMethod]
    public void ProcessFile_FileAlreadyAdded_NotAddedAgain() {
        _fileAlreadyAdded.Returns(true);
        _addFile.Verifiable();

        processFile();

        _libraryManager.Verify(x => x.AddFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public void ProcessFile_FileAlreadyAdded_FileDeleted() {
        _fileAlreadyAdded.Returns(true);
        _deleteFile.Verifiable();

        processFile();

        _filesystem.Verify(x => x.DeleteFile(FILE_PATH), Times.Once);
    }

    [TestMethod]
    public void ProcessFile_FileAlreadyAddedAndDirectoryIsEmpty_DirectoryDeleted() {
        _fileAlreadyAdded.Returns(true);
        _getFiles.Returns(new string[0]);
        _deleteFile.Verifiable();

        processFile();

        _filesystem.Verify(x => x.DeleteDirectory(DIRECTORY_PATH), Times.Once);
    }

    [TestMethod]
    public void ProcessFile_FileAlreadyAddedAndDirectoryNotEmpty_DirectoryNotDeleted() {
        _fileAlreadyAdded.Returns(true);
        _getFiles.Returns(new string[] { "/fakepath/image.jpg" });
        _deleteFile.Verifiable();

        processFile();

        _filesystem.Verify(x => x.DeleteDirectory(DIRECTORY_PATH), Times.Never);
    }

    [TestMethod]
    public void ProcessFile_FileNotAlreadyAdded_AddedToAvoidFutureDuplicates() {
        _fileAlreadyAdded.Returns(false);
        _addFile.Verifiable();
        
        processFile();

        _libraryManager.Verify(x => x.AddFile(FILE_HASH, RANDOM_GUID, ORIGINAL_DATE, ORIGINAL_FILENAME), Times.Once);
    }

    [TestMethod]
    public void ProcessFile_OriginalDateFound_OriginalDatePassedIntoDb() {
        _fileAlreadyAdded.Returns(false);
        _addFile.Verifiable();

        processFile();

        _libraryManager.Verify(x => x.AddFile(FILE_HASH, RANDOM_GUID, ORIGINAL_DATE, ORIGINAL_FILENAME), Times.Once);
    }

    [TestMethod]
    public void ProcessFile_OriginalDateNotFound_CreatedDatePassedIntoDb() {
        _fileAlreadyAdded.Returns(false);
        _getImageTakenDate.Returns(null as DateTime?);
        _addFile.Verifiable();

        processFile();

        _libraryManager.Verify(x => x.AddFile(FILE_HASH, RANDOM_GUID, CREATED_DATE, ORIGINAL_FILENAME), Times.Once);
    }

    [TestMethod]
    public void ProcessFile_TargetDirectoryNotExists_Created() {
        _fileAlreadyAdded.Returns(false);
        _directoryExists.Returns(false);
        _createDirectory.Verifiable();

        processFile();

        _filesystem.Verify(x => x.CreateDirectory($"{STORAGE_PATH}{ORIGINAL_DATE:yyyy-MM}"), Times.Once);
    }

    [TestMethod]
    public void ProcessFile_TargetDirectoryExists_NotCreated() {
        _fileAlreadyAdded.Returns(false);
        _directoryExists.Returns(true);
        _createDirectory.Verifiable();

        processFile();

        _filesystem.Verify(x => x.CreateDirectory(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public void ProcessFile_FileAlreadyAdded_NotCopiedToDestination() {
        _fileAlreadyAdded.Returns(true);
        _copyFile.Verifiable();

        processFile();

        _filesystem.Verify(x => x.CopyFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
    }

    [TestMethod]
    public void ProcessFile_FileNotAlreadyAdded_CopiedToDestination() {
        _fileAlreadyAdded.Returns(false);
        _copyFile.Verifiable();

        processFile();

        _filesystem.Verify(x => x.CopyFile(FILE_PATH, DESTINATION_PATH, true), Times.Once);
    }

    [TestMethod]
    public void ProcessFile_FileMoved_WrittenToConsole() {
        _fileAlreadyAdded.Returns(false);

        processFile();

        verifySingleVerboseMessage($"{FILE_PATH} successfully moved to {DESTINATION_PATH}");
    }

    [TestMethod]
    public void ProcessFile_FileVerified_OriginalDeleted() {
        _fileAlreadyAdded.Returns(false);
        _photoWasDelivered.Returns(true);
        _deleteFile.Verifiable();

        processFile();

        _filesystem.Verify(x => x.DeleteFile(FILE_PATH), Times.Once);
    }

    [TestMethod]
    public void ProcessFile_FileNotVerified_OriginalNotDeleted() {
        _fileAlreadyAdded.Returns(false);
        _photoWasDelivered.Returns(false);
        _deleteFile.Verifiable();

        processFile();

        _filesystem.Verify(x => x.DeleteFile(FILE_PATH), Times.Never);
    }

    [TestMethod]
    public void ProcessFile_FileNotVerified_DirectoryNotDeleted() {
        _fileAlreadyAdded.Returns(false);
        _photoWasDelivered.Returns(false);
        _deleteDirectory.Verifiable();

        processFile();

        _filesystem.Verify(x => x.DeleteDirectory(DIRECTORY_PATH), Times.Never);
    }

    [TestMethod]
    public void ProcessFile_DirectoryNotEmpty_DirectoryNotDeleted() {
        _fileAlreadyAdded.Returns(false);
        _photoWasDelivered.Returns(true);
        _getFiles.Returns(new string[] { "file1", "file2" });
        _deleteDirectory.Verifiable();

        processFile();

        _filesystem.Verify(x => x.DeleteDirectory(DIRECTORY_PATH), Times.Never);
    }

    [TestMethod]
    public void ProcessFile_DirectoryIsInImporterPath_DirectoryNotDeleted() {
        _fileAlreadyAdded.Returns(false);
        _photoWasDelivered.Returns(true);
        _getFiles.Returns(new string[0]);
        _deleteDirectory.Verifiable();

        processFile(SOURCE_PATH + "/" + ORIGINAL_FILENAME);

        _filesystem.Verify(x => x.DeleteDirectory(SOURCE_PATH), Times.Never);
    }

    [TestMethod]
    public void ProcessFile_DirectoryEmpty_DirectoryDeleted() {
        _fileAlreadyAdded.Returns(false);
        _photoWasDelivered.Returns(true);
        _getFiles.Returns(new string[0]);
        _deleteDirectory.Verifiable();

        processFile();

        _filesystem.Verify(x => x.DeleteDirectory(DIRECTORY_PATH), Times.Once);
    }

    void processFile(string path = null) => _processor.ProcessFile(path ?? FILE_PATH);
}