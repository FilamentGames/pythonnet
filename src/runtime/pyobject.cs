using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace Python.Runtime
{
    public interface IPyDisposable : IDisposable
    {
        IntPtr[] GetTrackedHandles();
    }

    /// <summary>
    /// Represents a generic Python object. The methods of this class are
    /// generally equivalent to the Python "abstract object API". See
    /// PY2: https://docs.python.org/2/c-api/object.html
    /// PY3: https://docs.python.org/3/c-api/object.html
    /// for details.
    /// </summary>
    public class PyObject : IEnumerable, IPyDisposable
    {
#if TRACE_ALLOC
        /// <summary>
        /// Trace stack for PyObject's construction
        /// </summary>
        public StackTrace Traceback { get; private set; }
#endif

        protected internal IntPtr obj = IntPtr.Zero;

        internal BorrowedReference Reference => new BorrowedReference(obj);

        /// <summary>
        /// PyObject Constructor
        /// </summary>
        /// <remarks>
        /// Creates a new PyObject from an IntPtr object reference. Note that
        /// the PyObject instance assumes ownership of the object reference
        /// and the reference will be DECREFed when the PyObject is garbage
        /// collected or explicitly disposed.
        /// </remarks>
        public PyObject(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero) throw new ArgumentNullException(nameof(ptr));

            obj = ptr;
            Finalizer.Instance.ThrottledCollect();
#if TRACE_ALLOC
            Traceback = new StackTrace(1);
#endif
        }

        /// <summary>
        /// Creates new <see cref="PyObject"/> pointing to the same object as
        /// the <paramref name="reference"/>. Increments refcount, allowing <see cref="PyObject"/>
        /// to have ownership over its own reference.
        /// </summary>
        internal PyObject(BorrowedReference reference)
        {
            if (reference.IsNull) throw new ArgumentNullException(nameof(reference));

            obj = Runtime.SelfIncRef(reference.DangerousGetAddress());
            Finalizer.Instance.ThrottledCollect();
#if TRACE_ALLOC
            Traceback = new StackTrace(1);
#endif
        }

        // Protected default constructor to allow subclasses to manage
        // initialization in different ways as appropriate.
        [Obsolete("Please, always use PyObject(*Reference)")]
        protected PyObject()
        {
            Finalizer.Instance.ThrottledCollect();
#if TRACE_ALLOC
            Traceback = new StackTrace(1);
#endif
        }

        // Ensure that encapsulated Python object is decref'ed appropriately
        // when the managed wrapper is garbage-collected.
        ~PyObject()
        {
            if (obj == IntPtr.Zero)
            {
                return;
            }
            Finalizer.Instance.AddFinalizedObject(this);
        }


        /// <summary>
        /// Handle Property
        /// </summary>
        /// <remarks>
        /// Gets the native handle of the underlying Python object. This
        /// value is generally for internal use by the PythonNet runtime.
        /// </remarks>
        public IntPtr Handle
        {
            get { return obj; }
        }


        /// <summary>
        /// Gets raw Python proxy for this object (bypasses all conversions,
        /// except <c>null</c> &lt;==&gt; <c>None</c>)
        /// </summary>
        /// <remarks>
        /// Given an arbitrary managed object, return a Python instance that
        /// reflects the managed object.
        /// </remarks>
        public static PyObject FromManagedObject(object ob)
        {
            // Special case: if ob is null, we return None.
            if (ob == null)
            {
                Runtime.XIncref(Runtime.PyNone);
                return new PyObject(Runtime.PyNone);
            }
            IntPtr op = CLRObject.GetInstHandle(ob);
            return new PyObject(op);
        }


        /// <summary>
        /// AsManagedObject Method
        /// </summary>
        /// <remarks>
        /// Return a managed object of the given type, based on the
        /// value of the Python object.
        /// </remarks>
        public object AsManagedObject(Type t)
        {
            object result;
            if (!Converter.ToManaged(obj, t, out result, false))
            {
                throw new InvalidCastException("cannot convert object to target type");
            }
            return result;
        }

        /// <summary>
        /// As Method
        /// </summary>
        /// <remarks>
        /// Return a managed object of the given type, based on the
        /// value of the Python object.
        /// </remarks>
        public T As<T>()
        {
            if (typeof(T) == typeof(PyObject) || typeof(T) == typeof(object))
            {
                return (T)(this as object);
            }
            object result;
            if (!Converter.ToManaged(obj, typeof(T), out result, false))
            {
                throw new InvalidCastException("cannot convert object to target type");
            }
            return (T)result;
        }


        /// <summary>
        /// Dispose Method
        /// </summary>
        /// <remarks>
        /// The Dispose method provides a way to explicitly release the
        /// Python object represented by a PyObject instance. It is a good
        /// idea to call Dispose on PyObjects that wrap resources that are
        /// limited or need strict lifetime control. Otherwise, references
        /// to Python objects will not be released until a managed garbage
        /// collection occurs.
        /// </remarks>
        protected virtual void Dispose(bool disposing)
        {
            if (this.obj == IntPtr.Zero)
            {
                return;
            }

            if (Runtime.Py_IsInitialized() == 0)
                throw new InvalidOperationException("Python runtime must be initialized");

            if (!Runtime.IsFinalizing)
            {
                long refcount = Runtime.Refcount(this.obj);
                Debug.Assert(refcount > 0, "Object refcount is 0 or less");

                if (refcount == 1)
                {
                    Runtime.PyErr_Fetch(out var errType, out var errVal, out var traceback);

                    try
                    {
                        Runtime.XDecref(this.obj);
                        Runtime.CheckExceptionOccurred();
                    }
                    finally
                    {
                        // Python requires finalizers to preserve exception:
                        // https://docs.python.org/3/extending/newtypes.html#finalization-and-de-allocation
                        Runtime.PyErr_Restore(errType, errVal, traceback);
                    }
                }
                else
                {
                    Runtime.XDecref(this.obj);
                }
            }
            this.obj = IntPtr.Zero;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IntPtr[] GetTrackedHandles()
        {
            return new IntPtr[] { obj };
        }

        /// <summary>
        /// GetPythonType Method
        /// </summary>
        /// <remarks>
        /// Returns the Python type of the object. This method is equivalent
        /// to the Python expression: type(object).
        /// </remarks>
        public PyObject GetPythonType()
        {
            IntPtr tp = Runtime.PyObject_Type(obj);
            return new PyObject(tp);
        }


        /// <summary>
        /// TypeCheck Method
        /// </summary>
        /// <remarks>
        /// Returns true if the object o is of type typeOrClass or a subtype
        /// of typeOrClass.
        /// </remarks>
        public bool TypeCheck(PyObject typeOrClass)
        {
            if (typeOrClass == null) throw new ArgumentNullException(nameof(typeOrClass));

            return Runtime.PyObject_TypeCheck(obj, typeOrClass.obj);
        }


        /// <summary>
        /// HasAttr Method
        /// </summary>
        /// <remarks>
        /// Returns true if the object has an attribute with the given name.
        /// </remarks>
        public bool HasAttr(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            return Runtime.PyObject_HasAttrString(obj, name) != 0;
        }


        /// <summary>
        /// HasAttr Method
        /// </summary>
        /// <remarks>
        /// Returns true if the object has an attribute with the given name,
        /// where name is a PyObject wrapping a string or unicode object.
        /// </remarks>
        public bool HasAttr(PyObject name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            return Runtime.PyObject_HasAttr(obj, name.obj) != 0;
        }


        /// <summary>
        /// GetAttr Method
        /// </summary>
        /// <remarks>
        /// Returns the named attribute of the Python object, or raises a
        /// PythonException if the attribute access fails.
        /// </remarks>
        public PyObject GetAttr(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            IntPtr op = Runtime.PyObject_GetAttrString(obj, name);
            if (op == IntPtr.Zero)
            {
                throw new PythonException();
            }
            return new PyObject(op);
        }


        /// <summary>
        /// GetAttr Method. Returns fallback value if getting attribute fails for any reason.
        /// </summary>
        /// <remarks>
        /// Returns the named attribute of the Python object, or the given
        /// default object if the attribute access fails.
        /// </remarks>
        public PyObject GetAttr(string name, PyObject _default)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            IntPtr op = Runtime.PyObject_GetAttrString(obj, name);
            if (op == IntPtr.Zero)
            {
                Runtime.PyErr_Clear();
                return _default;
            }
            return new PyObject(op);
        }


        /// <summary>
        /// GetAttr Method
        /// </summary>
        /// <remarks>
        /// Returns the named attribute of the Python object or raises a
        /// PythonException if the attribute access fails. The name argument
        /// is a PyObject wrapping a Python string or unicode object.
        /// </remarks>
        public PyObject GetAttr(PyObject name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            IntPtr op = Runtime.PyObject_GetAttr(obj, name.obj);
            if (op == IntPtr.Zero)
            {
                throw new PythonException();
            }
            return new PyObject(op);
        }


        /// <summary>
        /// GetAttr Method
        /// </summary>
        /// <remarks>
        /// Returns the named attribute of the Python object, or the given
        /// default object if the attribute access fails. The name argument
        /// is a PyObject wrapping a Python string or unicode object.
        /// </remarks>
        public PyObject GetAttr(PyObject name, PyObject _default)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            IntPtr op = Runtime.PyObject_GetAttr(obj, name.obj);
            if (op == IntPtr.Zero)
            {
                Runtime.PyErr_Clear();
                return _default;
            }
            return new PyObject(op);
        }


        /// <summary>
        /// SetAttr Method
        /// </summary>
        /// <remarks>
        /// Set an attribute of the object with the given name and value. This
        /// method throws a PythonException if the attribute set fails.
        /// </remarks>
        public void SetAttr(string name, PyObject value)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (value == null) throw new ArgumentNullException(nameof(value));

            int r = Runtime.PyObject_SetAttrString(obj, name, value.obj);
            if (r < 0)
            {
                throw new PythonException();
            }
        }


        /// <summary>
        /// SetAttr Method
        /// </summary>
        /// <remarks>
        /// Set an attribute of the object with the given name and value,
        /// where the name is a Python string or unicode object. This method
        /// throws a PythonException if the attribute set fails.
        /// </remarks>
        public void SetAttr(PyObject name, PyObject value)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (value == null) throw new ArgumentNullException(nameof(value));

            int r = Runtime.PyObject_SetAttr(obj, name.obj, value.obj);
            if (r < 0)
            {
                throw new PythonException();
            }
        }


        /// <summary>
        /// DelAttr Method
        /// </summary>
        /// <remarks>
        /// Delete the named attribute of the Python object. This method
        /// throws a PythonException if the attribute set fails.
        /// </remarks>
        public void DelAttr(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            int r = Runtime.PyObject_SetAttrString(obj, name, IntPtr.Zero);
            if (r < 0)
            {
                throw new PythonException();
            }
        }


        /// <summary>
        /// DelAttr Method
        /// </summary>
        /// <remarks>
        /// Delete the named attribute of the Python object, where name is a
        /// PyObject wrapping a Python string or unicode object. This method
        /// throws a PythonException if the attribute set fails.
        /// </remarks>
        public void DelAttr(PyObject name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            int r = Runtime.PyObject_SetAttr(obj, name.obj, IntPtr.Zero);
            if (r < 0)
            {
                throw new PythonException();
            }
        }


        /// <summary>
        /// GetItem Method
        /// </summary>
        /// <remarks>
        /// For objects that support the Python sequence or mapping protocols,
        /// return the item at the given object index. This method raises a
        /// PythonException if the indexing operation fails.
        /// </remarks>
        public virtual PyObject GetItem(PyObject key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            IntPtr op = Runtime.PyObject_GetItem(obj, key.obj);
            if (op == IntPtr.Zero)
            {
                throw new PythonException();
            }
            return new PyObject(op);
        }


        /// <summary>
        /// GetItem Method
        /// </summary>
        /// <remarks>
        /// For objects that support the Python sequence or mapping protocols,
        /// return the item at the given string index. This method raises a
        /// PythonException if the indexing operation fails.
        /// </remarks>
        public virtual PyObject GetItem(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            using (var pyKey = new PyString(key))
            {
                return GetItem(pyKey);
            }
        }


        /// <summary>
        /// GetItem Method
        /// </summary>
        /// <remarks>
        /// For objects that support the Python sequence or mapping protocols,
        /// return the item at the given numeric index. This method raises a
        /// PythonException if the indexing operation fails.
        /// </remarks>
        public virtual PyObject GetItem(int index)
        {
            using (var key = new PyInt(index))
            {
                return GetItem(key);
            }
        }


        /// <summary>
        /// SetItem Method
        /// </summary>
        /// <remarks>
        /// For objects that support the Python sequence or mapping protocols,
        /// set the item at the given object index to the given value. This
        /// method raises a PythonException if the set operation fails.
        /// </remarks>
        public virtual void SetItem(PyObject key, PyObject value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));

            int r = Runtime.PyObject_SetItem(obj, key.obj, value.obj);
            if (r < 0)
            {
                throw new PythonException();
            }
        }


        /// <summary>
        /// SetItem Method
        /// </summary>
        /// <remarks>
        /// For objects that support the Python sequence or mapping protocols,
        /// set the item at the given string index to the given value. This
        /// method raises a PythonException if the set operation fails.
        /// </remarks>
        public virtual void SetItem(string key, PyObject value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));

            using (var pyKey = new PyString(key))
            {
                SetItem(pyKey, value);
            }
        }


        /// <summary>
        /// SetItem Method
        /// </summary>
        /// <remarks>
        /// For objects that support the Python sequence or mapping protocols,
        /// set the item at the given numeric index to the given value. This
        /// method raises a PythonException if the set operation fails.
        /// </remarks>
        public virtual void SetItem(int index, PyObject value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            using (var pyindex = new PyInt(index))
            {
                SetItem(pyindex, value);
            }
        }


        /// <summary>
        /// DelItem Method
        /// </summary>
        /// <remarks>
        /// For objects that support the Python sequence or mapping protocols,
        /// delete the item at the given object index. This method raises a
        /// PythonException if the delete operation fails.
        /// </remarks>
        public virtual void DelItem(PyObject key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            int r = Runtime.PyObject_DelItem(obj, key.obj);
            if (r < 0)
            {
                throw new PythonException();
            }
        }


        /// <summary>
        /// DelItem Method
        /// </summary>
        /// <remarks>
        /// For objects that support the Python sequence or mapping protocols,
        /// delete the item at the given string index. This method raises a
        /// PythonException if the delete operation fails.
        /// </remarks>
        public virtual void DelItem(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            using (var pyKey = new PyString(key))
            {
                DelItem(pyKey);
            }
        }


        /// <summary>
        /// DelItem Method
        /// </summary>
        /// <remarks>
        /// For objects that support the Python sequence or mapping protocols,
        /// delete the item at the given numeric index. This method raises a
        /// PythonException if the delete operation fails.
        /// </remarks>
        public virtual void DelItem(int index)
        {
            using (var pyindex = new PyInt(index))
            {
                DelItem(pyindex);
            }
        }


        /// <summary>
        /// Length Method
        /// </summary>
        /// <remarks>
        /// Returns the length for objects that support the Python sequence
        /// protocol, or 0 if the object does not support the protocol.
        /// </remarks>
        public virtual long Length()
        {
            var s = Runtime.PyObject_Size(obj);
            if (s < 0)
            {
                Runtime.PyErr_Clear();
                return 0;
            }
            return s;
        }


        /// <summary>
        /// String Indexer
        /// </summary>
        /// <remarks>
        /// Provides a shorthand for the string versions of the GetItem and
        /// SetItem methods.
        /// </remarks>
        public virtual PyObject this[string key]
        {
            get { return GetItem(key); }
            set { SetItem(key, value); }
        }


        /// <summary>
        /// PyObject Indexer
        /// </summary>
        /// <remarks>
        /// Provides a shorthand for the object versions of the GetItem and
        /// SetItem methods.
        /// </remarks>
        public virtual PyObject this[PyObject key]
        {
            get { return GetItem(key); }
            set { SetItem(key, value); }
        }


        /// <summary>
        /// Numeric Indexer
        /// </summary>
        /// <remarks>
        /// Provides a shorthand for the numeric versions of the GetItem and
        /// SetItem methods.
        /// </remarks>
        public virtual PyObject this[int index]
        {
            get { return GetItem(index); }
            set { SetItem(index, value); }
        }


        /// <summary>
        /// GetIterator Method
        /// </summary>
        /// <remarks>
        /// Return a new (Python) iterator for the object. This is equivalent
        /// to the Python expression "iter(object)". A PythonException will be
        /// raised if the object cannot be iterated.
        /// </remarks>
        public PyObject GetIterator()
        {
            IntPtr r = Runtime.PyObject_GetIter(obj);
            if (r == IntPtr.Zero)
            {
                throw new PythonException();
            }
            return new PyObject(r);
        }

        /// <summary>
        /// GetEnumerator Method
        /// </summary>
        /// <remarks>
        /// Return a new PyIter object for the object. This allows any iterable
        /// python object to be iterated over in C#. A PythonException will be
        /// raised if the object is not iterable.
        /// </remarks>
        public IEnumerator GetEnumerator()
        {
            return new PyIter(this);
        }


        /// <summary>
        /// Invoke Method
        /// </summary>
        /// <remarks>
        /// Invoke the callable object with the given arguments, passed as a
        /// PyObject[]. A PythonException is raised if the invocation fails.
        /// </remarks>
        public PyObject Invoke(params PyObject[] args)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));
            if (args.Contains(null)) throw new ArgumentNullException();

            var t = new PyTuple(args);
            IntPtr r = Runtime.PyObject_Call(obj, t.obj, IntPtr.Zero);
            t.Dispose();
            if (r == IntPtr.Zero)
            {
                throw new PythonException();
            }
            return new PyObject(r);
        }


        /// <summary>
        /// Invoke Method
        /// </summary>
        /// <remarks>
        /// Invoke the callable object with the given arguments, passed as a
        /// Python tuple. A PythonException is raised if the invocation fails.
        /// </remarks>
        public PyObject Invoke(PyTuple args)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));

            IntPtr r = Runtime.PyObject_Call(obj, args.obj, IntPtr.Zero);
            if (r == IntPtr.Zero)
            {
                throw new PythonException();
            }
            return new PyObject(r);
        }


        /// <summary>
        /// Invoke Method
        /// </summary>
        /// <remarks>
        /// Invoke the callable object with the given positional and keyword
        /// arguments. A PythonException is raised if the invocation fails.
        /// </remarks>
        public PyObject Invoke(PyObject[] args, PyDict kw)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));
            if (args.Contains(null)) throw new ArgumentNullException();

            var t = new PyTuple(args);
            IntPtr r = Runtime.PyObject_Call(obj, t.obj, kw?.obj ?? IntPtr.Zero);
            t.Dispose();
            if (r == IntPtr.Zero)
            {
                throw new PythonException();
            }
            return new PyObject(r);
        }


        /// <summary>
        /// Invoke Method
        /// </summary>
        /// <remarks>
        /// Invoke the callable object with the given positional and keyword
        /// arguments. A PythonException is raised if the invocation fails.
        /// </remarks>
        public PyObject Invoke(PyTuple args, PyDict kw)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));

            IntPtr r = Runtime.PyObject_Call(obj, args.obj, kw?.obj ?? IntPtr.Zero);
            if (r == IntPtr.Zero)
            {
                throw new PythonException();
            }
            return new PyObject(r);
        }


        /// <summary>
        /// InvokeMethod Method
        /// </summary>
        /// <remarks>
        /// Invoke the named method of the object with the given arguments.
        /// A PythonException is raised if the invocation is unsuccessful.
        /// </remarks>
        public PyObject InvokeMethod(string name, params PyObject[] args)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (args == null) throw new ArgumentNullException(nameof(args));
            if (args.Contains(null)) throw new ArgumentNullException();

            PyObject method = GetAttr(name);
            PyObject result = method.Invoke(args);
            method.Dispose();
            return result;
        }


        /// <summary>
        /// InvokeMethod Method
        /// </summary>
        /// <remarks>
        /// Invoke the named method of the object with the given arguments.
        /// A PythonException is raised if the invocation is unsuccessful.
        /// </remarks>
        public PyObject InvokeMethod(string name, PyTuple args)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (args == null) throw new ArgumentNullException(nameof(args));

            PyObject method = GetAttr(name);
            PyObject result = method.Invoke(args);
            method.Dispose();
            return result;
        }

        /// <summary>
        /// InvokeMethod Method
        /// </summary>
        /// <remarks>
        /// Invoke the named method of the object with the given arguments.
        /// A PythonException is raised if the invocation is unsuccessful.
        /// </remarks>
        public PyObject InvokeMethod(PyObject name, params PyObject[] args)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (args == null) throw new ArgumentNullException(nameof(args));
            if (args.Contains(null)) throw new ArgumentNullException();

            PyObject method = GetAttr(name);
            PyObject result = method.Invoke(args);
            method.Dispose();
            return result;
        }


        /// <summary>
        /// InvokeMethod Method
        /// </summary>
        /// <remarks>
        /// Invoke the named method of the object with the given arguments.
        /// A PythonException is raised if the invocation is unsuccessful.
        /// </remarks>
        public PyObject InvokeMethod(PyObject name, PyTuple args)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (args == null) throw new ArgumentNullException(nameof(args));

            PyObject method = GetAttr(name);
            PyObject result = method.Invoke(args);
            method.Dispose();
            return result;
        }


        /// <summary>
        /// InvokeMethod Method
        /// </summary>
        /// <remarks>
        /// Invoke the named method of the object with the given arguments
        /// and keyword arguments. Keyword args are passed as a PyDict object.
        /// A PythonException is raised if the invocation is unsuccessful.
        /// </remarks>
        public PyObject InvokeMethod(string name, PyObject[] args, PyDict kw)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (args == null) throw new ArgumentNullException(nameof(args));
            if (args.Contains(null)) throw new ArgumentNullException();

            PyObject method = GetAttr(name);
            PyObject result = method.Invoke(args, kw);
            method.Dispose();
            return result;
        }


        /// <summary>
        /// InvokeMethod Method
        /// </summary>
        /// <remarks>
        /// Invoke the named method of the object with the given arguments
        /// and keyword arguments. Keyword args are passed as a PyDict object.
        /// A PythonException is raised if the invocation is unsuccessful.
        /// </remarks>
        public PyObject InvokeMethod(string name, PyTuple args, PyDict kw)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (args == null) throw new ArgumentNullException(nameof(args));

            PyObject method = GetAttr(name);
            PyObject result = method.Invoke(args, kw);
            method.Dispose();
            return result;
        }


        /// <summary>
        /// IsInstance Method
        /// </summary>
        /// <remarks>
        /// Return true if the object is an instance of the given Python type
        /// or class. This method always succeeds.
        /// </remarks>
        public bool IsInstance(PyObject typeOrClass)
        {
            if (typeOrClass == null) throw new ArgumentNullException(nameof(typeOrClass));

            int r = Runtime.PyObject_IsInstance(obj, typeOrClass.obj);
            if (r < 0)
            {
                Runtime.PyErr_Clear();
                return false;
            }
            return r != 0;
        }


        /// <summary>
        /// IsSubclass Method
        /// </summary>
        /// <remarks>
        /// Return true if the object is identical to or derived from the
        /// given Python type or class. This method always succeeds.
        /// </remarks>
        public bool IsSubclass(PyObject typeOrClass)
        {
            if (typeOrClass == null) throw new ArgumentNullException(nameof(typeOrClass));

            int r = Runtime.PyObject_IsSubclass(obj, typeOrClass.obj);
            if (r < 0)
            {
                Runtime.PyErr_Clear();
                return false;
            }
            return r != 0;
        }


        /// <summary>
        /// IsCallable Method
        /// </summary>
        /// <remarks>
        /// Returns true if the object is a callable object. This method
        /// always succeeds.
        /// </remarks>
        public bool IsCallable()
        {
            return Runtime.PyCallable_Check(obj) != 0;
        }


        /// <summary>
        /// IsIterable Method
        /// </summary>
        /// <remarks>
        /// Returns true if the object is iterable object. This method
        /// always succeeds.
        /// </remarks>
        public bool IsIterable()
        {
            return Runtime.PyObject_IsIterable(obj);
        }


        /// <summary>
        /// IsTrue Method
        /// </summary>
        /// <remarks>
        /// Return true if the object is true according to Python semantics.
        /// This method always succeeds.
        /// </remarks>
        public bool IsTrue()
        {
            return Runtime.PyObject_IsTrue(obj) != 0;
        }

        /// <summary>
        /// Return true if the object is None
        /// </summary>
        public bool IsNone() => CheckNone(this) == null;

        /// <summary>
        /// Dir Method
        /// </summary>
        /// <remarks>
        /// Return a list of the names of the attributes of the object. This
        /// is equivalent to the Python expression "dir(object)".
        /// </remarks>
        public PyList Dir()
        {
            IntPtr r = Runtime.PyObject_Dir(obj);
            if (r == IntPtr.Zero)
            {
                throw new PythonException();
            }
            return new PyList(r);
        }


        /// <summary>
        /// Repr Method
        /// </summary>
        /// <remarks>
        /// Return a string representation of the object. This method is
        /// the managed equivalent of the Python expression "repr(object)".
        /// </remarks>
        public string Repr()
        {
            IntPtr strval = Runtime.PyObject_Repr(obj);
            string result = Runtime.GetManagedString(strval);
            Runtime.XDecref(strval);
            return result;
        }


        /// <summary>
        /// ToString Method
        /// </summary>
        /// <remarks>
        /// Return the string representation of the object. This method is
        /// the managed equivalent of the Python expression "str(object)".
        /// </remarks>
        public override string ToString()
        {
            IntPtr strval = Runtime.PyObject_Unicode(obj);
            string result = Runtime.GetManagedString(strval);
            Runtime.XDecref(strval);
            return result;
        }


        /// <summary>
        /// Equals Method
        /// </summary>
        /// <remarks>
        /// Return true if this object is equal to the given object. This
        /// method is based on Python equality semantics.
        /// </remarks>
        public override bool Equals(object o)
        {
            if (!(o is PyObject))
            {
                return false;
            }
            if (obj == ((PyObject)o).obj)
            {
                return true;
            }
            int r = Runtime.PyObject_Compare(obj, ((PyObject)o).obj);
            if (Exceptions.ErrorOccurred())
            {
                throw new PythonException();
            }
            return r == 0;
        }


        /// <summary>
        /// GetHashCode Method
        /// </summary>
        /// <remarks>
        /// Return a hashcode based on the Python object. This returns the
        /// hash as computed by Python, equivalent to the Python expression
        /// "hash(obj)".
        /// </remarks>
        public override int GetHashCode()
        {
            return ((ulong)Runtime.PyObject_Hash(obj)).GetHashCode();
        }


        public long Refcount
        {
            get
            {
                return Runtime.Refcount(obj);
            }
        }

        private static void AddArgument(IntPtr argtuple, int i, object target)
        {
            IntPtr ptr = GetPythonObject(target);

            if (Runtime.PyTuple_SetItem(argtuple, i, ptr) < 0)
            {
                throw new PythonException();
            }
        }

        private static IntPtr GetPythonObject(object target)
        {
            IntPtr ptr;
            if (target is PyObject)
            {
                ptr = ((PyObject)target).Handle;
                Runtime.XIncref(ptr);
            }
            else
            {
                ptr = Converter.ToPython(target, target?.GetType());
            }

            return ptr;
        }

        // Workaround for https://bugzilla.xamarin.com/show_bug.cgi?id=41509
        // See https://github.com/pythonnet/pythonnet/pull/219
        private static object CheckNone(PyObject pyObj)
        {
            if (pyObj != null)
            {
                if (pyObj.obj == Runtime.PyNone)
                {
                    return null;
                }
            }

            return pyObj;
        }
    }
}
