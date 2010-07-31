
namespace VcsSharp {

    public class Hg : Vcs {
        public override bool Init(string path) {

            // Mercurial will abort with a suitable exit code if the
            // repository already exists
            //
            return (new Program("hg", "init -- "+path)).result;
        }
    }
}

