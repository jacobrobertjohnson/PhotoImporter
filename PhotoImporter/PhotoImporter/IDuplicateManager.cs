namespace PhotoImporter;

public interface IDuplicateManager {
    bool FileAlreadyAdded(string hash);
    void AddFile(string hash, string path);
}