/*
 * Copyright (c) James Birchall 2013
*/

using System;

namespace jbirchall_pandell_test
{
    /// <summary>
    /// Utility container class to hold first order statistics.
    /// </summary>
    class FirstOrderStatistic
    {
        /// <summary>
        /// Number of elements.
        /// </summary>
        public int Count;

        /// <summary>
        /// Deviation from the sample mean.
        /// </summary>
        public double Deviation;

        /// <summary>
        /// Squared deviation from the sample mean.
        /// </summary>
        public double SquaredDeviation;

        /// <summary>
        /// Constructor
        /// </summary>
        public FirstOrderStatistic()
        {
            Count = 1;
            SquaredDeviation = 0;
            Deviation = 0;
        }
    }
}
