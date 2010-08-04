using System;
using System.Collections.Generic;

namespace VcsSharp {

    public class Bzr : Vcs {
        public Bzr() {}

        public Bzr(string path) {
            root = path;
        }

        public override bool Init(string path) {
 
            // Bazaar will abort with a suitable exit code if the repository
            // already exists
            //
            return (new Program("bzr", "init -- "+path)).result;
        }
    }
}

