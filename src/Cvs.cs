using System;
using System.Collections.Generic;

namespace VcsSharp {

    public class Cvs : Vcs {

        public Cvs() {
            throw new NotImplementedException("Sorry - not done yet!");
        }

        public Cvs(string path) {
            root = path;
            throw new NotImplementedException("Sorry - not done yet!");
        }

        public override bool Init(string path) {
            return false;
        }

        public override List<string> Branches() {
            return null;
        }
    }
}

