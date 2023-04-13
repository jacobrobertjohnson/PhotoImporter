namespace PhotoImporter._Dependencies;

public class DependencyFactory : IDependencyFactory {
    IConfigReader _configReader;
    IConsoleWriter _consoleWriter;
    IFilesystem _filesystem;
    Messenger _messenger;
    IPhotoImporter _photoImporter;
    IPhotoProcessor _photoProcessor;
    ISqliteContext _sqliteContext;
    ILibraryManager _libraryManager;
    IValueProvider _valueProvider;

    public DependencyFactory()
    {
        _configReader = new ConfigReader();
        _consoleWriter = new ConsoleWriter();
        _filesystem = new Filesystem();
        _messenger = new Messenger(this);
        _photoImporter = new PhotoImporter(this);
        _photoProcessor = new PhotoProcessor(this);
        _sqliteContext = new SqliteContext();
        _libraryManager = new SqliteLibraryManager(this);
        _valueProvider = new ValueProvider();
    }

    public IConfigReader GetConfigReader() => _configReader;
    public IConsoleWriter GetConsoleWriter() => _consoleWriter;
    public IFilesystem GetFilesystem() => _filesystem;
    public Messenger GetMessenger() => _messenger;
    public IPhotoImporter GetPhotoImporter() => _photoImporter;
    public IPhotoProcessor GetPhotoProcessor() => _photoProcessor;
    public ISqliteContext GetSqliteContext() => _sqliteContext;
    public ILibraryManager GetLibraryManager() => _libraryManager;
    public IValueProvider GetValueProvider() => _valueProvider;
}