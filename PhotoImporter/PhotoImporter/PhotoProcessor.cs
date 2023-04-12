using PhotoImporter._Dependencies;

namespace PhotoImporter;

public class PhotoProcessor : IPhotoProcessor {
    Messenger _messenger;
    IDuplicateManager _duplicateManager;
    IFilesystem _filesystem;

    public PhotoProcessor(IDependencyFactory factory) {
        _messenger = factory.GetMessenger();
        _duplicateManager = factory.GetDuplicateManager();
        _filesystem = factory.GetFilesystem();
    }

    public void ProcessFile(string path) {
        string hash = _filesystem.GetFileHash(path);

        if (_duplicateManager.FileAlreadyAdded(hash))
            _messenger.FileAlreadyInLibrary(path);
        else
            addFileToLibrary(hash, path);
    }

    void addFileToLibrary(string hash, string path) {
        _duplicateManager.AddFile(hash, path);
    }
}