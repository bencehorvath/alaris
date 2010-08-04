using System;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Alaris.Extras
{
	public class DatabaseManager : MarshalByRefObject
	{
		private readonly MySqlConnection _connection;
		
		public DatabaseManager (string host, string username, string password, string database)
		{
			string cons = "SERVER="+host+";" +
				"DATABASE="+database+";" +
				"UID="+username+";" +
				"PASSWORD="+password+";";
			
			_connection = new MySqlConnection(cons);
			
			_connection.Open();
			
			
		}
		
		public DataTable Query(string query)
		{
			var adapter = new MySqlDataAdapter();
			var command = _connection.CreateCommand();
			command.CommandText = query;
			adapter.SelectCommand = command;
			
			var table = new DataTable();
			
			adapter.Fill(table);
			
			adapter.Dispose();
			command.Dispose();
			
			
			return table;
			
		}
		
		public DataRow QueryFirstRow(string query)
		{
			var table = Query(query);
			
			if(table != null)
				return table.Rows[0];
			else
				return null;
		}
		
		
	}
}

