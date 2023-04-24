namespace PhotoImporter.Photos;

public interface IPhotoVerifier {
    bool PhotoWasDelivered(string hash, string destinationFile);
}