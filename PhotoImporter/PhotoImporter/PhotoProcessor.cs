using PhotoImporter._Dependencies;

namespace PhotoImporter;

public class PhotoProcessor : IPhotoProcessor {
    Messenger _messenger;
    ILibraryManager _libraryManager;
    IFilesystem _filesystem;
    IValueProvider _valueProvider;
    AppConfig _config;

    public PhotoProcessor(IDependencyFactory factory) {
        _messenger = factory.GetMessenger();
        _libraryManager = factory.GetLibraryManager();
        _filesystem = factory.GetFilesystem();
        _valueProvider = factory.GetValueProvider();
        _config = factory.GetConfigReader().AppConfig;
    }

    public void ProcessFile(string path) {
        string hash = _filesystem.GetFileHash(path),
            fileId = _valueProvider.MakeGuid();
        DateTime dateTaken = _filesystem.GetImageTakenDate(path) ?? _filesystem.GetFileCreatedDate(path);

        if (_libraryManager.FileAlreadyAdded(hash))
            _messenger.FileAlreadyInLibrary(path);
        else {
            addFileToLibrary(hash, path, dateTaken, fileId);
            copyFileToStoragePath(path, dateTaken, fileId);
        }
    }

    void addFileToLibrary(string hash, string path, DateTime dateTaken, string fileId) {
        _libraryManager.AddFile(hash, path, dateTaken);
    }

    void copyFileToStoragePath(string sourcePath, DateTime dateTaken, string fileId) {
        string targetPath = makeFilePath(sourcePath, dateTaken, fileId);

        _filesystem.CopyFile(sourcePath, targetPath, true);
    }

    string makeFilePath(string path, DateTime dateTaken, string fileId) {
        string extension = Path.GetExtension(path),
            dateFolder = $"{dateTaken:yyyy-MM-dd}",
            storedFilename = fileId + extension;

        return Path.Combine(_config.StoragePath, dateFolder, storedFilename);
    }
}