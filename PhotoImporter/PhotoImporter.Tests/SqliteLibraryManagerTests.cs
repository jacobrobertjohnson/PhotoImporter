using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Language.Flow;
using Microsoft.Data.Sqlite;


namespace PhotoImporter.Tests;

[TestClass]
public class SqliteLibraryManagerTests : _TestBase {
    const string FILE_PATH = @"C:\fakepath\image.jpg",
        FILE_HASH = "The MD5",
        HASH_LOOKUP_QUERY = $"SELECT 1 FROM Photos WHERE Hash = '{FILE_HASH}'";

    ISetup<ISqliteContext> _runQuery;
    ILibraryManager _libMan;
    
    [TestInitialize]
    public void Setup() {
        _runQuery = _sqliteContext.Setup(x => x.RunQuery(It.IsAny<string>(), It.IsAny<Action<SqliteDataReader>>()));
        _runQuery.Verifiable();

        makeSqliteLibraryManager();
    }

    void makeSqliteLibraryManager() {
        _libMan = new SqliteLibraryManager(_dependencies.Object);
    }

    [TestMethod]
    public void Constructor_PhotosTableCreated() {
        verifyRunQuery("CREATE TABLE IF NOT EXISTS Photos (Hash TEXT, FilePath TEXT)");
    }

    [TestMethod]
    public void FileAlreadyAdded_HashPassedIntoQuery() {
        _libMan.FileAlreadyAdded(FILE_HASH);

        verifyRunQuery(HASH_LOOKUP_QUERY);
    }

    [TestMethod]
    public void FileAlreadyAdded_FileFound_TrueReturned() {
        _runQuery.Callback((string query, Action<SqliteDataReader> onRun) => {
            if (query == HASH_LOOKUP_QUERY)
                onRun(null);
        });
        
        Assert.IsTrue(_libMan.FileAlreadyAdded(FILE_HASH));
    }

    [TestMethod]
    public void FileAlreadyAdded_FileNotFound_FalseReturned() {
        _runQuery.Callback((string query, Action<SqliteDataReader> onRun) => { });
        
        Assert.IsFalse(_libMan.FileAlreadyAdded(FILE_HASH));
    }

    [TestMethod]
    public void AddFile_HashAndPathPassedIntoQuery() {
        _libMan.AddFile(FILE_HASH, FILE_PATH);

        verifyRunQuery($"INSERT INTO Photos (Hash, FilePath) VALUES({FILE_HASH}, {FILE_PATH})");
    }

    void verifyRunQuery(string query) => _sqliteContext.Verify(x => x.RunQuery(query, It.IsAny<Action<SqliteDataReader>>()), Times.Once);
}