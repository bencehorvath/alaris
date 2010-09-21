using System.Data.SQLite;

namespace Alaris.API
{
    public interface IManager
    {
        void Initialize();

        SQLiteConnection Connection { get; }
    }
}
