using PhotoImporter._Dependencies;

namespace PhotoImporter {
    public class Messenger {
        IConsoleWriter _consoleWriter;

        public Messenger(IConsoleWriter consoleWriter)
        {
            _consoleWriter = consoleWriter;
        }

        public void ProgramHelp() {
            _consoleWriter.WriteLine("Usage: PhotoImporter --configFile [pathToConfig].json");
            _consoleWriter.WriteLine("");
            _consoleWriter.WriteLine("Options:");
            _consoleWriter.WriteLine("  --configFile\tPath to the JSON file containing the configuration for this importer instance.");
        }

        public void ConfigDoesntExist()
            => _consoleWriter.WriteLine("Config file does not exist.");

        public void ConfigFileNotValid()
            => _consoleWriter.WriteLine("Config file is not valid.");
    }
}