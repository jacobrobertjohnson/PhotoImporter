namespace PhotoImporter.Configuration;

public class AppConfig {
    public string SourceDir { get; set; }
    public string SourceFilePattern { get; set; }
    public string DatabasePath { get; set; }
    public string StoragePath { get; set; }
    public bool VerboseOutput { get; set; }
}