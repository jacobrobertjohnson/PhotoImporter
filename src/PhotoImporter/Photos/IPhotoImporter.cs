namespace PhotoImporter.Photos;

public interface IPhotoImporter {
    void RunJob(AppConfig config);
}