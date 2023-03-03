using PhotoImporter._Dependencies;

namespace PhotoImporter {
    public class Program {
        static IConsoleWriter _consoleWriter = new ConsoleWriter();
        static IConfigReader _configReader = new ConfigReader();
        static IFilesystem _filesystem = new Filesystem();

        public static void InjectDependencies(
            IConsoleWriter consoleWriter,
            IConfigReader configReader,
            IFilesystem filesystem
        ) {
            _consoleWriter = consoleWriter;
            _configReader = configReader;
            _filesystem = filesystem;
        }

        public static void Main(string[] args) {
            string? configFilePath = getConfigFilePath(args);

            if (string.IsNullOrWhiteSpace(configFilePath))
                writeHelp();
            else if (!_filesystem.FileExists(configFilePath))
                _consoleWriter.WriteLine("Config file does not exist.");
            else {
                _configReader.ReadConfig(args[1]);

                if (!_configReader.ConfigIsValid)
                    _consoleWriter.WriteLine("Config file is not valid.");
            }

        }

        static string? getConfigFilePath(string[] args) {
            int flagPos = Array.IndexOf(args, "--configFile");
            string? configFilePath = null;

            if (flagPos > -1 && args.GetUpperBound(0) > flagPos)
                configFilePath = args[flagPos + 1];

            return configFilePath;
        }

        static void writeHelp() {
            _consoleWriter.WriteLine("Usage: PhotoImporter --configFile [pathToConfig].json");
            _consoleWriter.WriteLine("");
            _consoleWriter.WriteLine("Options:");
            _consoleWriter.WriteLine("  --configFile\tPath to the JSON file containing the configuration for this importer instance.");
        }
    }
}