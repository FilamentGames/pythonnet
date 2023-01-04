using System;
using System.Runtime.InteropServices;

namespace Python.Runtime
{
    /// <summary>
    /// Provides support for calling native code indirectly through
    /// function pointers. Most of the important parts of the Python
    /// C API can just be wrapped with p/invoke, but there are some
    /// situations (specifically, calling functions through Python
    /// type structures) where we need to call functions indirectly.
    /// </summary>
    internal unsafe class NativeCall
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void DeallocFunc(StolenReference obj);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate NewReference Call3Func(BorrowedReference a1, BorrowedReference a2, BorrowedReference a3);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int IntCall3Func(BorrowedReference a1, BorrowedReference a2, BorrowedReference a3);

        public static void CallDealloc(IntPtr fp, StolenReference a1)
        {
            var d = Marshal.GetDelegateForFunctionPointer<DeallocFunc>(fp);
            d(a1.AnalyzerWorkaround());
        }

        public static NewReference Call_3(IntPtr fp, BorrowedReference a1, BorrowedReference a2, BorrowedReference a3)
        {
            var d = Marshal.GetDelegateForFunctionPointer<Call3Func>(fp);
            return d(a1, a2, a3);
        }


        public static int Int_Call_3(IntPtr fp, BorrowedReference a1, BorrowedReference a2, BorrowedReference a3)
        {
            var d = Marshal.GetDelegateForFunctionPointer<IntCall3Func>(fp);
            return d(a1, a2, a3);
        }
    }
}
