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
        _context.RunQuery("CREATE TABLE IF NOT EXISTS Photos (Hash TEXT, FileId TEXT, DateTaken TEXT, OriginalFilename TEXT)");
        _context.RunQuery("CREATE TABLE IF NOT EXISTS AppState (ImportIsRunning INT)");
        _context.RunQuery("INSERT INTO AppState (ImportIsRunning) SELECT 0 WHERE NOT EXISTS (SELECT 1 FROM AppState)");
    }

    public bool FileAlreadyAdded(string hash) {
        bool fileFound = false;

        _context.RunQuery($"SELECT 1 FROM Photos WHERE Hash = '{hash}'", reader => {
            fileFound = true;
        });

        return fileFound;
    }

    public void AddFile(string hash, string fileId, DateTime dateTaken, string originalFilename) {
        _context.RunQuery($"INSERT INTO Photos (Hash, FileId, DateTaken, OriginalFilename) VALUES ('{hash}', '{fileId}', '{dateTaken}', '{originalFilename}')");
    }

    public void DeleteFile(string hash) {
        _context.RunQuery($"DELETE FROM Photos WHERE Hash = '{hash}'");
    }
}