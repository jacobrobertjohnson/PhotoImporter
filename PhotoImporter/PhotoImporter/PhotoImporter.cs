using PhotoImporter._Dependencies;

namespace PhotoImporter {
    public class PhotoImporter : IPhotoImporter {
        IFilesystem _filesystem;
        IPhotoProcessor _photoProcessor;
        ILibraryManager _libraryManager;
        Messenger _messenger;

        public PhotoImporter(IDependencyFactory factory)
        {
            _filesystem = factory.GetFilesystem();
            _photoProcessor = factory.GetPhotoProcessor();
            _libraryManager = factory.GetLibraryManager();
            _messenger = factory.GetMessenger();
        }

        public void RunJob(AppConfig config) {
            if (!_filesystem.DirectoryExists(config.SourceDir))
                _messenger.SourceDirectoryDoesntExist();
            else if (_libraryManager.ImportIsRunning())
                _messenger.AnotherProcessRunning();
            else
                findAndProcessFiles(config);
        }

        void findAndProcessFiles(AppConfig config)
        {
            string[] files = _filesystem.GetFiles(config.SourceDir, config.SourceFilePattern);

            _messenger.FilesWereFound(config, files);

            foreach (string file in files)
                _photoProcessor.ProcessFile(file);
        }
    }
}