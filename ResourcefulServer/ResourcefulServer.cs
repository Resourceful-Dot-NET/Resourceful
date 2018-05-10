using System.Collections.Generic;
using System.IO;
using System.Linq;
using ResourcefulShared;

namespace ResourcefulServer {
  public class ResourcefulServer {

    private string _path = "";

    public List<FileWatcher> FileWatchers { get; } = new List<FileWatcher>();
    public EventReporter EventReporter { private set; get; }
    public ProjectFileManager ProjectFileManager { private set; get; }
    public static bool LogToConsole { get; set; } = true;

    public string Path {
      private set { _path = value; }
      get => string.IsNullOrWhiteSpace(_path) ? "" : _path;
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
      EventReporter = new EventReporter((port ?? 0) != 0 ? port : null);
      ProjectFileManager = new ProjectFileManager(path);

      // For all project files, watch that directory
      foreach (var proj in ProjectFileManager.ProjectFiles) {
        foreach (var dir in proj.WatchableDirectories) {
          var watcher = new FileWatcher(dir);

          FileWatchers.Add(watcher);

          watcher.ResourceModified += e => {
            KnownResource resource = null;
            IEnumerable<ProjectFile> resourceProjFiles = null;

            foreach (var knownRes in ProjectFileManager.AllKnownResources) {
              if (knownRes.FullPath != e.FullPath) continue;

              resource = knownRes;
              resourceProjFiles = ProjectFileManager.ProjectFilesOfKnownResource(resource);
              break;
            }

            if (resource == null) return;

            Logger.Good($"Detected resource \"{resource.Identifier}\" was changed.");

            using (var streamReader = new StreamReader(e.FullPath)) {
              using (var stream = new MemoryStream()) {
                streamReader.BaseStream.CopyTo(stream);
                EventReporter.Report(new ResourceMessage {
                  AssemblyName = resourceProjFiles?.First().AssemblyName,
                  Identifier = resource.Identifier,
                  ResourceType = resource.ResourceType,
                  Bytes = stream.ToArray()
                });
              }
            }
          };
        }
      }

      Port = EventReporter.Port;
    }

    public void ShutDown() {
      foreach (var watcher in FileWatchers) {
        watcher.Stop();
      }

      EventReporter.Shutdown();
    }
  }
}
