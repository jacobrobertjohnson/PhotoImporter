namespace PhotoImporter.Tests;

public abstract class _TestBase {
    protected Mock<IConsoleWriter> _consoleWriter;
    protected Mock<IConfigReader> _configReader;
    protected Mock<IFilesystem> _filesystem;
    protected Mock<IDependencyFactory> _dependencies;
    protected Mock<IPhotoImporter> _photoImporter;
    protected Mock<IPhotoProcessor> _photoProcessor;
    protected Mock<ISqliteContext> _sqliteContext;
    protected Mock<ILibraryManager> _libraryManager;
    protected Mock<IValueProvider> _valueProvider;
    protected Mock<IPhotoVerifier> _photoVerifier;

    protected ISetup<IConsoleWriter> _writeLine,
        _writeVerboseLine;
    protected List<string> _writeLineResults,
        _writeVerboseLineResults;
    
    protected int _index;

    [TestInitialize]
    public void _GlobalSetup() {
        _consoleWriter = new Mock<IConsoleWriter>();
        _configReader = new Mock<IConfigReader>();
        _filesystem = new Mock<IFilesystem>();
        _photoImporter = new Mock<IPhotoImporter>();
        _photoProcessor = new Mock<IPhotoProcessor>();
        _sqliteContext = new Mock<ISqliteContext>();
        _libraryManager = new Mock<ILibraryManager>();
        _valueProvider = new Mock<IValueProvider>();
        _photoVerifier = new Mock<IPhotoVerifier>();

        _dependencies = new Mock<IDependencyFactory>();
        _dependencies.Setup(x => x.GetConsoleWriter()).Returns(_consoleWriter.Object);
        _dependencies.Setup(x => x.GetConfigReader()).Returns(_configReader.Object);
        _dependencies.Setup(x => x.GetFilesystem()).Returns(_filesystem.Object);
        _dependencies.Setup(x => x.GetMessenger()).Returns(new Messenger(_dependencies.Object));
        _dependencies.Setup(x => x.GetPhotoImporter()).Returns(_photoImporter.Object);
        _dependencies.Setup(x => x.GetPhotoProcessor()).Returns(_photoProcessor.Object);
        _dependencies.Setup(x => x.GetLibraryManager()).Returns(_libraryManager.Object);
        _dependencies.Setup(x => x.GetSqliteContext()).Returns(_sqliteContext.Object);
        _dependencies.Setup(x => x.GetValueProvider()).Returns(_valueProvider.Object);
        _dependencies.Setup(x => x.GetPhotoVerifier()).Returns(_photoVerifier.Object);

        _writeLine = _consoleWriter.Setup(x => x.WriteLine(It.IsAny<string>()));
        _writeLineResults = new List<string>();
        _writeLine.Callback((string line) => _writeLineResults.Add(line));

        _writeVerboseLine = _consoleWriter.Setup(x => x.WriteVerboseLine(It.IsAny<string>()));
        _writeVerboseLineResults = new List<string>();
        _writeVerboseLine.Callback((string line) => _writeVerboseLineResults.Add(line));

        _index = 0;
    }

    protected void verifySingleMessage(string message)
        => verifySingleMessage(_writeLineResults, message);

    protected void verifySingleMessageStartsWith(string message) {
        Assert.AreEqual(1, _writeLineResults.Count);
        Assert.IsTrue(_writeLineResults[_index++].StartsWith(message));
    }

    protected void verifySingleVerboseMessage(string message)
        => verifySingleMessage(_writeVerboseLineResults, message);

    protected void verifySingleMessage(List<string> list, string message) {
        Assert.AreEqual(1, list.Count);
        Assert.AreEqual(message, list[_index++]);
    }
}