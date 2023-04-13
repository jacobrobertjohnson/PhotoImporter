namespace PhotoImporter._Dependencies {
    public interface IFilesystem {
        bool FileExists(string path);
        bool DirectoryExists(string path);
        string[] GetFiles(string dirPath, string searchPattern);
        string GetFileHash(string path);
        DateTime? GetImageTakenDate(string path);
        DateTime GetFileCreatedDate(string path);
    }
}