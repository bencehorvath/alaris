using System;
using System.Collections.Generic;
using System.Text;
using Yanesdk.System;
using LuaInterface;
using System.Reflection;
using System.Collections;

namespace Alaris.Script
{
	/// <summary>
	/// LuaInterface.Luaの簡単なWrapper。
	/// </summary>
	public class Lua : IDisposable
	{
		static Lua()
		{
			DllManager d = DllManager.Instance;
			d.LoadLibrary("lua51");
			d.LoadLibrary("luanet");
		}
		
		Encoding fileEncoding = Encoding.UTF8;

		/// <summary>
		/// スクリプトファイルのエンコーディング。
		/// </summary>
		public Encoding FileEncoding
		{
			get { return fileEncoding; }
			set { fileEncoding = value; }
		}

		/// <summary>
		/// lua。
		/// </summary>
		private LuaInterface.Lua lua = new LuaInterface.Lua();

		/// <summary>
		/// Excutes a Lua file and returns all the chunk's return
		/// values in an array 
		/// </summary>
		/// <param name="filename">
		/// File to execute.
		/// </param>
		/// <returns>
		/// return values.
		/// </returns>
		public object[] DoFile(string filename)
		{
			string data = fileEncoding.GetString(FileSys.Read(filename));
			return DoString(data);
		}

		/// <summary>
		/// Excutes a Lua file and returns all the chunk's return
		/// values in an array 
		/// </summary>
		/// <param name="data">
		/// Lua string.
		/// </param>
		/// <returns>
		/// Return values.
		/// </returns>
		public object[] DoString(string data)
		{
			data = string.Join(Environment.NewLine,
				data.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None));
	
			return lua.DoString(data);
		}

		/// <summary>
		/// Loads and parses the specified Lua file. 
		/// </summary>
		/// <param name="filename">
		/// The file.
		/// </param>
		/// <returns>
		/// .
		/// </returns>
		public LuaFunction LoadFile(string filename)
		{
			string data = fileEncoding.GetString(FileSys.Read(filename));
			return LoadString(data);
		}

		
		public LuaFunction LoadString(string data)
		{
			data = string.Join(Environment.NewLine,
				data.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None));
			return lua.LoadString(data);
		}

		#region luaspec

		/// <summary>
		/// Indexer for global variables from the LuaInterpreter
		/// Supports navigation of tables by using . operator 
		/// </summary>
		/// <param name="fullPath">
		/// Variable.
		/// </param>
		public object this[string fullPath]
		{
			get { return lua[fullPath]; }
			set { lua[fullPath] = value; }
		}


		public LuaFunction GetFunction(string fullPath)
		{
			return lua.GetFunction(fullPath);
		}


		public Delegate GetFunction(Type delegateType, string fullPath)
		{
			return lua.GetFunction(delegateType, fullPath);
		}


		public double GetNumber(string fullPath)
		{
			return lua.GetNumber(fullPath);
		}


		public string GetString(string fullPath)
		{
			return lua.GetString(fullPath);
		}


		public LuaTable GetTable(string fullPath)
		{
			return lua.GetTable(fullPath);
		}


		public object GetTable(Type interfaceType, string fullPath)
		{
			return lua.GetTable(interfaceType, fullPath);
		}


		public IDictionary GetTableDict(LuaTable table)
		{
			return lua.GetTableDict(table);
		}


		public void NewTable(string fullPath)
		{
			lua.NewTable(fullPath);
		}

		public LuaFunction RegisterFunction(string path, object target, MethodInfo function)
		{
			return lua.RegisterFunction(path, target, function);
		}

		public void Dispose()
		{
			lua.Dispose();
		}

		#endregion
	}
}
