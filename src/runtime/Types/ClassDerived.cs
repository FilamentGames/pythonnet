using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using AOT;

using Python.Runtime.Native;

#pragma warning disable CS0618 // Type or member is obsolete. OK for internal use
using static Python.Runtime.PythonDerivedType;
#pragma warning restore CS0618 // Type or member is obsolete

namespace Python.Runtime
{
    /// <summary>
    /// Managed class that provides the implementation for reflected types.
    /// Managed classes and value types are represented in Python by actual
    /// Python type objects. Each of those type objects is associated with
    /// an instance of ClassObject, which provides its implementation.
    /// </summary>
    /// <remarks>
    /// interface used to identify which C# types were dynamically created as python subclasses
    /// </remarks>
    public interface IPythonDerivedType
    {
    }

    [Serializable]
    internal class ClassDerivedObject : ClassObject
    {

        static ClassDerivedObject()
        {
        }

        public static void Reset()
        {
        }

        internal ClassDerivedObject(Type tp) : base(tp)
        {
        }

        protected override NewReference NewObjectToPython(object obj, BorrowedReference tp)
        {
            var self = base.NewObjectToPython(obj, tp);

            SetPyObj((IPythonDerivedType)obj, self.Borrow());

            // Decrement the python object's reference count.
            // This doesn't actually destroy the object, it just sets the reference to this object
            // to be a weak reference and it will be destroyed when the C# object is destroyed.
            Runtime.XDecref(self.Steal());

            return Converter.ToPython(obj, type.Value);
        }

        protected override void SetTypeNewSlot(BorrowedReference pyType, SlotsHolder slotsHolder)
        {
            // Python derived types rely on base tp_new and overridden __init__
        }

        [MonoPInvokeCallback(typeof(TpFreeAction))]
        public new static void tp_dealloc(NewReference ob)
        {
            var self = (CLRObject?)GetManagedObject(ob.Borrow());

            // don't let the python GC destroy this object
            Runtime.PyObject_GC_UnTrack(ob.Borrow());

            // self may be null after Shutdown begun
            if (self is not null)
            {
                // The python should now have a ref count of 0, but we don't actually want to
                // deallocate the object until the C# object that references it is destroyed.
                // So we don't call PyObject_GC_Del here and instead we set the python
                // reference to a weak reference so that the C# object can be collected.
                GCHandle oldHandle = GetGCHandle(ob.Borrow());
                GCHandle gc = GCHandle.Alloc(self, GCHandleType.Weak);
                SetGCHandle(ob.Borrow(), gc);
                oldHandle.Free();
            }
        }

        /// <summary>
        /// No-op clear. Real cleanup happens in <seealso cref="Finalize(IntPtr)"/>
        /// </summary>
        [MonoPInvokeCallback(typeof(TpClearFunc))]
        public new static int tp_clear(BorrowedReference ob) => 0;

        /// <summary>
        /// Called from Converter.ToPython for types that are python subclasses of managed types.
        /// The referenced python object is returned instead of a new wrapper.
        /// </summary>
        internal static NewReference ToPython(IPythonDerivedType obj)
        {
            // derived types have a __pyobj__ field that gets set to the python
            // object in the overridden constructor
            BorrowedReference self;
            try
            {
                self = GetPyObj(obj).CheckRun();
            }
            catch (RuntimeShutdownException e)
            {
                Exceptions.SetError(e);
                return default;
            }

            var result = new NewReference(self);

            // when the C# constructor creates the python object it starts as a weak
            // reference with a reference count of 0. Now we're passing this object
            // to Python the reference count needs to be incremented and the reference
            // needs to be replaced with a strong reference to stop the C# object being
            // collected while Python still has a reference to it.
            if (Runtime.Refcount(self) == 1)
            {
                Runtime._Py_NewReference(self);
                GCHandle weak = GetGCHandle(self);
                var clrObject = GetManagedObject(self);
                GCHandle gc = GCHandle.Alloc(clrObject, GCHandleType.Normal);
                SetGCHandle(self, gc);
                weak.Free();

                // now the object has a python reference it's safe for the python GC to track it
                Runtime.PyObject_GC_Track(self);
            }

            return result;
        }

