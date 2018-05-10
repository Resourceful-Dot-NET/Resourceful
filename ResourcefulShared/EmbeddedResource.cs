using System.IO;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Resourceful")]
[assembly: InternalsVisibleTo("ResourcefulServer")]

namespace ResourcefulShared {

  public class EmbeddedResource {

    #region Fields

    private string _text;
    private byte[] _bytes = new byte[0];

    public delegate void ResourceUpdatedEventHandler(object sender, ResourceUpdatedEventArgs resourceUpdatedEventArgs);

    #endregion

    #region Properties

    public string AssemblyName { get; internal set; }
    public string Identifier { get; internal set; }
    public string FileName { get; internal set; }
    public string Extension { get; internal set; }
    public string Path { get; internal set; }
    public string ResourceType { get; internal set; }

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

    #region Events

    public event ResourceUpdatedEventHandler Updated;

    #endregion

    #region Constructors

    public EmbeddedResource(string identifier, byte[] bytes) {
      Identifier = identifier;
      Bytes = bytes;
    }

    public EmbeddedResource(string identifier, Stream stream) {
      Identifier = identifier;

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

    internal void Update(byte[] bytes) {
      Bytes = bytes;
      Updated?.Invoke(this, new ResourceUpdatedEventArgs(this));
    }

    #endregion

  }
}
