using Newtonsoft.Json;

namespace PhotoImporter.Configuration;

public class ConfigReader : IConfigReader {
    IFilesystem _filesystem;

    public ConfigReader(IDependencyFactory factory)
    {
        _filesystem = factory.GetFilesystem();
    }

    public void ReadConfig(string configPath) {
        string fileContents = _filesystem.ReadFile(configPath);

        this.AppConfig = JsonConvert.DeserializeObject<AppConfig>(fileContents);
    }

    public bool ConfigIsValid {
        get => !string.IsNullOrEmpty(AppConfig.DatabasePath)
            && !string.IsNullOrEmpty(AppConfig.SourceDir)
            && !string.IsNullOrEmpty(AppConfig.SourceFilePattern)
            && !string.IsNullOrEmpty(AppConfig.StoragePath);
    }

    public AppConfig AppConfig { get; private set; }
}