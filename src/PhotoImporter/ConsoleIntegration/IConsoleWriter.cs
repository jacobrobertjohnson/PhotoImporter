namespace PhotoImporter.ConsoleIntegration;

public interface IConsoleWriter {
    void WriteVerboseLine(string line);
    void WriteLine(string line);
}