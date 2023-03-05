using PhotoImporter._Dependencies;

namespace PhotoImporter {
    public class PhotoImporter : IPhotoImporter {
        IFilesystem _filesystem;
        Messenger _messenger;

        public PhotoImporter(IDependencyFactory factory)
        {
            _filesystem = factory.GetFilesystem();
            _messenger = factory.GetMessenger();
        }

        public void RunJob(AppConfig? config) {
            if (!_filesystem.DirectoryExists(config.SourceDir))
                _messenger.SourceDirectoryDoesntExist();
            else
                findAndProcessFiles(config);
        }

        void findAndProcessFiles(AppConfig config)
        {
            _filesystem.GetFiles(config.SourceDir, config.SourceFilePattern);
        }
    }
}