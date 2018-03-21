using MessagePack;

namespace ResourcefulShared {
  [MessagePackObject(keyAsPropertyName: true)]
  public class ResourceMessage {
    public string Name { get; internal set; }
    public byte[] Bytes { get; set; }
  }
}
