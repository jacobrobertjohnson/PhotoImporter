using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Language.Flow;
using Microsoft.Data.Sqlite;


namespace PhotoImporter.Tests;

[TestClass]
public class SqliteLibraryManagerTests : _TestBase {
    const string ORIGINAL_FILENAME = "image.jpg",
        FILE_PATH = @"C:\fakepath\" + ORIGINAL_FILENAME,
        FILE_HASH = "The MD5",
        HASH_LOOKUP_QUERY = $"SELECT 1 FROM Photos WHERE Hash = '{FILE_HASH}'";

    readonly DateTime DATE_TAKEN = DateTime.Parse("2023-01-01");

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
        verifyRunQueryNoOutput("CREATE TABLE IF NOT EXISTS Photos (Hash TEXT, FileId TEXT, DateTaken TEXT, OriginalFilename TEXT)");
    }

    [TestMethod]
    public void Constructor_AppStateTableCreated() {
        verifyRunQueryNoOutput("CREATE TABLE IF NOT EXISTS AppState (ImportIsRunning INT)");
    }

    [TestMethod]
    public void Constructor_AppStateTablePopulated() {
        verifyRunQueryNoOutput("INSERT INTO AppState (ImportIsRunning) SELECT 0 WHERE NOT EXISTS (SELECT 1 FROM AppState)");
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
    public void AddFile_HashPathAndDatePassedIntoQuery() {
        _libMan.AddFile(FILE_HASH, FILE_PATH, DATE_TAKEN, ORIGINAL_FILENAME);

        verifyRunQueryNoOutput($"INSERT INTO Photos (Hash, FileId, DateTaken, OriginalFilename) VALUES ('{FILE_HASH}', '{FILE_PATH}', '{DATE_TAKEN}', '{ORIGINAL_FILENAME}')");
    }

    void verifyRunQuery(string query) => _sqliteContext.Verify(x => x.RunQuery(query, It.IsAny<Action<SqliteDataReader>>()), Times.Once);

    void verifyRunQueryNoOutput(string query) => _sqliteContext.Verify(x => x.RunQuery(query), Times.Once);
}