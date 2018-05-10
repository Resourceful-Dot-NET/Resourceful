using System.Collections.Generic;

namespace Resourceful.Plugin {
  public interface IResourcefulPlugin {

    #region Properties

    IEnumerable<string> HandlesTypes { get; }
    IEnumerable<string> HandlesFileExtentsions { get; }

    #endregion

  }
}
