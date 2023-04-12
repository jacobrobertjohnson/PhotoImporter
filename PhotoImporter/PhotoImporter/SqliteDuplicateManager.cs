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
        _context.RunQuery("CREATE TABLE IF NOT EXISTS Photos (Hash TEXT, FilePath TEXT)", reader => { });
    }

    public bool FileAlreadyAdded(string hash) {
        bool fileFound = false;

        _context.RunQuery($"SELECT 1 FROM Photos WHERE Hash = '{hash}'", reader => {
            fileFound = true;
        });

        return fileFound;
    }

    public void AddFile(string path) {
        throw new NotImplementedException();
    }
}