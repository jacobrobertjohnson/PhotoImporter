namespace PhotoImporter.Filesystem;

public interface IFilesystem {
    bool FileExists(string path);
    void DeleteFile(string path);
    bool DirectoryExists(string path);
    void DeleteDirectory(string path);
    void CreateDirectory(string path);
    string[] GetFiles(string dirPath, string searchPattern);
    string GetFileHash(string path);
    DateTime? GetImageTakenDate(string path);
    DateTime GetFileCreatedDate(string path);
    string GetExifModel(string path);
    void CopyFile(string source, string destination, bool overwrite);
    string ReadFile(string path);
    Stream GetFileStream(string filePath, FileMode fileMode);
    IImage LoadImage(Stream stream);
}