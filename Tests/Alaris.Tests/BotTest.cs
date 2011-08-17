using System;
using System.Collections.Generic;
using Alaris.Framework;
using Alaris.Framework.Extensions;
using Alaris.Irc;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Alaris.Tests
{
    [TestClass]
    public class BotTest
    {
        [TestMethod]
        public void TestDefaultInstanceThroughHolder()
        {
            var uf = new UserInfo("test", "test", "test");
            uf.SetAsInstance();

            Assert.IsNotNull(InstanceHolder<UserInfo>.Instance, "The instance holder's value is null.");
            Assert.AreEqual(uf, InstanceHolder<UserInfo>.Instance, "The value held is not equal to the one set.");
        }

        [TestMethod]
        public void TestChannelListParsingAndExtracting()
        {
            var chanList = new List<string> {"#chan1", "#chan2", "#chan3"};
            var toList = new List<string>();

            toList.GetChannelsFrom(chanList);

            Assert.AreEqual(toList.Count, chanList.Count, "Channel parsing count mismatch.");

            for(var i = 0; i < toList.Count; ++i)
            {
                Assert.AreEqual(toList[i], chanList[i], "Parsed channel value/order mismatch");
            }
        }

        [TestMethod]
        public void TestChannelListParsingFilter()
        {
            var chanList = new List<string> { "#chan1", "#chan2", "@chan3" };
            var toList = new List<string>();

            toList.GetChannelsFrom(chanList);

            Assert.AreEqual(toList.Count, chanList.Count - 1, "Parsed and filtered channel count mismatch.");
            
            for(var i = 0; i < toList.Count; ++i)
            {
                Assert.AreEqual(toList[i], chanList[i], "Parsed and filtered channel value/order mismatch."); // used to hit with parallel channel parsing ofc
            }
        }

        [TestMethod]
        public void TestExtensionMetaCasting()
        {
            var im = Int32.MaxValue;

            var lm = im.Cast<long>();

            Assert.AreEqual(lm, im, "Cast value mismatch.");
            Assert.IsInstanceOfType(lm, typeof(long));
            Assert.IsInstanceOfType(im, typeof(int));
        }

        [TestMethod]
        public void TestIsNullExtensionAlsoOnStrings()
        {
            object bn = null;
            object bnn = new object();

            const string nos = "";
            var emps = string.Empty;
            const string noemps = "value";

            Assert.IsTrue(bn.IsNull());
            Assert.IsFalse(bnn.IsNull());
            Assert.IsTrue(nos.IsNull());
            Assert.IsTrue(emps.IsNull());
            Assert.IsFalse(noemps.IsNull());
        }

        [TestMethod]
        public void TestStringRelatedExtensions()
        {
            var sa = new[] {"this", "is", "to", "be", "one", "string"};

            var snospace = sa.Concatenate();
            var sspace = sa.ConcatenateWithSpaces();
            var scustom = sa.Concatenate(":");

            Assert.AreEqual(snospace, "thisistobeonestring", true, "Default concat failed.");
            Assert.AreEqual(sspace, "this is to be one string", true, "Space concat failed.");
            Assert.AreEqual(scustom, "this:is:to:be:one:string", true, "Custom concat failed.");
        }


    }
}
