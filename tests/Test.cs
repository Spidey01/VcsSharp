using System;
using VcsSharp;

class Test {

    public static void Main(string[] args) {
        Console.WriteLine(
                "Testing git... " + (Git() ? "pass" : "fail")
        );
    }

    static bool Git() {

        Vcs vcs = Factory.GetRepo("/tmp/test.git");
        if (vcs == null) {
            return false;
        }

        return true;
    }
}

