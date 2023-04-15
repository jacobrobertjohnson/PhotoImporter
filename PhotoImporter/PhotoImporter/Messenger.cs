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

        internal void AnotherProcessRunning()
            => _consoleWriter.WriteLine("Another import process is already running. This process will not continue.");

        internal void FileAlreadyInLibrary(string path)
            => _consoleWriter.WriteVerboseLine($"{path} already exists in the photo library. It will not be added again.");

        internal void FilesWereFound(AppConfig config, string[] files)
            => _consoleWriter.WriteLine($"{files.Length} files were found in {config.SourceDir} using wildcard {config.SourceFilePattern}\n");

        internal void ExceptionOccurredInProcessFile(string path, Exception e)
            => _consoleWriter.WriteLine($"An exception occurred while processing {path}:\n{e}");

        internal void FileCopied(string sourcePath, string targetPath)
            => _consoleWriter.WriteVerboseLine($"{sourcePath} successfully moved to {targetPath}");
    }
}