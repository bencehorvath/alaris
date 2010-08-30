using System.Data;
using Alaris.API;
using MySql.Data.MySqlClient;

namespace Alaris.Extras
{
	public class DatabaseManager
	{
		private MySqlConnection _connection;
		
		private DatabaseManager ()
		{
			
			
			
		}
		
		public void Initialize(string host, string username, string password, string database)
		{
			string cons = "SERVER="+host+";" +
				"DATABASE="+database+";" +
				"UID="+username+";" +
				"PASSWORD="+password+";";
			
			_connection = new MySqlConnection(cons);
			
			_connection.Open();
			
			Log.Success("DatabaseManager", "MySQL connection set up.");
		}
		
		public DataTable Query(string query)
		{
			try 
			{
				var adapter = new MySqlDataAdapter();
				
				var command = _connection.CreateCommand();
				command.CommandText = query;
				adapter.SelectCommand = command;
				
				var table = new DataTable();
				
				adapter.Fill(table);
				
				command.Dispose();
				adapter.Dispose();
				
				return table;
				
			}
			catch(MySqlException ex)
			{
				Log.Error("DatabaseManager", "Couldn't execute query. ("+ex.Message+")");
				return null;
			}
			
		}
		
		public DataRow QueryFirstRow(string query)
		{
		    var table = Query(query);

		    return !table.Equals(null) && table.Rows.Count > 0 ? table.Rows[0] : null;
		}
	}
}

