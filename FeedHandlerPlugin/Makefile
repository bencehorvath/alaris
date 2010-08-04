
EXTRA_DIST =  Makefile

# Warning: This is an automatically generated file, do not edit!

srcdir=.
top_srcdir=..

include $(top_srcdir)/config.make

ifeq ($(CONFIG),DEBUG)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG"
ASSEMBLY = ../alaris/bin/Debug/plugins/MangosRssPlugin.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../AlarisAPI/bin/Debug/AlarisAPI.dll \
	../irc/bin/Debug/irc.dll \
	../alaris/bin/Debug/alaris.exe
BUILD_DIR = ../alaris/bin/Debug/plugins

ALARISAPI_DLL_SOURCE=../AlarisAPI/bin/Debug/AlarisAPI.dll
IRC_DLL_SOURCE=../irc/bin/Debug/irc.dll
ALARIS_EXE_SOURCE=../alaris/bin/Debug/alaris.exe
MANGOSRSSPLUGIN_DLL_MDB_SOURCE=../alaris/bin/Debug/plugins/MangosRssPlugin.dll.mdb
MANGOSRSSPLUGIN_DLL_MDB=$(BUILD_DIR)/MangosRssPlugin.dll.mdb

endif

ifeq ($(CONFIG),RELEASE)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize-
ASSEMBLY = ../alaris/bin/Release/plugins/MangosRssPlugin.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../AlarisAPI/bin/Release/AlarisAPI.dll \
	../irc/bin/Release/irc.dll \
	../alaris/bin/Release/alaris.exe
BUILD_DIR = ../alaris/bin/Release/plugins

ALARISAPI_DLL_SOURCE=../AlarisAPI/bin/Release/AlarisAPI.dll
IRC_DLL_SOURCE=../irc/bin/Release/irc.dll
ALARIS_EXE_SOURCE=../alaris/bin/Release/alaris.exe
MANGOSRSSPLUGIN_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(ALARISAPI_DLL) \
	$(IRC_DLL) \
	$(ALARIS_EXE) \
	$(MANGOSRSSPLUGIN_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(MANGOSRSSPLUGIN_PC)  


RESGEN=resgen2

ALARISAPI_DLL = $(BUILD_DIR)/AlarisAPI.dll
IRC_DLL = $(BUILD_DIR)/irc.dll
ALARIS_EXE = $(BUILD_DIR)/alaris.exe
MANGOSRSSPLUGIN_PC = $(BUILD_DIR)/mangosrssplugin.pc

FILES = \
	Plugin.cs \
	AssemblyInfo.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	mangosrssplugin.pc.in 

REFERENCES =  \
	System

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

#Targets
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG)  $(top_srcdir)/config.make

include $(top_srcdir)/Makefile.include
#include $(srcdir)/custom-hooks.make



$(eval $(call emit-deploy-target,ALARISAPI_DLL))
$(eval $(call emit-deploy-target,IRC_DLL))
$(eval $(call emit-deploy-target,ALARIS_EXE))
$(eval $(call emit-deploy-wrapper,MANGOSRSSPLUGIN_PC,mangosrssplugin.pc))


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
	$(call cp,$(ALARISAPI_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(IRC_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(ALARIS_EXE),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(MANGOSRSSPLUGIN_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	mkdir -p '$(DESTDIR)$(libdir)/pkgconfig'
	$(call cp,$(MANGOSRSSPLUGIN_PC),$(DESTDIR)$(libdir)/pkgconfig)
	make post-install-local-hook prefix=$(prefix)

uninstall-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-uninstall-local-hook prefix=$(prefix)
	make uninstall-satellite-assemblies prefix=$(prefix)
	$(call rm,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(ALARISAPI_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(IRC_DLL),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(ALARIS_EXE),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(MANGOSRSSPLUGIN_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(MANGOSRSSPLUGIN_PC),$(DESTDIR)$(libdir)/pkgconfig)
	make post-uninstall-local-hook prefix=$(prefix)

install: install-local
uninstall: uninstall-local
clean: clean-local

include $(top_srcdir)/rules.make
