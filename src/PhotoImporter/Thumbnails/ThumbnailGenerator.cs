namespace PhotoImporter.Thumbnails
{
    public class ThumbnailGenerator : IThumbnailGenerator {
        ILibraryManager _libraryManager;

        public ThumbnailGenerator(IDependencyFactory dependencyFactory) {
            _libraryManager = dependencyFactory.GetLibraryManager();
        }
        

        public void MakeThumbnails(AppConfig appConfig) {
            _libraryManager.GetPhotosWithoutThumbnails();
        }
    }
}