using PhotoImporter._Dependencies;

namespace PhotoImporter {
    public class Program {
        static IConfigReader _configReader = new ConfigReader();
        static IFilesystem _filesystem = new Filesystem();
        static Messenger? _messenger;
        static IPhotoImporter _photoImporter = new PhotoImporter();


        public static void InjectDependencies(
            IConsoleWriter consoleWriter,
            IConfigReader configReader,
            IFilesystem filesystem,
            IPhotoImporter photoImporter
        ) {
            _messenger = new Messenger(consoleWriter);
            _configReader = configReader;
            _filesystem = filesystem;
            _photoImporter = photoImporter;
        }

        public static void Main(string[] args) {
            string? configFilePath = Arguments.GetConfigFilePath(args);

            if (string.IsNullOrWhiteSpace(configFilePath))
                _messenger?.ProgramHelp();
            else if (!_filesystem.FileExists(configFilePath))
                _messenger?.ConfigDoesntExist();
            else {
                _configReader.ReadConfig(args[1]);

                if (!_configReader.ConfigIsValid)
                    _messenger?.ConfigFileNotValid();
                else
                    _photoImporter.RunJob();
            }
        }
    }
}