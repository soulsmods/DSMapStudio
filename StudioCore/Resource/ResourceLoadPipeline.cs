using SoulsFormats;
using System;
namespace StudioCore.Resource;

using System.IO;
using System.Threading.Tasks.Dataflow;

public readonly record struct LoadByteResourceRequest(
    string VirtualPath, 
    Memory<byte> Data, 
    AccessLevel AccessLevel, 
    GameType GameType);

public readonly record struct LoadFileResourceRequest(
    string VirtualPath,
    string File,
    AccessLevel AccessLevel,
    GameType GameType);
    
public readonly record struct LoadTPFTextureResourceRequest(
    string VirtualPath,
    TPF Tpf,
    int Index,
    AccessLevel AccessLevel,
    GameType GameType);

public readonly record struct ResourceLoadedReply(
    string VirtualPath,
    AccessLevel AccessLevel,
    IResource Resource);

public interface IResourceLoadPipeline
{
    public ITargetBlock<LoadByteResourceRequest> LoadByteResourceBlock { get; }
    public ITargetBlock<LoadFileResourceRequest> LoadFileResourceRequest { get; }
    public ITargetBlock<LoadTPFTextureResourceRequest> LoadTPFTextureResourceRequest { get; }
}

public class ResourceLoadPipeline<T> : IResourceLoadPipeline where T : class, IResource, new()
{
    public ITargetBlock<LoadByteResourceRequest> LoadByteResourceBlock => _loadByteResourcesTransform;
    public ITargetBlock<LoadFileResourceRequest> LoadFileResourceRequest => _loadFileResourcesTransform;
    public ITargetBlock<LoadTPFTextureResourceRequest> LoadTPFTextureResourceRequest =>
        throw new NotImplementedException();

    private ActionBlock<LoadByteResourceRequest> _loadByteResourcesTransform;
    private ActionBlock<LoadFileResourceRequest> _loadFileResourcesTransform;

    private ITargetBlock<ResourceLoadedReply> _loadedResources = null;
    
    public ResourceLoadPipeline(ITargetBlock<ResourceLoadedReply> target)
    {
        var options = new ExecutionDataflowBlockOptions();
        options.MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded;
        _loadedResources = target;
        _loadByteResourcesTransform = new ActionBlock<LoadByteResourceRequest>(r =>
        {
            var res = new T();
            bool success = res._Load(r.Data, r.AccessLevel, r.GameType);
            if (success)
            {
                _loadedResources.Post(new ResourceLoadedReply(r.VirtualPath, r.AccessLevel, res));
            }
        }, options);
        _loadFileResourcesTransform = new ActionBlock<LoadFileResourceRequest>(r =>
        {
            try
            {
                var res = new T();
                bool success = res._Load(r.File, r.AccessLevel, r.GameType);
                if (success)
                {
                    _loadedResources.Post(new ResourceLoadedReply(r.VirtualPath, r.AccessLevel, res));
                }
            }
            catch (System.IO.FileNotFoundException) { }
            catch (System.IO.DirectoryNotFoundException) { }
            // Some DSR FLVERS can't be read due to mismatching layout and vertex sizes
            catch (InvalidDataException) { }
        }, options);
    }
}

public class TextureLoadPipeline : IResourceLoadPipeline
{
    public ITargetBlock<LoadByteResourceRequest> LoadByteResourceBlock => throw new NotImplementedException();
    public ITargetBlock<LoadFileResourceRequest> LoadFileResourceRequest => throw new NotImplementedException();

    public ITargetBlock<LoadTPFTextureResourceRequest> LoadTPFTextureResourceRequest =>
        _loadTPFResourcesTransform;

    private ActionBlock<LoadTPFTextureResourceRequest> _loadTPFResourcesTransform;

    private ITargetBlock<ResourceLoadedReply> _loadedResources = null;

    public TextureLoadPipeline(ITargetBlock<ResourceLoadedReply> target)
    {
        var options = new ExecutionDataflowBlockOptions();
        options.MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded;
        _loadedResources = target;
        _loadTPFResourcesTransform = new ActionBlock<LoadTPFTextureResourceRequest>(r =>
        {
            var res = new TextureResource(r.Tpf, r.Index);
            bool success = res._LoadTexture(r.AccessLevel);
            if (success)
            {
                _loadedResources.Post(new ResourceLoadedReply(r.VirtualPath, r.AccessLevel, res));
            }
        }, options);
    }
}