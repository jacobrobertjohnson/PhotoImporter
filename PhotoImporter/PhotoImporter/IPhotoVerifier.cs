namespace PhotoImporter;

public interface IPhotoVerifier {
    bool PhotoWasDelivered(string hash, string destinationFile);
}