using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.UnitTests
{
    [TestClass]
    public class ObjectExporterTests
    {
        private readonly Func<string> _lineSeparator = () => ";";
        private readonly Func<string, string, string> _lineFormatter = (name, value) => $"{name} - {value}";

        class A
        {
            public int V1 { get; set; }
            public double V2 { get; set; }
            public string V3 { get; set; }
        }

        class B
        {
            public A A1 { get; set; }
            public A A2 { get; set; }
        }

        [TestMethod]
        public void Test1()
        {
            var a = new A() { V1 = 10, V2 = 10.55, V3 = "string" };
            var expected = "V1 - 10;V2 - 10.55;V3 - string;";
            var result = new ObjectExporter<A>(_lineFormatter, _lineSeparator).Export(a);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Test2()
        {
            var a = new A() { V1 = 111000222, V2 = 0.00055 };
            var expected = "V1 - 111000222;V2 - 0.00055;V3 - NA;";
            var result = new ObjectExporter<A>(_lineFormatter, _lineSeparator).Export(a);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Test3()
        {
            var a = new B()
            {
                A1 = new A()
                {
                    V1=1,
                    V2 = 0.1,
                    V3="111"
                },
                A2 = new A()
                {
                    V1 = 2,
                    V2 = 0.2,
                    V3 = "222"
                }
            };

            var expected = "A1: ;V1 - 1;V2 - 0.1;V3 - 111;A2: ;V1 - 2;V2 - 0.2;V3 - 222;";
            var result = new ObjectExporter<B>(_lineFormatter, _lineSeparator).Export(a);
            Assert.AreEqual(expected, result);

        }
    }
}
