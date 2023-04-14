using System;

namespace PhotoImporter._Dependencies {
    public class ConsoleWriter : IConsoleWriter {
        IConfigReader _configReader;

        public ConsoleWriter(IDependencyFactory factory)
        {
            _configReader = factory.GetConfigReader();
        }

        public void WriteVerboseLine(string line)
        {
            if (_configReader.AppConfig.VerboseOutput)
                WriteLine(line);
        }
        public void WriteLine(string line) => Console.WriteLine(line);
    }
}