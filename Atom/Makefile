CSC = mcs /debug /res:mediatypes.txt /d:DEBUG /r:System.Web.dll
ASSEMBLY = Atom.NET.dll

SOURCES = \
./AssemblyInfo.cs \
./AtomReader.cs \
./AtomWriter.cs \
./Atom.AdditionalElements/ScopedElementCollection.cs \
./Atom.AdditionalElements/ScopedElement.cs \
./Atom.AdditionalElements/DublinCore/DcElement.cs \
./Atom.Core/AtomContentConstruct.cs \
./Atom.Core/AtomContent.cs \
./Atom.Core/AtomDateConstruct.cs \
./Atom.Core/AtomElement.cs \
./Atom.Core/AtomEntry.cs \
./Atom.Core/AtomFeed.cs \
./Atom.Core/AtomGenerator.cs \
./Atom.Core/AtomLink.cs \
./Atom.Core/AtomPersonConstruct.cs \
./Atom.Core/Enums.cs \
./Atom.Core/Exceptions.cs \
./Atom.Core.Collections/AtomContentCollection.cs \
./Atom.Core.Collections/AtomEntryCollection.cs \
./Atom.Core.Collections/AtomLinkCollection.cs \
./Atom.Core.Collections/AtomPersonConstructCollection.cs \
./Atom.Utils/DefaultValues.cs \
./Atom.Utils/Utils.cs \
./mvpxml/XmlNamespaces.cs \
./mvpxml/XPathNavigatorReader.cs \
./tests/Atom.Core/AtomContentTest.cs \
./tests/Atom.Core/AtomEntryTest.cs \
./tests/Atom.Core/AtomFeedTest.cs \
./tests/Atom.Core/AtomLinksTest.cs \
./tests/Atom.Utils/UtilsTest.cs

all : $(FILES)
	$(CSC) $(FILES) /target:library /out:$(ASSEMBLY)

clean: $(ASSEMBLY)
	rm -f $(ASSEMBLY)

