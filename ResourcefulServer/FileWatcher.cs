using System.IO;

namespace ResourcefulServer {
  public class FileWatcher {

    #region Fields

    private string _currentWatchingPath = "";

    #endregion

    #region Properties

    public string CurrentWatchingPath {
      get => string.IsNullOrEmpty(_currentWatchingPath) ? Directory.GetCurrentDirectory() : _currentWatchingPath;
      set {
        if (string.IsNullOrWhiteSpace(value)) throw new IOException("Path must not be null or whitespace.");

        if (!Directory.Exists(value)) throw new IOException("Path must be a valid, existing directory.");
        _currentWatchingPath = value;
      }
    }

    public FileSystemWatcher Watcher { get; set; }

    public delegate void ModifiedHandler(FileSystemEventArgs fsEvent);

    #endregion

    public event ModifiedHandler ResourceModified;

    #region Constructors

    public FileWatcher(string path = "", bool startWatching = true) {
      if (!string.IsNullOrEmpty(path)) CurrentWatchingPath = path;

      Watcher = new FileSystemWatcher {
        Path = CurrentWatchingPath,
        IncludeSubdirectories = true,
        EnableRaisingEvents = startWatching
      };

      Watcher.Changed += OnChanged;
      Watcher.Created += OnChanged;
      Watcher.Deleted += OnChanged;
    }

    #endregion

    #region Methods

    public void Stop() {
      Watcher.EnableRaisingEvents = false;
    }

    public void Resume() {
      Watcher.EnableRaisingEvents = true;
    }

    private void OnChanged(object source, FileSystemEventArgs e) {
      // Specify what is done when a file is changed, created, or deleted.
      Logger.Info($"File: {e.FullPath} was {e.ChangeType.ToString().ToLower()}.");
      ResourceModified?.Invoke(e);
    }

    #endregion

  }
}
