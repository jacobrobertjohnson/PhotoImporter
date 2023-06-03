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
    public void Constructor_DeleteAuditTableCreated() {
        verifyRunQueryNoOutput("CREATE TABLE IF NOT EXISTS DeleteAudit (DateDeleted TEXT, FileId TEXT, DeletedBy TEXT)");
    }

    [TestMethod]
    public void Constructor_ThumbnailColumnAdded() {
        verifyRunQueryNoOutput("ALTER TABLE Photos ADD COLUMN ThumbnailGenerated INT DEFAULT 0");
    }

    [TestMethod]
    public void Constructor_DeletedColumnAdded() {
        verifyRunQueryNoOutput("ALTER TABLE Photos ADD COLUMN Deleted INT DEFAULT 0");
    }

    [TestMethod]
    public void Constructor_ExifModelColumnAdded() {
        verifyRunQueryNoOutput("ALTER TABLE Photos ADD COLUMN ExifModel TEXT DEFAULT NULL");
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

    [TestMethod]
    public void AddFile_OriginalFileNameHasSingleQuotesEscaped() {
        _libMan.AddFile(FILE_HASH, FILE_PATH, DATE_TAKEN, "/path/with/apostrophe's/in/it's/path/");

        verifyRunQueryNoOutput($"INSERT INTO Photos (Hash, FileId, DateTaken, OriginalFilename) VALUES ('{FILE_HASH}', '{FILE_PATH}', '{DATE_TAKEN}', '/path/with/apostrophe''s/in/it''s/path/')");
    }

    [TestMethod]
    public void ImportIsRunning_RecordFound_TrueReturned() {
        _runQuery.Callback((string query, Action<SqliteDataReader> onRun) => {
            onRun(null);
        });

        Assert.IsTrue(_libMan.ImportIsRunning());
    }

    [TestMethod]
    public void ImportIsRunning_RecordNotFound_FalseReturned() {
        _runQuery.Callback((string query, Action<SqliteDataReader> onRun) => { });

        Assert.IsFalse(_libMan.ImportIsRunning());
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(0)]
    public void SetImportRunning(int isRunning) {
        _libMan.SetImportRunning(isRunning);

        verifyRunQueryNoOutput($"UPDATE AppState SET ImportIsRunning = {isRunning}");
    }

    [TestMethod]
    public void GetPhotosWithoutThumbnails_QueryRun() {
        _libMan.GetPhotosWithoutThumbnails();

        verifyRunQuery("SELECT FileId, DateTaken, OriginalFilename FROM Photos WHERE ThumbnailGenerated = 0");
    }

    [TestMethod]
    public void SetThumbnailGenerated_QueryRun() {
        _libMan.SetThumbnailGenerated("PhotoId123");

        verifyRunQueryNoOutput("UPDATE Photos SET ThumbnailGenerated = 1 WHERE FileId = 'PhotoId123'");
    }

    [TestMethod]
    public void GetImagesWithoutExifModel_QueryRun() {
        _libMan.GetImagesWithoutExifModel();

        verifyRunQuery("SELECT FileId, DateTaken, OriginalFilename FROM Photos WHERE ExifModel IS NULL");
    }

    [TestMethod]
    public void SetExifModel_QueryRun() {
        _libMan.SetExifModel("PhotoId123", "ExifModel123");

        verifyRunQueryNoOutput("UPDATE Photos SET ExifModel = 'ExifModel123' WHERE FileId = 'PhotoId123'");
    }

    void verifyRunQuery(string query) => _sqliteContext.Verify(x => x.RunQuery(query, It.IsAny<Action<SqliteDataReader>>()), Times.Once);

    void verifyRunQueryNoOutput(string query) => _sqliteContext.Verify(x => x.RunQuery(query), Times.Once);
}