namespace PhotoImporter.Thumbnails;

public interface IThumbnailCache
{
    void CacheThumbnails(PhotoWithoutThumbnail photoWithoutThumbnail);
}