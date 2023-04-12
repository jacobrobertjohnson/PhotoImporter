using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Language.Flow;
using PhotoImporter;
using Microsoft.Data.Sqlite;
using PhotoImporter._Dependencies;


namespace PhotoImporter.Tests;

[TestClass]
public class SqliteDuplicateManagerTests : _TestBase {
    ISetup<ISqliteContext> _runQuery;
    IDuplicateManager _dupeMan;
    
    [TestInitialize]
    public void Setup() {
        _runQuery = _sqliteContext.Setup(x => x.RunQuery(It.IsAny<string>(), It.IsAny<Action<SqliteDataReader>>()));
        _runQuery.Verifiable();

        makeSqliteDuplicateManager();
    }

    void makeSqliteDuplicateManager() {
        _dupeMan = new SqliteDuplicateManager(_dependencies.Object);
    }

    [TestMethod]
    public void Constructor_PhotosTableCreated() {
        verifyRunQuery("CREATE TABLE IF NOT EXISTS Photos");
    }

    void verifyRunQuery(string query) => _sqliteContext.Verify(x => x.RunQuery(query, It.IsAny<Action<SqliteDataReader>>()), Times.Once);
}