using PhotoImporter._Dependencies;

namespace PhotoImporter {
    public class AppConfig {
        public string SourceDir { get; set; }
        public string SourceFilePattern { get; set; }
        public string DatabasePath { get; set; }
        public string StoragePath { get; set; }
    }
}