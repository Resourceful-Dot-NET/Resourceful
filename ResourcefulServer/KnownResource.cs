namespace ResourcefulServer {
  public class KnownResource {
    public string FullPath { get; }
    public string ResourceType { get; }
    public string Identifier { get; }

    public KnownResource(string fullPath, string resourceType, string identifier) {
      FullPath = fullPath;
      ResourceType = resourceType;
      Identifier = identifier;
    }

    public override string ToString()  {
      return FullPath;
    }

    public override bool Equals(object obj) {
      if (obj is KnownResource resource) {
        return FullPath.Equals(resource.FullPath);
      }

      return false;
    }

    public override int GetHashCode() {
      return FullPath.GetHashCode();
    }
  }
}
