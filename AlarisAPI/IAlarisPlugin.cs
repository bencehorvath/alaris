using System.Collections.Generic;
using Alaris.Irc;

namespace Alaris.API
{
    public interface IAlarisPlugin
    {
        void Setup(ref Connection conn, List<string> channels);
        void Destroy();

        string Name { get; }
        string Author { get; }
    }
}
