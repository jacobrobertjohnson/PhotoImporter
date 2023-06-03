namespace PhotoImporter.Library;

public interface ILibraryManager {
    bool FileAlreadyAdded(string hash);
    void AddFile(string hash, string fileId, DateTime dateTaken, string originalFilename);
    void DeleteFile(string hash);
    bool ImportIsRunning();
    void SetImportRunning(int isRunning);
    IEnumerable<PhotoWithoutThumbnail> GetPhotosWithoutThumbnails();
    void SetThumbnailGenerated(string fileId);

    List<PhotoWithoutThumbnail> GetImagesWithoutExifModel();
    void SetExifModel(string fileId, string exifModel);
}