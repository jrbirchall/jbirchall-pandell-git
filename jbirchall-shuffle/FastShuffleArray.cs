/*
 * Copyright (c) James Birchall 2013
*/ 

using System;
using System.Runtime.InteropServices;

namespace jbirchall_shuffle
{
    /// <summary>
    /// Class to show how to solve the Pandell problem if we stick to the precise
    /// problem definition of shuffling the integers from 1 to 10,000.
    /// </summary>
    /// <remarks>
    /// Interestingly, we can go ~10% then Fisher-Yates in Debug but Fisher-Yates starts to beat us when we 
    /// make it compiled with optimised code and in release mode.
    /// 
    /// Always faster then Durstenfeld but that's not surprising as this is an implementation of Durstenfeld.
    /// 
    /// Included here as an example of how to use unsafe code and 'cause implementing RadixSort is fun.
    /// </remarks>
    public unsafe class FastShuffleArray : IDisposable
    {
        // Static random number generator.  
        // If we get multiple threads using this class we don't have to worry about generating duplicate sequences.
        protected static Random s_rand = new Random();

        protected int m_nSize;
        /// <summary>
        /// Size of the array in elements.
        /// </summary>
        public int Size
        {
            get
            {
                return m_nSize;
            }
        }

        // Internal buffer.
        protected IntPtr m_hBuffer;
        /// <summary>
        /// Read-only pointer to the internal buffer.  
        /// </summary>
        /// <remarks>
        /// This is dangerous in general since we're leaking the core reference out to who knows where, 
        /// but we're into optimisation style programming so the ability to shoot yourself 
        /// in the foot is part of the game and what it takes to go fast.
        /// 
        /// Drive safe!
        /// </remarks>
        public int* Pointer
        {
            get
            {
                return (int*)m_hBuffer;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="size">
        /// The number of elements in the array.
        /// </param>
        public FastShuffleArray(int size)
        {
            m_nSize = size;
            m_hBuffer = Marshal.AllocHGlobal(size * sizeof(int));
            if (m_hBuffer == IntPtr.Zero)
                throw new OutOfMemoryException();

            Shuffle();
        }

        /// <summary>
        /// Actual clean up.  Gets called from both Dispose and the destructor.
        /// </summary>
        /// <param name="disposing"></param>
        protected void Dispose(Boolean disposing)
        {
            if (IntPtr.Zero != m_hBuffer)
            {
                Marshal.FreeHGlobal(m_hBuffer);
                m_hBuffer = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Disposes of the array resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Destructor
        /// </summary>
        /// <remarks>
        /// Frees the underlying pointer.
        /// </remarks>
        ~FastShuffleArray()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Fast implementation of the specific problem of shuffling integers
        /// 1 ... n.
        /// </summary>
        public void Shuffle()
        {
            int* pArray = this.Pointer;
            pArray[0] = 1;
            for (int i = 1; i < m_nSize; i++)
            {
                int j = s_rand.Next(0, i + 1);
                pArray[i] = pArray[j];
                pArray[j] = i + 1;
            }
        }

        /// <summary>
        /// Sort the contents in-place.
        /// </summary>
        public void Sort()
        {
            RadixSort(Pointer, m_nSize);
        }

        /// <summary>
        /// Fast sort for a pointer array of integers.
        /// </summary>
        /// <param name="pNumbers">Array to be sorted in-place.</param>
        /// <param name="nSize">Size of the buffer.</param>
        /// <remarks>
        /// Crappy part about unsafe optimised code is that you often have to do things
        /// yourself.
        /// 
        /// But the nice thing is that because we can be specific, we can implement
        /// more ideal operations.
        /// 
        /// </remarks>
        protected unsafe void RadixSort(int* pNumbers, int nSize)
        {
            // (note dimensions 2^nBitsPerGroup which is the number of all possible values of n nBitsPerGroup-bit number) 
            int nBitsPerGroup = 4;
            int nBitsPerInt = 32;
            int nDimensions = 1 << nBitsPerGroup;

            // number of groups 
            int groups = (int)Math.Ceiling((double)nBitsPerInt / (double)nBitsPerGroup);

            // the mask to identify groups 
            int mask = (1 << nBitsPerGroup) - 1;

            // Working storage arrays.
            int* pTemp = null;
            int* pCount = null;
            int* pPrefixes = null;

            try
            {
                pTemp = (int*)Marshal.AllocHGlobal(nSize * sizeof(int));
                if (pTemp == null) throw new OutOfMemoryException();

                pCount = (int*)Marshal.AllocHGlobal(nDimensions * sizeof(int));
                if (pCount == null) throw new OutOfMemoryException();

                pPrefixes = (int*)Marshal.AllocHGlobal(nDimensions * sizeof(int));
                if (pPrefixes == null) throw new OutOfMemoryException();

                for (int c = 0, shift = 0; c < groups; c++, shift += nBitsPerGroup)
                {
                    // Reset pCount
                    for (int j = 0; j < nDimensions; j++)
                        pCount[j] = 0;

                    // counting elements of the c-th group 
                    for (int i = 0; i < nSize; i++)
                        pCount[(pNumbers[i] >> shift) & mask]++;

                    // calculating prefixes 
                    pPrefixes[0] = 0;
                    for (int i = 1; i < nDimensions; i++)
                        pPrefixes[i] = pPrefixes[i - 1] + pCount[i - 1];

                    // from pNumbers[] to pTemp[] elements ordered by c-th group 
                    for (int i = 0; i < nSize; i++)
                        pTemp[pPrefixes[(pNumbers[i] >> shift) & mask]++] = pNumbers[i];

                    // pNumbers[] = pTemp[] and start again
                    for (int i = 0; i < nSize; i++)
                        pNumbers[i] = pTemp[i];
                }
            }
            finally
            {

                if (null != pTemp) Marshal.FreeHGlobal((IntPtr)pTemp);
                if (null != pCount) Marshal.FreeHGlobal((IntPtr)pCount);
                if (null != pPrefixes) Marshal.FreeHGlobal((IntPtr)pPrefixes);
            }
        }
    }
}
