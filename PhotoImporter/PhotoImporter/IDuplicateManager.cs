namespace PhotoImporter;

public interface IDuplicateManager {
    bool FileAlreadyAdded(string hash);
    void AddFile(string path);
}