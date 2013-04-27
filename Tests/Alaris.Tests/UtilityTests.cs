using System;
using System.Collections.Generic;
using System.Diagnostics;
using Alaris.Framework;
using Alaris.Framework.Extensions;
using Alaris.Irc;
using NUnit.Framework;


namespace Alaris.Tests
{
    [TestFixture]
    public class UtilityTests
    {

        [Test]
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

        [Test]
        public void TestDelayedFunctionCall()
        {
            var watch = new Stopwatch();
            watch.Start();

            Utility.Delay(500, () =>
                {
                    watch.Stop();
                    Assert.GreaterOrEqual(watch.ElapsedMilliseconds, 500);
                });
        }

        [Test]
        public void TestMD5HashCalculation()
        {
            var references = new Dictionary<string, string>
                {
                    {"test1", "5a105e8b9d40e1329780d62ea2265d8a"},
                    {"test2", "ad0234829205b9033196ba818f7a872b"},
                    {"another test with spaces", "0c2e4d83f5b88e2e3cfc132ff616507b"},
                    {"and numb3rs", "8b26572d1abbdc27ad43d9ef5aed3eae"}
                };

            foreach(var item in references)
                Assert.AreEqual(Utility.MD5String(item.Key), item.Value);
        }

        [Test]
        public void TestSafeExecutionMethod()
        {
            try
            {
                Utility.ExecuteSafely(() => { throw new Exception("test"); });
            }
            catch 
            {
                Assert.Fail("Exception thrown in safe context.");
            }
        }

        [Test]
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

        [Test]
        public void TestExtensionMetaCasting()
        {
            const int im = Int32.MaxValue;

            var lm = im.Cast<long>();

            Assert.AreEqual(lm, im, "Cast value mismatch.");
            Assert.IsInstanceOf<long>(lm);
            Assert.IsInstanceOf<int>(im);
            
        }

        [Test]
        public void TestIsNullExtensionAlsoOnStrings()
        {
            object bn = null;
            var bnn = new object();

            const string nos = "";
            var emps = string.Empty;
            const string noemps = "value";

            Assert.IsTrue(bn.IsNull());
            Assert.IsFalse(bnn.IsNull());
            Assert.IsTrue(nos.IsNull());
            Assert.IsTrue(emps.IsNull());
            Assert.IsFalse(noemps.IsNull());
        }

        [Test]
        public void TestStringRelatedExtensions()
        {
            var sa = new[] {"this", "is", "to", "be", "one", "string"};

            var snospace = sa.Concatenate();
            var sspace = sa.ConcatenateWithSpaces();
            var scustom = sa.Concatenate(":");

            
            Assert.AreEqual(snospace, "thisistobeonestring", "Default concat failed.");
            Assert.AreEqual(sspace, "this is to be one string", "Space concat failed.");
            Assert.AreEqual(scustom, "this:is:to:be:one:string", "Custom concat failed.");
        }


    }
}
