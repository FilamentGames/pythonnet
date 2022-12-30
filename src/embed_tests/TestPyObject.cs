using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Python.Runtime;

namespace Python.EmbeddingTest
{
    public class TestPyObject
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            PythonEngine.Initialize();
        }

        [OneTimeTearDown]
        public void Dispose()
        {
            PythonEngine.Shutdown();
        }

        [Test]
        public void InvokeNull() {
            var list = PythonEngine.Eval("list");
            Assert.Throws<ArgumentNullException>(() => list.Invoke(new PyObject[] {null}));
        }
    }
}
