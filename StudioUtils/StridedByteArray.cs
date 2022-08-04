namespace StudioUtils;

using System.Runtime.InteropServices;

/// <summary>
/// An unstructured array backed by a byte array and has a specified stride for the array
/// </summary>
public class StridedByteArray
{
    public uint Capacity { get; private set; }
    public uint Count { get; private set; }
    public uint Stride { get; private set; }

    public bool BigEndian { get; private set; }
    
    private byte[] _backing;

    private List<uint> _freeEntries = new List<uint>();

    public StridedByteArray(uint initialCapacity, uint stride, bool bigEndian=false)
    {
        Capacity = initialCapacity;
        Count = 0;
        Stride = stride;
        _backing = new byte[Capacity * Stride];
        BigEndian = bigEndian;
    }

    public StridedByteArray(byte[] backing, uint stride, bool bigEndian=false)
    {
        if (backing.Length % (int)stride != 0)
            throw new ArgumentException();

        _backing = backing;
        Stride = stride;
        Capacity = (uint)backing.Length / stride;
        Count = Capacity;
        BigEndian = bigEndian;
    }

    private void GrowIfNeeded()
    {
        if (Count <= Capacity) return;
        
        while (Capacity < Count)
            Capacity += Capacity / 2;
        
        Array.Resize(ref _backing, (int)Capacity * (int)Stride);
    }

    private static void EndianSwap(Span<byte> swap)
    {
        if ((swap.Length == 0) || ((swap.Length & (swap.Length - 1)) != 0))
            throw new ArgumentException();
        
        swap.Reverse();
    }
    
    /// <summary>
    /// Adds a new element that is zeroed out
    /// </summary>
    /// <returns>The index of the new element</returns>
    public uint AddZeroedElement()
    {
        if (_freeEntries.Count > 0)
        {
            var index = _freeEntries[^1];
            _freeEntries.RemoveAt(_freeEntries.Count - 1);
            return index;
        }
        Count++;
        GrowIfNeeded();
        return Count - 1;
    }

    public void RemoveAt(uint index)
    {
        if (index >= Count)
            throw new IndexOutOfRangeException();
        
        // If we're not deleting the last index, mark it as empty and able to be allocated
        if (index < Count - 1)
        {
            _freeEntries.Add(index);
        }

        // Clear the element
        Array.Clear(_backing, (int)(index + 1) * (int)Stride, (int)Stride);

        Count -= 1;
    }

    /// <summary>
    /// Inserts a new element at an index, zeroes it out, and returns the index
    /// </summary>
    /// <param name="index"></param>
    /// <returns>The index of the new element</returns>
    /*public uint InsertAt(uint index)
    {
        if (index > Count)
            throw new IndexOutOfRangeException();

        // Grow array if needed
        Count += 1;
        GrowIfNeeded();
        
        // Move memory to clear out space at the index
        if (index <= Count)
            Array.Copy(_backing, (int)index * (int)Stride, _backing, (int)(index + 1) * (int)Stride, Stride);
        
        // Clear the space out
        Array.Clear(_backing, (int)index * (int)Stride, (int)Stride);

        return index;
    }*/

    /// <summary>
    /// Reads an element interpreted as a specific type at a specific offset of an element array
    /// </summary>
    /// <param name="element">The element to read</param>
    /// <param name="offset">The byte offset to get the data from</param>
    /// <typeparam name="T">The type that is being read</typeparam>
    /// <returns>The read element</returns>
    /// <exception cref="IndexOutOfRangeException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public unsafe T ReadValueAtElementOffset<T>(uint element, uint offset) where T : struct
    {
        if (element >= Count)
            throw new IndexOutOfRangeException();

        if (offset + (uint)Marshal.SizeOf<T>() > Stride)
            throw new ArgumentOutOfRangeException();

        T result;
        fixed (byte* p = &_backing[(int)element * (int)Stride + (int)offset])
        {
            if (BigEndian)
            {
                Span<byte> data = new Span<byte>(p, Marshal.SizeOf<T>());
                Span<byte> swap = stackalloc byte[Marshal.SizeOf<T>()];
                data.CopyTo(swap);
                EndianSwap(swap);
                fixed (byte* p2 = swap)
                    result = Marshal.PtrToStructure<T>(new IntPtr(p2));
            }
            else
            {
                result = Marshal.PtrToStructure<T>(new IntPtr(p));
            }
        }

        return result;
    }

    /// <summary>
    /// Writesx an element interpreted as a specific type at a specific offset of an element array
    /// </summary>
    /// <param name="element">The element to read</param>
    /// <param name="offset">The byte offset to get the data from</param>
    /// <param name="value">The value to write</param>
    /// <typeparam name="T">The type that is being read</typeparam>
    /// <exception cref="IndexOutOfRangeException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public unsafe void WriteValueAtElementOffset<T>(uint element, uint offset, T value) where T : struct
    {
        if (element >= Count)
            throw new IndexOutOfRangeException();

        if (offset + (uint)Marshal.SizeOf<T>() > Stride)
            throw new ArgumentOutOfRangeException();
        
        fixed (byte* p = &_backing[(int)element * (int)Stride + (int)offset])
        {
            Marshal.StructureToPtr<T>(value, new IntPtr(p), true);
            if (BigEndian)
            {
                // If big endian mode we need to byte swap the written data
                Span<byte> data = new Span<byte>(p, Marshal.SizeOf<T>());
                EndianSwap(data);
            }
        }
    }
}