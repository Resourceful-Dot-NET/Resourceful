using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Dom.Xml;
using AngleSharp.Extensions;
using AngleSharp.Parser.Xml;

namespace ResourcefulServer {
  public class ProjectFile {

    #region Fields

    private HashSet<KnownResource> _knownResources = new HashSet<KnownResource>();
    private HashSet<string> _watchableDirectories = new HashSet<string>();

    #endregion

    #region Properties

    public FileInfo CsProjFile { get; }
    public string AssemblyName { get; }
    public ReadOnlyCollection<KnownResource> KnownResources => _knownResources.ToList().AsReadOnly();
    public ReadOnlyCollection<string> WatchableDirectories => _watchableDirectories.ToList().AsReadOnly();

    private string Xml { get; }
    private ProjectFileManager Manager { get; }
    private IXmlDocument Document { get; }
    private List<string> FoundResourceMessageLogs { get; } = new List<string>();

    // language=css
    private string ResourceSelector { get; } =
      "[Include]:not(Reference):not(ProjectReference):not(PackageReference):not(Folder)";

    // language=css
    private string SharedProjSelector { get; } = "[Project$=\".projitems\"]";

    // language=css
    private string ProjReferenceSelector { get; } = "ProjectReference[Include$=\".csproj\"]";

    #endregion

    #region Constructors

    internal ProjectFile(FileInfo csProjFile, ProjectFileManager manager) {
      CsProjFile = csProjFile;
      _watchableDirectories.Add(csProjFile.DirectoryName);
      Manager = manager;
      Xml = File.ReadAllText(csProjFile.FullName);
      var parser = new XmlParser();
      Document = parser.Parse(Xml);
      AssemblyName = GetAssemblyName();
      ParseXml();
      ParseSharedProjects();
      ParseCsProjs();
    }

    #endregion

    #region Methods

    private void AddResource(IElement resourceDomElement, string root = null) {
      var resPath = resourceDomElement.GetAttribute("Include");
      var path = GetAbsolutePath(resPath, root);
      var type = resourceDomElement.TagName;

      // If the file is a source file or doesn't exist - return
      if (type.ToLower() == "compile" ||
          !File.Exists(path)) return;

      if (FoundResourceMessageLogs.Count > 0) {
        Console.SetCursorPosition(0, Console.CursorTop -1);
        Logger.Info(FoundResourceMessageLogs.Last().Replace("└", "├"));
      }

      var foundMessage = $" └ Found resource \"{resPath}\"";
      FoundResourceMessageLogs.Add(foundMessage);

      Logger.Info(foundMessage);
      _knownResources.Add(new KnownResource(path, type,
        GetResourceIdentifierFromElement(resourceDomElement)));
    }

    private string NormalizePath(string path) {
      return Regex.Replace(path, @"\\", Path.DirectorySeparatorChar.ToString());
    }

    private string GetAbsolutePath(string path, string rootPath = null) {
      if (rootPath == null) rootPath = CsProjFile.DirectoryName;

      return Path.GetFullPath(Path.Combine(rootPath,
        Regex.Replace(NormalizePath(path), @"\$\(MSBuildThisFileDirectory\)", "." + Path.DirectorySeparatorChar)));
    }

    private string GetAssemblyName() {
      var assemblyNames = Document.QuerySelectorAll("AssemblyName");

      if (assemblyNames.Length > 0) return assemblyNames.Last().Text();

      return Regex.Replace(CsProjFile.Name, "\\.csproj$", "");
    }

    private string GetResourceIdentifierFromElement(IElement resourceDomElement) {
      var path = resourceDomElement.GetAttribute("Include");
      return Regex.Replace(path, @"\$\(MSBuildThisFileDirectory\)", "");
    }

    private void ParseCsProjs() {
      var projReferences = Document.QuerySelectorAll(ProjReferenceSelector);

      foreach (var projReference in projReferences) {
        var path = GetAbsolutePath(projReference.GetAttribute("Include"));
        Manager.AddProjectFile(path);
      }
    }

    private void ParseSharedProjects() {
      var sharedProjects = Document.QuerySelectorAll(SharedProjSelector);

      foreach (var sharedProj in sharedProjects) {
        var path = GetAbsolutePath(sharedProj.GetAttribute("Project"));
        var parentDir = new FileInfo(path).DirectoryName;
        _watchableDirectories.Add(parentDir);
        var parser = new XmlParser();
        var document = parser.Parse(File.ReadAllText(path));

        var resources = document.QuerySelectorAll(ResourceSelector);

        foreach (var resource in resources) AddResource(resource, parentDir);
      }
    }

    private void ParseXml() {
      var resources = Document.QuerySelectorAll(ResourceSelector);
      foreach (var resource in resources) AddResource(resource);
    }

    #endregion

  }
}
