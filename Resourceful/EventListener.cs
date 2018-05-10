using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using BeaconLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using ResourcefulShared;
using WebSocketSharp;

namespace Resourceful {
  internal class EventListener {

    private List<WebSocket> WebSockets { get; set; } = new List<WebSocket>();

    internal delegate void ResourceUpdatedEventHandler(object sender, ResourceMessageEventArgs resourceMessageEventArgs);

    internal event ResourceUpdatedEventHandler ResourceUpdated;

    internal EventListener() {
      FindServer();
    }

    #region Methods

    private void FindServer() {
      var probe = new Probe("Resourceful");

      probe.BeaconsUpdated += beacons => {
        foreach (var beacon in beacons) {
          Debug.WriteLine(beacon.Address + ": " + beacon.Data);
          ConnectToServer(beacon.Address);
          /*using (var ws = new WebSocket ("ws://dragonsnest.far/Laputa")) {
            ws.OnMessage += (sender, e) =>
              Console.WriteLine ("Laputa says: " + e.Data);

            ws.Connect();
          }*/
        }

        probe.Stop();
      };

      probe.Start();
    }

    private void ConnectToServer(IPEndPoint address) {
      var ws = new WebSocket ($"ws://{address}/");
      WebSockets.Add(ws);
      ws.OnMessage += (sender, e) => {
        using (var memoryStream = new MemoryStream(e.RawData)) {
          using (var reader = new BsonDataReader(memoryStream)) {
            var serializer = new JsonSerializer();
            var res = serializer.Deserialize<ResourceMessage>(reader);
            // Debug.WriteLine(new EmbeddedResource(res.Name, res.Bytes));
            ResourceUpdated?.Invoke(this, new ResourceMessageEventArgs(res));
          }
        }
      };

      ws.Connect();
    }

    #endregion

  }
}
