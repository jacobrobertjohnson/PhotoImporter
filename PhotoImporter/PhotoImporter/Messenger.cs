using PhotoImporter._Dependencies;

namespace PhotoImporter {
    public class Messenger {
        IConsoleWriter _consoleWriter;

        public Messenger(IDependencyFactory factory)
        {
            _consoleWriter = factory.GetConsoleWriter();
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

        public void SourceDirectoryDoesntExist()
            => _consoleWriter.WriteLine("Source directory doesn't exist.");

        internal void FileAlreadyInLibrary(string path)
            => _consoleWriter.WriteLine($"{path} already exists in the photo library. It will not be added again.");
    }
}