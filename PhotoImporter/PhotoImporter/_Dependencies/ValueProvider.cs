namespace PhotoImporter._Dependencies;

public class ValueProvider : IValueProvider {
    public string MakeGuid() => Guid.NewGuid().ToString("N");
}