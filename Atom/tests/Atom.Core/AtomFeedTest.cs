/* 
  	* AtomFeedTest.cs
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
    public class AtomFeedTest
    {
		AtomFeed feed;
		Uri testUri;
		string feedspath;
		string[] files;
		
		[SetUp]
		public void Init()
		{
			feed = new AtomFeed();
			testUri = new Uri("http://www.w3.org");
			feedspath = @"..\..\tests\feeds\";
			files = Directory.GetFiles(feedspath);
		}

		#region Test of properties and elements

		[Test]
		public void TestVersion()
		{
			Assert.AreEqual(feed.Version, "0.3");
		}

		[Test]
		public void TestXmlLang()
		{
			Assert.AreEqual(feed.XmlLang, AU.DefaultValues.Language);

			feed.XmlLang = Language.it_IT;
		}

		[Test]
		public void TestTitle()
		{
			AtomContentConstruct title = new AtomContentConstruct();
			title.Content = "sample text";
			title.Mode = Mode.Xml;
			title.Type = MediaType.TextPlain;

			feed.Title = title;

			Assert.AreEqual(title.LocalName, "title");
			Assert.AreEqual(title.FullName, "atom:title");
			Assert.AreSame(feed.Title, title);
		}

		[Test]
		public void TestLinks()
		{
			AtomLink link = new AtomLink();
			link.HRef = testUri;
			link.Rel = Relationship.Alternate;
			link.Title = "sample text";
			link.Type = MediaType.TextPlain;
			
			feed.Links.Add(link);

			Assert.AreEqual(link.LocalName, "link");
			Assert.AreEqual(link.FullName, "atom:link");
			Assert.AreSame(feed.Links[0], link);
			Assert.AreEqual(feed.Links[0], link);
		}

		[Test]
		public void TestAuthor()
		{
			AtomPersonConstruct author = new AtomPersonConstruct();
			author.Email = "foo@bar.com";
			author.Name = "Uncle Tom";
			author.Url = testUri;

			feed.Author = author;

			Assert.AreEqual(author.LocalName, "author");
			Assert.AreEqual(author.FullName, "atom:author");
            Assert.AreSame(feed.Author, author);
		}

		[Test]
		public void TestContributors()
		{
			AtomPersonConstruct contributor = new AtomPersonConstruct("contributor");
			contributor.Email = "bar@foo.com";
			contributor.Name = "Uncle Bob";
			contributor.Url = testUri;

			feed.Contributors.Add(contributor);

			Assert.AreEqual(contributor.LocalName, "contributor");
			Assert.AreEqual(contributor.FullName, "atom:contributor");
			Assert.AreSame(feed.Contributors[0], contributor);
			Assert.AreEqual(feed.Contributors[0], contributor);
		}

		[Test]
		public void TestTagline()
		{
			AtomContentConstruct tagline = new AtomContentConstruct("tagline");
			tagline.Content = "sample text";
			tagline.Mode = Mode.Xml;
			tagline.Type = MediaType.TextPlain;

			feed.Tagline = tagline;

			Assert.AreEqual(tagline.LocalName, "tagline");
			Assert.AreEqual(tagline.FullName, "atom:tagline");
			Assert.AreSame(feed.Tagline, tagline);
		}

		[Test]
		public void TestId()
		{
			feed.Id = testUri;

			Assert.AreSame(feed.Id, testUri);
		}

		[Test]
		public void TestCopyright()
		{
			AtomContentConstruct copyright = new AtomContentConstruct("copyright");
			copyright.Content = " 2003 Buffalo Bill Corporations";
			copyright.Mode = Mode.Xml;
			copyright.Type = MediaType.TextPlain;

			feed.Copyright = copyright;
			Assert.AreEqual(copyright.LocalName, "copyright");
			Assert.AreEqual(copyright.FullName, "atom:copyright");
			Assert.AreSame(feed.Copyright, copyright);
		}

		[Test]
		public void TestInfo()
		{
			AtomContentConstruct info = new AtomContentConstruct("info");
			info.Content = "Some not so useful infos";
			info.Mode = Mode.Xml;
			info.Type = MediaType.TextPlain;

			feed.Info = info;

			Assert.AreEqual(info.LocalName, "info");
			Assert.AreEqual(info.FullName, "atom:info");
			Assert.AreSame(feed.Info, info);
		}

		[Test]
		public void TestModified()
		{
			AtomDateConstruct modifiedDate = new AtomDateConstruct();
			modifiedDate.DateTime = DateTime.Now;
			modifiedDate.UtcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Today);

			feed.Modified = modifiedDate;
			Assert.AreEqual(modifiedDate.LocalName, "modified");
			Assert.AreEqual(modifiedDate.FullName, "atom:modified");
			Assert.AreSame(feed.Modified, modifiedDate);
		}

		[Test]
		public void TestUri()
		{
			for(int i = 0; i < files.Length; i++)
			{
				Console.WriteLine("{0}: {1}", i, files[i]);
				feed = AtomFeed.Load(files[i]);
				Assert.IsNotNull(feed.Uri);
				foreach(AtomEntry entry in feed.Entries)
					Assert.IsNotNull(entry.Uri);
			}
		}

		#endregion

		#region Test of Load/Save methods

		[Test]
		public void TestSaveToFile()
		{
			Init();
			string filename = "test";
			for(int i = 0; i < files.Length; i++)
			{
				feed = AtomFeed.Load(files[i]);
				Assert.IsNotNull(feed);
				feed.Save(filename);
				File.Delete(filename);
			}
		}

		[Test]
		//[ExpectedException(typeof(RequiredElementNotFoundException))]
		[ExpectedException(typeof(XmlException))]
		public void TestInvalidFeeds()
		{
			string filename = "test";
			feedspath = @"..\..\tests\invalidfeeds\";
			files = Directory.GetFiles(feedspath);
			for(int i = 0; i < files.Length; i++)
			{
				feed = AtomFeed.Load(files[i]);
				Assert.IsNotNull(feed);
				feed.Save(filename);
				File.Delete(filename);
			}
		}

		[Test]
		public void TestStream()
		{
			for(int i = 0; i < files.Length; i++)
			{
				FileStream stream = File.OpenRead(files[i]);
				feed = AtomFeed.Load(stream);
				Assert.IsNotNull(feed);
				MemoryStream memStream = new MemoryStream();
				feed.Save(memStream);
				stream.Close();
			}
		}

		[Test]
		public void TestReaderWriter()
		{
			for(int i = 0; i < files.Length; i++)
			{
				StreamReader reader = new StreamReader(files[i]);
				feed = AtomFeed.Load(reader);
				Assert.IsNotNull(feed);
				MemoryStream stream = new MemoryStream();
				StreamWriter w = new StreamWriter(stream);
				feed.Save(w);
				stream.Close();
			}
		}

		[Test]
		public void TestXmlReaderXmlWriter()
		{
			for(int i = 0; i < files.Length; i++)
			{
				XmlTextReader reader = new XmlTextReader(files[i]);
				feed = AtomFeed.Load(reader);
				Assert.IsNotNull(feed);
				MemoryStream stream = new MemoryStream();
				XmlTextWriter writer = new XmlTextWriter(stream,
					System.Text.Encoding.Default);
				feed.Save(writer);
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
				feed = AtomFeed.LoadXml(content);
				Assert.IsNotNull(feed);
				feed.Save(filename);
				File.Delete(filename);
			}
		}

		[Test]
		public void TestXmlBaseFeedUri()
		{
			
			foreach(string file in files)
			{
				FileInfo info = new FileInfo(file);
				if(info.Name.ToLower() == "pilgrim.xml")
				{
					feed = AtomFeed.Load(file);
					Assert.IsNotNull(feed.XmlBase);
					Assert.AreEqual(feed.XmlBase.ToString(), "http://diveintomark.org/");
				}
			}
		}

		[Test]
		public void TestNameIsNotEmail()
		{
			foreach(string file in files)
			{
				feed = AtomFeed.Load(file);
				if(feed.Author != null)
					Assert.IsFalse(AU.Utils.IsEmail(feed.Author.Name));
				foreach(AtomPersonConstruct contributor in feed.Contributors)
					Assert.IsFalse(AU.Utils.IsEmail(contributor.Name));
				foreach(AtomEntry entry in feed.Entries)
				{
					if(entry.Author != null)
						Assert.IsFalse(AU.Utils.IsEmail(entry.Author.Name));
					foreach(AtomPersonConstruct contributor in entry.Contributors)
						Assert.IsFalse(AU.Utils.IsEmail(contributor.Name));
				}
			}
		}

		#endregion

		#region Test binary serialization

		[Test]
		public void TestSerialization()
		{
			MemoryStream stream = new MemoryStream();
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream, feed);
			stream.Close();
		}

		#endregion

		#region helper method

		/*private AtomFeed FillFeed(AtomFeed feed)
		{
			feed.Title = new AtomContentConstruct("title", "The title of the feed.");
			feed.Links.Add(new AtomLink(new Uri("http://www.w3.org"), Relationship.Alternate,
				MediaType.TextPlain, "The title of the link."));
			feed.Author = new AtomPersonConstruct("author", "Uncle Tom", new Uri("http://people.w3.org"), "foo@bar.com");
			feed.Contributors.Add(new AtomPersonConstruct("contributor", "Uncle Bob", new Uri("http://people.w3.org"), "foo@bar.com"));
			feed.Tagline = new AtomContentConstruct("tagline", "The tagline of the feed");
			feed.Id = new Uri("http://localhost/foo/id");
			feed.Copyright = new AtomContentConstruct("copyright", "Copyright  2003, 2004");
			feed.Info = new AtomContentConstruct("info", "The info of the feed.");
			feed.Modified = new AtomDateConstruct("modified", DateTime.Now, TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now));

			AtomEntry entry = new AtomEntry();
			entry.Title = new AtomContentConstruct("title", "The title of the entry.");
			entry.Links.Add(new AtomLink(new Uri("http://www.w3.org"), Relationship.Alternate,
				MediaType.TextPlain, "The title of the link."));
			entry.Contributors.Add(new AtomPersonConstruct("contributor", "Uncle Bob", new Uri("http://people.w3.org"), "foo@bar.com"));
			entry.Id = new Uri("http://localhost/foo/id");
			entry.Modified = new AtomDateConstruct("modified", DateTime.Now, TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now));
			entry.Issued = new AtomDateConstruct("issued", DateTime.Now, TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now));
			entry.Created = new AtomDateConstruct("created", DateTime.Now, TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now));
			entry.Summary = new AtomContentConstruct("summary", "The summary of the entry.");
			entry.Contents.Add(new AtomContent("The content of the entry."));

			feed.Entries.Add(entry);

			return feed;
		}*/

		#endregion
    }
}
