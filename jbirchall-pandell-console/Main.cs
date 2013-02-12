/*
 * Copyright (c) James Birchall 2013
*/ 

using System;
using System.Resources;
using jbirchall_shuffle;

namespace jbirchall_pandell_console
{
    /// <summary>
    /// Main class.
    /// </summary>
    /// <remarks>
    /// The specification was very unclear as to how to show the output.
    /// 
    /// If I were on the other side of this problem, I would want to pipe the output
    /// of any application through my own command line tool (sort, in particular)
    /// to confirm their correctness.
    /// 
    /// So I implemented a console application that does that.
    /// 
    /// Command line in Windows can be irritating for some devs though...so I also
    /// added a simple text box to the WPF version.
    /// </remarks>
    class PandellApp
    {
        /// <summary>
        /// Main method.
        /// </summary>
        /// <param name="args">Command line arguments</param>
        static int Main(string[] args)
        {
            try
            {
                int nCount = 10000;

                // This is fairly rudimentary argument parsing.
                // If we need more sophisticated work (and depending upon the project requirements)
                // we'd either:
                //   1)  Implement our own (if we're into a closely licensed situation, for example)
                //   2)  Buy one (if we do that sort of thing)
                //   3)  Use something like NDesk.Options

                // In general though, command line parsing in C# kinda sucks when compared to what's
                // available in Python and the like.  If we were doing a lot of command line work,
                // and had the tools available, I'd strongly consider something like that instead.

                if (args.Length > 0 && args[0].Equals("-h"))
                {
                    Console.Write(Usage());
                }
                else
                {
                    if (args.Length > 0)
                    {
                        nCount = Int32.Parse(args[0]);
                    }

                    EmitShuffle(nCount);
                }

                // By convention, zero exit codes indicates all ok.
                return 0;
            }
            catch (Exception e)
            {
                // Generally, we'd implement a logging scheme (log4net is nice) in here, but I don't want to leave junk on your 
                // computer so we'll leave it with a note.

                Console.Error.WriteLine(e.Message);
                Console.Write(Usage());

            }

            // By convention, non-zero exit codes indicate faults.
            return 1;
        }

        /// <summary>
        /// Emits a shuffled array of nCount elements to stdout.
        /// </summary>
        /// <param name="nCount">Number of elements to shuffle and emit.</param>
        static unsafe void EmitShuffle(int nCount)
        {
            FastShuffleArray out_array = new FastShuffleArray(nCount);
            for (int i = 0; i < out_array.Size; i++)
                Console.WriteLine(out_array.Pointer[i]);
        }

        
        /// <summary>
        /// Retrieve the usage string in the user's language.
        /// </summary>
        /// <returns>A string in the current user's language.</returns>
        /// <remarks>
        /// This is a simple use case where we would consider whether to localise the output or not using an embedded resource.
        /// </remarks>
        static string Usage()
        {
            ResourceManager rm = new ResourceManager("jbirchall_pandell_console.Properties.Resources", typeof(PandellApp).Assembly);
            return rm.GetString("Usage");
        }
    }
}
