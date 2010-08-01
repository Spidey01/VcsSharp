using System;
using System.IO;
using VcsSharp;

///
/// Tests that are run:
///     factory function for opening existing repos
///
///     default constructor and Init() works.
///         both with and without an existing repo
///
class Test {
    static string tmpdir;

    static void usage() {
            Console.WriteLine("usage: Test.exe [tmpdir]");
            Environment.Exit(1);
    }

    public static void Main(string[] args) {

        if (args.Length == 1) {
            tmpdir = args[0];
        } else {
            if (args.Length > 1) {
                Console.WriteLine("error: more than one tmpdir" +
                                  " specified, aborting!");
            }
            usage();
        }

        try {
            Console.WriteLine(
                    "Testing git... " + (Git() ? "pass" : "fail")
            );
            Console.WriteLine(
                    "Testing mercurial... " + (Hg() ? "pass" : "fail")
            );
            Console.WriteLine(
                    "Testing bazaar... " + (Bzr() ? "pass" : "fail")
            );
        } catch(Exception e) {
            Console.WriteLine(e);
        }
    }

    static bool Git() {
        string repo = Path.Combine(tmpdir, "test.git");
        Vcs git = new Git();

        if (!git.Init(repo)) {
            Console.WriteLine("Git.Init() failed");
            return false;
        }

        Vcs vcs = Factory.GetRepo(repo);
        if (vcs == null) {
            return false;
        }

        return true;
    }

    static bool Hg() {
        string repo = Path.Combine(tmpdir, "test.hg");
        Vcs hg = new Hg();

        if (!hg.Init(repo)) {
            Console.WriteLine("Hg.Init() failed");
            return false;
        }

        Vcs vcs = Factory.GetRepo(repo);
        if (vcs == null) {
            return false;
        }

        return true;
    }
    static bool Bzr() {
        string repo = Path.Combine(tmpdir, "test.bzr");
        Vcs bzr = new Bzr();

        if (!bzr.Init(repo)) {
            Console.WriteLine("Bzr.Init() failed");
            return false;
        }

        Vcs vcs = Factory.GetRepo(repo);
        if (vcs == null) {
            return false;
        }

        return true;
    }
}

