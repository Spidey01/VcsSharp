using System;
using VcsSharp;

///
/// Tests that are run:
///     factory function for opening existing repos
///
///     default constructor and Init() works.
///         both with and without an existing repo
///
class Test {

    public static void Main(string[] args) {
        Console.WriteLine(
                "Testing git... " + (Git() ? "pass" : "fail")
        );
        Console.WriteLine(
                "Testing mercurial... " + (Hg() ? "pass" : "fail")
        );
        Console.WriteLine(
                "Testing bazaar... " + (Bzr() ? "pass" : "fail")
        );
    }

    static bool Git() {

        Vcs git = new Git();
        if (!git.Init("/tmp/test.git")) {
            Console.WriteLine("Git.Init() failed");
            return false;
        }

        Vcs vcs = Factory.GetRepo("/tmp/test.git");
        if (vcs == null) {
            return false;
        }

        return true;
    }

    static bool Hg() {

        Vcs hg = new Hg();
        if (!hg.Init("/tmp/test.hg")) {
            Console.WriteLine("Hg.Init() failed");
            return false;
        }

        Vcs vcs = Factory.GetRepo("/tmp/test.hg");
        if (vcs == null) {
            return false;
        }

        return true;
    }
    static bool Bzr() {

        Vcs bzr = new Bzr();
        if (!bzr.Init("/tmp/test.bzr")) {
            Console.WriteLine("Bzr.Init() failed");
            return false;
        }

        Vcs vcs = Factory.GetRepo("/tmp/test.bzr");
        if (vcs == null) {
            return false;
        }

        return true;
    }
}

