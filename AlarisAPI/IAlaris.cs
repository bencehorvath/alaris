using System;
using Alaris.Irc;
using System.Collections.Generic;

namespace Alaris.Core
{
	public interface IAlarisBasic
	{
		void Initialize(ref Connection con);
		void Initialize(ref Connection con, ref List<string> channels);
		void OnPublicMessage(UserInfo user, string channel, string msg);
		void OnRegistered();
		void OnLoad();
		void OnUnload();
		string GetName();
	}
}