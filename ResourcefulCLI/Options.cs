using CommandLine;

namespace ResourcefulCLI {
  public class Options {
    [Option(HelpText = "The path to watch. Usually this should be the path containing the project file.")]
    public string Path { get; set; }

    [Option(HelpText = "The port the server should serve from.")]
    public ushort Port { get; set; }
  }
}
