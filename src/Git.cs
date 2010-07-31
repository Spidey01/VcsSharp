using System;
using System.IO;

namespace VcsSharp {

    public class Git : Vcs {

        public override bool Init(string path) {

            if (Directory.Exists(path)) {
                return false;
            }
            return run("git", "init -- "+path);
        }
    }
}

