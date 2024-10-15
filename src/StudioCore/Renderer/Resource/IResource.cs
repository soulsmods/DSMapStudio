using System;

namespace StudioCore.Renderer.Resource;

public interface IResource
{
    public bool _Load(Memory<byte> bytes, AccessLevel al, GameType type);
    public bool _Load(string file, AccessLevel al, GameType type);
}
