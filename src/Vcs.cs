///
/// @file
/// @author Terry Matthew Poulin <BigBoss1964@gmail.com>
///
/// This file is licensed under the terms of the zlib/libpng license and
/// copyrighted to the author named above. You should have received a file
/// named COPYING along with this projects source code, containing a copy of
/// the license text.
///
/// @see http://github.com/Spidey01/VcsSharp for the latest code.
///
/// @see http://spidey01.blogspot.com/search/label/Projects/VcsSharp 
///      for the authors blog posts associated with this code.
///

using System;
using System.IO;
using System.Collections.Generic;

namespace VcsSharp {

    ///
    /// Enumeration describing what repositories VcsSharp can handle.
    /// Type.Unknown is reserved as an obvious null value.
    ///
    public enum Type {
        Unknown,
        // listed in order of my respect
        Git,
        Hg,
        Bzr,
        Svn,
        Cvs,
    }

    public abstract class Vcs {
    }

    /// Factory class for accessing an existing repository via Vcs.
    ///
    ///     - If you don't know what VCS is being used, call Factory.GetRepo()
    ///        with the path to the repositories root directory.  You'll get a
    ///        ready to rock'n'roll Vcs object returned.  This ideally, is how
    ///        you open an existing repository via VcsSharp.
    ///
    ///     - If you want to initialize a <b>new</b> repository, then you
    ///       obviously know which class to use--along with the corresponding
    ///       Init() method it provides.
    ///
    public class Factory {

        /// Factory function for creating Vcs objects.
        ///
        /// @param path 
        ///     The root directory of the repository
        ///
        /// @returns 
        ///     An implementation of Vcs opened to path or null if the
        ///     repository could not be handled.
        ///
        public static Vcs GetRepo(string path) {

            switch (DetectVcsType(path)) {
                case Type.Git:
                    return new Git();
                case Type.Hg:
                    return new Hg();
                case Type.Bzr:
                    return new Bzr();
                case Type.Svn:
                    return new Svn();
                case Type.Cvs:
                    return new Cvs();
                case Type.Unknown:
                default:
                    return null;
            }
        }

        private static Dictionary<string, Type> known = null;

        /// Attempts to detect the Type of repository.
        ///
        /// Beware that this function is not very smart: if it looks like a
        /// duck repository, it is reported as a duck repository! This is at
        /// least, suffisant for most tasks, and you shouldn't be faking the
        /// appearance of any given duck anyway, unless you known how to quack
        /// just right ;p
        /// 
        /// @param path 
        ///     The repository root directory to check.
        ///
        /// @returns 
        ///     VcsSharp.Type describing path.
        ///
        public static Type DetectVcsType(string path) {

            // This isn't pretty but it saves us from having to alloc/dealloc
            // the dictionary + key/value pairs on *every* call.
            //
            // We don't know the clients performance needs, so show 
            // a little respect for others!
            //
            if (known == null) {
                known = new Dictionary<string, Type>();
                known.Add(".git", Type.Git);
                known.Add(".hg", Type.Hg);
                known.Add(".bzr", Type.Bzr);
                known.Add(".svn", Type.Svn);
                known.Add(".cvs", Type.Cvs);
            }

            foreach(var type in known.Keys) {
                if (Directory.Exists(Path.Combine(path, type))) {
                    return known[type];
                }
            }
            return Type.Unknown;
        }

    }
}

