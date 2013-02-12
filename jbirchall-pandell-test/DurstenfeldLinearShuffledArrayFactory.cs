/*
 * Copyright (c) James Birchall 2013
*/ 

using System;
using System.Collections.Generic;
using jbirchall_shuffle;

namespace jbirchall_pandell_test
{
    /// <summary>
    /// Class that embodies the factory pattern for creating interchangable shuffled arrays.  Uses Durstenfeld under the covers.
    /// </summary>
    class DurstenfeldLinearShuffledArrayFactory : IShuffledArrayFactory
    {
        /// <summary>
        /// Generate a shuffled integer array using the Durstenfeld shuffle.
        /// </summary>
        /// <param name="size"># of elements to allocate</param>
        /// <returns>A shuffled integer Array</returns>
        public Array Generate(int size)
        {
            return Shuffler.DurstenfeldShuffle<int>(size, IntGenerator.LinearSequence());
        }

    }
}
