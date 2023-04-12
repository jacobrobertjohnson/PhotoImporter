using PhotoImporter._Dependencies;

namespace PhotoImporter {
    public class PhotoImporter : IPhotoImporter {
        IFilesystem _filesystem;
        IPhotoProcessor _photoProcessor;
        Messenger _messenger;

        public PhotoImporter(IDependencyFactory factory)
        {
            _filesystem = factory.GetFilesystem();
            _photoProcessor = factory.GetPhotoProcessor();
            _messenger = factory.GetMessenger();
        }

        public void RunJob(AppConfig config) {
            if (!_filesystem.DirectoryExists(config.SourceDir))
                _messenger.SourceDirectoryDoesntExist();
            else
                findAndProcessFiles(config);
        }

        void findAndProcessFiles(AppConfig config)
        {
            foreach (string file in _filesystem.GetFiles(config.SourceDir, config.SourceFilePattern))
                _photoProcessor.ProcessFile(file);
        }
    }
}