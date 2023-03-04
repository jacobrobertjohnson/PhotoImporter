using PhotoImporter._Dependencies;

namespace PhotoImporter {
    public class PhotoImporter : IPhotoImporter {
        IFilesystem _filesystem;
        Messenger _messenger;

        public PhotoImporter(
            IFilesystem filesystem,
            Messenger messenger
        )
        {
            _filesystem = filesystem;
            _messenger = messenger;
        }

        public void RunJob(AppConfig? config) {
            _messenger.SourceDirectoryDoesntExist();
        }
    }
}