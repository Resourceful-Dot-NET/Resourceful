using System;
using System.Diagnostics;
using System.Drawing;
using System.IO.Compression;
using System.Linq;
using static ResourcefulShared.Utilities;

namespace ResourcefulServer {
  internal enum MessageType {
    Good,
    Bad,
    Info
  }

  public static class Logger {

    #region Fields

    private static readonly string _goodGutter = "✓ ";
    private static readonly string _badGutter = "✗ ";
    private static readonly string _infoGutter = "ℹ ";
    private static readonly string _windowsGoodGutter = "!) ";
    private static readonly string _windowsBadGutter = "x) ";
    private static readonly string _windowsInfoGutter = "i) ";
    private static readonly string _unixGoodColor = "\x1b[38;5;41m";
    private static readonly string _unixBadColor = "\x1b[38;5;203m";
    private static readonly string _unixInfoColor = "\x1b[38;5;49m";
    private static readonly string _unixColorReset = "\x1b[0m";

    #endregion

    #region Methods

    public static void Bad(string text) {
      WriteMessage(text, MessageType.Bad);
    }

    public static void ClearLine() {
      var currentLineCursor = Console.CursorTop;
      Console.SetCursorPosition(left: 0, top: Console.CursorTop);
      Console.Write(new string(c: ' ', count: Console.WindowWidth));
      Console.SetCursorPosition(left: 0, top: currentLineCursor);
    }

    public static void Good(string text) {
      WriteMessage(text, MessageType.Good);
    }

    public static void Info(string text) {
      WriteMessage(text, MessageType.Info);
    }

    private static string FixGutter(string text) {
      return string.Join("\n " + (IsWindows ? "  " : ""), text.Split('\n'));
    }

    private static void WriteMessage(string text, MessageType msgType) {
      ClearLine();

      string gutter;
      string unixColor;
      ConsoleColor windowsColor;

      switch (msgType) {
        case MessageType.Good:
          gutter = IsWindows ? _windowsGoodGutter : _goodGutter;
          unixColor = _unixGoodColor;
          windowsColor = ConsoleColor.Green;
          break;
        case MessageType.Bad:
          gutter = IsWindows ? _windowsBadGutter : _badGutter;
          unixColor = _unixBadColor;
          windowsColor = ConsoleColor.Red;
          break;
        case MessageType.Info:
          gutter = IsWindows ? _windowsInfoGutter : _infoGutter;
          unixColor = _unixInfoColor;
          windowsColor = ConsoleColor.Cyan;
          break;
        default:
          return;
      }

      var message = gutter + FixGutter(text);

      if (IsConsoleApp && ResourcefulServer.LogToConsole) {
        if (IsWindows) {
          Console.ForegroundColor = windowsColor;
          Console.WriteLine(message);
          Console.ResetColor();
        }
        else {
          Console.WriteLine(unixColor + message + _unixColorReset);
        }
      }
      else {
        Debug.WriteLine(text);
      }
    }

    #endregion

  }
}
