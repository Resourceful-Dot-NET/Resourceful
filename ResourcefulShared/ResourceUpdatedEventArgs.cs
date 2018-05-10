using System;
using ResourcefulShared;

namespace ResourcefulShared {
  public class ResourceUpdatedEventArgs : EventArgs {
    public EmbeddedResource EmbeddedResource { get; }

    internal ResourceUpdatedEventArgs(EmbeddedResource embeddedResource) {
      EmbeddedResource = embeddedResource;
    }
  }
}
