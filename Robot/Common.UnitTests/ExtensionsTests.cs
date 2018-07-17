using System;
using System.Collections.Generic;
using Common.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.UnitTests
{
    [TestClass]
    public class ExtensionsTests
    {
        class A
        {
            public int V1 { get; set; }
            public double V2 { get; set; }
        }

        class B
        {
            public A A { get; set; }
        }

        [TestMethod]
        public void IndexOfMax_Test1()
        {
            var list = new List<A>
            {
                new A() {V1 = 1, V2 = 0.1},
                new A() {V1 = 2, V2 = 0.2},
                new A() {V1 = 3, V2 = 0.3},
                new A() {V1 = 1, V2 = 0.4}
            };

            Assert.AreEqual(2, list.IndexOfMax(x => x.V1));
            Assert.AreEqual(3, list.IndexOfMax(x => x.V2));

        }

        [TestMethod]
        public void IndexOfMin_Test1()
        {
            var list = new List<A>
            {
                new A() {V1 = 1, V2 = 0.1},
                new A() {V1 = 2, V2 = 0.2},
                new A() {V1 = 0, V2 = 0.3},
                new A() {V1 = 1, V2 = 0}
            };

            Assert.AreEqual(2, list.IndexOfMin(x => x.V1));
            Assert.AreEqual(3, list.IndexOfMin(x => x.V2));

        }

        [TestMethod]
        public void IndexOfMax_Test2()
        {
            var list = new List<B>
            {
               new  B() {A=  new A() {V1 = 1, V2 = 0.1}},
               new  B() { A= new A() {V1 = 2, V2 = 0.2}},
               new  B() { A= new A() {V1 = 3, V2 = 0.3}},
            new  B() { A= new A() {V1 = 4, V2 = 0.1}}
            };

            Assert.AreEqual(3, list.IndexOfMax(x => x.A.V1));
            Assert.AreEqual(2, list.IndexOfMax(x => x.A.V2));
        }
    }
}
