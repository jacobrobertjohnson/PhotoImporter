namespace PhotoImporter.Configuration;

public interface IConfigReader {
    void ReadConfig(string configPath);
    public bool ConfigIsValid { get; }
    public AppConfig AppConfig { get; }
}