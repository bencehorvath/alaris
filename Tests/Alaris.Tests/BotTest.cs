using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Alaris.Tests
{
    [TestClass]
    public class BotTest
    {
        [TestMethod]
        public void TestDefaultInstanceThroughHolder()
        {
            var bot = new AlarisBot("alaris.config.xml");
            InstanceHolder<AlarisBot>.Set(bot);

            Assert.IsNotNull(InstanceHolder<AlarisBot>.Get(), "Bot instance retrieved from singleton is null.");
        }

        [TestMethod]
        public void TestScriptManagerFuncionality()
        {
            var bot = new AlarisBot();
            InstanceHolder<AlarisBot>.Set(bot);

            Assert.IsNotNull(bot, "Bot instance retrieved from singleton is null.");
            Assert.IsNotNull(bot.ScriptManager, "The bot's script manager instance is null.");
            Assert.IsNotNull(bot.ScriptManager.Connection, "The script manager's connection instance is null.");
        }

        [TestMethod]
        public void TestGeneratedGuids()
        {
            var bot = new AlarisBot();
            InstanceHolder<AlarisBot>.Set(bot);

            Assert.IsNotNull(InstanceHolder<AlarisBot>.Get(), "Bot instance retrieved from singleton is null.");

            Assert.IsNotNull(InstanceHolder<AlarisBot>.Get().ScriptManager.GetGuid(), "Script Manager's GUID is null.");
            Assert.IsNotNull(InstanceHolder<AlarisBot>.Get().GetGuid(), "The bot's GUID is null.");
        }
    }
}
