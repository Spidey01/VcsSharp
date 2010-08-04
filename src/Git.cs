using System;
using System.IO;
using System.Collections.Generic;

namespace VcsSharp {

    public class Git : Vcs {

        public Git() { }

        public Git(string path) {
            root = path;
        }

        public override bool Init(string path) {

            if (Directory.Exists(path)) {
                return false;
            }
            return (new Program("git", "init -- "+path)).result;
        }
    }
}

