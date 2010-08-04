using System;
using System.Collections.Generic;

namespace VcsSharp {

    public class Svn : Vcs {

        public Svn() {
            throw new NotImplementedException("Sorry - not done yet!");
        }

        public Svn(string path) {
            root = path;
            throw new NotImplementedException("Sorry - not done yet!");
        }

        public override bool Init(string path) {
            return false;
        }
    }
}

