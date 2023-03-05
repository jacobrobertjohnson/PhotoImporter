using PhotoImporter._Dependencies;

namespace PhotoImporter;

public class PhotoProcessor : IPhotoProcessor {
    Messenger _messenger;
    IDuplicateManager _duplicateManager;

    public PhotoProcessor(IDependencyFactory factory) {
        _messenger = factory.GetMessenger();
        _duplicateManager = factory.GetDuplicateManager();
    }

    public void ProcessFile(string path) {
        if (_duplicateManager.FileAlreadyAdded(path))
            _messenger.FileAlreadyInLibrary(path);
        else
            _duplicateManager.AddFile(path);
    }
}