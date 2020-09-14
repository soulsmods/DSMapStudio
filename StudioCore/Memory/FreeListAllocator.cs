using System;
using System.Collections.Generic;
using System.Text;

namespace StudioCore.Memory
{
    /// <summary>
    /// Simple and dumb free list allocator without memory backing. Used for megabuffer
    /// allocations. This is pretty slow and not smart at all, but it should work just fine
    /// for the use cases of it (GPU side megabuffer suballocations)
    /// </summary>
    public class FreeListAllocator
    {
        private class Block
        {
            public uint _addr;
            public bool _free;
            public uint _size;
            public LinkedListNode<Block> _node;
        }

        private LinkedList<Block> _blocks;
        private LinkedList<Block> _freeBlocks;

        private Dictionary<uint, LinkedListNode<Block>> _allocations;

        private uint _capacity;

        public FreeListAllocator(uint capacity)
        {
            _capacity = capacity;
            _blocks = new LinkedList<Block>();
            _allocations = new Dictionary<uint, LinkedListNode<Block>>();
            _freeBlocks = new LinkedList<Block>();

            Block initial = new Block();
            initial._free = true;
            initial._size = capacity;
            initial._addr = 0;
            initial._node = _blocks.AddLast(initial);
            _freeBlocks.AddLast(initial);
        }

        public bool AlignedAlloc(uint size, uint align, out uint addr)
        {
            var curr = _freeBlocks.First;
            while (curr != null)
            {
                var n = curr.Value;
                if (n._free && n._size >= size)
                {
                    uint alignadj = 0;
                    if ((n._addr % align) != 0)
                    {
                        alignadj = (align - (n._addr % align));
                        if (n._size < size + alignadj)
                        {
                            curr = curr.Next;
                            continue;
                        }
                        // Split the block such that it starts on the aligned boundary
                        var alnblk = new Block();
                        alnblk._free = true;
                        alnblk._size = alignadj;
                        alnblk._addr = n._addr;
                        alnblk._node = _blocks.AddBefore(n._node, alnblk);
                        n._size -= alignadj;
                        n._addr += alignadj;
                        _freeBlocks.AddLast(alnblk);
                    }

                    // This block fits the allocation. Mark it used and add a new free block on top
                    if (n._size > size)
                    {
                        var tfree = new Block();
                        tfree._free = true;
                        tfree._size = n._size - size;
                        tfree._addr = n._addr + size;
                        tfree._node = _blocks.AddAfter(n._node, tfree);
                        n._size = size;
                        _freeBlocks.AddLast(tfree);
                    }
                    n._free = false;
                    _freeBlocks.Remove(curr);
                    _allocations.Add(n._addr, n._node);
                    addr = n._addr;
                    return true;
                }
                curr = curr.Next;
            }

            addr = 0;
            return false;
        }

        public void Free(uint addr)
        {
            // Just mark the node free and merge it with above and below nodes if they're free
            var n = _allocations[addr];
            var b = n.Value;
            b._free = true;
            var prev = n.Previous;
            var next = n.Next;
            bool addToFreeList = true;
            Block maybeRemove = null;
            if (prev != null && prev.Value._free)
            {
                //b._addr = prev.Value._addr;
                //b._size += prev.Value._size;
                //_blocks.Remove(prev);
                prev.Value._size += b._size;
                _blocks.Remove(n);
                n = prev;
                b = n.Value;
                maybeRemove = b;
                addToFreeList = false;
            }
            if (next != null && next.Value._free)
            {
                //b._size += next.Value._size;
                //_blocks.Remove(next);
                next.Value._addr = b._addr;
                next.Value._size += b._size;
                _blocks.Remove(n);
                addToFreeList = false;
                if (maybeRemove != null)
                {
                    _freeBlocks.Remove(maybeRemove);
                }
            }
            if (addToFreeList)
            {
                _freeBlocks.AddLast(b);
            }
            _allocations.Remove(addr);
        }

        /// <summary>
        /// Compacts the buffer by trimming the upper free block off
        /// </summary>
        /// <returns>The new size</returns>
        public uint CompactTop()
        {
            var top = _blocks.Last;
            if (top.Value._free)
            {
                _capacity -= top.Value._size;
                _blocks.RemoveLast();
                return _capacity;
            }
            return _capacity;
        }

        public bool HasAllocations()
        {
            return _allocations.Count > 0;
        }
    }
}