        /// <summary>
        /// Creates a new managed type derived from a base type with any virtual
        /// methods overridden to call out to python if the associated python
        /// object has overridden the method.
        /// </summary>
        internal static Type CreateDerivedType(string name,
            Type baseType,
            BorrowedReference py_dict,
            string? namespaceStr,
            string? assemblyName,
            string moduleName = "Python.Runtime.Dynamic.dll")
        {
            // TODO: clean up
            if (null != namespaceStr)
            {
                name = namespaceStr + "." + name;
            }

            if (null == assemblyName)
            {
                assemblyName = "Python.Runtime.Dynamic";
            }

            Type baseClass = baseType;
            var interfaces = new List<Type> { typeof(IPythonDerivedType) };

            // if the base type is an interface then use System.Object as the base class
            // and add the base type to the list of interfaces this new class will implement.
            if (baseType.IsInterface)
            {
                interfaces.Add(baseType);
                baseClass = typeof(object);
            }

            // override any constructors
            ConstructorInfo[] constructors = baseClass.GetConstructors();

            // Override any properties explicitly overridden in python
            var pyProperties = new HashSet<string>();
            if (py_dict != null && Runtime.PyDict_Check(py_dict))
            {
                using var dict = new PyDict(py_dict);
                using var keys = dict.Keys();
                foreach (PyObject pyKey in keys)
                {
                    using var value = dict[pyKey];
                    if (value.HasAttr("_clr_property_type_"))
                    {
                        string propertyName = pyKey.ToString()!;
                        pyProperties.Add(propertyName);
                    }
                    pyKey.Dispose();
                }
            }

            // override any virtual methods not already overridden by the properties above
            MethodInfo[] methods = baseType.GetMethods();
            var virtualMethods = new HashSet<string>();
            foreach (MethodInfo method in methods)
            {
                if (!method.Attributes.HasFlag(MethodAttributes.Virtual) |
                    method.Attributes.HasFlag(MethodAttributes.Final))
                {
                    continue;
                }

                // skip if this property has already been overridden
                if ((method.Name.StartsWith("get_") || method.Name.StartsWith("set_"))
                    && pyProperties.Contains(method.Name.Substring(4)))
                {
                    continue;
                }

                // keep track of the virtual methods redirected to the python instance
                virtualMethods.Add(method.Name);
            }

            // Add any additional methods and properties explicitly exposed from Python.
            if (py_dict != null && Runtime.PyDict_Check(py_dict))
            {
                using var dict = new PyDict(py_dict);
                using var keys = dict.Keys();
                foreach (PyObject pyKey in keys)
                {
                    using var value = dict[pyKey];
                    if (value.HasAttr("_clr_return_type_") && value.HasAttr("_clr_arg_types_"))
                    {
                        string methodName = pyKey.ToString()!;

                        // if this method has already been redirected to the python method skip it
                        if (virtualMethods.Contains(methodName))
                        {
                            continue;
                        }
                    }
                    pyKey.Dispose();
                }
            }

            return null;
        }
    }

    /// <summary>
    /// PythonDerivedType contains static methods used by the dynamically created
    /// derived type that allow it to call back into python from overridden virtual
    /// methods, and also handle the construction and destruction of the python
    /// object.
    /// </summary>
    /// <remarks>
    /// This has to be public as it's called from methods on dynamically built classes
    /// potentially in other assemblies.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete(Util.InternalUseOnly)]
    public class PythonDerivedType
    {
        internal const string PyObjName = "__pyobj__";
        internal const BindingFlags PyObjFlags = BindingFlags.Instance | BindingFlags.NonPublic;

        /// <summary>
        /// This is the implementation of the overridden methods in the derived
        /// type. It looks for a python method with the same name as the method
        /// on the managed base class and if it exists and isn't the managed
        /// method binding (i.e. it has been overridden in the derived python
        /// class) it calls it, otherwise it calls the base method.
        /// </summary>
        public static T? InvokeMethod<T>(IPythonDerivedType obj, string methodName, string origMethodName,
            object[] args, RuntimeMethodHandle methodHandle, RuntimeTypeHandle declaringTypeHandle)
        {
            var self = GetPyObj(obj);

