/*
 * Copyright (c) James Birchall 2013
*/ 

using System;
using System.Collections.Generic;

namespace jbirchall_shuffle
{
    /// <summary>
    /// Generators that produce integers.
    /// </summary>
    public class IntGenerator
    {
        /// <summary>
        /// Infinite series generator that generates increasing value integers.
        /// </summary>
        /// <returns>Yields +1 per iteration</returns>
        public static IEnumerator<int> LinearSequence()
        {
            int i = 1;
            while (true) yield return i++;
        }
    }
}
