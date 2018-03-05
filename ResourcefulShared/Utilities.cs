using System;
using System.Runtime.InteropServices;

namespace ResourcefulShared {
  public static class Utilities {

    #region Properties

    public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    public static bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    public static bool IsMacOS => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

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
