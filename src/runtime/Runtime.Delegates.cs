using System;
using System.Runtime.InteropServices;

using Python.Runtime.Native;
using Python.Runtime.Platform;

namespace Python.Runtime;

public unsafe partial class Runtime
{
    internal static class Delegates
    {
        static readonly ILibraryLoader libraryLoader = LibraryLoader.Instance;

        static Delegates()
        {
            Py_IncRef = GetDelegateByName<BorrowedReferenceAction>(nameof(Py_IncRef), GetUnmanagedDll(_PythonDll));
            Py_DecRef = GetDelegateByName<StolenReferenceAction>(nameof(Py_DecRef), GetUnmanagedDll(_PythonDll));
            Py_Initialize = GetDelegateByName<Action>(nameof(Py_Initialize), GetUnmanagedDll(_PythonDll));
            Py_InitializeEx = GetDelegateByName<IntAction>(nameof(Py_InitializeEx), GetUnmanagedDll(_PythonDll));
            Py_IsInitialized = GetDelegateByName<IntFunc>(nameof(Py_IsInitialized), GetUnmanagedDll(_PythonDll));
            Py_Finalize = GetDelegateByName<Action>(nameof(Py_Finalize), GetUnmanagedDll(_PythonDll));
            Py_NewInterpreter = (PyThreadStateFunc)GetFunctionByName(nameof(Py_NewInterpreter), GetUnmanagedDll(_PythonDll));
            Py_EndInterpreter = (PyThreadStatevoidFunc)GetFunctionByName(nameof(Py_EndInterpreter), GetUnmanagedDll(_PythonDll));
            PyThreadState_New = (PyInterpreterStatePyThreadStateFunc)GetFunctionByName(nameof(PyThreadState_New), GetUnmanagedDll(_PythonDll));
            PyThreadState_Get = (PyThreadStateFunc)GetFunctionByName(nameof(PyThreadState_Get), GetUnmanagedDll(_PythonDll));
            _PyThreadState_UncheckedGet = (PyThreadStateFunc)GetFunctionByName(nameof(_PyThreadState_UncheckedGet), GetUnmanagedDll(_PythonDll));
            try
            {
                PyGILState_Check = GetDelegateByName<IntFunc>(nameof(PyGILState_Check), GetUnmanagedDll(_PythonDll));
            }
            catch (MissingMethodException e)
            {
                throw new NotSupportedException(Util.MinimalPythonVersionRequired, innerException: e);
            }
            PyGILState_Ensure = (PyGILStateFunc)GetFunctionByName(nameof(PyGILState_Ensure), GetUnmanagedDll(_PythonDll));
            PyGILState_Release = (PyGILStatevoidFunc)GetFunctionByName(nameof(PyGILState_Release), GetUnmanagedDll(_PythonDll));
            PyGILState_GetThisThreadState = (PyThreadStateFunc)GetFunctionByName(nameof(PyGILState_GetThisThreadState), GetUnmanagedDll(_PythonDll));
            Py_Main = (intIntPtrintFunc)GetFunctionByName(nameof(Py_Main), GetUnmanagedDll(_PythonDll));
            PyEval_InitThreads = GetDelegateByName<Action>(nameof(PyEval_InitThreads), GetUnmanagedDll(_PythonDll));
            PyEval_ThreadsInitialized = GetDelegateByName<IntFunc>(nameof(PyEval_ThreadsInitialized), GetUnmanagedDll(_PythonDll));
            PyEval_AcquireLock = GetDelegateByName<Action>(nameof(PyEval_AcquireLock), GetUnmanagedDll(_PythonDll));
            PyEval_ReleaseLock = GetDelegateByName<Action>(nameof(PyEval_ReleaseLock), GetUnmanagedDll(_PythonDll));
            PyEval_AcquireThread = (PyThreadStatevoidFunc)GetFunctionByName(nameof(PyEval_AcquireThread), GetUnmanagedDll(_PythonDll));
            PyEval_ReleaseThread = (PyThreadStatevoidFunc)GetFunctionByName(nameof(PyEval_ReleaseThread), GetUnmanagedDll(_PythonDll));
            PyEval_SaveThread = (PyThreadStateFunc)GetFunctionByName(nameof(PyEval_SaveThread), GetUnmanagedDll(_PythonDll));
            PyEval_RestoreThread = (PyThreadStatevoidFunc)GetFunctionByName(nameof(PyEval_RestoreThread), GetUnmanagedDll(_PythonDll));
            PyEval_GetBuiltins = (BorrowedReferenceFunc)GetFunctionByName(nameof(PyEval_GetBuiltins), GetUnmanagedDll(_PythonDll));
            PyEval_GetGlobals = (BorrowedReferenceFunc)GetFunctionByName(nameof(PyEval_GetGlobals), GetUnmanagedDll(_PythonDll));
            PyEval_GetLocals = (BorrowedReferenceFunc)GetFunctionByName(nameof(PyEval_GetLocals), GetUnmanagedDll(_PythonDll));
            Py_GetProgramName = GetDelegateByName<IntPtrFunc>(nameof(Py_GetProgramName), GetUnmanagedDll(_PythonDll));
            Py_SetProgramName = (IntPtrvoidFunc)GetFunctionByName(nameof(Py_SetProgramName), GetUnmanagedDll(_PythonDll));
            Py_GetPythonHome = GetDelegateByName<IntPtrFunc>(nameof(Py_GetPythonHome), GetUnmanagedDll(_PythonDll));
            Py_SetPythonHome = (IntPtrvoidFunc)GetFunctionByName(nameof(Py_SetPythonHome), GetUnmanagedDll(_PythonDll));
            Py_GetPath = GetDelegateByName<IntPtrFunc>(nameof(Py_GetPath), GetUnmanagedDll(_PythonDll));
            Py_SetPath = (IntPtrvoidFunc)GetFunctionByName(nameof(Py_SetPath), GetUnmanagedDll(_PythonDll));
            Py_GetVersion = GetDelegateByName<IntPtrFunc>(nameof(Py_GetVersion), GetUnmanagedDll(_PythonDll));
            Py_GetPlatform = GetDelegateByName<IntPtrFunc>(nameof(Py_GetPlatform), GetUnmanagedDll(_PythonDll));
            Py_GetCopyright = GetDelegateByName<IntPtrFunc>(nameof(Py_GetCopyright), GetUnmanagedDll(_PythonDll));
            Py_GetCompiler = GetDelegateByName<IntPtrFunc>(nameof(Py_GetCompiler), GetUnmanagedDll(_PythonDll));
            Py_GetBuildInfo = GetDelegateByName<IntPtrFunc>(nameof(Py_GetBuildInfo), GetUnmanagedDll(_PythonDll));
            PyRun_SimpleStringFlags = (StrPtrPyCompilerFlagsintFunc)GetFunctionByName(nameof(PyRun_SimpleStringFlags), GetUnmanagedDll(_PythonDll));
            PyRun_StringFlags = (delegate* unmanaged[Cdecl]<StrPtr, RunFlagType, BorrowedReference, BorrowedReference, in PyCompilerFlags, NewReference>)GetFunctionByName(nameof(PyRun_StringFlags), GetUnmanagedDll(_PythonDll));
            PyEval_EvalCode = (BorrowedReferenceBorrowedReferenceBorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyEval_EvalCode), GetUnmanagedDll(_PythonDll));
            Py_CompileStringObject = (delegate* unmanaged[Cdecl]<StrPtr, BorrowedReference, int, in PyCompilerFlags, int, NewReference>)GetFunctionByName(nameof(Py_CompileStringObject), GetUnmanagedDll(_PythonDll));
            PyImport_ExecCodeModule = (StrPtrBorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyImport_ExecCodeModule), GetUnmanagedDll(_PythonDll));
            PyObject_HasAttrString = (BorrowedReferenceStrPtrintFunc)GetFunctionByName(nameof(PyObject_HasAttrString), GetUnmanagedDll(_PythonDll));
            PyObject_GetAttrString = (BorrowedReferenceStrPtrNewReferenceFunc)GetFunctionByName(nameof(PyObject_GetAttrString), GetUnmanagedDll(_PythonDll));
            PyObject_SetAttrString = (BorrowedReferenceStrPtrBorrowedReferenceintFunc)GetFunctionByName(nameof(PyObject_SetAttrString), GetUnmanagedDll(_PythonDll));
            PyObject_HasAttr = (BorrowedReferenceBorrowedReferenceintFunc)GetFunctionByName(nameof(PyObject_HasAttr), GetUnmanagedDll(_PythonDll));
            PyObject_GetAttr = (BorrowedReferenceBorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyObject_GetAttr), GetUnmanagedDll(_PythonDll));
            PyObject_SetAttr = (BorrowedReferenceBorrowedReferenceBorrowedReferenceintFunc)GetFunctionByName(nameof(PyObject_SetAttr), GetUnmanagedDll(_PythonDll));
            PyObject_GetItem = (BorrowedReferenceBorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyObject_GetItem), GetUnmanagedDll(_PythonDll));
            PyObject_SetItem = (BorrowedReferenceBorrowedReferenceBorrowedReferenceintFunc)GetFunctionByName(nameof(PyObject_SetItem), GetUnmanagedDll(_PythonDll));
            PyObject_DelItem = (BorrowedReferenceBorrowedReferenceintFunc)GetFunctionByName(nameof(PyObject_DelItem), GetUnmanagedDll(_PythonDll));
            PyObject_GetIter = (BorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyObject_GetIter), GetUnmanagedDll(_PythonDll));
            PyObject_Call = (BorrowedReferenceBorrowedReferenceBorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyObject_Call), GetUnmanagedDll(_PythonDll));
            PyObject_CallObject = (BorrowedReferenceBorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyObject_CallObject), GetUnmanagedDll(_PythonDll));
            PyObject_RichCompareBool = (BorrowedReferenceBorrowedReferenceintintFunc)GetFunctionByName(nameof(PyObject_RichCompareBool), GetUnmanagedDll(_PythonDll));
            PyObject_IsInstance = (BorrowedReferenceBorrowedReferenceintFunc)GetFunctionByName(nameof(PyObject_IsInstance), GetUnmanagedDll(_PythonDll));
            PyObject_IsSubclass = (BorrowedReferenceBorrowedReferenceintFunc)GetFunctionByName(nameof(PyObject_IsSubclass), GetUnmanagedDll(_PythonDll));
            PyObject_ClearWeakRefs = (BorrowedReferencevoidFunc)GetFunctionByName(nameof(PyObject_ClearWeakRefs), GetUnmanagedDll(_PythonDll));
            PyCallable_Check = (BorrowedReferenceintFunc)GetFunctionByName(nameof(PyCallable_Check), GetUnmanagedDll(_PythonDll));
            PyObject_IsTrue = (BorrowedReferenceintFunc)GetFunctionByName(nameof(PyObject_IsTrue), GetUnmanagedDll(_PythonDll));
            PyObject_Not = (BorrowedReferenceintFunc)GetFunctionByName(nameof(PyObject_Not), GetUnmanagedDll(_PythonDll));
            PyObject_Size = (BorrowedReferencenintFunc)GetFunctionByName("PyObject_Size", GetUnmanagedDll(_PythonDll));
            PyObject_Hash = (BorrowedReferencenintFunc)GetFunctionByName(nameof(PyObject_Hash), GetUnmanagedDll(_PythonDll));
            PyObject_Repr = (BorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyObject_Repr), GetUnmanagedDll(_PythonDll));
            PyObject_Str = (BorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyObject_Str), GetUnmanagedDll(_PythonDll));
            PyObject_Type = (BorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyObject_Type), GetUnmanagedDll(_PythonDll));
            PyObject_Dir = (BorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyObject_Dir), GetUnmanagedDll(_PythonDll));
            PyObject_GetBuffer = (BorrowedReferencePy_bufferintintFunc)GetFunctionByName(nameof(PyObject_GetBuffer), GetUnmanagedDll(_PythonDll));
            PyBuffer_Release = (Py_buffervoidFunc)GetFunctionByName(nameof(PyBuffer_Release), GetUnmanagedDll(_PythonDll));
            try
            {
                PyBuffer_SizeFromFormat = (StrPtrIntPtrFunc)GetFunctionByName(nameof(PyBuffer_SizeFromFormat), GetUnmanagedDll(_PythonDll));
            }
            catch (MissingMethodException)
            {
                // only in 3.9+
            }
            PyBuffer_IsContiguous = (Py_buffercharintFunc)GetFunctionByName(nameof(PyBuffer_IsContiguous), GetUnmanagedDll(_PythonDll));
            PyBuffer_GetPointer = (delegate* unmanaged[Cdecl]<ref Py_buffer, nint[], IntPtr>)GetFunctionByName(nameof(PyBuffer_GetPointer), GetUnmanagedDll(_PythonDll));
            PyBuffer_FromContiguous = (delegate* unmanaged[Cdecl]<ref Py_buffer, IntPtr, IntPtr, char, int>)GetFunctionByName(nameof(PyBuffer_FromContiguous), GetUnmanagedDll(_PythonDll));
            PyBuffer_ToContiguous = (delegate* unmanaged[Cdecl]<IntPtr, ref Py_buffer, IntPtr, char, int>)GetFunctionByName(nameof(PyBuffer_ToContiguous), GetUnmanagedDll(_PythonDll));
            PyBuffer_FillContiguousStrides = (delegate* unmanaged[Cdecl]<int, IntPtr, IntPtr, int, char, void>)GetFunctionByName(nameof(PyBuffer_FillContiguousStrides), GetUnmanagedDll(_PythonDll));
            PyBuffer_FillInfo = (delegate* unmanaged[Cdecl]<ref Py_buffer, BorrowedReference, IntPtr, IntPtr, int, int, int>)GetFunctionByName(nameof(PyBuffer_FillInfo), GetUnmanagedDll(_PythonDll));
            PyNumber_Long = (BorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyNumber_Long), GetUnmanagedDll(_PythonDll));
            PyNumber_Float = (BorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyNumber_Float), GetUnmanagedDll(_PythonDll));
            PyNumber_Check = (BorrowedReferenceboolFunc)GetFunctionByName(nameof(PyNumber_Check), GetUnmanagedDll(_PythonDll));
            PyLong_FromLongLong = (longNewReferenceFunc)GetFunctionByName(nameof(PyLong_FromLongLong), GetUnmanagedDll(_PythonDll));
            PyLong_FromUnsignedLongLong = (ulongNewReferenceFunc)GetFunctionByName(nameof(PyLong_FromUnsignedLongLong), GetUnmanagedDll(_PythonDll));
            PyLong_FromString = (StrPtrIntPtrintNewReferenceFunc)GetFunctionByName(nameof(PyLong_FromString), GetUnmanagedDll(_PythonDll));
            PyLong_AsLongLong = (BorrowedReferencelongFunc)GetFunctionByName(nameof(PyLong_AsLongLong), GetUnmanagedDll(_PythonDll));
            PyLong_AsUnsignedLongLong = (BorrowedReferenceulongFunc)GetFunctionByName(nameof(PyLong_AsUnsignedLongLong), GetUnmanagedDll(_PythonDll));
            PyLong_FromVoidPtr = (IntPtrNewReferenceFunc)GetFunctionByName(nameof(PyLong_FromVoidPtr), GetUnmanagedDll(_PythonDll));
            PyLong_AsVoidPtr = (BorrowedReferenceIntPtrFunc)GetFunctionByName(nameof(PyLong_AsVoidPtr), GetUnmanagedDll(_PythonDll));
            PyFloat_FromDouble = (doubleNewReferenceFunc)GetFunctionByName(nameof(PyFloat_FromDouble), GetUnmanagedDll(_PythonDll));
            PyFloat_FromString = (BorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyFloat_FromString), GetUnmanagedDll(_PythonDll));
            PyFloat_AsDouble = (BorrowedReferencedoubleFunc)GetFunctionByName(nameof(PyFloat_AsDouble), GetUnmanagedDll(_PythonDll));
            PyNumber_Add = (BorrowedReferenceBorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyNumber_Add), GetUnmanagedDll(_PythonDll));
            PyNumber_Subtract = (BorrowedReferenceBorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyNumber_Subtract), GetUnmanagedDll(_PythonDll));
            PyNumber_Multiply = (BorrowedReferenceBorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyNumber_Multiply), GetUnmanagedDll(_PythonDll));
            PyNumber_TrueDivide = (BorrowedReferenceBorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyNumber_TrueDivide), GetUnmanagedDll(_PythonDll));
            PyNumber_And = (BorrowedReferenceBorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyNumber_And), GetUnmanagedDll(_PythonDll));
            PyNumber_Xor = (BorrowedReferenceBorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyNumber_Xor), GetUnmanagedDll(_PythonDll));
            PyNumber_Or = (BorrowedReferenceBorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyNumber_Or), GetUnmanagedDll(_PythonDll));
            PyNumber_Lshift = (BorrowedReferenceBorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyNumber_Lshift), GetUnmanagedDll(_PythonDll));
            PyNumber_Rshift = (BorrowedReferenceBorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyNumber_Rshift), GetUnmanagedDll(_PythonDll));
            PyNumber_Power = (BorrowedReferenceBorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyNumber_Power), GetUnmanagedDll(_PythonDll));
            PyNumber_Remainder = (BorrowedReferenceBorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyNumber_Remainder), GetUnmanagedDll(_PythonDll));
            PyNumber_InPlaceAdd = (BorrowedReferenceBorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyNumber_InPlaceAdd), GetUnmanagedDll(_PythonDll));
            PyNumber_InPlaceSubtract = (BorrowedReferenceBorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyNumber_InPlaceSubtract), GetUnmanagedDll(_PythonDll));
            PyNumber_InPlaceMultiply = (BorrowedReferenceBorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyNumber_InPlaceMultiply), GetUnmanagedDll(_PythonDll));
            PyNumber_InPlaceTrueDivide = (BorrowedReferenceBorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyNumber_InPlaceTrueDivide), GetUnmanagedDll(_PythonDll));
            PyNumber_InPlaceAnd = (BorrowedReferenceBorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyNumber_InPlaceAnd), GetUnmanagedDll(_PythonDll));
            PyNumber_InPlaceXor = (BorrowedReferenceBorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyNumber_InPlaceXor), GetUnmanagedDll(_PythonDll));
            PyNumber_InPlaceOr = (BorrowedReferenceBorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyNumber_InPlaceOr), GetUnmanagedDll(_PythonDll));
            PyNumber_InPlaceLshift = (BorrowedReferenceBorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyNumber_InPlaceLshift), GetUnmanagedDll(_PythonDll));
            PyNumber_InPlaceRshift = (BorrowedReferenceBorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyNumber_InPlaceRshift), GetUnmanagedDll(_PythonDll));
            PyNumber_InPlacePower = (BorrowedReferenceBorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyNumber_InPlacePower), GetUnmanagedDll(_PythonDll));
            PyNumber_InPlaceRemainder = (BorrowedReferenceBorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyNumber_InPlaceRemainder), GetUnmanagedDll(_PythonDll));
            PyNumber_Negative = (BorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyNumber_Negative), GetUnmanagedDll(_PythonDll));
            PyNumber_Positive = (BorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyNumber_Positive), GetUnmanagedDll(_PythonDll));
            PyNumber_Invert = (BorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyNumber_Invert), GetUnmanagedDll(_PythonDll));
            PySequence_Check = (BorrowedReferenceboolFunc)GetFunctionByName(nameof(PySequence_Check), GetUnmanagedDll(_PythonDll));
            PySequence_GetItem = (BorrowedReferencenintNewReferenceFunc)GetFunctionByName(nameof(PySequence_GetItem), GetUnmanagedDll(_PythonDll));
            PySequence_SetItem = (BorrowedReferencenintBorrowedReferenceintFunc)GetFunctionByName(nameof(PySequence_SetItem), GetUnmanagedDll(_PythonDll));
            PySequence_DelItem = (BorrowedReferencenintintFunc)GetFunctionByName(nameof(PySequence_DelItem), GetUnmanagedDll(_PythonDll));
            PySequence_GetSlice = (BorrowedReferencenintnintNewReferenceFunc)GetFunctionByName(nameof(PySequence_GetSlice), GetUnmanagedDll(_PythonDll));
            PySequence_SetSlice = (delegate* unmanaged[Cdecl]<BorrowedReference, nint, nint, BorrowedReference, int>)GetFunctionByName(nameof(PySequence_SetSlice), GetUnmanagedDll(_PythonDll));
            PySequence_DelSlice = (BorrowedReferencenintnintintFunc)GetFunctionByName(nameof(PySequence_DelSlice), GetUnmanagedDll(_PythonDll));
            PySequence_Size = (BorrowedReferencenintFunc)GetFunctionByName(nameof(PySequence_Size), GetUnmanagedDll(_PythonDll));
            PySequence_Contains = (BorrowedReferenceBorrowedReferenceintFunc)GetFunctionByName(nameof(PySequence_Contains), GetUnmanagedDll(_PythonDll));
            PySequence_Concat = (BorrowedReferenceBorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PySequence_Concat), GetUnmanagedDll(_PythonDll));
            PySequence_Repeat = (BorrowedReferencenintNewReferenceFunc)GetFunctionByName(nameof(PySequence_Repeat), GetUnmanagedDll(_PythonDll));
            PySequence_Index = (BorrowedReferenceBorrowedReferencenintFunc)GetFunctionByName(nameof(PySequence_Index), GetUnmanagedDll(_PythonDll));
            PySequence_Count = (BorrowedReferenceBorrowedReferencenintFunc)GetFunctionByName(nameof(PySequence_Count), GetUnmanagedDll(_PythonDll));
            PySequence_Tuple = (BorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PySequence_Tuple), GetUnmanagedDll(_PythonDll));
            PySequence_List = (BorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PySequence_List), GetUnmanagedDll(_PythonDll));
            PyBytes_AsString = (BorrowedReferenceIntPtrFunc)GetFunctionByName(nameof(PyBytes_AsString), GetUnmanagedDll(_PythonDll));
            PyBytes_FromString = (IntPtrNewReferenceFunc)GetFunctionByName(nameof(PyBytes_FromString), GetUnmanagedDll(_PythonDll));
            PyByteArray_FromStringAndSize = (IntPtrnintNewReferenceFunc)GetFunctionByName(nameof(PyByteArray_FromStringAndSize), GetUnmanagedDll(_PythonDll));
            PyBytes_Size = (BorrowedReferencenintFunc)GetFunctionByName(nameof(PyBytes_Size), GetUnmanagedDll(_PythonDll));
            PyUnicode_AsUTF8 = (BorrowedReferenceIntPtrFunc)GetFunctionByName(nameof(PyUnicode_AsUTF8), GetUnmanagedDll(_PythonDll));
            PyUnicode_DecodeUTF16 = (delegate* unmanaged[Cdecl]<IntPtr, nint, IntPtr, IntPtr, NewReference>)GetFunctionByName(nameof(PyUnicode_DecodeUTF16), GetUnmanagedDll(_PythonDll));
            PyUnicode_GetLength = (BorrowedReferencenintFunc)GetFunctionByName(nameof(PyUnicode_GetLength), GetUnmanagedDll(_PythonDll));
            PyUnicode_AsUnicode = (BorrowedReferenceIntPtrFunc)GetFunctionByName(nameof(PyUnicode_AsUnicode), GetUnmanagedDll(_PythonDll));
            PyUnicode_AsUTF16String = (BorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyUnicode_AsUTF16String), GetUnmanagedDll(_PythonDll));
            PyUnicode_FromOrdinal = (intNewReferenceFunc)GetFunctionByName(nameof(PyUnicode_FromOrdinal), GetUnmanagedDll(_PythonDll));
            PyUnicode_InternFromString = (StrPtrNewReferenceFunc)GetFunctionByName(nameof(PyUnicode_InternFromString), GetUnmanagedDll(_PythonDll));
            PyUnicode_Compare = (BorrowedReferenceBorrowedReferenceintFunc)GetFunctionByName(nameof(PyUnicode_Compare), GetUnmanagedDll(_PythonDll));
            PyDict_New = (NewReferenceFunc)GetFunctionByName(nameof(PyDict_New), GetUnmanagedDll(_PythonDll));
            PyDict_GetItem = (BorrowedReferenceBorrowedReferenceBorrowedReferenceFunc)GetFunctionByName(nameof(PyDict_GetItem), GetUnmanagedDll(_PythonDll));
            PyDict_GetItemString = (BorrowedReferenceStrPtrBorrowedReferenceFunc)GetFunctionByName(nameof(PyDict_GetItemString), GetUnmanagedDll(_PythonDll));
            PyDict_SetItem = (BorrowedReferenceBorrowedReferenceBorrowedReferenceintFunc)GetFunctionByName(nameof(PyDict_SetItem), GetUnmanagedDll(_PythonDll));
            PyDict_SetItemString = (BorrowedReferenceStrPtrBorrowedReferenceintFunc)GetFunctionByName(nameof(PyDict_SetItemString), GetUnmanagedDll(_PythonDll));
            PyDict_DelItem = (BorrowedReferenceBorrowedReferenceintFunc)GetFunctionByName(nameof(PyDict_DelItem), GetUnmanagedDll(_PythonDll));
            PyDict_DelItemString = (BorrowedReferenceStrPtrintFunc)GetFunctionByName(nameof(PyDict_DelItemString), GetUnmanagedDll(_PythonDll));
            PyMapping_HasKey = (BorrowedReferenceBorrowedReferenceintFunc)GetFunctionByName(nameof(PyMapping_HasKey), GetUnmanagedDll(_PythonDll));
            PyDict_Keys = (BorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyDict_Keys), GetUnmanagedDll(_PythonDll));
            PyDict_Values = (BorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyDict_Values), GetUnmanagedDll(_PythonDll));
            PyDict_Items = (BorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyDict_Items), GetUnmanagedDll(_PythonDll));
            PyDict_Copy = (BorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyDict_Copy), GetUnmanagedDll(_PythonDll));
            PyDict_Update = (BorrowedReferenceBorrowedReferenceintFunc)GetFunctionByName(nameof(PyDict_Update), GetUnmanagedDll(_PythonDll));
            PyDict_Clear = (BorrowedReferencevoidFunc)GetFunctionByName(nameof(PyDict_Clear), GetUnmanagedDll(_PythonDll));
            PyDict_Size = (BorrowedReferencenintFunc)GetFunctionByName(nameof(PyDict_Size), GetUnmanagedDll(_PythonDll));
            PySet_New = (BorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PySet_New), GetUnmanagedDll(_PythonDll));
            PySet_Add = (BorrowedReferenceBorrowedReferenceintFunc)GetFunctionByName(nameof(PySet_Add), GetUnmanagedDll(_PythonDll));
            PySet_Contains = (BorrowedReferenceBorrowedReferenceintFunc)GetFunctionByName(nameof(PySet_Contains), GetUnmanagedDll(_PythonDll));
            PyList_New = (nintNewReferenceFunc)GetFunctionByName(nameof(PyList_New), GetUnmanagedDll(_PythonDll));
            PyList_GetItem = (BorrowedReferenceIntPtrBorrowedReferenceFunc)GetFunctionByName(nameof(PyList_GetItem), GetUnmanagedDll(_PythonDll));
            PyList_SetItem = (BorrowedReferencenintStolenReferenceintFunc)GetFunctionByName(nameof(PyList_SetItem), GetUnmanagedDll(_PythonDll));
            PyList_Insert = (BorrowedReferencenintBorrowedReferenceintFunc)GetFunctionByName(nameof(PyList_Insert), GetUnmanagedDll(_PythonDll));
            PyList_Append = (BorrowedReferenceBorrowedReferenceintFunc)GetFunctionByName(nameof(PyList_Append), GetUnmanagedDll(_PythonDll));
            PyList_Reverse = (BorrowedReferenceintFunc)GetFunctionByName(nameof(PyList_Reverse), GetUnmanagedDll(_PythonDll));
            PyList_Sort = (BorrowedReferenceintFunc)GetFunctionByName(nameof(PyList_Sort), GetUnmanagedDll(_PythonDll));
            PyList_GetSlice = (BorrowedReferencenintnintNewReferenceFunc)GetFunctionByName(nameof(PyList_GetSlice), GetUnmanagedDll(_PythonDll));
            PyList_SetSlice = (delegate* unmanaged[Cdecl]<BorrowedReference, nint, nint, BorrowedReference, int>)GetFunctionByName(nameof(PyList_SetSlice), GetUnmanagedDll(_PythonDll));
            PyList_Size = (BorrowedReferencenintFunc)GetFunctionByName(nameof(PyList_Size), GetUnmanagedDll(_PythonDll));
            PyTuple_New = (nintNewReferenceFunc)GetFunctionByName(nameof(PyTuple_New), GetUnmanagedDll(_PythonDll));
            PyTuple_GetItem = (BorrowedReferenceIntPtrBorrowedReferenceFunc)GetFunctionByName(nameof(PyTuple_GetItem), GetUnmanagedDll(_PythonDll));
            PyTuple_SetItem = (BorrowedReferencenintStolenReferenceintFunc)GetFunctionByName(nameof(PyTuple_SetItem), GetUnmanagedDll(_PythonDll));
            PyTuple_GetSlice = (BorrowedReferencenintnintNewReferenceFunc)GetFunctionByName(nameof(PyTuple_GetSlice), GetUnmanagedDll(_PythonDll));
            PyTuple_Size = (BorrowedReferenceIntPtrFunc)GetFunctionByName(nameof(PyTuple_Size), GetUnmanagedDll(_PythonDll));
            try
            {
                PyIter_Check = (BorrowedReferenceintFunc)GetFunctionByName(nameof(PyIter_Check), GetUnmanagedDll(_PythonDll));
            }
            catch (MissingMethodException) { }
            PyIter_Next = (BorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyIter_Next), GetUnmanagedDll(_PythonDll));
            PyModule_New = (StrPtrNewReferenceFunc)GetFunctionByName(nameof(PyModule_New), GetUnmanagedDll(_PythonDll));
            PyModule_GetDict = (BorrowedReferenceBorrowedReferenceFunc)GetFunctionByName(nameof(PyModule_GetDict), GetUnmanagedDll(_PythonDll));
            PyModule_AddObject = (BorrowedReferenceStrPtrIntPtrintFunc)GetFunctionByName(nameof(PyModule_AddObject), GetUnmanagedDll(_PythonDll));
            PyImport_Import = (BorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyImport_Import), GetUnmanagedDll(_PythonDll));
            PyImport_ImportModule = (StrPtrNewReferenceFunc)GetFunctionByName(nameof(PyImport_ImportModule), GetUnmanagedDll(_PythonDll));
            PyImport_ReloadModule = (BorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyImport_ReloadModule), GetUnmanagedDll(_PythonDll));
            PyImport_AddModule = (StrPtrBorrowedReferenceFunc)GetFunctionByName(nameof(PyImport_AddModule), GetUnmanagedDll(_PythonDll));
            PyImport_GetModuleDict = (BorrowedReferenceFunc)GetFunctionByName(nameof(PyImport_GetModuleDict), GetUnmanagedDll(_PythonDll));
            PySys_SetArgvEx = (intIntPtrintvoidFunc)GetFunctionByName(nameof(PySys_SetArgvEx), GetUnmanagedDll(_PythonDll));
            PySys_GetObject = (StrPtrBorrowedReferenceFunc)GetFunctionByName(nameof(PySys_GetObject), GetUnmanagedDll(_PythonDll));
            PySys_SetObject = (StrPtrBorrowedReferenceintFunc)GetFunctionByName(nameof(PySys_SetObject), GetUnmanagedDll(_PythonDll));
            PyType_Modified = (BorrowedReferencevoidFunc)GetFunctionByName(nameof(PyType_Modified), GetUnmanagedDll(_PythonDll));
            PyType_IsSubtype = (BorrowedReferenceBorrowedReferenceboolFunc)GetFunctionByName(nameof(PyType_IsSubtype), GetUnmanagedDll(_PythonDll));
            PyType_GenericNew = (BorrowedReferenceBorrowedReferenceBorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyType_GenericNew), GetUnmanagedDll(_PythonDll));
            PyType_GenericAlloc = (BorrowedReferencenintNewReferenceFunc)GetFunctionByName(nameof(PyType_GenericAlloc), GetUnmanagedDll(_PythonDll));
            PyType_Ready = (BorrowedReferenceintFunc)GetFunctionByName(nameof(PyType_Ready), GetUnmanagedDll(_PythonDll));
            _PyType_Lookup = (BorrowedReferenceBorrowedReferenceBorrowedReferenceFunc)GetFunctionByName(nameof(_PyType_Lookup), GetUnmanagedDll(_PythonDll));
            PyObject_GenericGetAttr = (BorrowedReferenceBorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyObject_GenericGetAttr), GetUnmanagedDll(_PythonDll));
            PyObject_GenericGetDict = (BorrowedReferenceIntPtrNewReferenceFunc)GetFunctionByName(nameof(PyObject_GenericGetDict), GetUnmanagedDll(PythonDLL));
            PyObject_GenericSetAttr = (BorrowedReferenceBorrowedReferenceBorrowedReferenceintFunc)GetFunctionByName(nameof(PyObject_GenericSetAttr), GetUnmanagedDll(_PythonDll));
            PyObject_GC_Del = (StolenReferencevoidFunc)GetFunctionByName(nameof(PyObject_GC_Del), GetUnmanagedDll(_PythonDll));
            try
            {
                PyObject_GC_IsTracked = (BorrowedReferenceintFunc)GetFunctionByName(nameof(PyObject_GC_IsTracked), GetUnmanagedDll(_PythonDll));
            }
            catch (MissingMethodException) { }
            PyObject_GC_Track = (BorrowedReferencevoidFunc)GetFunctionByName(nameof(PyObject_GC_Track), GetUnmanagedDll(_PythonDll));
            PyObject_GC_UnTrack = (BorrowedReferencevoidFunc)GetFunctionByName(nameof(PyObject_GC_UnTrack), GetUnmanagedDll(_PythonDll));
            _PyObject_Dump = (BorrowedReferencevoidFunc)GetFunctionByName(nameof(_PyObject_Dump), GetUnmanagedDll(_PythonDll));
            PyMem_Malloc = (IntPtrIntPtrFunc)GetFunctionByName(nameof(PyMem_Malloc), GetUnmanagedDll(_PythonDll));
            PyMem_Realloc = (IntPtrIntPtrIntPtrFunc)GetFunctionByName(nameof(PyMem_Realloc), GetUnmanagedDll(_PythonDll));
            PyMem_Free = (IntPtrvoidFunc)GetFunctionByName(nameof(PyMem_Free), GetUnmanagedDll(_PythonDll));
            PyErr_SetString = (BorrowedReferenceStrPtrvoidFunc)GetFunctionByName(nameof(PyErr_SetString), GetUnmanagedDll(_PythonDll));
            PyErr_SetObject = (BorrowedReferenceBorrowedReferencevoidFunc)GetFunctionByName(nameof(PyErr_SetObject), GetUnmanagedDll(_PythonDll));
            PyErr_ExceptionMatches = (BorrowedReferenceintFunc)GetFunctionByName(nameof(PyErr_ExceptionMatches), GetUnmanagedDll(_PythonDll));
            PyErr_GivenExceptionMatches = (BorrowedReferenceBorrowedReferenceintFunc)GetFunctionByName(nameof(PyErr_GivenExceptionMatches), GetUnmanagedDll(_PythonDll));
            PyErr_NormalizeException = (NewReferenceNewReferenceNewReferencevoidFunc)GetFunctionByName(nameof(PyErr_NormalizeException), GetUnmanagedDll(_PythonDll));
            PyErr_Occurred = (BorrowedReferenceFunc)GetFunctionByName(nameof(PyErr_Occurred), GetUnmanagedDll(_PythonDll));
            PyErr_Fetch = (NewReferenceNewReferenceNewReferencevoidFunc)GetFunctionByName(nameof(PyErr_Fetch), GetUnmanagedDll(_PythonDll));
            PyErr_Restore = (StolenReferenceStolenReferenceStolenReferencevoidFunc)GetFunctionByName(nameof(PyErr_Restore), GetUnmanagedDll(_PythonDll));
            PyErr_Clear = GetDelegateByName<Action>(nameof(PyErr_Clear), GetUnmanagedDll(_PythonDll));
            PyErr_Print = GetDelegateByName<Action>(nameof(PyErr_Print), GetUnmanagedDll(_PythonDll));
            PyCell_Get = (BorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyCell_Get), GetUnmanagedDll(_PythonDll));
            PyCell_Set = (BorrowedReferenceBorrowedReferenceintFunc)GetFunctionByName(nameof(PyCell_Set), GetUnmanagedDll(_PythonDll));
            PyGC_Collect = (nintFunc)GetFunctionByName(nameof(PyGC_Collect), GetUnmanagedDll(_PythonDll));
            PyCapsule_New = (IntPtrIntPtrIntPtrNewReferenceFunc)GetFunctionByName(nameof(PyCapsule_New), GetUnmanagedDll(_PythonDll));
            PyCapsule_GetPointer = (BorrowedReferenceIntPtrIntPtrFunc)GetFunctionByName(nameof(PyCapsule_GetPointer), GetUnmanagedDll(_PythonDll));
            PyCapsule_SetPointer = (BorrowedReferenceIntPtrintFunc)GetFunctionByName(nameof(PyCapsule_SetPointer), GetUnmanagedDll(_PythonDll));
            PyLong_AsUnsignedSize_t = (BorrowedReferencenuintFunc)GetFunctionByName("PyLong_AsSize_t", GetUnmanagedDll(_PythonDll));
            PyLong_AsSignedSize_t = (BorrowedReferencenintFunc)GetFunctionByName("PyLong_AsSsize_t", GetUnmanagedDll(_PythonDll));
            PyDict_GetItemWithError = (BorrowedReferenceBorrowedReferenceBorrowedReferenceFunc)GetFunctionByName(nameof(PyDict_GetItemWithError), GetUnmanagedDll(_PythonDll));
            PyException_GetCause = (BorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyException_GetCause), GetUnmanagedDll(_PythonDll));
            PyException_GetTraceback = (BorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyException_GetTraceback), GetUnmanagedDll(_PythonDll));
            PyException_SetCause = (BorrowedReferenceStolenReferencevoidFunc)GetFunctionByName(nameof(PyException_SetCause), GetUnmanagedDll(_PythonDll));
            PyException_SetTraceback = (BorrowedReferenceBorrowedReferenceintFunc)GetFunctionByName(nameof(PyException_SetTraceback), GetUnmanagedDll(_PythonDll));
            PyThreadState_SetAsyncExcLLP64 = (uintBorrowedReferenceintFunc)GetFunctionByName("PyThreadState_SetAsyncExc", GetUnmanagedDll(_PythonDll));
            PyThreadState_SetAsyncExcLP64 = (ulongBorrowedReferenceintFunc)GetFunctionByName("PyThreadState_SetAsyncExc", GetUnmanagedDll(_PythonDll));
            PyType_GetSlot = (BorrowedReferenceTypeSlotIDIntPtrFunc)GetFunctionByName(nameof(PyType_GetSlot), GetUnmanagedDll(_PythonDll));
            PyType_FromSpecWithBases = (NativeTypeSpecBorrowedReferenceNewReferenceFunc)GetFunctionByName(nameof(PyType_FromSpecWithBases), GetUnmanagedDll(PythonDLL));

            try
            {
                _Py_NewReference = (BorrowedReferencevoidFunc)GetFunctionByName(nameof(_Py_NewReference), GetUnmanagedDll(_PythonDll));
            }
            catch (MissingMethodException) { }
            try
            {
                _Py_IsFinalizing = GetDelegateByName<IntFunc>(nameof(_Py_IsFinalizing), GetUnmanagedDll(_PythonDll));
            }
            catch (MissingMethodException) { }

            PyType_Type = GetFunctionByName(nameof(PyType_Type), GetUnmanagedDll(_PythonDll));
            Py_NoSiteFlag = GetFunctionByName(nameof(Py_NoSiteFlag), GetUnmanagedDll(_PythonDll));
        }

        static global::System.IntPtr GetUnmanagedDll(string? libraryName)
        {
            if (libraryName is null) return IntPtr.Zero;
            return libraryLoader.Load(libraryName);
        }

        static IntPtr GetFunctionByName(string functionName, global::System.IntPtr libraryHandle)
        {
            try
            {
                return libraryLoader.GetFunction(libraryHandle, functionName);
            }
            catch (MissingMethodException e) when (libraryHandle == IntPtr.Zero)
            {
                throw new BadPythonDllException(
                    "Runtime.PythonDLL was not set or does not point to a supported Python runtime DLL." +
                    " See https://github.com/pythonnet/pythonnet#embedding-python-in-net",
                    e);
            }
        }

        static T GetDelegateByName<T>(string functionName, global::System.IntPtr libraryHandle) where T : Delegate
        {
            return Marshal.GetDelegateForFunctionPointer<T>(GetFunctionByName(functionName, libraryHandle));
        }

        internal delegate void BorrowedReferenceAction(BorrowedReference obj);
        internal delegate void StolenReferenceAction(ref StolenReference obj);
        internal delegate void IntAction(int obj);
        internal delegate int IntFunc();
        internal delegate nint nintFunc();
        internal delegate IntPtr IntPtrFunc();
        internal delegate NewReference BorrowedReferenceBorrowedReferenceBorrowedReferenceNewReferenceFunc(BorrowedReference a, BorrowedReference b, BorrowedReference c);
        internal delegate PyThreadState* PyThreadStateFunc();
        internal delegate PyGILState PyGILStateFunc();
        internal delegate BorrowedReference BorrowedReferenceFunc();
        internal delegate NewReference NewReferenceFunc();
        internal delegate void PyThreadStatevoidFunc(PyThreadState* a);
        internal delegate PyThreadState* PyInterpreterStatePyThreadStateFunc(PyInterpreterState* a);
        internal delegate void BorrowedReferencevoidFunc(BorrowedReference a);
        internal delegate int BorrowedReferenceintFunc(BorrowedReference a);
        internal delegate nint StrPtrnintFunc(StrPtr a);
        internal delegate void IntPtrvoidFunc(IntPtr a);
        internal delegate NewReference BorrowedReferenceNewReferenceFunc(BorrowedReference a);
        internal delegate bool BorrowedReferenceboolFunc(BorrowedReference a);
        internal delegate NewReference longNewReferenceFunc(long a);
        internal delegate NewReference ulongNewReferenceFunc(ulong a);
        internal delegate long BorrowedReferencelongFunc(BorrowedReference a);
        internal delegate ulong BorrowedReferenceulongFunc(BorrowedReference a);
        internal delegate NewReference IntPtrNewReferenceFunc(IntPtr a);
        internal delegate IntPtr BorrowedReferenceIntPtrFunc(BorrowedReference a);
        internal delegate NewReference doubleNewReferenceFunc(double a);
        internal delegate double BorrowedReferencedoubleFunc(BorrowedReference a);
        internal delegate nint BorrowedReferencenintFunc(BorrowedReference a);
        internal delegate NewReference intNewReferenceFunc(int a);
        internal delegate NewReference StrPtrNewReferenceFunc(StrPtr a);
        internal delegate NewReference nintNewReferenceFunc(nint a);
        internal delegate void PyGILStatevoidFunc(PyGILState a);
        internal delegate BorrowedReference BorrowedReferenceBorrowedReferenceFunc(BorrowedReference a);
        internal delegate BorrowedReference StrPtrBorrowedReferenceFunc(StrPtr a);
        internal delegate void StolenReferencevoidFunc(StolenReference a);
        internal delegate IntPtr nintIntPtrFunc(nint a);
        internal delegate nuint BorrowedReferencenuintFunc(BorrowedReference a);
        internal delegate int intIntPtrintFunc(int a, IntPtr b);
        internal delegate NewReference StrPtrBorrowedReferenceNewReferenceFunc(StrPtr a, BorrowedReference b);
        internal delegate int BorrowedReferenceStrPtrintFunc(BorrowedReference a, StrPtr b);
        internal delegate NewReference BorrowedReferenceStrPtrNewReferenceFunc(BorrowedReference a, StrPtr b);
        internal delegate int BorrowedReferenceBorrowedReferenceintFunc(BorrowedReference a, BorrowedReference b);
        internal delegate NewReference BorrowedReferenceBorrowedReferenceNewReferenceFunc(BorrowedReference a, BorrowedReference b);
        internal delegate NewReference BorrowedReferencenintNewReferenceFunc(BorrowedReference a, nint b);
        internal delegate int BorrowedReferencenintintFunc(BorrowedReference a, nint b);
        internal delegate nint BorrowedReferenceBorrowedReferencenintFunc(BorrowedReference a, BorrowedReference b);
        internal delegate NewReference IntPtrnintNewReferenceFunc(IntPtr a, nint b);
        internal delegate BorrowedReference BorrowedReferenceBorrowedReferenceBorrowedReferenceFunc(BorrowedReference a, BorrowedReference b);
        internal delegate BorrowedReference BorrowedReferenceStrPtrBorrowedReferenceFunc(BorrowedReference a, StrPtr b);
        internal delegate BorrowedReference BorrowedReferencenintBorrowedReferenceFunc(BorrowedReference a, nint b);
        internal delegate int StrPtrBorrowedReferenceintFunc(StrPtr a, BorrowedReference b);
        internal delegate bool BorrowedReferenceBorrowedReferenceboolFunc(BorrowedReference a, BorrowedReference b);
        internal delegate IntPtr IntPtrnintIntPtrFunc(IntPtr a, nint b);
        internal delegate void BorrowedReferenceStrPtrvoidFunc(BorrowedReference a, StrPtr b);
        internal delegate void BorrowedReferenceBorrowedReferencevoidFunc(BorrowedReference a, BorrowedReference b);
        internal delegate IntPtr BorrowedReferenceIntPtrIntPtrFunc(BorrowedReference a, IntPtr b);
        internal delegate int BorrowedReferenceIntPtrintFunc(BorrowedReference a, IntPtr b);
        internal delegate void BorrowedReferenceStolenReferencevoidFunc(BorrowedReference a, StolenReference b);
        internal delegate int uintBorrowedReferenceintFunc(uint a, BorrowedReference b);
        internal delegate int ulongBorrowedReferenceintFunc(ulong a, BorrowedReference b);
        internal delegate NewReference BorrowedReferenceIntPtrNewReferenceFunc(BorrowedReference a, IntPtr b);
        internal delegate IntPtr BorrowedReferenceTypeSlotIDIntPtrFunc(BorrowedReference a, TypeSlotID b);
        internal delegate int BorrowedReferenceStrPtrBorrowedReferenceintFunc(BorrowedReference a, StrPtr b, BorrowedReference c);
        internal delegate int BorrowedReferenceBorrowedReferenceBorrowedReferenceintFunc(BorrowedReference a, BorrowedReference b, BorrowedReference c);
        internal delegate int BorrowedReferenceBorrowedReferenceintintFunc(BorrowedReference a, BorrowedReference b, int c);
        internal delegate NewReference StrPtrIntPtrintNewReferenceFunc(StrPtr a, IntPtr b, int c);
        internal delegate int BorrowedReferencenintBorrowedReferenceintFunc(BorrowedReference a, nint b, BorrowedReference c);
        internal delegate NewReference BorrowedReferencenintnintNewReferenceFunc(BorrowedReference a, nint b, nint c);
        internal delegate int BorrowedReferencenintnintintFunc(BorrowedReference a, nint b, nint c);
        internal delegate int BorrowedReferencenintStolenReferenceintFunc(BorrowedReference a, nint b, StolenReference c);
        internal delegate int BorrowedReferenceStrPtrIntPtrintFunc(BorrowedReference a, StrPtr b, IntPtr c);
        internal delegate void intIntPtrintvoidFunc(int a, IntPtr b, int c);
        internal delegate void StolenReferenceStolenReferenceStolenReferencevoidFunc(StolenReference a, StolenReference b, StolenReference c);
        internal delegate NewReference IntPtrIntPtrIntPtrNewReferenceFunc(IntPtr a, IntPtr b, IntPtr c);
        internal delegate void Py_buffervoidFunc(ref Py_buffer a);
        internal delegate int StrPtrPyCompilerFlagsintFunc(StrPtr a, in PyCompilerFlags b);
        internal delegate int Py_buffercharintFunc(ref Py_buffer a, char b);
        internal delegate NewReference NativeTypeSpecBorrowedReferenceNewReferenceFunc(in NativeTypeSpec a, BorrowedReference b);
        internal delegate int BorrowedReferencePy_bufferintintFunc(BorrowedReference a, out Py_buffer b, int c);
        internal delegate void NewReferenceNewReferenceNewReferencevoidFunc(ref NewReference a, ref NewReference b, ref NewReference c);


        internal static BorrowedReferenceAction Py_IncRef { get; }
        internal static StolenReferenceAction Py_DecRef { get; }
        internal static Action Py_Initialize { get; }
        internal static IntAction Py_InitializeEx { get; }
        internal static IntFunc Py_IsInitialized { get; }
        internal static Action Py_Finalize { get; }
        internal static PyThreadStateFunc Py_NewInterpreter { get; }
        internal static PyThreadStatevoidFunc Py_EndInterpreter { get; }
        internal static PyInterpreterStatePyThreadStateFunc PyThreadState_New { get; }
        internal static PyThreadStateFunc PyThreadState_Get { get; }
        internal static PyThreadStateFunc _PyThreadState_UncheckedGet { get; }
        internal static IntFunc PyGILState_Check { get; }
        internal static PyGILStateFunc PyGILState_Ensure { get; }
        internal static PyGILStatevoidFunc PyGILState_Release { get; }
        internal static PyThreadStateFunc PyGILState_GetThisThreadState { get; }
        internal static intIntPtrintFunc Py_Main { get; }
        internal static Action PyEval_InitThreads { get; }
        internal static IntFunc PyEval_ThreadsInitialized { get; }
        internal static Action PyEval_AcquireLock { get; }
        internal static Action PyEval_ReleaseLock { get; }
        internal static PyThreadStatevoidFunc PyEval_AcquireThread { get; }
        internal static PyThreadStatevoidFunc PyEval_ReleaseThread { get; }
        internal static PyThreadStateFunc PyEval_SaveThread { get; }
        internal static PyThreadStatevoidFunc PyEval_RestoreThread { get; }
        internal static BorrowedReferenceFunc PyEval_GetBuiltins { get; }
        internal static BorrowedReferenceFunc PyEval_GetGlobals { get; }
        internal static BorrowedReferenceFunc PyEval_GetLocals { get; }
        internal static IntPtrFunc Py_GetProgramName { get; }
        internal static IntPtrvoidFunc Py_SetProgramName { get; }
        internal static IntPtrFunc Py_GetPythonHome { get; }
        internal static IntPtrvoidFunc Py_SetPythonHome { get; }
        internal static IntPtrFunc Py_GetPath { get; }
        internal static IntPtrvoidFunc Py_SetPath { get; }
        internal static IntPtrFunc Py_GetVersion { get; }
        internal static IntPtrFunc Py_GetPlatform { get; }
        internal static IntPtrFunc Py_GetCopyright { get; }
        internal static IntPtrFunc Py_GetCompiler { get; }
        internal static IntPtrFunc Py_GetBuildInfo { get; }
        internal static StrPtrPyCompilerFlagsintFunc PyRun_SimpleStringFlags { get; }
        internal static delegate* unmanaged[Cdecl]<StrPtr, RunFlagType, BorrowedReference, BorrowedReference, in PyCompilerFlags, NewReference> PyRun_StringFlags { get; }
        internal static BorrowedReferenceBorrowedReferenceBorrowedReferenceNewReferenceFunc PyEval_EvalCode { get; }
        internal static delegate* unmanaged[Cdecl]<StrPtr, BorrowedReference, int, in PyCompilerFlags, int, NewReference> Py_CompileStringObject { get; }
        internal static StrPtrBorrowedReferenceNewReferenceFunc PyImport_ExecCodeModule { get; }
        internal static BorrowedReferenceStrPtrintFunc PyObject_HasAttrString { get; }
        internal static BorrowedReferenceStrPtrNewReferenceFunc PyObject_GetAttrString { get; }
        internal static BorrowedReferenceStrPtrBorrowedReferenceintFunc PyObject_SetAttrString { get; }
        internal static BorrowedReferenceBorrowedReferenceintFunc PyObject_HasAttr { get; }
        internal static BorrowedReferenceBorrowedReferenceNewReferenceFunc PyObject_GetAttr { get; }
        internal static BorrowedReferenceBorrowedReferenceBorrowedReferenceintFunc PyObject_SetAttr { get; }
        internal static BorrowedReferenceBorrowedReferenceNewReferenceFunc PyObject_GetItem { get; }
        internal static BorrowedReferenceBorrowedReferenceBorrowedReferenceintFunc PyObject_SetItem { get; }
        internal static BorrowedReferenceBorrowedReferenceintFunc PyObject_DelItem { get; }
        internal static BorrowedReferenceNewReferenceFunc PyObject_GetIter { get; }
        internal static BorrowedReferenceBorrowedReferenceBorrowedReferenceNewReferenceFunc PyObject_Call { get; }
        internal static BorrowedReferenceBorrowedReferenceNewReferenceFunc PyObject_CallObject { get; }
        internal static BorrowedReferenceBorrowedReferenceintintFunc PyObject_RichCompareBool { get; }
        internal static BorrowedReferenceBorrowedReferenceintFunc PyObject_IsInstance { get; }
        internal static BorrowedReferenceBorrowedReferenceintFunc PyObject_IsSubclass { get; }
        internal static BorrowedReferencevoidFunc PyObject_ClearWeakRefs { get; }
        internal static BorrowedReferenceintFunc PyCallable_Check { get; }
        internal static BorrowedReferenceintFunc PyObject_IsTrue { get; }
        internal static BorrowedReferenceintFunc PyObject_Not { get; }
        internal static BorrowedReferencenintFunc PyObject_Size { get; }
        internal static BorrowedReferencenintFunc PyObject_Hash { get; }
        internal static BorrowedReferenceNewReferenceFunc PyObject_Repr { get; }
        internal static BorrowedReferenceNewReferenceFunc PyObject_Str { get; }
        internal static BorrowedReferenceNewReferenceFunc PyObject_Type { get; }
        internal static BorrowedReferenceNewReferenceFunc PyObject_Dir { get; }
        internal static BorrowedReferencePy_bufferintintFunc PyObject_GetBuffer { get; }
        internal static Py_buffervoidFunc PyBuffer_Release { get; }
        internal static StrPtrnintFunc PyBuffer_SizeFromFormat { get; }
        internal static Py_buffercharintFunc PyBuffer_IsContiguous { get; }
        internal static delegate* unmanaged[Cdecl]<ref Py_buffer, nint[], IntPtr> PyBuffer_GetPointer { get; }
        internal static delegate* unmanaged[Cdecl]<ref Py_buffer, IntPtr, IntPtr, char, int> PyBuffer_FromContiguous { get; }
        internal static delegate* unmanaged[Cdecl]<IntPtr, ref Py_buffer, IntPtr, char, int> PyBuffer_ToContiguous { get; }
        internal static delegate* unmanaged[Cdecl]<int, IntPtr, IntPtr, int, char, void> PyBuffer_FillContiguousStrides { get; }
        internal static delegate* unmanaged[Cdecl]<ref Py_buffer, BorrowedReference, IntPtr, IntPtr, int, int, int> PyBuffer_FillInfo { get; }
        internal static BorrowedReferenceNewReferenceFunc PyNumber_Long { get; }
        internal static BorrowedReferenceNewReferenceFunc PyNumber_Float { get; }
        internal static BorrowedReferenceboolFunc PyNumber_Check { get; }
        internal static longNewReferenceFunc PyLong_FromLongLong { get; }
        internal static ulongNewReferenceFunc PyLong_FromUnsignedLongLong { get; }
        internal static StrPtrIntPtrintNewReferenceFunc PyLong_FromString { get; }
        internal static BorrowedReferencelongFunc PyLong_AsLongLong { get; }
        internal static BorrowedReferenceulongFunc PyLong_AsUnsignedLongLong { get; }
        internal static IntPtrNewReferenceFunc PyLong_FromVoidPtr { get; }
        internal static BorrowedReferenceIntPtrFunc PyLong_AsVoidPtr { get; }
        internal static doubleNewReferenceFunc PyFloat_FromDouble { get; }
        internal static BorrowedReferenceNewReferenceFunc PyFloat_FromString { get; }
        internal static BorrowedReferencedoubleFunc PyFloat_AsDouble { get; }
        internal static BorrowedReferenceBorrowedReferenceNewReferenceFunc PyNumber_Add { get; }
        internal static BorrowedReferenceBorrowedReferenceNewReferenceFunc PyNumber_Subtract { get; }
        internal static BorrowedReferenceBorrowedReferenceNewReferenceFunc PyNumber_Multiply { get; }
        internal static BorrowedReferenceBorrowedReferenceNewReferenceFunc PyNumber_TrueDivide { get; }
        internal static BorrowedReferenceBorrowedReferenceNewReferenceFunc PyNumber_And { get; }
        internal static BorrowedReferenceBorrowedReferenceNewReferenceFunc PyNumber_Xor { get; }
        internal static BorrowedReferenceBorrowedReferenceNewReferenceFunc PyNumber_Or { get; }
        internal static BorrowedReferenceBorrowedReferenceNewReferenceFunc PyNumber_Lshift { get; }
        internal static BorrowedReferenceBorrowedReferenceNewReferenceFunc PyNumber_Rshift { get; }
        internal static BorrowedReferenceBorrowedReferenceNewReferenceFunc PyNumber_Power { get; }
        internal static BorrowedReferenceBorrowedReferenceNewReferenceFunc PyNumber_Remainder { get; }
        internal static BorrowedReferenceBorrowedReferenceNewReferenceFunc PyNumber_InPlaceAdd { get; }
        internal static BorrowedReferenceBorrowedReferenceNewReferenceFunc PyNumber_InPlaceSubtract { get; }
        internal static BorrowedReferenceBorrowedReferenceNewReferenceFunc PyNumber_InPlaceMultiply { get; }
        internal static BorrowedReferenceBorrowedReferenceNewReferenceFunc PyNumber_InPlaceTrueDivide { get; }
        internal static BorrowedReferenceBorrowedReferenceNewReferenceFunc PyNumber_InPlaceAnd { get; }
        internal static BorrowedReferenceBorrowedReferenceNewReferenceFunc PyNumber_InPlaceXor { get; }
        internal static BorrowedReferenceBorrowedReferenceNewReferenceFunc PyNumber_InPlaceOr { get; }
        internal static BorrowedReferenceBorrowedReferenceNewReferenceFunc PyNumber_InPlaceLshift { get; }
        internal static BorrowedReferenceBorrowedReferenceNewReferenceFunc PyNumber_InPlaceRshift { get; }
        internal static BorrowedReferenceBorrowedReferenceNewReferenceFunc PyNumber_InPlacePower { get; }
        internal static BorrowedReferenceBorrowedReferenceNewReferenceFunc PyNumber_InPlaceRemainder { get; }
        internal static BorrowedReferenceNewReferenceFunc PyNumber_Negative { get; }
        internal static BorrowedReferenceNewReferenceFunc PyNumber_Positive { get; }
        internal static BorrowedReferenceNewReferenceFunc PyNumber_Invert { get; }
        internal static BorrowedReferenceboolFunc PySequence_Check { get; }
        internal static BorrowedReferencenintNewReferenceFunc PySequence_GetItem { get; }
        internal static BorrowedReferencenintBorrowedReferenceintFunc PySequence_SetItem { get; }
        internal static BorrowedReferencenintintFunc PySequence_DelItem { get; }
        internal static BorrowedReferencenintnintNewReferenceFunc PySequence_GetSlice { get; }
        internal static delegate* unmanaged[Cdecl]<BorrowedReference, nint, nint, BorrowedReference, int> PySequence_SetSlice { get; }
        internal static BorrowedReferencenintnintintFunc PySequence_DelSlice { get; }
        internal static BorrowedReferencenintFunc PySequence_Size { get; }
        internal static BorrowedReferenceBorrowedReferenceintFunc PySequence_Contains { get; }
        internal static BorrowedReferenceBorrowedReferenceNewReferenceFunc PySequence_Concat { get; }
        internal static BorrowedReferencenintNewReferenceFunc PySequence_Repeat { get; }
        internal static BorrowedReferenceBorrowedReferencenintFunc PySequence_Index { get; }
        internal static BorrowedReferenceBorrowedReferencenintFunc PySequence_Count { get; }
        internal static BorrowedReferenceNewReferenceFunc PySequence_Tuple { get; }
        internal static BorrowedReferenceNewReferenceFunc PySequence_List { get; }
        internal static BorrowedReferenceIntPtrFunc PyBytes_AsString { get; }
        internal static IntPtrNewReferenceFunc PyBytes_FromString { get; }
        internal static IntPtrnintNewReferenceFunc PyByteArray_FromStringAndSize { get; }
        internal static BorrowedReferencenintFunc PyBytes_Size { get; }
        internal static BorrowedReferenceIntPtrFunc PyUnicode_AsUTF8 { get; }
        internal static delegate* unmanaged[Cdecl]<IntPtr, nint, IntPtr, IntPtr, NewReference> PyUnicode_DecodeUTF16 { get; }
        internal static BorrowedReferencenintFunc PyUnicode_GetLength { get; }
        internal static BorrowedReferenceIntPtrFunc PyUnicode_AsUnicode { get; }
        internal static BorrowedReferenceNewReferenceFunc PyUnicode_AsUTF16String { get; }
        internal static intNewReferenceFunc PyUnicode_FromOrdinal { get; }
        internal static StrPtrNewReferenceFunc PyUnicode_InternFromString { get; }
        internal static BorrowedReferenceBorrowedReferenceintFunc PyUnicode_Compare { get; }
        internal static NewReferenceFunc PyDict_New { get; }
        internal static BorrowedReferenceBorrowedReferenceBorrowedReferenceFunc PyDict_GetItem { get; }
        internal static BorrowedReferenceStrPtrBorrowedReferenceFunc PyDict_GetItemString { get; }
        internal static BorrowedReferenceBorrowedReferenceBorrowedReferenceintFunc PyDict_SetItem { get; }
        internal static BorrowedReferenceStrPtrBorrowedReferenceintFunc PyDict_SetItemString { get; }
        internal static BorrowedReferenceBorrowedReferenceintFunc PyDict_DelItem { get; }
        internal static BorrowedReferenceStrPtrintFunc PyDict_DelItemString { get; }
        internal static BorrowedReferenceBorrowedReferenceintFunc PyMapping_HasKey { get; }
        internal static BorrowedReferenceNewReferenceFunc PyDict_Keys { get; }
        internal static BorrowedReferenceNewReferenceFunc PyDict_Values { get; }
        internal static BorrowedReferenceNewReferenceFunc PyDict_Items { get; }
        internal static BorrowedReferenceNewReferenceFunc PyDict_Copy { get; }
        internal static BorrowedReferenceBorrowedReferenceintFunc PyDict_Update { get; }
        internal static BorrowedReferencevoidFunc PyDict_Clear { get; }
        internal static BorrowedReferencenintFunc PyDict_Size { get; }
        internal static BorrowedReferenceNewReferenceFunc PySet_New { get; }
        internal static BorrowedReferenceBorrowedReferenceintFunc PySet_Add { get; }
        internal static BorrowedReferenceBorrowedReferenceintFunc PySet_Contains { get; }
        internal static nintNewReferenceFunc PyList_New { get; }
        internal static BorrowedReferencenintBorrowedReferenceFunc PyList_GetItem { get; }
        internal static BorrowedReferencenintStolenReferenceintFunc PyList_SetItem { get; }
        internal static BorrowedReferencenintBorrowedReferenceintFunc PyList_Insert { get; }
        internal static BorrowedReferenceBorrowedReferenceintFunc PyList_Append { get; }
        internal static BorrowedReferenceintFunc PyList_Reverse { get; }
        internal static BorrowedReferenceintFunc PyList_Sort { get; }
        internal static BorrowedReferencenintnintNewReferenceFunc PyList_GetSlice { get; }
        internal static delegate* unmanaged[Cdecl]<BorrowedReference, nint, nint, BorrowedReference, int> PyList_SetSlice { get; }
        internal static BorrowedReferencenintFunc PyList_Size { get; }
        internal static nintNewReferenceFunc PyTuple_New { get; }
        internal static BorrowedReferencenintBorrowedReferenceFunc PyTuple_GetItem { get; }
        internal static BorrowedReferencenintStolenReferenceintFunc PyTuple_SetItem { get; }
        internal static BorrowedReferencenintnintNewReferenceFunc PyTuple_GetSlice { get; }
        internal static BorrowedReferencenintFunc PyTuple_Size { get; }
        internal static BorrowedReferenceintFunc PyIter_Check { get; }
        internal static BorrowedReferenceNewReferenceFunc PyIter_Next { get; }
        internal static StrPtrNewReferenceFunc PyModule_New { get; }
        internal static BorrowedReferenceBorrowedReferenceFunc PyModule_GetDict { get; }
        internal static BorrowedReferenceStrPtrIntPtrintFunc PyModule_AddObject { get; }
        internal static BorrowedReferenceNewReferenceFunc PyImport_Import { get; }
        internal static StrPtrNewReferenceFunc PyImport_ImportModule { get; }
        internal static BorrowedReferenceNewReferenceFunc PyImport_ReloadModule { get; }
        internal static StrPtrBorrowedReferenceFunc PyImport_AddModule { get; }
        internal static BorrowedReferenceFunc PyImport_GetModuleDict { get; }
        internal static intIntPtrintvoidFunc PySys_SetArgvEx { get; }
        internal static StrPtrBorrowedReferenceFunc PySys_GetObject { get; }
        internal static StrPtrBorrowedReferenceintFunc PySys_SetObject { get; }
        internal static BorrowedReferencevoidFunc PyType_Modified { get; }
        internal static BorrowedReferenceBorrowedReferenceboolFunc PyType_IsSubtype { get; }
        internal static BorrowedReferenceBorrowedReferenceBorrowedReferenceNewReferenceFunc PyType_GenericNew { get; }
        internal static BorrowedReferencenintNewReferenceFunc PyType_GenericAlloc { get; }
        internal static BorrowedReferenceintFunc PyType_Ready { get; }
        internal static BorrowedReferenceBorrowedReferenceBorrowedReferenceFunc _PyType_Lookup { get; }
        internal static BorrowedReferenceBorrowedReferenceNewReferenceFunc PyObject_GenericGetAttr { get; }
        internal static BorrowedReferenceBorrowedReferenceBorrowedReferenceintFunc PyObject_GenericSetAttr { get; }
        internal static StolenReferencevoidFunc PyObject_GC_Del { get; }
        internal static BorrowedReferenceintFunc PyObject_GC_IsTracked { get; }
        internal static BorrowedReferencevoidFunc PyObject_GC_Track { get; }
        internal static BorrowedReferencevoidFunc PyObject_GC_UnTrack { get; }
        internal static BorrowedReferencevoidFunc _PyObject_Dump { get; }
        internal static nintIntPtrFunc PyMem_Malloc { get; }
        internal static IntPtrnintIntPtrFunc PyMem_Realloc { get; }
        internal static IntPtrvoidFunc PyMem_Free { get; }
        internal static BorrowedReferenceStrPtrvoidFunc PyErr_SetString { get; }
        internal static BorrowedReferenceBorrowedReferencevoidFunc PyErr_SetObject { get; }
        internal static BorrowedReferenceintFunc PyErr_ExceptionMatches { get; }
        internal static BorrowedReferenceBorrowedReferenceintFunc PyErr_GivenExceptionMatches { get; }
        internal static NewReferenceNewReferenceNewReferencevoidFunc PyErr_NormalizeException { get; }
        internal static BorrowedReferenceFunc PyErr_Occurred { get; }
        internal static NewReferenceNewReferenceNewReferencevoidFunc PyErr_Fetch { get; }
        internal static StolenReferenceStolenReferenceStolenReferencevoidFunc PyErr_Restore { get; }
        internal static Action PyErr_Clear { get; }
        internal static Action PyErr_Print { get; }
        internal static BorrowedReferenceNewReferenceFunc PyCell_Get { get; }
        internal static BorrowedReferenceBorrowedReferenceintFunc PyCell_Set { get; }
        internal static nintFunc PyGC_Collect { get; }
        internal static IntPtrIntPtrIntPtrNewReferenceFunc PyCapsule_New { get; }
        internal static BorrowedReferenceIntPtrIntPtrFunc PyCapsule_GetPointer { get; }
        internal static BorrowedReferenceIntPtrintFunc PyCapsule_SetPointer { get; }
        internal static BorrowedReferencenuintFunc PyLong_AsUnsignedSize_t { get; }
        internal static BorrowedReferencenintFunc PyLong_AsSignedSize_t { get; }
        internal static BorrowedReferenceBorrowedReferenceBorrowedReferenceFunc PyDict_GetItemWithError { get; }
        internal static BorrowedReferenceNewReferenceFunc PyException_GetCause { get; }
        internal static BorrowedReferenceNewReferenceFunc PyException_GetTraceback { get; }
        internal static BorrowedReferenceStolenReferencevoidFunc PyException_SetCause { get; }
        internal static BorrowedReferenceBorrowedReferenceintFunc PyException_SetTraceback { get; }
        internal static uintBorrowedReferenceintFunc PyThreadState_SetAsyncExcLLP64 { get; }
        internal static ulongBorrowedReferenceintFunc PyThreadState_SetAsyncExcLP64 { get; }
        internal static BorrowedReferenceIntPtrNewReferenceFunc PyObject_GenericGetDict { get; }
        internal static BorrowedReferenceTypeSlotIDIntPtrFunc PyType_GetSlot { get; }
        internal static NativeTypeSpecBorrowedReferenceNewReferenceFunc PyType_FromSpecWithBases { get; }
        internal static BorrowedReferencevoidFunc _Py_NewReference { get; }
        internal static IntFunc _Py_IsFinalizing { get; }
        internal static IntPtr PyType_Type { get; }
        internal static IntPtr Py_NoSiteFlag { get; }
    }
}
