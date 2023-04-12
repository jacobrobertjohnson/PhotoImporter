using PhotoImporter._Dependencies;

namespace PhotoImporter;

public class SqliteDuplicateManager : IDuplicateManager {
    ISqliteContext _context;
    IConfigReader _configReader;

    public SqliteDuplicateManager(IDependencyFactory factory) {
        _configReader = factory.GetConfigReader();
        _context = factory.GetSqliteContext();

        buildDatabaseStructure();
    }

    void buildDatabaseStructure() {
        _context.RunQuery("CREATE TABLE IF NOT EXISTS Photos", reader => { });
    }

    public bool FileAlreadyAdded(string path) {
        throw new NotImplementedException();
    }

    public void AddFile(string path) {
        throw new NotImplementedException();
    }
}