using Microsoft.Data.Sqlite;

public class SqliteContext : ISqliteContext {
    public void RunQuery(string query, Action<SqliteDataReader> onRun) {
        
    }
}