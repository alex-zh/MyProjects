using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trans2Quik.Core;

namespace Trans2Quik.Tests
{    

    [TestClass]
    public class ConnectionTests
    {
        [TestMethod]
        public void CanConnect()
        {
            var cw = new ConnectionListener(Mother.CONST_PathToQuik);

            Assert.IsTrue(cw.Connect());            
            Assert.IsTrue(cw.IsDllConnected);
            Assert.IsTrue(cw.IsQuikConnected);
            Assert.IsTrue(cw.Disconnect());
        }
    }
}
