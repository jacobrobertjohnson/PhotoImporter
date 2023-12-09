namespace PhotoImporter.Thumbnails
{
    public class ThumbnailGenerator : IThumbnailGenerator {
        Messenger _messenger;
        ILibraryManager _libraryManager;
        IThumbnailCache _thumbnailCache;

        public ThumbnailGenerator(IDependencyFactory dependencyFactory) {
            _messenger = dependencyFactory.GetMessenger();
            _libraryManager = dependencyFactory.GetLibraryManager();
            _thumbnailCache = dependencyFactory.GetThumbnailCache();
        }
        

        public void MakeThumbnails() {
            Parallel.ForEach(_libraryManager.GetPhotosWithoutThumbnails(), photo =>
            {
                try
                {
                    _thumbnailCache.CacheThumbnails(photo);
                    _libraryManager.SetThumbnailGenerated(photo.Id);
                }
                catch (Exception e)
                {
                    _messenger.ExceptionOccurredInThumbnailGeneration(photo.Id, e);
                }
            });
        }
    }
}