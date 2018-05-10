using System;
using ResourcefulShared;

namespace Resourceful {
  public class ResourceMessageEventArgs : EventArgs {
    public ResourceMessage ResourceMessage { get; }

    internal ResourceMessageEventArgs(ResourceMessage resourceMessage) {
      ResourceMessage = resourceMessage;
    }
  }
}
