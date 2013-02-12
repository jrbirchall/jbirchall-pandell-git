/*
 * Copyright (c) James Birchall 2013
*/ 

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using System.Runtime.InteropServices;

namespace jbirchall_shuffle
{
    /// <summary>
    /// Class to shuffle arrays.
    /// </summary>
    public class Shuffler
    {
        /// <summary>
        /// Static random number generator.  
        /// </summary>
        /// <remarks>
        /// If we get multiple threads using this class we don't have to worry about generating duplicate sequences.
        /// </remarks>
        protected static Random s_rand = new Random();

        /// <summary>
        /// Classic implemenation of the Fisher-Yates shuffle.  Shuffles the contents of the array passed in.
        /// </summary>
        /// <typeparam name="T">The type of the contents of the array.</typeparam>
        /// <param name="arry">The array to shuffle.  In place modification.</param>
        public static void FisherYatesShuffle<T>(T[] arry)
        {
            for (int i = arry.Length - 1; i >= 0; i--)
            {
                int j = s_rand.Next(0, i + 1);
                T tmp = arry[i];
                arry[i] = arry[j];
                arry[j] = tmp;
            }
        }

        /// <summary>
        /// Generator based implemenation of Durstenfeld shuffle.  Initialises and returns a shuffled array based
        /// on the generator passed in.
        /// </summary>
        /// <typeparam name="T">Type contained in the output array.</typeparam>
        /// <param name="count">Number of elements.</param>
        /// <param name="gen_ex">Generator to produce elements in the output array.</param>
        /// <returns>An array of [count] items of type [T] generated using [gen_ex] and shuffled using the Durstenfeld algorithm.</returns>
        /// <remarks>
        /// Generator based implementation of Durstenfeld shuffle, a little more 
        /// flexible and elegant though performance is dependant upon the generator.
        /// 
        /// Handy for initialising a shuffled array in one go rather then shuffling
        /// something else but generally performs worse then FisherYates (the 
        /// overhead of enumerating the generator usually outweighs the gains
        /// of not doing the initialisation).
        /// 
        /// Much more extensible though.
        /// </remarks>
        public static T[] DurstenfeldShuffle<T>(int nCount, IEnumerator<T> gen_ex)
        {
            T[] outVal = new T[nCount];

            // Iterate to the first value
            gen_ex.MoveNext();
            outVal[0] = gen_ex.Current;

            for (int i = 1; i < nCount; i++)
            {
                int j = s_rand.Next(0, i + 1);
                outVal[i] = outVal[j];

                gen_ex.MoveNext();
                outVal[j] = gen_ex.Current;
            }

            return outVal;
        }
    }
}
