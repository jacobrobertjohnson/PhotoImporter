using Microsoft.Data.Sqlite;

namespace PhotoImporter.Library;

public interface ISqliteContext {
    void RunQuery(string query);
    void RunQuery(string query, Action<SqliteDataReader> onRun);
}