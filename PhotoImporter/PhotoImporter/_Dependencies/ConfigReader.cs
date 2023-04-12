namespace PhotoImporter._Dependencies {
    public class ConfigReader : IConfigReader {
        public void ReadConfig(string configPath) {
            
        }

        public bool ConfigIsValid { get; private set; }

        public AppConfig AppConfig { get; private set; }
    }
}