namespace PhotoImporter._Dependencies {
    public interface IFilesystem {
        bool FileExists(string path);
        bool DirectoryExists(string path);
    }
}