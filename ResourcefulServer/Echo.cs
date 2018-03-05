using WebSocketSharp;
using WebSocketSharp.Server;

namespace ResourcefulServer {

  public class Echo : WebSocketBehavior {

    #region Methods

    protected override void OnMessage(MessageEventArgs e) {
      Send(e.Data + " ☺");
    }

    #endregion

  }
}
