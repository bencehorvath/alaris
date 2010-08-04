/* 
  	* UtilsTest.cs
	* [ part of Atom.NET library: http://atomnet.sourceforge.net ]
	* Author: Lawrence Oluyede
	* License: BSD-License (see below)
    
	Copyright (c) 2003, Lawrence Oluyede
    All rights reserved.

    Redistribution and use in source and binary forms, with or without
    modification, are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice,
    * this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
    * notice, this list of conditions and the following disclaimer in the
    * documentation and/or other materials provided with the distribution.
    * Neither the name of the copyright owner nor the names of its
    * contributors may be used to endorse or promote products derived from
    * this software without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
    AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
    ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
    LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
    CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
    SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
    INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
    CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
    ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
    POSSIBILITY OF SUCH DAMAGE.
*/
using System;
using System.Text;
using NUnit.Framework;
using AU = Atom.Utils;

namespace Atom.Utils.Test
{
	[TestFixture]
	public class UtilsTest
	{
		string escaped = @"&lt;simpletag&gt;&lt;&lt;/simpletag&gt;";
		string unescaped = @"<simpletag><</simpletag>";
		string encoded = "dGVzdCBzdHJpbmc=";
		string decoded = "test string";
		string temp	= String.Empty;
		string invalidDate = "2003-12-1312:29:29";
		string isoDate = "2003-12-13T12:29:29";
		string isoDateTZ = "2003-12-13T12:29:29Z";
		string isoDateLocal = "2003-12-13T12:29:29+01:00";

		#region Escaping stuff
		[Test]
		public void TestEscape()
		{
			temp = AU.Utils.Escape(unescaped);
			Assert.AreEqual(escaped, temp);
		}

		[Test]
		public void TestUnescape()
		{
			temp = AU.Utils.Unescape(escaped);
			Assert.AreEqual(unescaped, temp);
		}
		#endregion

		#region Base64 stuff
		[Test]
		public void TestBase64EncodeString()
		{
			temp = AU.Utils.Base64Encode(decoded);
			Assert.AreEqual(encoded, temp);
		}

		[Test]
		public void TestBase64EncodeByteArray()
		{
			byte[] arr = Encoding.ASCII.GetBytes(decoded);
			temp = AU.Utils.Base64Encode(arr);
			Assert.AreEqual(encoded, temp);
		}

		[Test]
		public void TestBase64DecodeString()
		{
			byte[] arr = AU.Utils.Base64Decode(encoded);
			temp = AU.Utils.Base64Encode(arr);
			Assert.AreEqual(encoded, temp);
		}

		[Test]
		public void TestBase64DecodeCharArray()
		{
			char[] carr = encoded.ToCharArray();
			byte[] barr = AU.Utils.Base64Decode(carr);
			temp = AU.Utils.Base64Encode(barr);
			Assert.AreEqual(encoded, temp);
		}
		#endregion

		#region ISO 8601 Date stuff
		[Test]
		public void TestIso8601Date()
		{
			Assert.IsTrue(AU.Utils.IsIso8601Date(isoDate));
			Assert.IsFalse(AU.Utils.IsIso8601Date(invalidDate));
			Assert.IsTrue(AU.Utils.IsIso8601Date(isoDateTZ));
			Assert.IsTrue(AU.Utils.IsIso8601DateLocal(isoDateLocal));
		}

		[Test]
		public void TestIso8601DateTZ()
		{

			Assert.IsFalse(AU.Utils.IsIso8601DateTZ(isoDate));
			Assert.IsFalse(AU.Utils.IsIso8601DateTZ(invalidDate));
			Assert.IsTrue(AU.Utils.IsIso8601DateTZ(isoDateTZ));
			Assert.IsTrue(AU.Utils.IsIso8601DateTZ(isoDateLocal));
		}

		[Test]
		public void TestIso8601DateLocal()
		{

			Assert.IsTrue(AU.Utils.IsIso8601DateLocal(isoDate));
			Assert.IsFalse(AU.Utils.IsIso8601DateLocal(invalidDate));
			Assert.IsFalse(AU.Utils.IsIso8601DateLocal(isoDateTZ));
			Assert.IsTrue(AU.Utils.IsIso8601DateLocal(isoDateLocal));
		}
		#endregion

		#region Email stuff
		[Test]
		public void TestEmail()
		{
			string email = "uncle@bob.com";
			string invalidEmail = "uncle@b.c";
			string invalidEmail2 = "uncle@.com";

            Assert.IsTrue(AU.Utils.IsEmail(email));
			Assert.IsFalse(AU.Utils.IsEmail(invalidEmail));
			Assert.IsFalse(AU.Utils.IsEmail(invalidEmail2));
		}
		#endregion

		#region Version stuff

		[Test]
		public void TestVersion()
		{
			Assert.IsNotNull(AU.Utils.GetVersion());
			Assert.AreEqual(AU.Utils.GetVersion(), AU.Utils.GetVersion());
		}

		#endregion
	}
}
