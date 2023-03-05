namespace PhotoImporter;

public interface IDuplicateManager {
    bool FileAlreadyAdded(string path);
    void AddFile(string path);
}