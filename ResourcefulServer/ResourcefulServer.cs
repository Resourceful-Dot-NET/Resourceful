namespace ResourcefulServer {
  public class ResourcefulServer {

    private string _path = "";

    public FileWatcher FileWatcher { private set; get; }
    public EventReporter EventReporter { private set; get; }
    public static bool LogToConsole { get; set; } = true;

    public string Path {
      private set { _path = value; }
      get => string.IsNullOrWhiteSpace(_path) ? FileWatcher.CurrentWatchingPath : _path;
    }

    public int Port { private set; get; }

    public ResourcefulServer() {
      SetUp();
    }

    public ResourcefulServer(string path) {
      SetUp(path);
    }

    public ResourcefulServer(int port) {
      SetUp("", port);
    }

    public ResourcefulServer(string path, int port, bool? logToConsole = null) {
      LogToConsole = logToConsole ?? LogToConsole;
      SetUp(path, port);
    }

    private void SetUp(string path = "", int? port = null) {
      FileWatcher = new FileWatcher(path);
      EventReporter = new EventReporter(port);
      Port = EventReporter.Port;
    }

    public void ShutDown() {
      FileWatcher.Stop();
      EventReporter.Shutdown();
    }
  }
}
