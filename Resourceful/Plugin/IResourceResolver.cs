using System.Reflection;

namespace Resourceful.Plugin {
  public interface IResourceResolver {
    string PathToIdentifier(string path, Assembly forAssembly);
  }
}
