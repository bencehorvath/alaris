namespace Alaris
{
    public partial class AlarisBot
    {
        /// <summary>
        /// Returns the bot instance.
        /// </summary>
        /// <returns>The bot</returns>
        public static AlarisBot GetBot()
        {
            return Singleton<AlarisBot>.Instance;
        }
    }
}
