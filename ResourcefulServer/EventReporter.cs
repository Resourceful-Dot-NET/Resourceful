using System.IO;
using System.Reflection;
using BeaconLib;
using Colorful;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using ResourcefulShared;
using WebSocketSharp;
using WebSocketSharp.Server;
using static ResourcefulServer.Utilities;

namespace ResourcefulServer {
  public class EventReporter {
    private WebSocketServer Server { get; set; }
    private Beacon Beacon { get; set; }
    public int Port { get; }

    public EventReporter(int? port = null) {
      var usePort = Port = port ?? 12690;
      Logger.Info($"Port: {usePort}");
      Server = new WebSocketServer($"ws://{LocalIPAddress}:{usePort}");
      //Server.Log.Level = LogLevel.Trace;
      //Server.Log.Output = (data, data2) => { Console.WriteLine(data2 + data); };
      Server.AddWebSocketService<ResourceChangedService>("/");
      Server.Start();
      AdvertiseServer();
      Logger.Info($"Listening on {LocalIPAddress}:{Port}");
    }

    public void Shutdown() {
      //Server.Stop();
      Beacon.Stop();
      Logger.Info($"Stopped listening on {LocalIPAddress}:{Port}");
    }

    public void Report(ResourceMessage resourceMessage) {
      using (var memoryStream = new MemoryStream()) {
        using (var writer = new BsonDataWriter(memoryStream)) {
          var serializer = new JsonSerializer();
          serializer.Serialize(writer, resourceMessage);
          Server.WebSocketServices.Broadcast(memoryStream.ToArray());
        }
      }
    }

    private void AdvertiseServer() {
      Beacon = new Beacon("Resourceful", (ushort)Port);
      Beacon.BeaconData = $"Resourceful Server on {LocalIPAddress}";
      Beacon.Start();
    }
  }
}
