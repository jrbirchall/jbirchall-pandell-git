/*
 * Copyright (c) James Birchall 2013
*/ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        static unsafe int Main(string[] args)
        {
            try
            {
                int nCount = 10000;

                if (args.Length > 0 && args[0] == "-h")
                {
                    Console.Write(Usage());
                }
                else if (args.Length > 0)
                {
                    nCount = Int32.Parse(args[0]);
                }
                FastShuffleArray out_array = new FastShuffleArray(nCount);
                for (int i = 0; i < out_array.Size; i++)
                    Console.WriteLine(out_array.Pointer[i]);
                return 0;
            }
            catch (Exception e)
            {
                // Generally, we'd implement a logging scheme (log4net is nice) in here, but I don't want to leave junk on your 
                // computer so we'll leave it with a note.

                Console.Error.WriteLine(e.Message);
                Console.Write(Usage());

            }

            return 1;
        }

        /// <summary>
        /// Retrieve the usage string.
        /// </summary>
        /// <returns></returns>
        static string Usage()
        {
            return @"
Pandell application console implementation.  Emits a shuffled list of integers with each integer on a seperate line.
Usage:  jbirchall-pandell-console  [-h] [# of items]
    -h  Print help.
";

        }
    }
}
