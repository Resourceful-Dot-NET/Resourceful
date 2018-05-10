using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Colorful;
using CommandLine;
using Kurukuru;
using ResourcefulServer;
using Console = Colorful.Console;
using static ResourcefulShared.Utilities;

namespace ResourcefulCLI {
  internal class Program {

    private static ManualResetEvent signalEvent = new ManualResetEvent(false);
    private static bool _watching = true;

    private static ResourcefulServer.ResourcefulServer Server { get; set; }
    private static TaskCompletionSource<bool> IsFinishedWatching { get; } = new TaskCompletionSource<bool>();

    private static bool Watching {
      get { return _watching; }
      set {
        if (value == false && _watching != false) {
          IsFinishedWatching.SetResult(true);
        }
        _watching = value;
      }
    }


    #region Methods

    private static async Task Main(string[] args) {
      try {
        Options options = null;
        CommandLine.Parser.Default.ParseArguments<Options>(args)
          .WithParsed(opts => options = opts);

        if (options != null) await Start(options);
      }
      catch (Exception e) {
        Logger.Bad(e.Message + "\n" + e.StackTrace);
      }
    }

    private static async Task Start(Options options) {
      // Bind exit handler.
      SystemEvents.OnExit += ctrlType => {
        Watching = false;
        Server.ShutDown();
        return true;
      };

      // Print the "Resourceful" logo to the console.
      WriteLogo();

      foreach (Assembly assem in AppDomain.CurrentDomain.GetAssemblies())
        if (!assem.ToString().StartsWith("System"))Console.WriteLine(assem.ToString());

      // Create the file system watcher
      Server = new ResourcefulServer.ResourcefulServer(options.Path, options.Port);

      // Show the spinner indicator and start the assocaited watching loop
      await Spinner.StartAsync($"Watching {Server.Path}", async spinner => {
        spinner.Color = ConsoleColor.Green;

        Logger.Info("Watcher Started");

        await IsFinishedWatching.Task;
      }, Patterns.Arc);
    }

    private static void WriteLogo() {
      if (IsWindows) WriteLogoForUnix();
      else WriteLogoForUnix();
    }

    private static void WriteLogoForWindows() {
      var font = FigletFont.Load("Fonts/slant.flf");
      var figlet = new Figlet(font);

      for (var line = 0; line < figlet.ToAscii("Resourceful").CharacterGeometry.GetLength(0); line++) {
        var lineChars = figlet.ToAscii("Resourceful").CharacterGeometry;
        var rowChars = new List<char>();

        for (var col = 0; col < lineChars.GetLength(1); col++) {;
          rowChars.Add(lineChars[line, col]);
        }

        Console.WriteWithGradient(
          input: rowChars,
          startColor: Color.Yellow,
          endColor: Color.Fuchsia);

        Console.Write("\n");
      }
    }

    private static void WriteLogoForUnix() {
      var font = FigletFont.Load("Fonts/slant.flf");
      var figlet = new Figlet(font);

      for (var line = 0; line < figlet.ToAscii("Resourceful").CharacterGeometry.GetLength(0); line++) {
        var lineChars = figlet.ToAscii("Resourceful").CharacterGeometry;
        var startColor = Color.Yellow;
        var endColor = Color.Fuchsia;
        var currentColor = startColor;
        var rStep = (startColor.R - endColor.R) / lineChars.GetLength(1);
        var gStep = (startColor.G - endColor.G) / lineChars.GetLength(1);
        var bStep = (startColor.B - endColor.B) / lineChars.GetLength(1);

        void PrintWithRgb(char text, byte r, byte g, byte b) {
          Console.Write($"\x1b[38;2;{r};{g};{b}m{text}\x1b[0m");
        }

        PrintWithRgb(lineChars[line, 0], startColor.R, startColor.G, startColor.B);
        for (var col = 1; col < lineChars.GetLength(1); col++) {
          currentColor = Color.FromArgb(
            red: currentColor.R - rStep,
            green: currentColor.G - gStep,
            blue: currentColor.B - bStep);
          PrintWithRgb(lineChars[line, col], currentColor.R, currentColor.G, currentColor.B);
        }

        Console.Write("\n");
      }
    }

    #endregion

  }
}
