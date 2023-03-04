namespace PhotoImporter._Dependencies {
    public class Filesystem : IFilesystem {
        public bool FileExists(string path) => File.Exists(path);

        public bool DirectoryExists(string path) => Directory.Exists(path);
    }
}