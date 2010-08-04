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

        public override List<string> Branches() {
            using (DirManager d = new DirManager(root)) {
                var branches = new List<string>();
                Program task = new Program("bzr", "branches");

                string b;
                while ((b = task.stdout.ReadLine()) != null) {
                    branches.Add(b);
                }
                return branches;
            }
        }
    }
}

