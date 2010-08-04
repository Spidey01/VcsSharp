using System;
using System.IO;
using System.Collections.Generic;

namespace VcsSharp {

    public class Hg : Vcs {

        public Hg() {}

        public Hg(string path) {
            root = path;
        }

        public override bool Init(string path) {
            root = path;

            // Mercurial will abort with a suitable exit code if the
            // repository already exists
            //
            return (new Program("hg", "init -- "+path)).result;
        }
    }
}

