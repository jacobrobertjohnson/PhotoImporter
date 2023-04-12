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

        if (_libraryManager.FileAlreadyAdded(hash))
            _messenger.FileAlreadyInLibrary(path);
        else
            addFileToLibrary(hash, path);
    }

    void addFileToLibrary(string hash, string path) {
        _libraryManager.AddFile(hash, path);
    }
}