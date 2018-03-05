using System;
using System.Runtime.InteropServices;
using static ResourcefulShared.Utilities;

namespace ResourcefulCLI {

  public enum CtrlType {
    CTRL_C_EVENT = 0,
    CTRL_BREAK_EVENT = 1,
    CTRL_CLOSE_EVENT = 2,
    CTRL_LOGOFF_EVENT = 5,
    CTRL_SHUTDOWN_EVENT = 6
  }

  public static class SystemEvents {

    #region Fields

    public delegate bool EventHandler(CtrlType sig);

    #endregion

    #region Events

    public static event EventHandler OnExit;

    #endregion

    #region Constructors

    static SystemEvents() {
      Console.CancelKeyPress += (s, ev) => ev.Cancel = OnExit?.Invoke(CtrlType.CTRL_C_EVENT) ?? false;
      AppDomain.CurrentDomain.ProcessExit += (s, ev) => OnExit?.Invoke(CtrlType.CTRL_CLOSE_EVENT);

      if (IsWindows) {
        SetConsoleCtrlHandler(ctrlType => OnExit?.Invoke(ctrlType) ?? true, add: true);
      }
    }

    #endregion

    #region Methods

    [DllImport("Kernel32")]
    private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

    #endregion

  }
}
