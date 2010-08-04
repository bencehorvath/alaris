
EXTRA_DIST =  Makefile

# Warning: This is an automatically generated file, do not edit!

srcdir=.
top_srcdir=..

include $(top_srcdir)/config.make

ifeq ($(CONFIG),DEBUG)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ "-nowarn:169,414" -debug "-define:DEBUG" "-main:Alaris.Core.Entry"
ASSEMBLY = bin/Debug/alaris.exe
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = exe
PROJECT_REFERENCES =  \
	../irc/bin/Debug/irc.dll \
	../AlarisAPI/bin/Debug/AlarisAPI.dll
BUILD_DIR = bin/Debug

IRC_DLL_SOURCE=../irc/bin/Debug/irc.dll
ALARISAPI_DLL_SOURCE=../AlarisAPI/bin/Debug/AlarisAPI.dll
ALARIS_EXE_MDB_SOURCE=bin/Debug/alaris.exe.mdb
ALARIS_EXE_MDB=$(BUILD_DIR)/alaris.exe.mdb

endif

ifeq ($(CONFIG),RELEASE)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- "-main:Alaris.Core.Entry"
ASSEMBLY = bin/Release/alaris.exe
ASSEMBLY_MDB = 
COMPILE_TARGET = exe
PROJECT_REFERENCES =  \
	../irc/bin/Release/irc.dll \
	../AlarisAPI/bin/Release/AlarisAPI.dll
BUILD_DIR = bin/Release

IRC_DLL_SOURCE=../irc/bin/Release/irc.dll
ALARISAPI_DLL_SOURCE=../AlarisAPI/bin/Release/AlarisAPI.dll
ALARIS_EXE_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(IRC_DLL) \
	$(ALARISAPI_DLL) \
	$(ALARIS_EXE_MDB)  

BINARIES = \
	$(ALARIS)  


RESGEN=resgen2

IRC_DLL = $(BUILD_DIR)/irc.dll
ALARISAPI_DLL = $(BUILD_DIR)/AlarisAPI.dll
ALARIS = $(BUILD_DIR)/alaris

FILES = \
	Alaris.cs \
	Main.cs \
	AssemblyInfo.cs \
	ScriptManager.cs \
	Log.cs \
	Exceptions.cs \
	PublicMessageHandler.cs \
	API.cs \
	CrashHandler.cs \
	WebHelper.cs \
	Constants.cs \
	RssHandlers.cs \
	Utilities.cs \
	HaskellEvaluator.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	bin/Debug/alaris.conf \
	../Doxyfile \
	alaris.in 

REFERENCES =  \
	ICSharpCode.SharpZipLib \
	Mono.Data \
	Mono.Http \
	Mono.Posix \
	Mono.Security \
	-pkg:monodoc \
	System \
	System.Core \
	System.Data \
	System.Web \
	System.Xml \
	System.Xml.Linq \
	Mono.CSharp \
	Mono.Management \
	System.Management \
	-pkg:mono-debugger

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(BINARIES) 

#Targets
all: $(ASSEMBLY) $(PROGRAMFILES) $(BINARIES)  $(top_srcdir)/config.make

include $(top_srcdir)/Makefile.include
#include $(srcdir)/custom-hooks.make



$(eval $(call emit-deploy-target,IRC_DLL))
$(eval $(call emit-deploy-target,ALARISAPI_DLL))
$(eval $(call emit-deploy-wrapper,ALARIS,alaris,x))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'


$(ASSEMBLY_MDB): $(ASSEMBLY)
$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	make pre-all-local-hook prefix=$(prefix)
	mkdir -p $(shell dirname $(ASSEMBLY))
	make $(CONFIG)_BeforeBuild
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
	make $(CONFIG)_AfterBuild
	make post-all-local-hook prefix=$(prefix)

install-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-install-local-hook prefix=$(prefix)
	make install-satellite-assemblies prefix=$(prefix)
	mkdir -p '$(DESTDIR)$(libdir)/$(PACKAGE)'
	$(call cp,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(IRC_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(ALARISAPI_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(ALARIS_EXE_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	mkdir -p '$(DESTDIR)$(bindir)'
	$(call cp,$(ALARIS),$(DESTDIR)$(bindir))
	make post-install-local-hook prefix=$(prefix)

uninstall-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-uninstall-local-hook prefix=$(prefix)
	make uninstall-satellite-assemblies prefix=$(prefix)
	$(call rm,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(IRC_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(ALARISAPI_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(ALARIS_EXE_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(ALARIS),$(DESTDIR)$(bindir))
	make post-uninstall-local-hook prefix=$(prefix)

install: install-local
uninstall: uninstall-local
clean: clean-local

include $(top_srcdir)/rules.make
