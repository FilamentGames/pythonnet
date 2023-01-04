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
            Py_NewInterpreter = GetDelegateByName<PyThreadStateFunc>(nameof(Py_NewInterpreter), GetUnmanagedDll(_PythonDll));
            Py_EndInterpreter = GetDelegateByName<PyThreadStatevoidFunc>(nameof(Py_EndInterpreter), GetUnmanagedDll(_PythonDll));
            PyThreadState_New = GetDelegateByName<PyInterpreterStatePyThreadStateFunc>(nameof(PyThreadState_New), GetUnmanagedDll(_PythonDll));
            PyThreadState_Get = GetDelegateByName<PyThreadStateFunc>(nameof(PyThreadState_Get), GetUnmanagedDll(_PythonDll));
            _PyThreadState_UncheckedGet = GetDelegateByName<PyThreadStateFunc>(nameof(_PyThreadState_UncheckedGet), GetUnmanagedDll(_PythonDll));
            try
            {
                PyGILState_Check = GetDelegateByName<IntFunc>(nameof(PyGILState_Check), GetUnmanagedDll(_PythonDll));
            }
            catch (MissingMethodException e)
            {
                throw new NotSupportedException(Util.MinimalPythonVersionRequired, innerException: e);
            }
            PyGILState_Ensure = GetDelegateByName<PyGILStateFunc>(nameof(PyGILState_Ensure), GetUnmanagedDll(_PythonDll));
            PyGILState_Release = GetDelegateByName<PyGILStatevoidFunc>(nameof(PyGILState_Release), GetUnmanagedDll(_PythonDll));
            PyGILState_GetThisThreadState = GetDelegateByName<PyThreadStateFunc>(nameof(PyGILState_GetThisThreadState), GetUnmanagedDll(_PythonDll));
            Py_Main = GetDelegateByName<intIntPtrintFunc>(nameof(Py_Main), GetUnmanagedDll(_PythonDll));
            PyEval_InitThreads = GetDelegateByName<Action>(nameof(PyEval_InitThreads), GetUnmanagedDll(_PythonDll));
            PyEval_ThreadsInitialized = GetDelegateByName<IntFunc>(nameof(PyEval_ThreadsInitialized), GetUnmanagedDll(_PythonDll));
            PyEval_AcquireLock = GetDelegateByName<Action>(nameof(PyEval_AcquireLock), GetUnmanagedDll(_PythonDll));
            PyEval_ReleaseLock = GetDelegateByName<Action>(nameof(PyEval_ReleaseLock), GetUnmanagedDll(_PythonDll));
            PyEval_AcquireThread = GetDelegateByName<PyThreadStatevoidFunc>(nameof(PyEval_AcquireThread), GetUnmanagedDll(_PythonDll));
            PyEval_ReleaseThread = GetDelegateByName<PyThreadStatevoidFunc>(nameof(PyEval_ReleaseThread), GetUnmanagedDll(_PythonDll));
            PyEval_SaveThread = GetDelegateByName<PyThreadStateFunc>(nameof(PyEval_SaveThread), GetUnmanagedDll(_PythonDll));
            PyEval_RestoreThread = GetDelegateByName<PyThreadStatevoidFunc>(nameof(PyEval_RestoreThread), GetUnmanagedDll(_PythonDll));
            PyEval_GetBuiltins = GetDelegateByName<BorrowedReferenceFunc>(nameof(PyEval_GetBuiltins), GetUnmanagedDll(_PythonDll));
            PyEval_GetGlobals = GetDelegateByName<BorrowedReferenceFunc>(nameof(PyEval_GetGlobals), GetUnmanagedDll(_PythonDll));
            PyEval_GetLocals = GetDelegateByName<BorrowedReferenceFunc>(nameof(PyEval_GetLocals), GetUnmanagedDll(_PythonDll));
            Py_GetProgramName = GetDelegateByName<IntPtrFunc>(nameof(Py_GetProgramName), GetUnmanagedDll(_PythonDll));
            Py_SetProgramName = GetDelegateByName<IntPtrvoidFunc>(nameof(Py_SetProgramName), GetUnmanagedDll(_PythonDll));
            Py_GetPythonHome = GetDelegateByName<IntPtrFunc>(nameof(Py_GetPythonHome), GetUnmanagedDll(_PythonDll));
            Py_SetPythonHome = GetDelegateByName<IntPtrvoidFunc>(nameof(Py_SetPythonHome), GetUnmanagedDll(_PythonDll));
            Py_GetPath = GetDelegateByName<IntPtrFunc>(nameof(Py_GetPath), GetUnmanagedDll(_PythonDll));
            Py_SetPath = GetDelegateByName<IntPtrvoidFunc>(nameof(Py_SetPath), GetUnmanagedDll(_PythonDll));
            Py_GetVersion = GetDelegateByName<IntPtrFunc>(nameof(Py_GetVersion), GetUnmanagedDll(_PythonDll));
            Py_GetPlatform = GetDelegateByName<IntPtrFunc>(nameof(Py_GetPlatform), GetUnmanagedDll(_PythonDll));
            Py_GetCopyright = GetDelegateByName<IntPtrFunc>(nameof(Py_GetCopyright), GetUnmanagedDll(_PythonDll));
            Py_GetCompiler = GetDelegateByName<IntPtrFunc>(nameof(Py_GetCompiler), GetUnmanagedDll(_PythonDll));
            Py_GetBuildInfo = GetDelegateByName<IntPtrFunc>(nameof(Py_GetBuildInfo), GetUnmanagedDll(_PythonDll));
            PyRun_SimpleStringFlags = GetDelegateByName<StrPtrPyCompilerFlagsintFunc>(nameof(PyRun_SimpleStringFlags), GetUnmanagedDll(_PythonDll));
            PyRun_StringFlags = GetDelegateByName<StrPtrRunFlagTypeBorrowedReferenceBorrowedReferenceinPyCompilerFlagsNewReferenceFunc>(nameof(PyRun_StringFlags), GetUnmanagedDll(_PythonDll));
            PyEval_EvalCode = GetDelegateByName<BorrowedReferenceBorrowedReferenceBorrowedReferenceNewReferenceFunc>(nameof(PyEval_EvalCode), GetUnmanagedDll(_PythonDll));
            Py_CompileStringObject = GetDelegateByName<StrPtrBorrowedReferenceintinPyCompilerFlagsintNewReferenceFunc>(nameof(Py_CompileStringObject), GetUnmanagedDll(_PythonDll));
            PyImport_ExecCodeModule = GetDelegateByName<StrPtrBorrowedReferenceNewReferenceFunc>(nameof(PyImport_ExecCodeModule), GetUnmanagedDll(_PythonDll));
            PyObject_HasAttrString = GetDelegateByName<BorrowedReferenceStrPtrintFunc>(nameof(PyObject_HasAttrString), GetUnmanagedDll(_PythonDll));
            PyObject_GetAttrString = GetDelegateByName<BorrowedReferenceStrPtrNewReferenceFunc>(nameof(PyObject_GetAttrString), GetUnmanagedDll(_PythonDll));
            PyObject_SetAttrString = GetDelegateByName<BorrowedReferenceStrPtrBorrowedReferenceintFunc>(nameof(PyObject_SetAttrString), GetUnmanagedDll(_PythonDll));
            PyObject_HasAttr = GetDelegateByName<BorrowedReferenceBorrowedReferenceintFunc>(nameof(PyObject_HasAttr), GetUnmanagedDll(_PythonDll));
            PyObject_GetAttr = GetDelegateByName<BorrowedReferenceBorrowedReferenceNewReferenceFunc>(nameof(PyObject_GetAttr), GetUnmanagedDll(_PythonDll));
            PyObject_SetAttr = GetDelegateByName<BorrowedReferenceBorrowedReferenceBorrowedReferenceintFunc>(nameof(PyObject_SetAttr), GetUnmanagedDll(_PythonDll));
            PyObject_GetItem = GetDelegateByName<BorrowedReferenceBorrowedReferenceNewReferenceFunc>(nameof(PyObject_GetItem), GetUnmanagedDll(_PythonDll));
            PyObject_SetItem = GetDelegateByName<BorrowedReferenceBorrowedReferenceBorrowedReferenceintFunc>(nameof(PyObject_SetItem), GetUnmanagedDll(_PythonDll));
            PyObject_DelItem = GetDelegateByName<BorrowedReferenceBorrowedReferenceintFunc>(nameof(PyObject_DelItem), GetUnmanagedDll(_PythonDll));
            PyObject_GetIter = GetDelegateByName<BorrowedReferenceNewReferenceFunc>(nameof(PyObject_GetIter), GetUnmanagedDll(_PythonDll));
            PyObject_Call = GetDelegateByName<BorrowedReferenceBorrowedReferenceBorrowedReferenceNewReferenceFunc>(nameof(PyObject_Call), GetUnmanagedDll(_PythonDll));
            PyObject_CallObject = GetDelegateByName<BorrowedReferenceBorrowedReferenceNewReferenceFunc>(nameof(PyObject_CallObject), GetUnmanagedDll(_PythonDll));
            PyObject_RichCompareBool = GetDelegateByName<BorrowedReferenceBorrowedReferenceintintFunc>(nameof(PyObject_RichCompareBool), GetUnmanagedDll(_PythonDll));
            PyObject_IsInstance = GetDelegateByName<BorrowedReferenceBorrowedReferenceintFunc>(nameof(PyObject_IsInstance), GetUnmanagedDll(_PythonDll));
            PyObject_IsSubclass = GetDelegateByName<BorrowedReferenceBorrowedReferenceintFunc>(nameof(PyObject_IsSubclass), GetUnmanagedDll(_PythonDll));
            PyObject_ClearWeakRefs = GetDelegateByName<BorrowedReferencevoidFunc>(nameof(PyObject_ClearWeakRefs), GetUnmanagedDll(_PythonDll));
            PyCallable_Check = GetDelegateByName<BorrowedReferenceintFunc>(nameof(PyCallable_Check), GetUnmanagedDll(_PythonDll));
            PyObject_IsTrue = GetDelegateByName<BorrowedReferenceintFunc>(nameof(PyObject_IsTrue), GetUnmanagedDll(_PythonDll));
            PyObject_Not = GetDelegateByName<BorrowedReferenceintFunc>(nameof(PyObject_Not), GetUnmanagedDll(_PythonDll));
            PyObject_Size = GetDelegateByName<BorrowedReferencenintFunc>("PyObject_Size", GetUnmanagedDll(_PythonDll));
            PyObject_Hash = GetDelegateByName<BorrowedReferencenintFunc>(nameof(PyObject_Hash), GetUnmanagedDll(_PythonDll));
            PyObject_Repr = GetDelegateByName<BorrowedReferenceNewReferenceFunc>(nameof(PyObject_Repr), GetUnmanagedDll(_PythonDll));
            PyObject_Str = GetDelegateByName<BorrowedReferenceNewReferenceFunc>(nameof(PyObject_Str), GetUnmanagedDll(_PythonDll));
            PyObject_Type = GetDelegateByName<BorrowedReferenceNewReferenceFunc>(nameof(PyObject_Type), GetUnmanagedDll(_PythonDll));
            PyObject_Dir = GetDelegateByName<BorrowedReferenceNewReferenceFunc>(nameof(PyObject_Dir), GetUnmanagedDll(_PythonDll));
            PyObject_GetBuffer = GetDelegateByName<BorrowedReferencePy_bufferintintFunc>(nameof(PyObject_GetBuffer), GetUnmanagedDll(_PythonDll));
            PyBuffer_Release = GetDelegateByName<Py_buffervoidFunc>(nameof(PyBuffer_Release), GetUnmanagedDll(_PythonDll));
            try
            {
                PyBuffer_SizeFromFormat = GetDelegateByName<StrPtrnintFunc>(nameof(PyBuffer_SizeFromFormat), GetUnmanagedDll(_PythonDll));
            }
            catch (MissingMethodException)
            {
                // only in 3.9+
            }
            PyBuffer_IsContiguous = GetDelegateByName<Py_buffercharintFunc>(nameof(PyBuffer_IsContiguous), GetUnmanagedDll(_PythonDll));
            PyBuffer_GetPointer = GetDelegateByName<refPy_buffernintArrayIntPtrFunc>(nameof(PyBuffer_GetPointer), GetUnmanagedDll(_PythonDll));
            PyBuffer_FromContiguous = GetDelegateByName<refPy_bufferIntPtrIntPtrcharintFunc>(nameof(PyBuffer_FromContiguous), GetUnmanagedDll(_PythonDll));
            PyBuffer_ToContiguous = GetDelegateByName<IntPtrrefPy_bufferIntPtrcharintFunc>(nameof(PyBuffer_ToContiguous), GetUnmanagedDll(_PythonDll));
            PyBuffer_FillContiguousStrides = GetDelegateByName<intIntPtrIntPtrintcharvoidFunc>(nameof(PyBuffer_FillContiguousStrides), GetUnmanagedDll(_PythonDll));
            PyBuffer_FillInfo = GetDelegateByName<refPy_bufferBorrowedReferenceIntPtrIntPtrintintintFunc>(nameof(PyBuffer_FillInfo), GetUnmanagedDll(_PythonDll));
            PyNumber_Long = GetDelegateByName<BorrowedReferenceNewReferenceFunc>(nameof(PyNumber_Long), GetUnmanagedDll(_PythonDll));
            PyNumber_Float = GetDelegateByName<BorrowedReferenceNewReferenceFunc>(nameof(PyNumber_Float), GetUnmanagedDll(_PythonDll));
            PyNumber_Check = GetDelegateByName<BorrowedReferenceboolFunc>(nameof(PyNumber_Check), GetUnmanagedDll(_PythonDll));
            PyLong_FromLongLong = GetDelegateByName<longNewReferenceFunc>(nameof(PyLong_FromLongLong), GetUnmanagedDll(_PythonDll));
            PyLong_FromUnsignedLongLong = GetDelegateByName<ulongNewReferenceFunc>(nameof(PyLong_FromUnsignedLongLong), GetUnmanagedDll(_PythonDll));
            PyLong_FromString = GetDelegateByName<StrPtrIntPtrintNewReferenceFunc>(nameof(PyLong_FromString), GetUnmanagedDll(_PythonDll));
            PyLong_AsLongLong = GetDelegateByName<BorrowedReferencelongFunc>(nameof(PyLong_AsLongLong), GetUnmanagedDll(_PythonDll));
            PyLong_AsUnsignedLongLong = GetDelegateByName<BorrowedReferenceulongFunc>(nameof(PyLong_AsUnsignedLongLong), GetUnmanagedDll(_PythonDll));
            PyLong_FromVoidPtr = GetDelegateByName<IntPtrNewReferenceFunc>(nameof(PyLong_FromVoidPtr), GetUnmanagedDll(_PythonDll));
            PyLong_AsVoidPtr = GetDelegateByName<BorrowedReferenceIntPtrFunc>(nameof(PyLong_AsVoidPtr), GetUnmanagedDll(_PythonDll));
            PyFloat_FromDouble = GetDelegateByName<doubleNewReferenceFunc>(nameof(PyFloat_FromDouble), GetUnmanagedDll(_PythonDll));
            PyFloat_FromString = GetDelegateByName<BorrowedReferenceNewReferenceFunc>(nameof(PyFloat_FromString), GetUnmanagedDll(_PythonDll));
            PyFloat_AsDouble = GetDelegateByName<BorrowedReferencedoubleFunc>(nameof(PyFloat_AsDouble), GetUnmanagedDll(_PythonDll));
            PyNumber_Add = GetDelegateByName<BorrowedReferenceBorrowedReferenceNewReferenceFunc>(nameof(PyNumber_Add), GetUnmanagedDll(_PythonDll));
            PyNumber_Subtract = GetDelegateByName<BorrowedReferenceBorrowedReferenceNewReferenceFunc>(nameof(PyNumber_Subtract), GetUnmanagedDll(_PythonDll));
            PyNumber_Multiply = GetDelegateByName<BorrowedReferenceBorrowedReferenceNewReferenceFunc>(nameof(PyNumber_Multiply), GetUnmanagedDll(_PythonDll));
            PyNumber_TrueDivide = GetDelegateByName<BorrowedReferenceBorrowedReferenceNewReferenceFunc>(nameof(PyNumber_TrueDivide), GetUnmanagedDll(_PythonDll));
            PyNumber_And = GetDelegateByName<BorrowedReferenceBorrowedReferenceNewReferenceFunc>(nameof(PyNumber_And), GetUnmanagedDll(_PythonDll));
            PyNumber_Xor = GetDelegateByName<BorrowedReferenceBorrowedReferenceNewReferenceFunc>(nameof(PyNumber_Xor), GetUnmanagedDll(_PythonDll));
            PyNumber_Or = GetDelegateByName<BorrowedReferenceBorrowedReferenceNewReferenceFunc>(nameof(PyNumber_Or), GetUnmanagedDll(_PythonDll));
            PyNumber_Lshift = GetDelegateByName<BorrowedReferenceBorrowedReferenceNewReferenceFunc>(nameof(PyNumber_Lshift), GetUnmanagedDll(_PythonDll));
            PyNumber_Rshift = GetDelegateByName<BorrowedReferenceBorrowedReferenceNewReferenceFunc>(nameof(PyNumber_Rshift), GetUnmanagedDll(_PythonDll));
            PyNumber_Power = GetDelegateByName<BorrowedReferenceBorrowedReferenceNewReferenceFunc>(nameof(PyNumber_Power), GetUnmanagedDll(_PythonDll));
            PyNumber_Remainder = GetDelegateByName<BorrowedReferenceBorrowedReferenceNewReferenceFunc>(nameof(PyNumber_Remainder), GetUnmanagedDll(_PythonDll));
            PyNumber_InPlaceAdd = GetDelegateByName<BorrowedReferenceBorrowedReferenceNewReferenceFunc>(nameof(PyNumber_InPlaceAdd), GetUnmanagedDll(_PythonDll));
            PyNumber_InPlaceSubtract = GetDelegateByName<BorrowedReferenceBorrowedReferenceNewReferenceFunc>(nameof(PyNumber_InPlaceSubtract), GetUnmanagedDll(_PythonDll));
            PyNumber_InPlaceMultiply = GetDelegateByName<BorrowedReferenceBorrowedReferenceNewReferenceFunc>(nameof(PyNumber_InPlaceMultiply), GetUnmanagedDll(_PythonDll));
            PyNumber_InPlaceTrueDivide = GetDelegateByName<BorrowedReferenceBorrowedReferenceNewReferenceFunc>(nameof(PyNumber_InPlaceTrueDivide), GetUnmanagedDll(_PythonDll));
            PyNumber_InPlaceAnd = GetDelegateByName<BorrowedReferenceBorrowedReferenceNewReferenceFunc>(nameof(PyNumber_InPlaceAnd), GetUnmanagedDll(_PythonDll));
            PyNumber_InPlaceXor = GetDelegateByName<BorrowedReferenceBorrowedReferenceNewReferenceFunc>(nameof(PyNumber_InPlaceXor), GetUnmanagedDll(_PythonDll));
            PyNumber_InPlaceOr = GetDelegateByName<BorrowedReferenceBorrowedReferenceNewReferenceFunc>(nameof(PyNumber_InPlaceOr), GetUnmanagedDll(_PythonDll));
            PyNumber_InPlaceLshift = GetDelegateByName<BorrowedReferenceBorrowedReferenceNewReferenceFunc>(nameof(PyNumber_InPlaceLshift), GetUnmanagedDll(_PythonDll));
            PyNumber_InPlaceRshift = GetDelegateByName<BorrowedReferenceBorrowedReferenceNewReferenceFunc>(nameof(PyNumber_InPlaceRshift), GetUnmanagedDll(_PythonDll));
            PyNumber_InPlacePower = GetDelegateByName<BorrowedReferenceBorrowedReferenceNewReferenceFunc>(nameof(PyNumber_InPlacePower), GetUnmanagedDll(_PythonDll));
            PyNumber_InPlaceRemainder = GetDelegateByName<BorrowedReferenceBorrowedReferenceNewReferenceFunc>(nameof(PyNumber_InPlaceRemainder), GetUnmanagedDll(_PythonDll));
            PyNumber_Negative = GetDelegateByName<BorrowedReferenceNewReferenceFunc>(nameof(PyNumber_Negative), GetUnmanagedDll(_PythonDll));
            PyNumber_Positive = GetDelegateByName<BorrowedReferenceNewReferenceFunc>(nameof(PyNumber_Positive), GetUnmanagedDll(_PythonDll));
            PyNumber_Invert = GetDelegateByName<BorrowedReferenceNewReferenceFunc>(nameof(PyNumber_Invert), GetUnmanagedDll(_PythonDll));
            PySequence_Check = GetDelegateByName<BorrowedReferenceboolFunc>(nameof(PySequence_Check), GetUnmanagedDll(_PythonDll));
            PySequence_GetItem = GetDelegateByName<BorrowedReferencenintNewReferenceFunc>(nameof(PySequence_GetItem), GetUnmanagedDll(_PythonDll));
            PySequence_SetItem = GetDelegateByName<BorrowedReferencenintBorrowedReferenceintFunc>(nameof(PySequence_SetItem), GetUnmanagedDll(_PythonDll));
            PySequence_DelItem = GetDelegateByName<BorrowedReferencenintintFunc>(nameof(PySequence_DelItem), GetUnmanagedDll(_PythonDll));
            PySequence_GetSlice = GetDelegateByName<BorrowedReferencenintnintNewReferenceFunc>(nameof(PySequence_GetSlice), GetUnmanagedDll(_PythonDll));
            PySequence_SetSlice = GetDelegateByName<BorrowedReferencenintnintBorrowedReferenceIntFunc>(nameof(PySequence_SetSlice), GetUnmanagedDll(_PythonDll));
            PySequence_DelSlice = GetDelegateByName<BorrowedReferencenintnintintFunc>(nameof(PySequence_DelSlice), GetUnmanagedDll(_PythonDll));
            PySequence_Size = GetDelegateByName<BorrowedReferencenintFunc>(nameof(PySequence_Size), GetUnmanagedDll(_PythonDll));
            PySequence_Contains = GetDelegateByName<BorrowedReferenceBorrowedReferenceintFunc>(nameof(PySequence_Contains), GetUnmanagedDll(_PythonDll));
            PySequence_Concat = GetDelegateByName<BorrowedReferenceBorrowedReferenceNewReferenceFunc>(nameof(PySequence_Concat), GetUnmanagedDll(_PythonDll));
            PySequence_Repeat = GetDelegateByName<BorrowedReferencenintNewReferenceFunc>(nameof(PySequence_Repeat), GetUnmanagedDll(_PythonDll));
            PySequence_Index = GetDelegateByName<BorrowedReferenceBorrowedReferencenintFunc>(nameof(PySequence_Index), GetUnmanagedDll(_PythonDll));
            PySequence_Count = GetDelegateByName<BorrowedReferenceBorrowedReferencenintFunc>(nameof(PySequence_Count), GetUnmanagedDll(_PythonDll));
            PySequence_Tuple = GetDelegateByName<BorrowedReferenceNewReferenceFunc>(nameof(PySequence_Tuple), GetUnmanagedDll(_PythonDll));
            PySequence_List = GetDelegateByName<BorrowedReferenceNewReferenceFunc>(nameof(PySequence_List), GetUnmanagedDll(_PythonDll));
            PyBytes_AsString = GetDelegateByName<BorrowedReferenceIntPtrFunc>(nameof(PyBytes_AsString), GetUnmanagedDll(_PythonDll));
            PyBytes_FromString = GetDelegateByName<IntPtrNewReferenceFunc>(nameof(PyBytes_FromString), GetUnmanagedDll(_PythonDll));
            PyByteArray_FromStringAndSize = GetDelegateByName<IntPtrnintNewReferenceFunc>(nameof(PyByteArray_FromStringAndSize), GetUnmanagedDll(_PythonDll));
            PyBytes_Size = GetDelegateByName<BorrowedReferencenintFunc>(nameof(PyBytes_Size), GetUnmanagedDll(_PythonDll));
            PyUnicode_AsUTF8 = GetDelegateByName<BorrowedReferenceIntPtrFunc>(nameof(PyUnicode_AsUTF8), GetUnmanagedDll(_PythonDll));
            PyUnicode_DecodeUTF16 = GetDelegateByName<IntPtrnintIntPtrIntPtrNewReferenceFunc>(nameof(PyUnicode_DecodeUTF16), GetUnmanagedDll(_PythonDll));
            PyUnicode_GetLength = GetDelegateByName<BorrowedReferencenintFunc>(nameof(PyUnicode_GetLength), GetUnmanagedDll(_PythonDll));
            PyUnicode_AsUnicode = GetDelegateByName<BorrowedReferenceIntPtrFunc>(nameof(PyUnicode_AsUnicode), GetUnmanagedDll(_PythonDll));
            PyUnicode_AsUTF16String = GetDelegateByName<BorrowedReferenceNewReferenceFunc>(nameof(PyUnicode_AsUTF16String), GetUnmanagedDll(_PythonDll));
            PyUnicode_FromOrdinal = GetDelegateByName<intNewReferenceFunc>(nameof(PyUnicode_FromOrdinal), GetUnmanagedDll(_PythonDll));
            PyUnicode_InternFromString = GetDelegateByName<StrPtrNewReferenceFunc>(nameof(PyUnicode_InternFromString), GetUnmanagedDll(_PythonDll));
            PyUnicode_Compare = GetDelegateByName<BorrowedReferenceBorrowedReferenceintFunc>(nameof(PyUnicode_Compare), GetUnmanagedDll(_PythonDll));
            PyDict_New = GetDelegateByName<NewReferenceFunc>(nameof(PyDict_New), GetUnmanagedDll(_PythonDll));
            PyDict_GetItem = GetDelegateByName<BorrowedReferenceBorrowedReferenceBorrowedReferenceFunc>(nameof(PyDict_GetItem), GetUnmanagedDll(_PythonDll));
            PyDict_GetItemString = GetDelegateByName<BorrowedReferenceStrPtrBorrowedReferenceFunc>(nameof(PyDict_GetItemString), GetUnmanagedDll(_PythonDll));
            PyDict_SetItem = GetDelegateByName<BorrowedReferenceBorrowedReferenceBorrowedReferenceintFunc>(nameof(PyDict_SetItem), GetUnmanagedDll(_PythonDll));
            PyDict_SetItemString = GetDelegateByName<BorrowedReferenceStrPtrBorrowedReferenceintFunc>(nameof(PyDict_SetItemString), GetUnmanagedDll(_PythonDll));
            PyDict_DelItem = GetDelegateByName<BorrowedReferenceBorrowedReferenceintFunc>(nameof(PyDict_DelItem), GetUnmanagedDll(_PythonDll));
            PyDict_DelItemString = GetDelegateByName<BorrowedReferenceStrPtrintFunc>(nameof(PyDict_DelItemString), GetUnmanagedDll(_PythonDll));
            PyMapping_HasKey = GetDelegateByName<BorrowedReferenceBorrowedReferenceintFunc>(nameof(PyMapping_HasKey), GetUnmanagedDll(_PythonDll));
            PyDict_Keys = GetDelegateByName<BorrowedReferenceNewReferenceFunc>(nameof(PyDict_Keys), GetUnmanagedDll(_PythonDll));
            PyDict_Values = GetDelegateByName<BorrowedReferenceNewReferenceFunc>(nameof(PyDict_Values), GetUnmanagedDll(_PythonDll));
            PyDict_Items = GetDelegateByName<BorrowedReferenceNewReferenceFunc>(nameof(PyDict_Items), GetUnmanagedDll(_PythonDll));
            PyDict_Copy = GetDelegateByName<BorrowedReferenceNewReferenceFunc>(nameof(PyDict_Copy), GetUnmanagedDll(_PythonDll));
            PyDict_Update = GetDelegateByName<BorrowedReferenceBorrowedReferenceintFunc>(nameof(PyDict_Update), GetUnmanagedDll(_PythonDll));
            PyDict_Clear = GetDelegateByName<BorrowedReferencevoidFunc>(nameof(PyDict_Clear), GetUnmanagedDll(_PythonDll));
            PyDict_Size = GetDelegateByName<BorrowedReferencenintFunc>(nameof(PyDict_Size), GetUnmanagedDll(_PythonDll));
            PySet_New = GetDelegateByName<BorrowedReferenceNewReferenceFunc>(nameof(PySet_New), GetUnmanagedDll(_PythonDll));
            PySet_Add = GetDelegateByName<BorrowedReferenceBorrowedReferenceintFunc>(nameof(PySet_Add), GetUnmanagedDll(_PythonDll));
            PySet_Contains = GetDelegateByName<BorrowedReferenceBorrowedReferenceintFunc>(nameof(PySet_Contains), GetUnmanagedDll(_PythonDll));
            PyList_New = GetDelegateByName<nintNewReferenceFunc>(nameof(PyList_New), GetUnmanagedDll(_PythonDll));
            PyList_GetItem = GetDelegateByName<BorrowedReferencenintBorrowedReferenceFunc>(nameof(PyList_GetItem), GetUnmanagedDll(_PythonDll));
            PyList_SetItem = GetDelegateByName<BorrowedReferencenintStolenReferenceintFunc>(nameof(PyList_SetItem), GetUnmanagedDll(_PythonDll));
            PyList_Insert = GetDelegateByName<BorrowedReferencenintBorrowedReferenceintFunc>(nameof(PyList_Insert), GetUnmanagedDll(_PythonDll));
            PyList_Append = GetDelegateByName<BorrowedReferenceBorrowedReferenceintFunc>(nameof(PyList_Append), GetUnmanagedDll(_PythonDll));
            PyList_Reverse = GetDelegateByName<BorrowedReferenceintFunc>(nameof(PyList_Reverse), GetUnmanagedDll(_PythonDll));
            PyList_Sort = GetDelegateByName<BorrowedReferenceintFunc>(nameof(PyList_Sort), GetUnmanagedDll(_PythonDll));
            PyList_GetSlice = GetDelegateByName<BorrowedReferencenintnintNewReferenceFunc>(nameof(PyList_GetSlice), GetUnmanagedDll(_PythonDll));
            PyList_SetSlice = GetDelegateByName<BorrowedReferencenintnintBorrowedReferenceIntFunc>(nameof(PyList_SetSlice), GetUnmanagedDll(_PythonDll));
            PyList_Size = GetDelegateByName<BorrowedReferencenintFunc>(nameof(PyList_Size), GetUnmanagedDll(_PythonDll));
            PyTuple_New = GetDelegateByName<nintNewReferenceFunc>(nameof(PyTuple_New), GetUnmanagedDll(_PythonDll));
            PyTuple_GetItem = GetDelegateByName<BorrowedReferencenintBorrowedReferenceFunc>(nameof(PyTuple_GetItem), GetUnmanagedDll(_PythonDll));
            PyTuple_SetItem = GetDelegateByName<BorrowedReferencenintStolenReferenceintFunc>(nameof(PyTuple_SetItem), GetUnmanagedDll(_PythonDll));
            PyTuple_GetSlice = GetDelegateByName<BorrowedReferencenintnintNewReferenceFunc>(nameof(PyTuple_GetSlice), GetUnmanagedDll(_PythonDll));
            PyTuple_Size = GetDelegateByName<BorrowedReferencenintFunc>(nameof(PyTuple_Size), GetUnmanagedDll(_PythonDll));
            try
            {
                PyIter_Check = GetDelegateByName<BorrowedReferenceintFunc>(nameof(PyIter_Check), GetUnmanagedDll(_PythonDll));
            }
            catch (MissingMethodException) { }
            PyIter_Next = GetDelegateByName<BorrowedReferenceNewReferenceFunc>(nameof(PyIter_Next), GetUnmanagedDll(_PythonDll));
            PyModule_New = GetDelegateByName<StrPtrNewReferenceFunc>(nameof(PyModule_New), GetUnmanagedDll(_PythonDll));
            PyModule_GetDict = GetDelegateByName<BorrowedReferenceBorrowedReferenceFunc>(nameof(PyModule_GetDict), GetUnmanagedDll(_PythonDll));
            PyModule_AddObject = GetDelegateByName<BorrowedReferenceStrPtrIntPtrintFunc>(nameof(PyModule_AddObject), GetUnmanagedDll(_PythonDll));
            PyImport_Import = GetDelegateByName<BorrowedReferenceNewReferenceFunc>(nameof(PyImport_Import), GetUnmanagedDll(_PythonDll));
            PyImport_ImportModule = GetDelegateByName<StrPtrNewReferenceFunc>(nameof(PyImport_ImportModule), GetUnmanagedDll(_PythonDll));
            PyImport_ReloadModule = GetDelegateByName<BorrowedReferenceNewReferenceFunc>(nameof(PyImport_ReloadModule), GetUnmanagedDll(_PythonDll));
            PyImport_AddModule = GetDelegateByName<StrPtrBorrowedReferenceFunc>(nameof(PyImport_AddModule), GetUnmanagedDll(_PythonDll));
            PyImport_GetModuleDict = GetDelegateByName<BorrowedReferenceFunc>(nameof(PyImport_GetModuleDict), GetUnmanagedDll(_PythonDll));
            PySys_SetArgvEx = GetDelegateByName<intIntPtrintvoidFunc>(nameof(PySys_SetArgvEx), GetUnmanagedDll(_PythonDll));
            PySys_GetObject = GetDelegateByName<StrPtrBorrowedReferenceFunc>(nameof(PySys_GetObject), GetUnmanagedDll(_PythonDll));
            PySys_SetObject = GetDelegateByName<StrPtrBorrowedReferenceintFunc>(nameof(PySys_SetObject), GetUnmanagedDll(_PythonDll));
            PyType_Modified = GetDelegateByName<BorrowedReferencevoidFunc>(nameof(PyType_Modified), GetUnmanagedDll(_PythonDll));
            PyType_IsSubtype = GetDelegateByName<BorrowedReferenceBorrowedReferenceboolFunc>(nameof(PyType_IsSubtype), GetUnmanagedDll(_PythonDll));
            PyType_GenericNew = GetDelegateByName<BorrowedReferenceBorrowedReferenceBorrowedReferenceNewReferenceFunc>(nameof(PyType_GenericNew), GetUnmanagedDll(_PythonDll));
            PyType_GenericNewPtr = GetFunctionByName(nameof(PyType_GenericNew), GetUnmanagedDll(_PythonDll));
            PyType_GenericAlloc = GetDelegateByName<BorrowedReferencenintNewReferenceFunc>(nameof(PyType_GenericAlloc), GetUnmanagedDll(_PythonDll));
            PyType_Ready = GetDelegateByName<BorrowedReferenceintFunc>(nameof(PyType_Ready), GetUnmanagedDll(_PythonDll));
            _PyType_Lookup = GetDelegateByName<BorrowedReferenceBorrowedReferenceBorrowedReferenceFunc>(nameof(_PyType_Lookup), GetUnmanagedDll(_PythonDll));
            PyObject_GenericGetAttr = GetDelegateByName<BorrowedReferenceBorrowedReferenceNewReferenceFunc>(nameof(PyObject_GenericGetAttr), GetUnmanagedDll(_PythonDll));
            PyObject_GenericGetDict = GetDelegateByName<BorrowedReferenceIntPtrNewReferenceFunc>(nameof(PyObject_GenericGetDict), GetUnmanagedDll(PythonDLL));
            PyObject_GenericSetAttr = GetDelegateByName<BorrowedReferenceBorrowedReferenceBorrowedReferenceintFunc>(nameof(PyObject_GenericSetAttr), GetUnmanagedDll(_PythonDll));
            PyObject_GC_Del = GetDelegateByName<StolenReferencevoidFunc>(nameof(PyObject_GC_Del), GetUnmanagedDll(_PythonDll));
            try
            {
                PyObject_GC_IsTracked = GetDelegateByName<BorrowedReferenceintFunc>(nameof(PyObject_GC_IsTracked), GetUnmanagedDll(_PythonDll));
            }
            catch (MissingMethodException) { }
            PyObject_GC_Track = GetDelegateByName<BorrowedReferencevoidFunc>(nameof(PyObject_GC_Track), GetUnmanagedDll(_PythonDll));
            PyObject_GC_UnTrack = GetDelegateByName<BorrowedReferencevoidFunc>(nameof(PyObject_GC_UnTrack), GetUnmanagedDll(_PythonDll));
            _PyObject_Dump = GetDelegateByName<BorrowedReferencevoidFunc>(nameof(_PyObject_Dump), GetUnmanagedDll(_PythonDll));
            PyMem_Malloc = GetDelegateByName<nintIntPtrFunc>(nameof(PyMem_Malloc), GetUnmanagedDll(_PythonDll));
            PyMem_Realloc = GetDelegateByName<IntPtrnintIntPtrFunc>(nameof(PyMem_Realloc), GetUnmanagedDll(_PythonDll));
            PyMem_Free = GetDelegateByName<IntPtrvoidFunc>(nameof(PyMem_Free), GetUnmanagedDll(_PythonDll));
            PyErr_SetString = GetDelegateByName<BorrowedReferenceStrPtrvoidFunc>(nameof(PyErr_SetString), GetUnmanagedDll(_PythonDll));
            PyErr_SetObject = GetDelegateByName<BorrowedReferenceBorrowedReferencevoidFunc>(nameof(PyErr_SetObject), GetUnmanagedDll(_PythonDll));
            PyErr_ExceptionMatches = GetDelegateByName<BorrowedReferenceintFunc>(nameof(PyErr_ExceptionMatches), GetUnmanagedDll(_PythonDll));
            PyErr_GivenExceptionMatches = GetDelegateByName<BorrowedReferenceBorrowedReferenceintFunc>(nameof(PyErr_GivenExceptionMatches), GetUnmanagedDll(_PythonDll));
            PyErr_NormalizeException = GetDelegateByName<NewReferenceNewReferenceNewReferencevoidFunc>(nameof(PyErr_NormalizeException), GetUnmanagedDll(_PythonDll));
            PyErr_Occurred = GetDelegateByName<BorrowedReferenceFunc>(nameof(PyErr_Occurred), GetUnmanagedDll(_PythonDll));
            PyErr_Fetch = GetDelegateByName<NewReferenceNewReferenceNewReferencevoidFunc>(nameof(PyErr_Fetch), GetUnmanagedDll(_PythonDll));
            PyErr_Restore = GetDelegateByName<StolenReferenceStolenReferenceStolenReferencevoidFunc>(nameof(PyErr_Restore), GetUnmanagedDll(_PythonDll));
            PyErr_Clear = GetDelegateByName<Action>(nameof(PyErr_Clear), GetUnmanagedDll(_PythonDll));
            PyErr_Print = GetDelegateByName<Action>(nameof(PyErr_Print), GetUnmanagedDll(_PythonDll));
            PyCell_Get = GetDelegateByName<BorrowedReferenceNewReferenceFunc>(nameof(PyCell_Get), GetUnmanagedDll(_PythonDll));
            PyCell_Set = GetDelegateByName<BorrowedReferenceBorrowedReferenceintFunc>(nameof(PyCell_Set), GetUnmanagedDll(_PythonDll));
            PyGC_Collect = GetDelegateByName<nintFunc>(nameof(PyGC_Collect), GetUnmanagedDll(_PythonDll));
            PyCapsule_New = GetDelegateByName<IntPtrIntPtrIntPtrNewReferenceFunc>(nameof(PyCapsule_New), GetUnmanagedDll(_PythonDll));
            PyCapsule_GetPointer = GetDelegateByName<BorrowedReferenceIntPtrIntPtrFunc>(nameof(PyCapsule_GetPointer), GetUnmanagedDll(_PythonDll));
            PyCapsule_SetPointer = GetDelegateByName<BorrowedReferenceIntPtrintFunc>(nameof(PyCapsule_SetPointer), GetUnmanagedDll(_PythonDll));
            PyLong_AsUnsignedSize_t = GetDelegateByName<BorrowedReferencenuintFunc>("PyLong_AsSize_t", GetUnmanagedDll(_PythonDll));
            PyLong_AsSignedSize_t = GetDelegateByName<BorrowedReferencenintFunc>("PyLong_AsSsize_t", GetUnmanagedDll(_PythonDll));
            PyDict_GetItemWithError = GetDelegateByName<BorrowedReferenceBorrowedReferenceBorrowedReferenceFunc>(nameof(PyDict_GetItemWithError), GetUnmanagedDll(_PythonDll));
            PyException_GetCause = GetDelegateByName<BorrowedReferenceNewReferenceFunc>(nameof(PyException_GetCause), GetUnmanagedDll(_PythonDll));
            PyException_GetTraceback = GetDelegateByName<BorrowedReferenceNewReferenceFunc>(nameof(PyException_GetTraceback), GetUnmanagedDll(_PythonDll));
            PyException_SetCause = GetDelegateByName<BorrowedReferenceStolenReferencevoidFunc>(nameof(PyException_SetCause), GetUnmanagedDll(_PythonDll));
            PyException_SetTraceback = GetDelegateByName<BorrowedReferenceBorrowedReferenceintFunc>(nameof(PyException_SetTraceback), GetUnmanagedDll(_PythonDll));
            PyThreadState_SetAsyncExcLLP64 = GetDelegateByName<uintBorrowedReferenceintFunc>("PyThreadState_SetAsyncExc", GetUnmanagedDll(_PythonDll));
            PyThreadState_SetAsyncExcLP64 = GetDelegateByName<ulongBorrowedReferenceintFunc>("PyThreadState_SetAsyncExc", GetUnmanagedDll(_PythonDll));
            PyType_GetSlot = GetDelegateByName<BorrowedReferenceTypeSlotIDIntPtrFunc>(nameof(PyType_GetSlot), GetUnmanagedDll(_PythonDll));
            PyType_FromSpecWithBases = GetDelegateByName<NativeTypeSpecBorrowedReferenceNewReferenceFunc>(nameof(PyType_FromSpecWithBases), GetUnmanagedDll(PythonDLL));

            try
            {
                _Py_NewReference = GetDelegateByName<BorrowedReferencevoidFunc>(nameof(_Py_NewReference), GetUnmanagedDll(_PythonDll));
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


        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void BorrowedReferenceAction(BorrowedReference obj);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void StolenReferenceAction(ref StolenReference obj);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void IntAction(int obj);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int IntFunc();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate nint nintFunc();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr IntPtrFunc();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate NewReference BorrowedReferenceBorrowedReferenceBorrowedReferenceNewReferenceFunc(BorrowedReference a, BorrowedReference b, BorrowedReference c);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate PyThreadState* PyThreadStateFunc();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate PyGILState PyGILStateFunc();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate BorrowedReference BorrowedReferenceFunc();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate NewReference NewReferenceFunc();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void PyThreadStatevoidFunc(PyThreadState* a);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate PyThreadState* PyInterpreterStatePyThreadStateFunc(PyInterpreterState* a);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void BorrowedReferencevoidFunc(BorrowedReference a);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int BorrowedReferenceintFunc(BorrowedReference a);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate nint StrPtrnintFunc(StrPtr a);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void IntPtrvoidFunc(IntPtr a);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate NewReference BorrowedReferenceNewReferenceFunc(BorrowedReference a);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate bool BorrowedReferenceboolFunc(BorrowedReference a);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate NewReference longNewReferenceFunc(long a);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate NewReference ulongNewReferenceFunc(ulong a);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate long BorrowedReferencelongFunc(BorrowedReference a);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate ulong BorrowedReferenceulongFunc(BorrowedReference a);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate NewReference IntPtrNewReferenceFunc(IntPtr a);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr BorrowedReferenceIntPtrFunc(BorrowedReference a);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate NewReference doubleNewReferenceFunc(double a);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate double BorrowedReferencedoubleFunc(BorrowedReference a);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate nint BorrowedReferencenintFunc(BorrowedReference a);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate NewReference intNewReferenceFunc(int a);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate NewReference StrPtrNewReferenceFunc(StrPtr a);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate NewReference nintNewReferenceFunc(nint a);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void PyGILStatevoidFunc(PyGILState a);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate BorrowedReference BorrowedReferenceBorrowedReferenceFunc(BorrowedReference a);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate BorrowedReference StrPtrBorrowedReferenceFunc(StrPtr a);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void StolenReferencevoidFunc(ref StolenReference a);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr nintIntPtrFunc(nint a);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate nuint BorrowedReferencenuintFunc(BorrowedReference a);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int intIntPtrintFunc(int a, IntPtr b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate NewReference StrPtrBorrowedReferenceNewReferenceFunc(StrPtr a, BorrowedReference b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int BorrowedReferenceStrPtrintFunc(BorrowedReference a, StrPtr b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate NewReference BorrowedReferenceStrPtrNewReferenceFunc(BorrowedReference a, StrPtr b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int BorrowedReferenceBorrowedReferenceintFunc(BorrowedReference a, BorrowedReference b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate NewReference BorrowedReferenceBorrowedReferenceNewReferenceFunc(BorrowedReference a, BorrowedReference b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate NewReference BorrowedReferencenintNewReferenceFunc(BorrowedReference a, nint b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int BorrowedReferencenintintFunc(BorrowedReference a, nint b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate nint BorrowedReferenceBorrowedReferencenintFunc(BorrowedReference a, BorrowedReference b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate NewReference IntPtrnintNewReferenceFunc(IntPtr a, nint b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate BorrowedReference BorrowedReferenceBorrowedReferenceBorrowedReferenceFunc(BorrowedReference a, BorrowedReference b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate BorrowedReference BorrowedReferenceStrPtrBorrowedReferenceFunc(BorrowedReference a, StrPtr b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate BorrowedReference BorrowedReferencenintBorrowedReferenceFunc(BorrowedReference a, nint b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int StrPtrBorrowedReferenceintFunc(StrPtr a, BorrowedReference b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate bool BorrowedReferenceBorrowedReferenceboolFunc(BorrowedReference a, BorrowedReference b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr IntPtrnintIntPtrFunc(IntPtr a, nint b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void BorrowedReferenceStrPtrvoidFunc(BorrowedReference a, StrPtr b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void BorrowedReferenceBorrowedReferencevoidFunc(BorrowedReference a, BorrowedReference b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr BorrowedReferenceIntPtrIntPtrFunc(BorrowedReference a, IntPtr b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int BorrowedReferenceIntPtrintFunc(BorrowedReference a, IntPtr b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void BorrowedReferenceStolenReferencevoidFunc(BorrowedReference a, ref StolenReference b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int uintBorrowedReferenceintFunc(uint a, BorrowedReference b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int ulongBorrowedReferenceintFunc(ulong a, BorrowedReference b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate NewReference BorrowedReferenceIntPtrNewReferenceFunc(BorrowedReference a, IntPtr b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr BorrowedReferenceTypeSlotIDIntPtrFunc(BorrowedReference a, TypeSlotID b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int BorrowedReferenceStrPtrBorrowedReferenceintFunc(BorrowedReference a, StrPtr b, BorrowedReference c);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int BorrowedReferenceBorrowedReferenceBorrowedReferenceintFunc(BorrowedReference a, BorrowedReference b, BorrowedReference c);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int BorrowedReferenceBorrowedReferenceintintFunc(BorrowedReference a, BorrowedReference b, int c);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate NewReference StrPtrIntPtrintNewReferenceFunc(StrPtr a, IntPtr b, int c);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int BorrowedReferencenintBorrowedReferenceintFunc(BorrowedReference a, nint b, BorrowedReference c);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate NewReference BorrowedReferencenintnintNewReferenceFunc(BorrowedReference a, nint b, nint c);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int BorrowedReferencenintnintintFunc(BorrowedReference a, nint b, nint c);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int BorrowedReferencenintStolenReferenceintFunc(BorrowedReference a, nint b, ref StolenReference c);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int BorrowedReferenceStrPtrIntPtrintFunc(BorrowedReference a, StrPtr b, IntPtr c);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void intIntPtrintvoidFunc(int a, IntPtr b, int c);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void StolenReferenceStolenReferenceStolenReferencevoidFunc(ref StolenReference a, ref StolenReference b, ref StolenReference c);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate NewReference IntPtrIntPtrIntPtrNewReferenceFunc(IntPtr a, IntPtr b, IntPtr c);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void Py_buffervoidFunc(ref Py_buffer a);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int StrPtrPyCompilerFlagsintFunc(StrPtr a, in PyCompilerFlags b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int Py_buffercharintFunc(ref Py_buffer a, char b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate NewReference NativeTypeSpecBorrowedReferenceNewReferenceFunc(in NativeTypeSpec a, BorrowedReference b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int BorrowedReferencePy_bufferintintFunc(BorrowedReference a, out Py_buffer b, int c);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void NewReferenceNewReferenceNewReferencevoidFunc(out NewReference a, out NewReference b, out NewReference c);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate NewReference StrPtrRunFlagTypeBorrowedReferenceBorrowedReferenceinPyCompilerFlagsNewReferenceFunc(StrPtr a, RunFlagType b, BorrowedReference c, BorrowedReference d, in PyCompilerFlags e);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate NewReference StrPtrBorrowedReferenceintinPyCompilerFlagsintNewReferenceFunc(StrPtr a, BorrowedReference b, int c, in PyCompilerFlags d, int e);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr refPy_buffernintArrayIntPtrFunc(ref Py_buffer a, nint[] b);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int refPy_bufferIntPtrIntPtrcharintFunc(ref Py_buffer a, IntPtr b, IntPtr c, char d);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int IntPtrrefPy_bufferIntPtrcharintFunc(IntPtr a, ref Py_buffer b, IntPtr c, char d);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void intIntPtrIntPtrintcharvoidFunc(int a, IntPtr b, IntPtr c, int d, char e);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int refPy_bufferBorrowedReferenceIntPtrIntPtrintintintFunc(ref Py_buffer a, BorrowedReference b, IntPtr c, IntPtr d, int e, int f);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int BorrowedReferencenintnintBorrowedReferenceIntFunc(BorrowedReference a, nint b, nint c, BorrowedReference d);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate NewReference IntPtrnintIntPtrIntPtrNewReferenceFunc(IntPtr a, nint b, IntPtr c, IntPtr d);

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
        internal static StrPtrRunFlagTypeBorrowedReferenceBorrowedReferenceinPyCompilerFlagsNewReferenceFunc PyRun_StringFlags { get; }
        internal static BorrowedReferenceBorrowedReferenceBorrowedReferenceNewReferenceFunc PyEval_EvalCode { get; }
        internal static StrPtrBorrowedReferenceintinPyCompilerFlagsintNewReferenceFunc Py_CompileStringObject { get; }
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
        internal static refPy_buffernintArrayIntPtrFunc PyBuffer_GetPointer { get; }
        internal static refPy_bufferIntPtrIntPtrcharintFunc PyBuffer_FromContiguous { get; }
        internal static IntPtrrefPy_bufferIntPtrcharintFunc PyBuffer_ToContiguous { get; }
        internal static intIntPtrIntPtrintcharvoidFunc PyBuffer_FillContiguousStrides { get; }
        internal static refPy_bufferBorrowedReferenceIntPtrIntPtrintintintFunc PyBuffer_FillInfo { get; }
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
        internal static BorrowedReferencenintnintBorrowedReferenceIntFunc PySequence_SetSlice { get; }
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
        internal static IntPtrnintIntPtrIntPtrNewReferenceFunc PyUnicode_DecodeUTF16 { get; }
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
        internal static BorrowedReferencenintnintBorrowedReferenceIntFunc PyList_SetSlice { get; }
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
        internal static IntPtr PyType_GenericNewPtr { get; }
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
