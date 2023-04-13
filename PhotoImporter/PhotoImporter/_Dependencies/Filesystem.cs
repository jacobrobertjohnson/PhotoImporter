using System.Security.Cryptography;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;

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

        public DateTime? GetImageTakenDate(string path) {
            IExifValue<string> rawExifDate = null;
            DateTime parsedDate;
            DateTime? result = null;

            using (var image = Image.Load(path)) {
                image.Metadata.ExifProfile.TryGetValue(ExifTag.DateTimeOriginal, out rawExifDate);
            }

            if (rawExifDate != null)
                if (DateTime.TryParse(rawExifDate.Value, out parsedDate))
                    result = parsedDate;

            return result;
        }

        public DateTime GetFileCreatedDate(string path)
        {
            return File.GetCreationTime(path);
        }
    }
}