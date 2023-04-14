using PhotoImporter._Dependencies;

namespace PhotoImporter;

public class PhotoVerifier : IPhotoVerifier {
    IFilesystem _filesystem;
    ILibraryManager _libraryManager;

    public PhotoVerifier(IDependencyFactory factory)
    {
        _filesystem = factory.GetFilesystem();
        _libraryManager = factory.GetLibraryManager();
    }

    public bool PhotoWasDelivered(string hash, string destinationFile) {
        bool fileExists = _filesystem.FileExists(destinationFile),
            hashesMatch = hash == _filesystem.GetFileHash(destinationFile),
            dbExists = _libraryManager.FileAlreadyAdded(hash);

        if (fileExists && !(dbExists && hashesMatch))
            _filesystem.DeleteFile(destinationFile);

        if (dbExists && !(fileExists && hashesMatch))
            _libraryManager.DeleteFile(hash);

        return fileExists && hashesMatch && dbExists;
    }
}