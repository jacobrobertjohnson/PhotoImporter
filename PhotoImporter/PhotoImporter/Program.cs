using PhotoImporter._Dependencies;

namespace PhotoImporter {
    public class Program {
        static IConfigReader _configReader;
        static IFilesystem _filesystem;
        static Messenger _messenger;
        static IPhotoImporter _photoImporter;

        static Program() {
            InjectDependencies(new DependencyFactory());
        }

        public static void InjectDependencies(IDependencyFactory factory) {
            _messenger = factory.GetMessenger();
            _configReader = factory.GetConfigReader();
            _filesystem = factory.GetFilesystem();
            _photoImporter = factory.GetPhotoImporter();
        }

        public static void Main(string[] args) {
            string configFilePath = Arguments.GetConfigFilePath(args);

            if (string.IsNullOrWhiteSpace(configFilePath))
                _messenger.ProgramHelp();
            else if (!_filesystem.FileExists(configFilePath))
                _messenger.ConfigDoesntExist();
            else {
                _configReader.ReadConfig(args[1]);

                if (!_configReader.ConfigIsValid)
                    _messenger.ConfigFileNotValid();
                else
                    _photoImporter.RunJob(_configReader.AppConfig);
            }
        }
    }
}