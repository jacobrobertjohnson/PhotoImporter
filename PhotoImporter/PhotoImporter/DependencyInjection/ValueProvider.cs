namespace PhotoImporter.DependencyInjection;

public class ValueProvider : IValueProvider {
    public string MakeGuid() => Guid.NewGuid().ToString("N");
}