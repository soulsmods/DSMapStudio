using System;
using System.Collections.Generic;
using System.Text;

namespace StudioCore.Memory
{
    public sealed class ArrayRangeAllocator<T> where T : struct
    {
        private T[] _backing;
        private FreeListAllocator _allocator;

        public ArrayRangeAllocator(int capacity)
        {
            _allocator = new FreeListAllocator((uint)capacity);
            _backing = new T[capacity];
        }

        public void Resize(int newsize)
        {
            if (newsize <= _backing.Length)
            {
                throw new ArgumentException("Only support growing");
            }
            var nback = new T[newsize];
            Array.Copy(_backing, nback, _backing.Length);
            _backing = nback;
            _allocator.Resize((uint)newsize);
        }

        public int Allocate(int size)
        {
            uint addr;
            while (!_allocator.AlignedAlloc((uint)size, 1, out addr))
            {
                Resize(_backing.Length + _backing.Length / 2);
            }
            return (int)addr;
        }

        public void Free(int handle)
        {
            _allocator.Free((uint)handle);
        }

        public Span<T> GetSpan(int handle, int size)
        {
            return new Span<T>(_backing, handle, size);
        }
    }
}
