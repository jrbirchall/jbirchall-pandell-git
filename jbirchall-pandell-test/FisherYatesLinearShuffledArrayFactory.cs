/*
 * Copyright (c) James Birchall 2013
*/ 

using System;
using System.Collections.Generic;

using jbirchall_shuffle;

namespace jbirchall_pandell_test
{
    /// <summary>
    /// Class that embodies the factory pattern for creating interchangable shuffled arrays.  Uses Fisher-Yates under the covers. 
    /// </summary>
    class FisherYatesLinearShuffledArrayFactory : IShuffledArrayFactory
    {
        /// <summary>
        /// Generate a shuffled integer array using the Fisher-Yates shuffle.
        /// </summary>
        /// <param name="size"># of elements to allocate</param>
        /// <returns>A shuffled integer Array</returns>
        public Array Generate(int size)
        {
            int [] arry = new int[size];
            for (int i = 0; i < size; i++)
                arry[i] = i + 1;

            Shuffler.FisherYatesShuffle<int>(arry);
            return arry;
        }
    }
}
