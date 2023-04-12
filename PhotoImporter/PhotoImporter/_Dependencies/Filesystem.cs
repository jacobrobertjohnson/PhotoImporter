using System.Security.Cryptography;

namespace PhotoImporter._Dependencies {
    public class Filesystem : IFilesystem {
        public bool FileExists(string path) => File.Exists(path);

        public bool DirectoryExists(string path) => Directory.Exists(path);

        public string[] GetFiles(string dirPath, string searchPattern)
            => Directory.EnumerateFiles(dirPath, searchPattern).ToArray();

        public string GetFileHash(string path) {
            byte[] hashBytes;
            string hashString;

            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(path)) {
                hashBytes = md5.ComputeHash(stream);
                hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }

            return hashString;
        }
    }
}