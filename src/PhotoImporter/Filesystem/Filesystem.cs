using System.Security.Cryptography;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;

namespace PhotoImporter.Filesystem;

public class Filesystem : IFilesystem {
    public bool FileExists(string path) => File.Exists(path);

    public void DeleteFile(string path) => File.Delete(path);

    public bool DirectoryExists(string path) => Directory.Exists(path);

    public void CreateDirectory(string path) => Directory.CreateDirectory(path);

    public void DeleteDirectory(string path) => Directory.Delete(path);

    public string[] GetFiles(string dirPath, string searchPattern) {
        string[] acceptedExtensions = searchPattern
            .ToLower()
            .Split(",", StringSplitOptions.RemoveEmptyEntries)
            .Select(ext => "." + ext.Trim())
            .ToArray();

        return Directory.EnumerateFiles(dirPath, "*", new EnumerationOptions() {
                RecurseSubdirectories = true
            })
            .Where(file => acceptedExtensions.Length == 0
                || acceptedExtensions.Contains(Path.GetExtension(file).ToLower()))
            .ToArray();
    }

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
            image?.Metadata?.ExifProfile?.TryGetValue(ExifTag.DateTimeOriginal, out rawExifDate);
        }

        if (rawExifDate != null)
            if (DateTime.TryParse(rawExifDate.Value, out parsedDate))
                result = parsedDate;

        return result;
    }

    public DateTime GetFileCreatedDate(string path) => File.GetCreationTime(path);

    public void CopyFile(string source, string destination, bool overwrite) => File.Copy(source, destination, overwrite);

    public string ReadFile(string path) => File.ReadAllText(path);

    public Stream GetFileStream(string filePath, FileMode fileMode) => new FileStream(filePath, fileMode);

    public IImage LoadImage(Stream stream) => new ImageSharpImage(stream);
}