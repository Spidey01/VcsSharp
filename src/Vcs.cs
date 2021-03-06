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

        public string root { get; protected set; }

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

        ///
        /// @returns
        ///     a List enumerating all branches in the repository.
        ///
        public abstract List<string> Branches();


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

                Action<StreamReader> test = (stream) => {
                    string line;
                    while ((line = stream.ReadLine()) != null) {
                        Console.WriteLine("\t"+line);
                    }
                };
                Console.WriteLine("standard output:");
                test(stdout);
                Console.WriteLine("standard error:");
                test(stderr);
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

                // XXX System.Func<> - requires C# 3.5 or later
                //
                Func<string, char, string[]> split;
                split = (s, sep) => s.Split(new Char[] { sep });

                switch (Environment.OSVersion.Platform) {
                    //
                    // handle %PathExt% to add the extension
                    //
                    case PlatformID.Win32S:
                    case PlatformID.Win32Windows:
                    case PlatformID.Win32NT:
                        string exts = Environment.GetEnvironmentVariable("PathExt");
                        if (exts == null) goto FAIL;

                        foreach (string e in split(exts, ';')) {
                            foreach (string p in split(path, ';')) {
                                string check = Path.Combine(p, cmd+e);
                                if (File.Exists(check)) {
                                    return check;
                                }
                            }
                        }

                        break;
                    //
                    // OS doesn't need an extension
                    //
                    case PlatformID.Unix:
                    case PlatformID.MacOSX:
                        foreach (string p in path.Split(new Char[] { ':' })) {
                            string check = Path.Combine(p, cmd);
                            if (File.Exists(check)) {
                                return check;
                            }
                        }

                        break;
                    default:
                        throw new NotImplementedException("OS not supported yet!");
                }

            FAIL:
                return cmd;
            }
        }

        /// CWD helper class for Vcs subclasses
        ///
        /// The C# equal to a RAII class for changing/restoring the processes
        /// current working directory.
        ///
        ///
        protected sealed class DirManager : IDisposable {
            private string pwd;
            private bool disposed = false;

            public DirManager(string dir) {
                pwd = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(dir);
            }

            ~DirManager() {
                Dispose(false);
            }

            public void Dispose() {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            public void Dispose(bool disposing) {
                if (this.disposed) {
                    return;
                }

                Directory.SetCurrentDirectory(pwd);
                disposed = true;
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
                    return new Git(path);
                case Type.Hg:
                    return new Hg(path);
                case Type.Bzr:
                    return new Bzr(path);
                case Type.Svn:
                    return new Svn(path);
                case Type.Cvs:
                    return new Cvs(path);
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
                known = new Dictionary<string, Type> {
                    { "git", Type.Git },
                    { ".hg", Type.Hg },
                    { ".bzr", Type.Bzr },
                    { ".svn", Type.Svn },
                    { ".cvs", Type.Cvs },
                };
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

