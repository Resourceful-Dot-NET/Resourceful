using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ResourcefulShared {
  public static class Utilities {

    #region Properties

#if NET45
    public static bool IsWindows {
      get {
        var windir = Environment.GetEnvironmentVariable("windir");
        return (!string.IsNullOrEmpty(windir) && windir.Contains(@"\") && Directory.Exists(windir));
      }
    }

    public static bool IsLinux {
      get {
        if (!File.Exists(@"/proc/sys/kernel/ostype")) return false;
        var osType = File.ReadAllText(@"/proc/sys/kernel/ostype");
        return osType.StartsWith("Linux", StringComparison.OrdinalIgnoreCase);
      }
    }

    public static bool IsMacOS => File.Exists(@"/System/Library/CoreServices/SystemVersion.plist");

#else
    public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    public static bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    public static bool IsMacOS => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
#endif

    public static bool IsConsoleApp {
      get {
        try {
          return Console.WindowHeight > 0;
        }
        catch {
          return false;
        }
      }
    }

    #endregion

  }
}
