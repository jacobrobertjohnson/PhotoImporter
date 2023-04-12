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
    const string FILE_HASH = "The MD5",
        HASH_LOOKUP_QUERY = $"SELECT 1 FROM Photos WHERE Hash = '{FILE_HASH}'";

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
        verifyRunQuery("CREATE TABLE IF NOT EXISTS Photos (Hash TEXT, FilePath TEXT)");
    }

    [TestMethod]
    public void FileAlreadyAdded_HashPassedIntoQuery() {
        _dupeMan.FileAlreadyAdded(FILE_HASH);

        verifyRunQuery(HASH_LOOKUP_QUERY);
    }

    [TestMethod]
    public void FileAlreadyAdded_FileFound_TrueReturned() {
        _runQuery.Callback((string query, Action<SqliteDataReader> onRun) => {
            if (query == HASH_LOOKUP_QUERY)
                onRun(null);
        });
        
        Assert.IsTrue(_dupeMan.FileAlreadyAdded(FILE_HASH));
    }

    [TestMethod]
    public void FileAlreadyAdded_FileNotFound_FalseReturned() {
        _runQuery.Callback((string query, Action<SqliteDataReader> onRun) => { });
        
        Assert.IsFalse(_dupeMan.FileAlreadyAdded(FILE_HASH));
    }

    void verifyRunQuery(string query) => _sqliteContext.Verify(x => x.RunQuery(query, It.IsAny<Action<SqliteDataReader>>()), Times.Once);
}