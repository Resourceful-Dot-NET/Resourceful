using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Glob;

namespace ResourcefulServer {
  public class ProjectFileManager {

    #region Fields

    private readonly List<ProjectFile> _projectFiles = new List<ProjectFile>();

    #endregion

    #region Properties

    public ReadOnlyCollection<ProjectFile> ProjectFiles => _projectFiles.AsReadOnly();
    public string Directory { get; }
    public ReadOnlyCollection<KnownResource> AllKnownResources {
      get {
        var allKnownResources = new HashSet<KnownResource>();

        foreach (var projectFile in ProjectFiles) {
          foreach (var knownResource in projectFile.KnownResources) {
            allKnownResources.Add(knownResource);
          }
        }

        return allKnownResources.ToList().AsReadOnly();
      }
    }

    public ReadOnlyCollection<string> AllWatchableDirectories {
      get {
        var allWatchableDirectories = new HashSet<string>();

        foreach (var projectFile in ProjectFiles) {
          foreach (var watchableDirectory in projectFile.WatchableDirectories) {
            allWatchableDirectories.Add(watchableDirectory);
          }
        }

        return allWatchableDirectories.ToList().AsReadOnly();
      }
    }

    #endregion

    public ProjectFileManager(string forDirectory) {
      Directory = forDirectory;
      LocateProjectFiles();
    }

    #region Methods

    public void LocateProjectFiles() {
      var directoryInfo = new DirectoryInfo(Directory);
      var files = directoryInfo.GlobFiles("**/*.csproj");

      _projectFiles.Clear();
      foreach (var file in files) {
        Logger.Good($"Found project file \"{file.Name}\"");
        AddProjectFile(file);
      }
    }

    public IEnumerable<ProjectFile> ProjectFilesOfKnownResource(KnownResource knownResource) {
      var projFiles = new List<ProjectFile>();

      foreach (var proj in ProjectFiles) {
        if (proj.KnownResources.Contains(knownResource)) {
          projFiles.Add(proj);
        }
      }

      return projFiles;
    }

    internal void AddProjectFile(FileInfo file) {
      _projectFiles.Add(new ProjectFile(file, this));
    }

    internal void AddProjectFile(string file) {
      var fileInfo = new FileInfo(file);
      AddProjectFile(fileInfo);
    }

    #endregion

  }
}
