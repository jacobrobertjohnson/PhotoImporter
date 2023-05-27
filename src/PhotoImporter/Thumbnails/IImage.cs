namespace PhotoImporter.Thumbnails;

public interface IImage : IDisposable {
    int Height { get; }
    int Width { get; }
    void Resize(int width, int height);
    void Save(string path);
}