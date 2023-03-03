using PhotoImporter._Dependencies;

namespace PhotoImporter {
    public class Arguments {
        public static string? GetConfigFilePath(string[] args) {
            int flagPos = Array.IndexOf(args, "--configFile");
            string? configFilePath = null;

            if (flagPos > -1 && args.GetUpperBound(0) > flagPos)
                configFilePath = args[flagPos + 1];

            return configFilePath;
        }
    }
}