namespace PhotoImporter.Thumbnails;

public class ThumbnailCache : IThumbnailCache {
    readonly int[] THUMBNAIL_SIZES = new[] {
        100,
        200,
        300,
    };

    IFilesystem _filesystem;
    IThumbnailMaker _thumbnailMaker;
    AppConfig _config;

    public ThumbnailCache(IDependencyFactory dependencyFactory) {
        _filesystem = dependencyFactory.GetFilesystem();
        _thumbnailMaker = dependencyFactory.GetThumbnailMaker();
        _config = dependencyFactory.GetConfigReader().AppConfig;
    }

    public void CacheThumbnails(PhotoWithoutThumbnail photo) {
        foreach (int size in THUMBNAIL_SIZES)
            cacheThumbnailIfNotExisting(photo, size);
    }

    void cacheThumbnailIfNotExisting(PhotoWithoutThumbnail photo, int size) {
        string thumbnailPath = makeThumbnailPath(photo, size),
            fullSizePath = PhotoProcessor.MakeFilePath(_config, photo.OriginalFilename, photo.DateTaken, photo.Id);

        if (!_filesystem.FileExists(thumbnailPath)) {
            cacheThumbnail(fullSizePath, thumbnailPath, size);
        }
    }

    void cacheThumbnail(string fullSizePath, string thumbnailPath, int size) {
        ensureDirectoryExists(thumbnailPath);
        _thumbnailMaker.MakeThumbnail(fullSizePath, thumbnailPath, size);
    }

    void ensureDirectoryExists(string thumbnailPath) {
        string thumbnailDir = Path.GetDirectoryName(thumbnailPath);

        if (!_filesystem.DirectoryExists(thumbnailDir))
            _filesystem.CreateDirectory(thumbnailDir);
    }

    string makeThumbnailPath(PhotoWithoutThumbnail photo, int size) {
        return Path.Combine(
            _config.ThumbnailPath,
            $"{size}",
            $"{photo.DateTaken:yyyy-MM}",
            $"{photo.DateTaken:yyyy-MM-dd}_{photo.Id}{photo.Extension}"
        );
    }
}