using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpcHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpcHelper.Tests
{
    [TestClass()]
    public class OpcClientHelperTests
    {
        [TestMethod()]
        public void OpcClientHelperTest()
        {
            Assert.Fail();
        }
        [TestMethod()]
        public void GetOpcServersTest()
        {
            //OpcHelper.ClientHelper clientHelper = new ClientHelper();
            var result = OpcClientHelper.GetOpcServers("127.0.17");
            result = OpcClientHelper.GetOpcServers();

            Assert.AreNotEqual(null, result);
        }

        [TestMethod()]
        public void ConnectTest()
        {
            var servers = OpcClientHelper.GetOpcServers();
            OpcHelper.OpcClientHelper clientHelper = new OpcClientHelper();

            clientHelper.Connect(servers.First());

            Assert.IsTrue(true);
        }
    }
}