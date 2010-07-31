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
using System.Diagnostics;
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

        /// Initialize a new empty repository within path.
        ///
        /// Example:
        ///     @code
        ///
        ///         Vcs git = new Git();
        ///         if (!git.Init("Projects/myproject.git")) {
        ///             // handle failure to initialize repository
        ///         }
        ///         // use git object to manipulate repository
        ///
        ///     @endcode
        ///     
        ///
        public abstract bool Init(string path);


        /// Helper class for Vcs subclasses
        ///
        /// It basically allows you an easy way to run an external program
        /// without having to write much more code than you would use at a
        /// command prompt.  Propeterties are provided to access the result of
        /// the command, e.g.  output, exit status, etc. See the
        /// documentation.
        ///
        /// Note bene that invoking this classes constructor should be
        /// considered a blocking event, e.g. until the program has completed
        /// its run. The ability to delegate this to a later time might be
        /// added later. I don't see a need for it. If you need it before I
        /// find a reason to code it, patches are welcome ;).
        ///
        protected class Program {

            private Process proc;
            private ProcessStartInfo procinfo;

            ///
            /// This works as a wrapper around running the given version
            /// control systems program with the execution details most likely
            /// to be preferred. Note that this method may throw exceptions.
            ///
            /// @param cmd
            ///     The command to run, e.g. "cvs".  If cmd is not found,
            ///     expect an Exception to be thrown.
            ///
            /// @param args
            ///     Command line arguments for cmd, e.g. "status -l f1 f2"
            ///
            public Program(string cmd, string args) {
                procinfo = new ProcessStartInfo(find(cmd), args);

                procinfo.UseShellExecute = false;
                procinfo.CreateNoWindow = true;
                procinfo.ErrorDialog = false;
                procinfo.RedirectStandardError = true;
                procinfo.RedirectStandardInput = true;
                procinfo.RedirectStandardOutput = true;

                proc = Process.Start(procinfo);
                proc.WaitForExit();

#if VCSSHARP_DEBUG
                Console.WriteLine("Running: '"+cmd+" "+args+
                                  "' generated the following output:");

                string line;
                while ((line = proc.StandardOutput.ReadLine()) != null) {
                    Console.WriteLine("\t"+line);
                }
#endif
            }

            public string args {
                get { return procinfo.Arguments; }
            }

            public string file {
                get { return procinfo.FileName; }
            }

            public StreamReader stdout {
                get { return proc.StandardOutput; }
            }

            public StreamWriter stdin {
                get { return proc.StandardInput; }
            }

            public StreamReader stderr {
                get { return proc.StandardError; }
            }

            public bool result {
                get {
                    return proc.ExitCode == 0 ? true : false;
                }
            }

            /// Finds a command in the operating system search path.
            ///
            /// @param cmd
            ///     The command to locate in the PATH environment variable.
            ///
            /// @returns
            ///     Full path to cmd on success, or cmd on failure.
            ///
            protected string find(string cmd) {
                string path = Environment.GetEnvironmentVariable("PATH");
                if (path == null) {
                    goto FAIL;
                }

                foreach (string p in path.Split(new Char[] { ':' })) {
                    // TODO: handle %Pathext% on Windows.
                    string check = Path.Combine(p, cmd);
                    if (File.Exists(check)) {
                        return check;
                    }
                }
            FAIL:
                return cmd;
            }
        }
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

