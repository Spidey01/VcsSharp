#
# Makefile for Microsoft NMake and .NET Framework.
#
CSC=csc /nologo
CSCFLAGS=/optimize+ /warn:4

all: bin bin\VcsSharp.dll

test: all bin\Test.exe 
	bin\Test.exe

bin\VcsSharp.dll: src\Vcs.cs src\Git.cs src\Hg.cs src\Bzr.cs src\Svn.cs src\Cvs.cs
	$(CSC) $(CSCFLAGS) /target:library /out:"$@" $**

bin:
	mkdir bin

bin\Test.exe: tests\Test.cs
	$(CSC) $(CSCFLAGS) /lib:bin /r:VcsSharp.dll /out:"$@" $**

clean:
	rmdir /q /s bin
