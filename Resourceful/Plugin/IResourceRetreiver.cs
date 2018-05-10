using System.Collections.Generic;
using System.Reflection;
using ResourcefulShared;

namespace Resourceful.Plugin {
  public interface IResourceRetreiver {

    #region Methods

    IEnumerable<EmbeddedResource> GetAllResourcesForAssembly(Assembly forAssembly);

    #endregion

  }
}
