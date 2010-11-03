using System.Data;
using System.Data.SQLite;
using Alaris.Irc;

namespace Alaris.API.Database
{
    /// <summary>
    /// Class providing static methods for managing the SQL databse.
    /// </summary>
	public static class DatabaseManager
    {

	    private static SQLiteConnection _connection;

        /// <summary>
        /// Gets the database connection.
        /// </summary>
	    public static SQLiteConnection Connection
	    {
	        get
	        {
	            return _connection;
	        }

            private set
            {
                _connection = value;
            }
	    }
		
        /// <summary>
        /// Initializes the <see>DatabaseManager</see>
        /// </summary>
        /// <param name="database"></param>
		public static void Initialize(string database)
		{
		    Log.Notice("DatabaseManager", "Reading local database...");
			Connection = new SQLiteConnection("Data Source=Alaris.s3db");

            //_connection.ChangeDatabase(database);

            Log.Success("DatabaseManager", "Databases are correctly set up.");

		}

        /// <summary>
        /// Executes the given query on the database.
        /// </summary>
        /// <param name="sql">The query</param>
        /// <returns>Result from the database.</returns>
        public static DataTable Query(string sql)
        {
            try
            {
                var adapter = new SQLiteDataAdapter();

                var command = _connection.CreateCommand();
                command.CommandText = sql;

                adapter.SelectCommand = command;

                var table = new DataTable();

                adapter.Fill(table);

                command.Dispose();
                adapter.Dispose();

                return table;
            }
            catch(SQLiteException x)
            {
                Log.Error("DatabaseManager", string.Format("Couldn't execute query. ({0})", x.Message));
                return null;
            }
        }

        /// <summary>
        /// Executes the given query on the database and returns the result's first row.
        /// </summary>
        /// <param name="query">Query to execute</param>
        /// <returns>The row</returns>
        public static DataRow QueryFirstRow(string query)
        {
            var table = Query(query);

            return !table.Equals(null) && table.Rows.Count > 0 ? table.Rows[0] : null;
        }
	}
}

