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

        public override List<string> Branches() {
            using (DirManager d = new DirManager(root)) {
                var branches = new List<string>();

                Program task = new Program("hg", "branches");

                string line;
                while ((line = task.stdout.ReadLine()) != null) {
                    branches.Add(line.Substring(0, line.IndexOf(' ')));
                }
                return branches;
            }
        }
    }
}

