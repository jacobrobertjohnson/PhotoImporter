using System;

namespace PhotoImporter._Dependencies {
    public class ConsoleWriter : IConsoleWriter {
        public void WriteLine(string line) => Console.WriteLine(line);
    }
}