using System.Reflection;
using Colorful;
using WebSocketSharp;
using WebSocketSharp.Server;
using static ResourcefulServer.Utilities;

namespace ResourcefulServer {
  public class EventReporter {
    private WebSocketServer Server { get; set; }
    public int Port { get; }

    public EventReporter(int? port = null) {
      var usePort = Port = port ?? 2691;
      Server = new WebSocketServer($"ws://{LocalIPAddress}:{usePort}");
      Server.Log.Level = LogLevel.Trace;
      Server.Log.Output = (data, data2) => { Console.WriteLine(data2 + data); };
      Server.AddWebSocketService<Echo> ("/Echo");
      Server.Start();
      Logger.Info($"Listening on {LocalIPAddress}:{Port}");
    }

    public void Shutdown() {
      Server.Stop();
      Logger.Info($"Stopped listening on {LocalIPAddress}:{Port}");
    }
  }
}
