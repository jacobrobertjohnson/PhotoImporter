namespace PhotoImporter.DependencyInjection;

public class DependencyFactory : IDependencyFactory {
    IConfigReader _configReader = null;
    IConsoleWriter _consoleWriter = null;
    IFilesystem _filesystem = null;
    Messenger _messenger = null;
    IPhotoImporter _photoImporter = null;
    IPhotoProcessor _photoProcessor = null;
    ISqliteContext _sqliteContext = null;
    ILibraryManager _libraryManager = null;
    IValueProvider _valueProvider = null;
    IPhotoVerifier _photoVerifier = null;
    IThumbnailGenerator _thumbnailGenerator = null;

    public IConfigReader GetConfigReader() {
        if (_configReader == null)
            _configReader = new ConfigReader(this);

        return _configReader;
    }

    public IConsoleWriter GetConsoleWriter() {
        if (_consoleWriter == null)
            _consoleWriter = new ConsoleWriter(this);

        return _consoleWriter;
    }

    public IFilesystem GetFilesystem() {
        if (_filesystem == null)
            _filesystem = new Filesystem.Filesystem();

        return _filesystem;
    }

    public Messenger GetMessenger() {
        if (_messenger == null)
            _messenger = new Messenger(this);
        
        return _messenger;
    }

    public IPhotoImporter GetPhotoImporter()  {
        if (_photoImporter == null)
            _photoImporter = new Photos.PhotoImporter(this);
        
        return _photoImporter;
    }

    public IPhotoProcessor GetPhotoProcessor() {
        if (_photoProcessor == null)
            _photoProcessor = new PhotoProcessor(this);
        
        return _photoProcessor;
    }

    public ISqliteContext GetSqliteContext() {
        if (_sqliteContext == null)
            _sqliteContext = new SqliteContext(this);
        
        return _sqliteContext;
    }

    public ILibraryManager GetLibraryManager() {
        if (_libraryManager == null)
            _libraryManager = new SqliteLibraryManager(this);
        
        return _libraryManager;
    }

    public IValueProvider GetValueProvider() {
        if (_valueProvider == null)
            _valueProvider = new ValueProvider();
        
        return _valueProvider;
    }

    public IPhotoVerifier GetPhotoVerifier() {
        if (_photoVerifier == null)
            _photoVerifier = new PhotoVerifier(this);
        
        return _photoVerifier;
    }

    public IThumbnailGenerator GetThumbnailGenerator() {
        if (_thumbnailGenerator == null)
            _thumbnailGenerator = new ThumbnailGenerator(this);
        
        return _thumbnailGenerator;
    }
}