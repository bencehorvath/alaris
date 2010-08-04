/* 
  	* AtomLinksTest.cs
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
using NUnit.Framework;
using Atom.Core;
using Atom.Core.Collections;
using Atom.Utils;

namespace Atom.Core.Test
{
	[TestFixture]
	public class AtomLinksTest
	{
		AtomLinkCollection links;
		AtomLink fistLink;
		AtomLink secondLink;
		
		[SetUp]
		public void Init()
		{
			links = new AtomLinkCollection();
			fistLink = new AtomLink();
			secondLink = new AtomLink();
		}

//		[Test]
//		[ExpectedException(typeof(MainAlternateLinkMissingException))]
//		public void TestAlternateLinkMissing()
//		{
//			// add a link without a non alternate rel
//			// as the first element
//			fistLink.HRef = new Uri("http://www.w3.org");
//			fistLink.Rel = Relationship.Start;
//			fistLink.Title = "sample text";
//			fistLink.Type = MediaType.TextPlain;
//			
//			links.Add(fistLink);
//		}

		[Test]
		[ExpectedException(typeof(DuplicateLinkException))]
		public void TestDuplicateLinks()
		{
			// add a link with the same rel and type
			fistLink.HRef = new Uri("http://www.w3.org");
			fistLink.Rel = Relationship.Alternate;
			fistLink.Title = "sample text";
			fistLink.Type = MediaType.TextPlain;

			secondLink.HRef = new Uri("http://www.w3.org");
			secondLink.Rel = Relationship.Alternate;
			secondLink.Title = "sample text";
			secondLink.Type = MediaType.TextPlain;

			links.Add(fistLink);
			links.Add(secondLink);
		}

		[Test]
		public void TestLinksSameRelDifferentTypes()
		{
			// add two links with the same rel but different
			// types
			fistLink.HRef = new Uri("http://www.w3.org");
			fistLink.Rel = Relationship.Alternate;
			fistLink.Title = "sample text";
			fistLink.Type = MediaType.TextPlain;

			secondLink.HRef = new Uri("http://www.w3.org");
			secondLink.Rel = Relationship.Alternate;
			secondLink.Title = "sample text";
			secondLink.Type = MediaType.TextXml;
	
			links.Add(fistLink);
			links.Add(secondLink);
		}
	}
}
