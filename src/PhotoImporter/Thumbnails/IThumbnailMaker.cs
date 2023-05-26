namespace PhotoImporter.Thumbnails;

public interface IThumbnailMaker
{
    void MakeThumbnail(string sourcePath, string targetPath, int newHeight);
}