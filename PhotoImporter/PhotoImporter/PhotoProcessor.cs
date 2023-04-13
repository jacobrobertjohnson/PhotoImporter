using PhotoImporter._Dependencies;

namespace PhotoImporter;

public class PhotoProcessor : IPhotoProcessor {
    Messenger _messenger;
    ILibraryManager _libraryManager;
    IFilesystem _filesystem;

    public PhotoProcessor(IDependencyFactory factory) {
        _messenger = factory.GetMessenger();
        _libraryManager = factory.GetLibraryManager();
        _filesystem = factory.GetFilesystem();
    }

    public void ProcessFile(string path) {
        string hash = _filesystem.GetFileHash(path);
        DateTime dateTaken = _filesystem.GetImageTakenDate(path) ?? _filesystem.GetFileCreatedDate(path);

        if (_libraryManager.FileAlreadyAdded(hash))
            _messenger.FileAlreadyInLibrary(path);
        else
            addFileToLibrary(hash, path, dateTaken);
    }

    void addFileToLibrary(string hash, string path, DateTime dateTaken) {
        _libraryManager.AddFile(hash, path, dateTaken);
    }
}