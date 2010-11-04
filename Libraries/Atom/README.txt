
Atom.NET 0.4.3
--------------

Atom.NET is an open source library entirely developed in C# aimed to generate and parse Atom feeds in an handy way.
It currently support only Atom 0.3 draft specification.


Requirements
------------

- MS .NET Framework 1.0/1.1
or
- Mono 0.29 and later

- Windows (tested on Windows 2000 and XP)
or
- Linux (tested on Gentoo Linux)

Compilation
-----------

MS.NET:
  Create a VS.NET project and import Atom.NET, compile and build it
  -or-
  Use nant (http://nant.sf.net) against the atomnet.build nant file

Mono:
  Type make in the root directory
  -or-
  Use nant (http://nant.sf.net) against the atomnet.build nant file


Installation
------------

MS.NET:
  Put the Atom.NET.dll in your project folder and reference it. Otherwise you can
  simply install in the your Windows GAC folder executing "gacutil /i Atom.NET.dll"

Mono:
  Copy the Atom.NET.dll and reference it in your project.

Uninstallation
--------------

MS.NET:
 Simply remove the Atom.NET.dll from your project folder and drop the reference to
 it. If you have installed it into the GAC, run "gacutil /u Atom.NET"

Mono:
  Remove the referenced dll

Usage
-----

See the example page online. You can also take a look at the unit testing suite.

Contact
-------

For bugs please use the bug tracker (https://sourceforge.net/tracker/?group_id=98691&atid=621734),
for feature requests use the "feature requests" section of Sourceforge project page:
https://sourceforge.net/tracker/?group_id=98691&atid=621737 and for other questions please
contact me at l.oluyede@virgilio.it.

There is also a public mailing list:
http://lists.sourceforge.net/lists/listinfo/atomnet-discuss

If you wish to join the project, let me know.

Links
-----

Project website: http://atomnet.sourceforge.net
Sourceforge project page: http://sourceforge.net/projects/atomnet
Atom 0.3 draft spec: http://www.mnot.net/drafts/draft-nottingham-atom-format-02.html
Atom Wiki: http://www.intertwingly.net/wiki/pie/FrontPage
RSS.NET project website: http://rss-net.sourceforge.net/
MVP-XML library: http://sourceforge.net/projects/mvp-xml
