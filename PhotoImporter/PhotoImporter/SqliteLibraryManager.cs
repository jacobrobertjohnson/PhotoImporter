using Microsoft.Data.Sqlite;
using PhotoImporter._Dependencies;

namespace PhotoImporter;

public class SqliteLibraryManager : ILibraryManager {
    ISqliteContext _context;
    IConfigReader _configReader;

    public SqliteLibraryManager(IDependencyFactory factory) {
        _configReader = factory.GetConfigReader();
        _context = factory.GetSqliteContext();

        buildDatabaseStructure();
    }

    void buildDatabaseStructure() {
        _context.RunQuery("CREATE TABLE IF NOT EXISTS Photos (Hash TEXT, FilePath TEXT, DateTaken TEXT)", noQuery);
    }

    public bool FileAlreadyAdded(string hash) {
        bool fileFound = false;

        _context.RunQuery($"SELECT 1 FROM Photos WHERE Hash = '{hash}'", reader => {
            fileFound = true;
        });

        return fileFound;
    }

    public void AddFile(string hash, string path, DateTime dateTaken) {
        _context.RunQuery($"INSERT INTO Photos (Hash, FilePath, DateTaken) VALUES ('{hash}', '{path}', '{dateTaken}')", noQuery);
    }

    void noQuery(SqliteDataReader reader) { }
}