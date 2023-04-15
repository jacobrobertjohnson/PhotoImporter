namespace PhotoImporter;

public interface ILibraryManager {
    bool FileAlreadyAdded(string hash);
    void AddFile(string hash, string fileId, DateTime dateTaken, string originalFilename);
    void DeleteFile(string hash);
    bool ImportIsRunning();
    void SetImportRunning(int isRunning);
}