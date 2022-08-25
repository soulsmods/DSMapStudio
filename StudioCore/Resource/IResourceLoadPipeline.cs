namespace StudioCore.Resource;

using System.Threading.Tasks.Dataflow;

public interface IResourceLoadPipeline
{
    public ITargetBlock<LoadByteResourceRequest> LoadByteResourceBlock { get; }
    public ITargetBlock<LoadFileResourceRequest> LoadFileResourceRequest { get; }
    public ITargetBlock<LoadTPFTextureResourceRequest> LoadTPFTextureResourceRequest { get; }
}