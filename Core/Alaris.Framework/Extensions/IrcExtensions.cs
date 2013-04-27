using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alaris.Irc;

namespace Alaris.Framework.Extensions
{
    /// <summary>
    /// IRC related extension methods.
    /// </summary>
    public static class IrcExtensions
    {
        private readonly static NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Joins to the specified channels.
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="connection"></param>
        public static void JoinToChannels(this IEnumerable<string> channels, Connection connection)
        {

            Parallel.ForEach(channels, chan =>
                                           {
                                               if (chan.IsValidChannelName())
                                                   connection.Sender.Join(chan);

                                               Log.Debug("Joined channel: {0}", chan);
                                           });
        }

        /// <summary>
        /// Fills the channel list from the specified enumerable type.
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="chns"></param>
        public static void GetChannelsFrom(this IList<string> channels, IEnumerable<string> chns)
        {
            foreach (var chan in chns.Where(chn => chn.IsValidChannelName()))
            {
                channels.Add(chan);
            }
        }

        /// <summary>
        /// Joins the channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        public static void JoinChannel(this string channel)
        {
            if(channel.IsValidChannelName())
                AlarisBase.Instance.Connection.Sender.Join(channel);
        }
    }
}
