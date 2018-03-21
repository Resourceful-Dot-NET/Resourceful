using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ResourcefulShared;

namespace Resourceful {
  public class ResourceManager {

    #region Properties

    public static string Test => string.Join(",\n", Default.Assembly.GetManifestResourceNames());
    public Assembly Assembly { get; }
    public static ResourceManager Default => ForAssembly(Assembly.GetCallingAssembly());
    private static Dictionary<string, ResourceManager> ResourceManagers { get; } = new Dictionary<string, ResourceManager>();
    private static EventListener EventListener { get; } = new EventListener();
    private List<EmbeddedResource> EmbeddedResources = new List<EmbeddedResource>();

    #endregion

    #region Constructors

    private ResourceManager(Assembly forAssembly) {
      Assembly = forAssembly;
      foreach (var embeddedResourceName in Assembly.GetManifestResourceNames()) {
        var stream = Assembly.GetManifestResourceStream(embeddedResourceName);
        EmbeddedResources.Add(new EmbeddedResource(embeddedResourceName, stream));
      }
    }

    #endregion

    #region Methods

    public EmbeddedResource GetEmbeddedResource(string resourcePath) {
      return EmbeddedResources.FirstOrDefault(er => er.Name == PathToEmbeddedResourceName(resourcePath));
    }

    public void BindToEmbeddedResource(string resourcePath, Action<EmbeddedResource> setter) {
      setter(GetEmbeddedResource(resourcePath));
    }

    public static ResourceManager ForAssembly(Assembly assembly) {
      if (ResourceManagers.ContainsKey(assembly.FullName)) return ResourceManagers[assembly.FullName];

      var resourceManager = new ResourceManager(assembly);
      ResourceManagers.Add(assembly.FullName, resourceManager);
      return resourceManager;
    }

    private string PathToEmbeddedResourceName(string path) {
      // Strip out leading dots and slashes
      path = Regex.Replace(path, @"^(\.)*(?:\/|\\)?", "");

      // Return the correct resource name for a given path
      return $"{Assembly.GetName().Name}.{Regex.Replace(path, @"\/|\\", ".")}";
    }

    #endregion

  }
}