            if (null != self.Ref)
            {
                var disposeList = new List<PyObject>();
                PyGILState gs = Runtime.PyGILState_Ensure();
                try
                {
                    using var pyself = new PyObject(self.CheckRun());
                    using PyObject method = pyself.GetAttr(methodName, Runtime.None);
                    if (method.Reference != Runtime.PyNone)
                    {
                        // if the method hasn't been overridden then it will be a managed object
                        ManagedType? managedMethod = ManagedType.GetManagedObject(method.Reference);
                        if (null == managedMethod)
                        {
                            var pyargs = new PyObject[args.Length];
                            for (var i = 0; i < args.Length; ++i)
                            {
                                pyargs[i] = Converter.ToPythonImplicit(args[i]).MoveToPyObject();
                                disposeList.Add(pyargs[i]);
                            }

                            PyObject py_result = method.Invoke(pyargs);
                            var clrMethod = methodHandle != default
                                ? MethodBase.GetMethodFromHandle(methodHandle, declaringTypeHandle)
                                : null;
                            PyTuple? result_tuple = MarshalByRefsBack(args, clrMethod, py_result, outsOffset: 1);
                            return result_tuple is not null
                                ? result_tuple[0].As<T>()
                                : py_result.As<T>();
                        }
                    }
                }
                finally
                {
                    foreach (PyObject x in disposeList)
                    {
                        x?.Dispose();
                    }
                    Runtime.PyGILState_Release(gs);
                }
            }

            if (origMethodName == null)
            {
                throw new NotImplementedException("Python object does not have a '" + methodName + "' method");
            }

            return (T)obj.GetType().InvokeMember(origMethodName,
                BindingFlags.InvokeMethod,
                null,
                obj,
                args);
        }

        public static void InvokeMethodVoid(IPythonDerivedType obj, string methodName, string origMethodName,
            object?[] args, RuntimeMethodHandle methodHandle, RuntimeTypeHandle declaringTypeHandle)
        {
            var self = GetPyObj(obj);
            if (null != self.Ref)
            {
                var disposeList = new List<PyObject>();
                PyGILState gs = Runtime.PyGILState_Ensure();
                try
                {
                    using var pyself = new PyObject(self.CheckRun());
                    using PyObject method = pyself.GetAttr(methodName, Runtime.None);
                    if (method.Reference != Runtime.None)
                    {
                        // if the method hasn't been overridden then it will be a managed object
                        ManagedType? managedMethod = ManagedType.GetManagedObject(method);
                        if (null == managedMethod)
                        {
                            var pyargs = new PyObject[args.Length];
                            for (var i = 0; i < args.Length; ++i)
                            {
                                pyargs[i] = Converter.ToPythonImplicit(args[i]).MoveToPyObject();
                                disposeList.Add(pyargs[i]);
                            }

                            PyObject py_result = method.Invoke(pyargs);
                            var clrMethod = methodHandle != default
                                ? MethodBase.GetMethodFromHandle(methodHandle, declaringTypeHandle)
                                : null;
                            MarshalByRefsBack(args, clrMethod, py_result, outsOffset: 0);
                            return;
                        }
                    }
                }
                finally
                {
                    foreach (PyObject x in disposeList)
                    {
                        x?.Dispose();
                    }
                    Runtime.PyGILState_Release(gs);
                }
            }

            if (origMethodName == null)
            {
                throw new NotImplementedException($"Python object does not have a '{methodName}' method");
            }

            obj.GetType().InvokeMember(origMethodName,
                BindingFlags.InvokeMethod,
                null,
                obj,
                args);
        }

