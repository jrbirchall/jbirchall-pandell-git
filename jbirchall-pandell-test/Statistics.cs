/*
 * Copyright (c) James Birchall 2013
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbirchall_pandell_test
{
    /// <summary>
    /// Container for statistical methods.
    /// </summary>
    class Statistics
    {
        /// <summary>
        /// Classic factorial (n!) algorithm.
        /// </summary>
        /// <param name="fact">The integer to factorial</param>
        /// <returns>n!</returns>
        public static int factorial(int fact)
        {
            int outVal = 1;
            for (int i = fact; i > 0; i--)
            {
                outVal *= i;
            }

            return outVal;
        }
    }
}
