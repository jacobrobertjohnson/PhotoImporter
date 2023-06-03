namespace PhotoImporter.ExifData;

public class ExifModelSetter : IExifModelSetter {
    AppConfig _config;
    ILibraryManager _libraryManager;
    IFilesystem _filesystem;

    public ExifModelSetter(IDependencyFactory factory) {
        _config = factory.GetConfigReader().AppConfig;
        _libraryManager = factory.GetLibraryManager();
        _filesystem = factory.GetFilesystem();
    }

    public void SetModel() {
        foreach (var image in _libraryManager.GetImagesWithoutExifModel()) {
            string imagePath = makeImagePath(image),
                exifModel = _filesystem.GetExifModel(imagePath);

            _libraryManager.SetExifModel(image.Id, exifModel);
        }
    }

    string makeImagePath(PhotoWithoutThumbnail photo) {
        return Path.Combine(
            _config.StoragePath,
            $"{photo.DateTaken:yyyy-MM}",
            $"{photo.DateTaken:yyyy-MM-dd}_{photo.Id}{photo.Extension}"
        );
    }
}