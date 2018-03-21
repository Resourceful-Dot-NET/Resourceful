using System.IO;
using System.Runtime.CompilerServices;
using MessagePack;
using ProtoBuf;

[assembly: InternalsVisibleTo("Resourceful")]
[assembly: InternalsVisibleTo("ResourcefulServer")]
namespace ResourcefulShared {

  public class EmbeddedResource {

    #region Fields

    private string _text;

    private byte[] _bytes;

    #endregion

    #region Properties

    public string Name { get; internal set; }
    public string FileName { get; internal set; }
    public string Extension { get; internal set; }
    public string Path { get; internal set; }

    public string Text {
      get {
        if (_text != null) return _text;
        using (var reader = new StreamReader(Stream)) {
          _text = reader.ReadToEnd();
        }

        return _text;
      }
    }

    public Stream Stream => new MemoryStream(Bytes);

    public byte[] Bytes {
      get => _bytes;
      internal set {
        _text = null;
        _bytes = value;
      }
    }

    #endregion

    #region Constructors

    internal EmbeddedResource(string name, byte[] bytes) {
      Name = name;
      Bytes = bytes;
    }

    internal EmbeddedResource(string name, Stream stream) {
      Name = name;

      using (var memoryStream = new MemoryStream()) {
        stream.CopyTo(memoryStream);
        Bytes = memoryStream.ToArray();
      }
    }

    #endregion

    #region Methods

    public static implicit operator string(EmbeddedResource er) {
      return er.Text;
    }

    public static implicit operator Stream(EmbeddedResource er) {
      return er.Stream;
    }

    public static implicit operator byte[](EmbeddedResource er) {
      return er.Bytes;
    }

    #endregion

  }
}
