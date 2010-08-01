#
# Makefile for Microsoft NMake and .NET Framework.
#
CSC=csc /nologo
CSCFLAGS=/optimize+ /warn:4

all: $(TMPDIR) bin bin\VcsSharp.dll

test: all bin\Test.exe 
	bin\Test.exe $(TMPDIR)

bin\VcsSharp.dll: src\Vcs.cs src\Git.cs src\Hg.cs src\Bzr.cs src\Svn.cs src\Cvs.cs
	$(CSC) $(CSCFLAGS) /target:library /out:"$@" $**

bin:
	MKDIR bin

$(TMPDIR):
	MKDIR $(TMPDIR)

bin\Test.exe: tests\Test.cs
	$(CSC) $(CSCFLAGS) /lib:bin /r:VcsSharp.dll /out:"$@" $**

clean:
	IF EXIST bin RMDIR /q /s bin
	IF EXIST $(TMPDIR)\test.git RMDIR /q /s $(TMPDIR)\test.git
	IF EXIST $(TMPDIR)\test.hg RMDIR /q /s $(TMPDIR)\test.hg
	IF EXIST $(TMPDIR)\test.bzr RMDIR /q /s $(TMPDIR)\test.bzr
