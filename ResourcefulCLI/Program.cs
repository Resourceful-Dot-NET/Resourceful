using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Colorful;
using Kurukuru;
using ResourcefulServer;
using Console = Colorful.Console;
using ResourcefulServer;

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
      /*var watcherThread = new Thread(Start);
      watcherThread.Start();
      signalEvent.WaitOne(); // This thread will block here until the reset event is sent.
      signalEvent.Reset();*/
      await Start();
    }

    private static async Task Start() {
      // Bind exit handler.
      SystemEvents.OnExit += ctrlType => {
        Watching = false;

        //signalEvent.Set(); // Unblock main thread so the program can continue to exiting.
        return true;
      };

      // Print the "Resourceful" logo to the console.
      WriteLogo();

      // Create the file system watcher
      Server = new ResourcefulServer.ResourcefulServer();

      Server.ShutDown();

      Console.WriteLine("test");

      // Show the spinner indicator and start the assocaited watching loop
      /*await Spinner.StartAsync($"Watching {Server.Path}", async spinner => {
        spinner.Color = ConsoleColor.Magenta;

        Logger.Info("Watcher Started");

        await IsFinishedWatching.Task;
      }, Patterns.Arc);*/
      await IsFinishedWatching.Task;
    }

    private static void WriteLogo() {
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

    #endregion

  }
}
