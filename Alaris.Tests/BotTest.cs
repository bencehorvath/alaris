using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Alaris.Tests
{
    [TestClass]
    public class BotTest
    {
        [TestMethod]
        public void TestDefaultInstanceThroughSingleton()
        {
            var bot = Singleton<AlarisBot>.Instance;

            Assert.IsNotNull(bot, "Bot instance retrieved from singleton is null.");
        }

        [TestMethod]
        public void TestDefaultMysqlDetails()
        {
            var bot = Singleton<AlarisBot>.Instance;

            Assert.IsNotNull(bot, "Bot instance retrieved from singleton is null.");
            Assert.IsNotNull(bot.MysqlData, "MySQL data array is null.");
            Assert.AreNotEqual(true, bot.MysqlEnabled, "MySQL should not be enabled on creation.");

        }

        [TestMethod]
        public void TestScriptManagerFuncionality()
        {
            var bot = Singleton<AlarisBot>.Instance;

            Assert.IsNotNull(bot, "Bot instance retrieved from singleton is null.");
            Assert.IsNotNull(bot.ScriptManager, "The bot's script manager instance is null.");
            Assert.IsNotNull(bot.ScriptManager.Connection, "The script manager's connection instance is null.");
        }

        [TestMethod]
        public void TestGeneratedGuids()
        {
            var bot = Singleton<AlarisBot>.Instance;

            Assert.IsNotNull(bot, "Bot instance retrieved from singleton is null.");

            Assert.IsNotNull(bot.ScriptManager.GetGuid(), "Script Manager's GUID is null.");
            Assert.IsNotNull(bot.GetGuid(), "The bot's GUID is null.");
        }
    }
}
