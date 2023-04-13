namespace PhotoImporter;

public interface ILibraryManager {
    bool FileAlreadyAdded(string hash);
    void AddFile(string hash, string path, DateTime dateTaken);
}