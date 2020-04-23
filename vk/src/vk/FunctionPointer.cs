using System;
using System.Runtime.InteropServices;

namespace Vulkan
{
    public struct FunctionPointer<TFunc>
    {
        public IntPtr Pointer;

        public FunctionPointer(TFunc func)
        {
            Pointer = Marshal.GetFunctionPointerForDelegate(func);
        }
    }
}
