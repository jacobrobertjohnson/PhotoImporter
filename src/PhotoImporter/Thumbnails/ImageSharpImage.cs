namespace PhotoImporter.Thumbnails;

public class ImageSharpImage : IImage
{
    Image _image;

    public ImageSharpImage(Stream stream) {
        _image = Image.Load(stream);
    }

    public int Height { get => _image.Height; }
    public int Width { get => _image.Width; }

    public void Resize(int width, int height) {
        _image.Mutate(x => x.Resize(width, height, KnownResamplers.Lanczos3));
    }

    public void Save(string path) {
        _image.Save(path);
    }

    public void Dispose()
    {
        _image.Dispose();
    }
}