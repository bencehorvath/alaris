/* 
  	* AtomEntryTest.cs
	* [ part of Atom.NET library: http://atomnet.sourceforge.net ]
	* Author: Lawrence Oluyede
	* License: BSD-License (see below)
    
	Copyright (c) 2003, 2004 Lawrence Oluyede
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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using NUnit.Framework;
using Atom.Core;
using Atom.Core.Collections;
using AU = Atom.Utils;

namespace Atom.Core.Test
{
	[TestFixture]
    public class AtomEntryTest
    {
		AtomEntry entry;
		Uri testUri;
		string entriespath;
		string[] files;
		
		[SetUp]
		public void Init()
		{
			entry = new AtomEntry();
			testUri = new Uri("http://www.w3.org");
			entriespath = @"..\..\tests\entries\";
			files = Directory.GetFiles(entriespath);
		}

		#region Test of properties and elements

		[Test]
		public void TestTitle()
		{
			AtomContentConstruct title = new AtomContentConstruct();
			title.Content = "sample text";
			title.Mode = Mode.Xml;
			title.Type = MediaType.TextPlain;

			entry.Title = title;

			Assert.AreEqual(title.LocalName, "title");
			Assert.AreEqual(title.FullName, "atom:title");
			Assert.AreSame(entry.Title, title);
		}

		[Test]
		public void TestLink()
		{
			AtomLink link = new AtomLink();
			link.HRef = testUri;
			link.Rel = Relationship.Alternate;
			link.Title = "sample text";
			link.Type = MediaType.TextPlain;
			
			entry.Links.Add(link);

			Assert.AreEqual(link.LocalName, "link");
			Assert.AreEqual(link.FullName, "atom:link");
			Assert.AreSame(entry.Links[0], link);
			Assert.AreEqual(entry.Links[0], link);
		}

		[Test]
		public void TestAuthor()
		{
			AtomPersonConstruct author = new AtomPersonConstruct();
			author.Email = "foo@bar.com";
			author.Name = "Uncle Tom";
			author.Url = testUri;

			entry.Author = author;
			
			entry.Author.ToString();

			Assert.AreEqual(author.LocalName, "author");
			Assert.AreEqual(author.FullName, "atom:author");
            Assert.AreSame(entry.Author, author);
		}

		[Test]
		public void TestContributors()
		{
			AtomPersonConstruct contributor = new AtomPersonConstruct("contributor");
			contributor.Email = "bar@foo.com";
			contributor.Name = "Uncle Bob";
			contributor.Url = testUri;

			entry.Contributors.Add(contributor);


			Assert.AreEqual(contributor.LocalName, "contributor");
			Assert.AreEqual(contributor.FullName, "atom:contributor");
			Assert.AreSame(entry.Contributors[0], contributor);
			Assert.AreEqual(entry.Contributors[0], contributor);
		}

		[Test]
		public void TestId()
		{
			entry.Id = testUri;

			Assert.AreSame(entry.Id, testUri);
			Assert.AreEqual(entry.Id.ToString(), testUri.ToString());
		}

		[Test]
		public void TestModified()
		{
			AtomDateConstruct modifiedDate = new AtomDateConstruct();
			modifiedDate.DateTime = DateTime.Now;
			modifiedDate.UtcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Today);

			entry.Modified = modifiedDate;

			Assert.AreSame(entry.Modified, modifiedDate);

			modifiedDate = new AtomDateConstruct();

			entry.Modified = modifiedDate;

			Assert.AreEqual(modifiedDate.LocalName, "modified");
			Assert.AreEqual(modifiedDate.FullName, "atom:modified");
			Assert.AreSame(entry.Modified, modifiedDate);
		}

		[Test]
		public void TestIssued()
		{
			AtomDateConstruct issuedDate = new AtomDateConstruct("issued");
			issuedDate.DateTime = DateTime.Now;

			entry.Issued = issuedDate;

			Assert.AreSame(entry.Issued, issuedDate);

			issuedDate = new AtomDateConstruct("issued");

			entry.Issued = issuedDate;

			Assert.AreEqual(issuedDate.LocalName, "issued");
			Assert.AreEqual(issuedDate.FullName, "atom:issued");
			Assert.AreSame(entry.Issued, issuedDate);
		}

		[Test]
		public void TestCreated()
		{
			AtomDateConstruct createdDate = new AtomDateConstruct("created");
			createdDate.DateTime = DateTime.Now;
			createdDate.UtcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Today);

			entry.Created = createdDate;

			Assert.AreSame(entry.Created, createdDate);

			createdDate = new AtomDateConstruct("created");

			entry.Created = createdDate;

			Assert.AreEqual(createdDate.LocalName, "created");
			Assert.AreEqual(createdDate.FullName, "atom:created");
			Assert.AreSame(entry.Created, createdDate);
		}

		[Test]
		public void TestSummary()
		{
			AtomContentConstruct summary = new AtomContentConstruct("summary");
			summary.Content = "sample text";
			summary.Mode = Mode.Xml;
			summary.Type = MediaType.TextPlain;

			entry.Summary = summary;

			Assert.AreEqual(summary.LocalName, "summary");
			Assert.AreEqual(summary.FullName, "atom:summary");
			Assert.AreSame(entry.Summary, summary);
		}

		[Test]
		public void TestContents()
		{
			AtomContent content = new AtomContent();
			content.Content = "sample text";
			content.Mode = Mode.Xml;
			content.Type = MediaType.TextPlain;
			
			entry.Contents.Add(content);

			Assert.AreEqual(content.LocalName, "content");
			Assert.AreEqual(content.FullName, "atom:content");
			Assert.AreSame(entry.Contents[0], content);
			Assert.AreEqual(entry.Contents[0], content);
		}

		[Test]
		public void TestUri()
		{
			for(int i = 0; i < files.Length; i++)
			{
				Console.WriteLine("{0}: {1}", i, files[i]);
				entry = AtomEntry.Load(files[i]);
				Assert.IsNotNull(entry.Uri);
			}
		}

		#endregion

		#region Test of Load/Save methods

		[Test]
		public void TestSaveToFile()
		{
			string filename = "test";
			for(int i = 0; i < files.Length; i++)
			{
				entry = AtomEntry.Load(files[i]);
				Assert.IsNotNull(entry);
				entry.Save(filename);
				File.Delete(filename);
			}
		}

		[Test]
		[ExpectedException(typeof(FormatException))]
		public void TestInvalidEntries()
		{
			string filename = "test";
			entriespath = @"..\..\tests\invalidentries\";
			files = Directory.GetFiles(entriespath);
			for(int i = 0; i < files.Length; i++)
			{
				entry = AtomEntry.Load(files[i]);
				Assert.IsNotNull(entry);
				entry.Save(filename);
				File.Delete(filename);
			}
		}

		[Test]
		public void TestStream()
		{
			for(int i = 0; i < files.Length; i++)
			{
				FileStream stream = File.OpenRead(files[i]);
				entry = AtomEntry.Load(stream);
				Assert.IsNotNull(entry);
				MemoryStream memStream = new MemoryStream();
				entry.Save(memStream);
				stream.Close();
			}
		}

		[Test]
		public void TestTextReaderWriter()
		{
			for(int i = 0; i < files.Length; i++)
			{
				StreamReader reader = new StreamReader(files[i]);
				entry = AtomEntry.Load(reader);
				Assert.IsNotNull(entry);
				MemoryStream stream = new MemoryStream();
				StreamWriter w = new StreamWriter(stream);
				entry.Save(w);
				stream.Close();
			}
		}

		[Test]
		public void TestXmlReaderXmlWriter()
		{
			for(int i = 0; i < files.Length; i++)
			{
				XmlTextReader reader = new XmlTextReader(files[i]);
				entry = AtomEntry.Load(reader);
				Assert.IsNotNull(entry);
				MemoryStream stream = new MemoryStream();
				XmlTextWriter writer = new XmlTextWriter(stream,
					System.Text.Encoding.Default);
				entry.Save(writer);
				stream.Close();
			}
		}

		[Test]
		public void TestXmlFragment()
		{
			string filename = "test";
			for(int i = 0; i < files.Length; i++)
			{
				StreamReader reader = new StreamReader(files[i]);
				string content = reader.ReadToEnd();
				entry = AtomEntry.LoadXml(content);
				Assert.IsNotNull(entry);
				entry.Save(filename);
				File.Delete(filename);
			}
		}

		[Test]
		public void TestNameIsNotEmail()
		{
			foreach(string file in files)
			{
				entry = AtomEntry.Load(file);
				if(entry.Author != null)
					Assert.IsFalse(AU.Utils.IsEmail(entry.Author.Name));
				foreach(AtomPersonConstruct contributor in entry.Contributors)
					Assert.IsFalse(AU.Utils.IsEmail(contributor.Name));
			}
		}

		#endregion

		#region Test binary serialization

		[Test]
		public void TestSerialization()
		{
			MemoryStream stream = new MemoryStream();
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream, entry);
			stream.Close();
		}

		#endregion
    }
}
