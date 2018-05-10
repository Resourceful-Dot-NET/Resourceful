using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ResourcefulShared;

namespace Resourceful.Plugin {
  public class EmbeddedResourcePlugin : IResourcefulPlugin, IResourceRetreiver, IResourceResolver {

    #region Properties

    public IEnumerable<string> HandlesTypes => new[] {
      "EmbeddedResource"
    };

    public IEnumerable<string> HandlesFileExtentsions => null;

    #endregion

    #region Methods

    public IEnumerable<EmbeddedResource> GetAllResourcesForAssembly(Assembly forAssembly) {
      return (from embeddedResourceName in forAssembly.GetManifestResourceNames()
              let stream = forAssembly.GetManifestResourceStream(embeddedResourceName)
              select new EmbeddedResource(embeddedResourceName, stream)).ToList();
    }

    #endregion

    public string PathToIdentifier(string path, Assembly forAssembly) {
      return $"{forAssembly.GetName().Name}.{Regex.Replace(path, @"\/|\\", ".")}";
    }
  }
}
