using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Resourceful.Plugin;
using ResourcefulShared;

namespace Resourceful {
  public class ResourceManager {

    #region Fields

    private List<EmbeddedResource> EmbeddedResources { get; } = new List<EmbeddedResource>();

    internal delegate void ResourceUpdatedEventdHandler(object sender, ResourceUpdatedEventArgs resourceUpdatedEventArgs);

    internal event ResourceUpdatedEventdHandler ResourceUpdated;

    #endregion

    #region Properties

    public static string Test => string.Join(",\n", Default.Assembly.GetManifestResourceNames());
    public Assembly Assembly { get; }
    public static ResourceManager Default => ForAssembly(Assembly.GetCallingAssembly());
    private static Dictionary<string, ResourceManager> ResourceManagers { get; } = new Dictionary<string, ResourceManager>();
    private static EventListener EventListener { get; } = new EventListener();
    private static List<IResourcefulPlugin> Plugins { get; } = new List<IResourcefulPlugin>();
    private static List<IResourceRetreiver> RetreiverPlugins => Plugins.OfType<IResourceRetreiver>().ToList();
    private static List<IResourceUpdatedHandler> UpdateHandlerPlugins => Plugins.OfType<IResourceUpdatedHandler>().ToList();
    private static List<IResourceResolver> ResolverPlugins => Plugins.OfType<IResourceResolver>().ToList();

    #endregion

    #region Constructors

    private ResourceManager(Assembly forAssembly) {
      Assembly = forAssembly;
      foreach (var embeddedResourceName in Assembly.GetManifestResourceNames()) {
        var stream = Assembly.GetManifestResourceStream(embeddedResourceName);
        EmbeddedResources.Add(new EmbeddedResource(embeddedResourceName, stream));
      }
    }

    static ResourceManager() {
      FindPlugins();
      EventListener.ResourceUpdated += ResourceUpdatedHandler;
    }

    #endregion

    #region Methods

    public static ResourceManager ForAssembly(Assembly assembly) {
      if (ResourceManagers.ContainsKey(assembly.FullName)) return ResourceManagers[assembly.FullName];

      var resourceManager = new ResourceManager(assembly);
      ResourceManagers.Add(assembly.FullName, resourceManager);
      return resourceManager;
    }

    public ResourceManager BindToEmbeddedResource(string resourceType, string resourcePath, Action<EmbeddedResource> setter) {
      var res = GetEmbeddedResource(resourceType, resourcePath);
      BindResource(res, setter);
      return this;
    }

    public ResourceManager BindToEmbeddedResource(string resourcePath, Action<EmbeddedResource> setter) {
      var res = GetEmbeddedResource(resourcePath);
      BindResource(res, setter);
      return this;
    }

    private void BindResource(EmbeddedResource res, Action<EmbeddedResource> setter) {
      setter(res);
      res.Updated += (sender, e) => setter(e.EmbeddedResource);
    }

    public EmbeddedResource GetEmbeddedResource(string resourcePath) {
      return GetEmbeddedResource("EmbeddedResource", resourcePath);
    }

    public EmbeddedResource GetEmbeddedResource(string resourceType, string resourcePath) {
      return EmbeddedResources.FirstOrDefault(er => er.Identifier == PathToEmbeddedResourceName(resourcePath, resourceType, Assembly));
    }

    private static void ResourceUpdatedHandler(object sender, ResourceMessageEventArgs resourceMessageEventArgs) {
      var resourceMessage = resourceMessageEventArgs.ResourceMessage;
      foreach (var resourceManagerEntry in ResourceManagers) {
        var resourceManager = resourceManagerEntry.Value;

        resourceManager.EmbeddedResources
          .Where(r => r.Identifier ==
                      PathToEmbeddedResourceName(
                        resourceMessage.Identifier,
                        resourceMessage.ResourceType,
                        resourceManager.Assembly)).ToList()
          .ForEach(r => {
            r.Update(resourceMessage.Bytes);
            resourceManager.ResourceUpdated?.Invoke(resourceManager, new ResourceUpdatedEventArgs(r));
          });
      }
    }

    private static void FindPlugins() {
      var pluginInterfaceType = typeof(IResourcefulPlugin);
      AppDomain.CurrentDomain.GetAssemblies()
        .Where(assembly => !assembly.ToString().StartsWith("System"))
        .SelectMany(assembly => assembly.GetTypes())
        .Where(type => (pluginInterfaceType.IsAssignableFrom(type) && !type.IsInterface))
        .ToList().ForEach(plugin =>
          Plugins.Add((IResourcefulPlugin)Activator.CreateInstance(type: plugin)));
    }

    private static string PathToEmbeddedResourceName(string path, string resourceType, Assembly forAssembly) {
      // Strip out leading dots and slashes
      path = Regex.Replace(path, @"^(\.)*(?:\/|\\)?", "");

      // Normalize slashes
      path = Regex.Replace(path, @"(\/|\\)+", "/");

      var resolver = ResolverPlugins
        .FirstOrDefault(plugin =>
          (plugin as IResourcefulPlugin)?.HandlesTypes.Contains(resourceType) ?? false);

      return resolver?.PathToIdentifier(path, forAssembly) ?? "";
    }

    #endregion

  }
}
