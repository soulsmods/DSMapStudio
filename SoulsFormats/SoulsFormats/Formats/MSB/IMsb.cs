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

        IMsbParam<IMsbEvent> Events { get;  }

        // Writing methods
        public byte[] Write();
        public byte[] Write(DCX.Type compression);
        public void Write(string path);
        public void Write(string path, DCX.Type compression);
    }

    public interface IMsbParam<T> where T : IMsbEntry
    {
        void Add(T item);

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
        MSB.Shape Shape { get; set; }

        Vector3 Position { get; set; }

        Vector3 Rotation { get; set; }
    }

    public interface IMsbEvent : IMsbEntry { }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
