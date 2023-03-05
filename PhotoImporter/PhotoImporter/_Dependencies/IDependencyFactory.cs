namespace PhotoImporter._Dependencies;

public interface IDependencyFactory {
    IConfigReader GetConfigReader();
    IConsoleWriter GetConsoleWriter();
    IFilesystem GetFilesystem();
    Messenger GetMessenger();
    IPhotoImporter GetPhotoImporter();
}