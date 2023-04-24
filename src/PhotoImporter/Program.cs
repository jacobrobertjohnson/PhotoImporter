namespace PhotoImporter {
    public class Program {
        public static IDependencyFactory DependencyFactory { get; set; } = new DependencyFactory();
        public static void Main(string[] args) {
            Messenger messenger = DependencyFactory.GetMessenger();
            IConfigReader configReader = DependencyFactory.GetConfigReader();
            string configFilePath = Arguments.GetConfigFilePath(args);

            if (string.IsNullOrWhiteSpace(configFilePath))
                messenger.ProgramHelp();
            else if (!DependencyFactory.GetFilesystem().FileExists(configFilePath))
                messenger.ConfigDoesntExist();
            else {
                configReader.ReadConfig(args[1]);

                if (!configReader.ConfigIsValid)
                    messenger.ConfigFileNotValid();
                else
                    DependencyFactory.GetPhotoImporter().RunJob(configReader.AppConfig);
            }
        }
    }
}