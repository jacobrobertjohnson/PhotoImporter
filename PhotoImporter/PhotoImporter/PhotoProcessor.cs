using PhotoImporter._Dependencies;

namespace PhotoImporter;

public class PhotoProcessor : IPhotoProcessor {
    Messenger _messenger;
    ILibraryManager _libraryManager;
    IFilesystem _filesystem;
    IValueProvider _valueProvider;
    IPhotoVerifier _photoVerifier;
    AppConfig _config;

    public PhotoProcessor(IDependencyFactory factory) {
        _messenger = factory.GetMessenger();
        _libraryManager = factory.GetLibraryManager();
        _filesystem = factory.GetFilesystem();
        _valueProvider = factory.GetValueProvider();
        _photoVerifier = factory.GetPhotoVerifier();
        _config = factory.GetConfigReader().AppConfig;
    }

    public void ProcessFile(string path) {
        try {
            processFile(path);
        } catch (Exception e) {
            _messenger.ExceptionOccurredInProcessFile(path, e);
        }
    }

    void processFile(string path) {
        string hash = _filesystem.GetFileHash(path);

        if (_libraryManager.FileAlreadyAdded(hash)) {
            _messenger.FileAlreadyInLibrary(path);
            clearSourceFile(path);
        } else {
            addFileToLibrary(hash, path);
        }
    }

    void addFileToLibrary(string hash, string path) {
        string fileId = _valueProvider.MakeGuid(),
            originalFilename = Path.GetFileName(path);
        DateTime dateTaken = _filesystem.GetImageTakenDate(path) ?? _filesystem.GetFileCreatedDate(path);

        _libraryManager.AddFile(hash, fileId, dateTaken, originalFilename);

        copyFileToStoragePath(path, dateTaken, fileId);

        verifyCopyAndDeleteOriginal(hash, path);
    }

    void copyFileToStoragePath(string sourcePath, DateTime dateTaken, string fileId) {
        string targetPath = makeFilePath(sourcePath, dateTaken, fileId);

        ensureDirectoryExists(targetPath);

        _filesystem.CopyFile(sourcePath, targetPath, true);
        _messenger.FileCopied(sourcePath, targetPath);
    }

    void ensureDirectoryExists(string filePath) {
        string dirPath = Path.GetDirectoryName(filePath);

        if (!_filesystem.DirectoryExists(dirPath))
            _filesystem.CreateDirectory(dirPath);
    }

    string makeFilePath(string path, DateTime dateTaken, string fileId) {
        string extension = Path.GetExtension(path),
            dateFolder = $"{dateTaken:yyyy-MM}",
            storedFilename = $"{dateTaken:yyyy-MM-dd}_{fileId}{extension}";

        return Path.Combine(_config.StoragePath, dateFolder, storedFilename);
    }

    void verifyCopyAndDeleteOriginal(string hash, string path)
    {
        if (_photoVerifier.PhotoWasDelivered(hash, path))
            clearSourceFile(path);
    }

    void clearSourceFile(string path) {
        _filesystem.DeleteFile(path);
        deleteParentDirectoryIfEmpty(path);
    }

    void deleteParentDirectoryIfEmpty(string path) {
        string dirPath = Path.GetDirectoryName(path);

        if (!isImporterDirectory(dirPath) && _filesystem.GetFiles(dirPath, "").Length == 0)
            _filesystem.DeleteDirectory(dirPath);
    }

    bool isImporterDirectory(string dirPath) {
        string trimmedDirPath = Path.TrimEndingDirectorySeparator(dirPath).ToLower(),
            trimmedImporterDirectory = Path.TrimEndingDirectorySeparator(_config.SourceDir).ToLower();

        return trimmedImporterDirectory.Contains(trimmedDirPath);
    }
}