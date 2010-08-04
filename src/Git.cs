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

        public override List<string> Branches() {
            using (DirManager d = new DirManager(root)) {
                var branches = new List<string>();
                Program task = new Program("git", "branch -a");

                string line;
                while ((line = task.stdout.ReadLine()) != null) {
                    var b = line.TrimStart();

                    if (b.StartsWith("*")) {
                        branches.Add(b.Substring(2));
                    } else {
                        branches.Add(b);
                    }
                }

                return branches;
            }
        }
    }
}

