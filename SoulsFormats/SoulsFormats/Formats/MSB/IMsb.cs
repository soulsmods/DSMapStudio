using System.Collections.Generic;
using System.Numerics;

namespace SoulsFormats
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public interface IMsb
    {
        IMsbParam<IMsbModel> Models { get; }

        IMsbParam<IMsbPart> Parts { get; }

        IMsbParam<IMsbRegion> Regions { get; }
    }

    public interface IMsbParam<T> where T : IMsbEntry
    {
        IReadOnlyList<T> GetEntries();
    }

    public interface IMsbEntry
    {
        string Name { get; set; }
    }

    public interface IMsbModel : IMsbEntry { }

    public interface IMsbPart : IMsbEntry
    {
        string ModelName { get; set; }

        Vector3 Position { get; set; }

        Vector3 Rotation { get; set; }

        Vector3 Scale { get; set; }
    }

    public interface IMsbRegion : IMsbEntry
    {
        Vector3 Position { get; set; }

        Vector3 Rotation { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
