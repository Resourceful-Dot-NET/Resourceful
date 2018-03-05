using System;
using System.Net;
using System.Net.Sockets;

namespace ResourcefulServer {
  public static class Utilities {
    public static string LocalIPAddress {
      get {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList) {
          if (ip.AddressFamily == AddressFamily.InterNetwork) {
            return ip.ToString();
          }
        }

        throw new Exception("No network adapters with an IPv4 address in the system!");
      }
    }
  }
}
