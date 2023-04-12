using Microsoft.Data.Sqlite;

public interface ISqliteContext {
    void RunQuery(string query, Action<SqliteDataReader> onRun);
}