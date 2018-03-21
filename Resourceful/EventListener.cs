using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using BeaconLib;
using ProtoBuf;
using ResourcefulShared;
using WebSocketSharp;

namespace Resourceful {
  internal class EventListener {

    private List<WebSocket> WebSockets = new List<WebSocket>();

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
        var res = Serializer.Deserialize<EmbeddedResource>(new MemoryStream(e.RawData));
        Debug.WriteLine(res);
      };

      ws.Connect();
    }

    #endregion

  }
}