        /// <summary>
        /// If the method has byref arguments, reinterprets Python return value
        /// as a tuple of new values for those arguments, and updates corresponding
        /// elements of <paramref name="args"/> array.
        /// </summary>
        private static PyTuple? MarshalByRefsBack(object?[] args, MethodBase? method, PyObject pyResult, int outsOffset)
        {
            if (method is null) return null;

            var parameters = method.GetParameters();
            PyTuple? outs = null;
            int byrefIndex = 0;
            for (int i = 0; i < parameters.Length; ++i)
            {
                Type type = parameters[i].ParameterType;
                if (!type.IsByRef)
                {
                    continue;
                }

                type = type.GetElementType();

                if (outs is null)
                {
                    outs = new PyTuple(pyResult);
                    pyResult.Dispose();
                }

                args[i] = outs[byrefIndex + outsOffset].AsManagedObject(type);
                byrefIndex++;
            }
            if (byrefIndex > 0 && outs!.Length() > byrefIndex + outsOffset)
                throw new ArgumentException("Too many output parameters");

            return outs;
        }

        public static T? InvokeGetProperty<T>(IPythonDerivedType obj, string propertyName)
        {
            var self = GetPyObj(obj);

            if (null == self.Ref)
            {
                throw new NullReferenceException("Instance must be specified when getting a property");
            }

            PyGILState gs = Runtime.PyGILState_Ensure();
            try
            {
                using var pyself = new PyObject(self.CheckRun());
                using var pyvalue = pyself.GetAttr(propertyName);
                return pyvalue.As<T>();
            }
            finally
            {
                Runtime.PyGILState_Release(gs);
            }
        }

        public static void InvokeSetProperty<T>(IPythonDerivedType obj, string propertyName, T value)
        {
            var self = GetPyObj(obj);

            if (null == self.Ref)
            {
                throw new NullReferenceException("Instance must be specified when setting a property");
            }

            PyGILState gs = Runtime.PyGILState_Ensure();
            try
            {
                using var pyself = new PyObject(self.CheckRun());
                using var pyvalue = Converter.ToPythonImplicit(value).MoveToPyObject();
                pyself.SetAttr(propertyName, pyvalue);
            }
            finally
            {
                Runtime.PyGILState_Release(gs);
            }
        }

        public static void InvokeCtor(IPythonDerivedType obj, string origCtorName, object[] args)
        {
            var selfRef = GetPyObj(obj);
            if (selfRef.Ref == null)
            {
                // this might happen when the object is created from .NET
                using var _ = Py.GIL();
                // In the end we decrement the python object's reference count.
                // This doesn't actually destroy the object, it just sets the reference to this object
                // to be a weak reference and it will be destroyed when the C# object is destroyed.
                using var self = CLRObject.GetReference(obj, obj.GetType());
                SetPyObj(obj, self.Borrow());
            }

            // call the base constructor
            obj.GetType().InvokeMember(origCtorName,
                BindingFlags.InvokeMethod,
                null,
                obj,
                args);
        }

        public static void PyFinalize(IPythonDerivedType obj)
        {
            // the C# object is being destroyed which must mean there are no more
            // references to the Python object as well
            var self = GetPyObj(obj);
            Finalizer.Instance.AddDerivedFinalizedObject(ref self.RawObj, self.Run);
        }

        internal static void Finalize(IntPtr derived)
        {
            var @ref = NewReference.DangerousFromPointer(derived);

            ClassBase.tp_clear(@ref.Borrow());

            var type = Runtime.PyObject_TYPE(@ref.Borrow());

            if (!Runtime.HostedInPython || Runtime.TypeManagerInitialized)
            {
                // rare case when it's needed
                // matches correspdonging PyObject_GC_UnTrack
                // in ClassDerivedObject.tp_dealloc
                Runtime.PyObject_GC_Del(@ref.Steal());

                // must decref our type
                Runtime.XDecref(StolenReference.DangerousFromPointer(type.DangerousGetAddress()));
            }
        }

        internal static FieldInfo? GetPyObjField(Type type) => type.GetField(PyObjName, PyObjFlags);

        internal static UnsafeReferenceWithRun GetPyObj(IPythonDerivedType obj)
        {
            FieldInfo fi = GetPyObjField(obj.GetType())!;
            return (UnsafeReferenceWithRun)fi.GetValue(obj);
        }

        internal static void SetPyObj(IPythonDerivedType obj, BorrowedReference pyObj)
        {
            FieldInfo fi = GetPyObjField(obj.GetType())!;
            fi.SetValue(obj, new UnsafeReferenceWithRun(pyObj));
        }
    }
}
