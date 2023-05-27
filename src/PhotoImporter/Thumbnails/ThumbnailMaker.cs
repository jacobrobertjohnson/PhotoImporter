namespace PhotoImporter.Thumbnails;

public class ThumbnailMaker : IThumbnailMaker {
    IFilesystem _filesystem;

    public ThumbnailMaker(IDependencyFactory dependencyFactory) {
        _filesystem = dependencyFactory.GetFilesystem();
    }

    public void MakeThumbnail(string sourcePath, string targetPath, int newHeight) { 
        using (var stream = _filesystem.GetFileStream(sourcePath, FileMode.Open))
        using (var image = _filesystem.LoadImage(stream)) {
            int newWidth = (image.Width * newHeight) / image.Height;

            image.Resize(newWidth, newHeight);
            image.Save(targetPath);
        }
    }
}