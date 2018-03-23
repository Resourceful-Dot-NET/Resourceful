using System.IO;
using System.Net;
using ResourcefulShared;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace ResourcefulServer {

  public class ResourceChangedService : WebSocketBehavior {

    public ResourceChangedService() {}

    #region Methods

    protected override void OnMessage(MessageEventArgs e) {
      Send(e.Data + " ☺");
    }

    public void SendMessage(EmbeddedResource embeddedResource) {
      using (var stream = new MemoryStream()) {
        //Serializer.Serialize(stream, embeddedResource);
        Sessions.Broadcast(stream, (int)stream.Length);
      }
    }

    #endregion

  }
}
