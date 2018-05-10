using System.Reflection;
using ResourcefulShared;

namespace Resourceful.Plugin {
  public interface IResourceUpdatedHandler {

    #region Methods

    void OnResourceUpdated(EmbeddedResource resource, Assembly forAssembly);

    #endregion

  }
}
