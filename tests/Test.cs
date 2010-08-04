using System;
using System.IO;
using System.Collections.Generic;
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

        git = Factory.GetRepo(repo);
        if (git == null) {
            return false;
        }

        if (!(git.Branches().Count > 0)) {
            Console.WriteLine("Git.Branches() failed");
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

        hg = Factory.GetRepo(repo);
        if (hg == null) {
            return false;
        }

        if (!(hg.Branches().Count > 0)) {
            Console.WriteLine("Hg.Branches() failed");
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

        bzr = Factory.GetRepo(repo);
        if (bzr == null) {
            return false;
        }

        if (!(bzr.Branches().Count > 0)) {
            Console.WriteLine("Bzr.Branches() failed");
            return false;
        }

        return true;
    }
}

