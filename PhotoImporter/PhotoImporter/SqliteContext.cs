using Microsoft.Data.Sqlite;
using PhotoImporter;
using PhotoImporter._Dependencies;

public class SqliteContext : ISqliteContext {
    AppConfig _config;

    public SqliteContext(IDependencyFactory factory)
    {
        _config = factory.GetConfigReader().AppConfig;
    }

    public void RunQuery(string query) => RunQuery(query, (reader) => { });

    public void RunQuery(string query, Action<SqliteDataReader> onRun) {
        using (SqliteConnection connection = new SqliteConnection($"Data Source={_config.DatabasePath}")) {
            connection.Open();

            using (SqliteCommand command = connection.CreateCommand()) {
                command.CommandText = query;

                using (SqliteDataReader reader = command.ExecuteReader()) {
                    while (reader.Read())
                        onRun(reader);
                }
            }
        }
    }
}