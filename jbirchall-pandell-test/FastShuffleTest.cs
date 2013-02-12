/*
 * Copyright (c) James Birchall 2013
*/ 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using jbirchall_shuffle;

namespace jbirchall_pandell_test
{
    /// <summary>
    /// Tests for the FastShuffleArray
    /// </summary>
    /// <seealso cref="jbirchall_pandell_app.FastShuffleArray"/>
    /// <remarks>
    /// And now you see another circumstance where unit-testing in a compiled language comes 
    /// with some maintenance costs of its own.
    /// 
    /// In order to confirm that the FastShuffleArray actually works, we have to re-implement
    /// TestShufflerRandomness and TestShufflerCorrectness from ShufflerTest virtually
    /// verbatim; the only difference between the two is the semantic change of working with
    /// pointers instead of working with Arrays.
    /// 
    /// We could, of course, make the pointer implement the Array interface but then we'd
    /// undo all of the optimisation we just put in.
    /// </remarks>
    [TestClass]
    public class FastShuffleTest
    {
        /// <summary>
        /// Test to confirm that the distribution of FastShuffleArray is Gaussian.
        /// </summary>
        /// <seealso cref="ShufflerTest.TestShufflerRandomness"/>
        [TestMethod]
        public unsafe void TestFastShufflerRandomness()
        {
            IDictionary<string, FirstOrderStatistic> distribution = new Dictionary<string, FirstOrderStatistic>();

            // You need to tune this parameter based on your particular environment, your randomness specifications 
            // and the number of unit tests you're writing.  
            // More samples = fewer false negatives but takes longer to calculate.
            int samples = 100000;
            int items = 5;

            int distributions = Statistics.factorial(items);
            int expected_distribution_count = samples / distributions;

            for (int i = 0; i < samples; i++)
            {
                FastShuffleArray fast_array = new FastShuffleArray(items);
                int* pFastArray = fast_array.Pointer;

                // This section creates a unique signature for a given distribution and then
                // increments the count for the distribution.

                // Casting it to a string is a little ugly from an efficiency perspective 
                // but it's simple, correct and clear.

                // The string format is hardcoded to minimise the efficiency penalty.
                string distribution_id = string.Format("{0}|{1}|{2}|{3}|{4}",
                    pFastArray[0], pFastArray[1], pFastArray[2], pFastArray[3], pFastArray[4]);

                FirstOrderStatistic stats;
                distribution.TryGetValue(distribution_id, out stats);
                if (null == stats)
                {
                    stats = new FirstOrderStatistic();
                }
                stats.Count = stats.Count + 1;
                distribution[distribution_id] = stats;
            }

            // Check that the distribution is roughly equal by confirming that the distribution of results 
            // conforms to a normal distribution.

            // First, we need to create a set of second order statistics.  
            // We do it here as a second pass to save having to do intermediate calculations that are irrelevant.
            foreach (KeyValuePair<string, FirstOrderStatistic> kvp in distribution)
            {
                kvp.Value.Deviation = kvp.Value.Count - expected_distribution_count;
                kvp.Value.SquaredDeviation = Math.Pow(kvp.Value.Deviation, 2);
            }

            // Now, finally, we need one more pass to consolidate all of that information and actually check that the distribution is normal.
            double dMeanSs = 0.0;
            foreach (KeyValuePair<string, FirstOrderStatistic> kvp in distribution)
            {
                dMeanSs += kvp.Value.SquaredDeviation;
            }

            double dMeanVar = dMeanSs / samples;
            double dStdErr = Math.Sqrt(dMeanVar);

            // This is basically a derived tolerance.  If you were specifying something be random in the
            // real world, just how random it needed to be would be part of the specification.
            Assert.IsTrue(dStdErr < 1.5);
        }

        /// <summary>
        /// Tests that FastShuffleArray satisfies the requirement that it 
        /// generate numbers from 1 to 10000 inclusive.
        /// </summary>
        /// <seealso cref="ShufflerTest.TestShufflerCorrectness"/>
        [TestMethod]
        public unsafe void TestFastShufflerCorrectness()
        {
            int size = 10000;
            FastShuffleArray fast_array = new FastShuffleArray(size);
            fast_array.Sort();
            int* pFastArray = fast_array.Pointer;

            for (int i = 0; i < size; i++)
            {
                Assert.AreEqual(i + 1, pFastArray[i]);
            }

        }

        /// <summary>
        /// Test that the fast array is actually faster then the other options.
        /// </summary>
        /// <remarks>
        /// Generally, we don't do performance tests in unit tests.  However, the
        /// specification for FastShufflerArray is that it actually *BE* faster
        /// then the other methods so it's fair to put that test in here.
        /// 
        /// I've also put a performance advantage of .5% Fisher-Yates and 1% over Durnstenfeld in here.  
        /// If this test starts to fail then it's a good indicator that we no longer need FastShufflerArray
        /// and should improve it or remove it from the code base.
        /// 
        /// Downside is that in continuous integration environments changes that are irrelevant may be
        /// suspected of breaking the build if we're getting close to the wire and the test is failing due
        /// to environmental load.
        /// 
        /// Finally, this is measuring a CPU intense process which can change drastically based on the 
        /// system load at the time of the test.
        /// </remarks>
        [TestMethod]
        public void TestActuallyFaster()
        {
            int iterations = 100;
            int nSize = 10000;
            long lFastArrayTicks, lYatesShufflerTicks, lDurstenfeldShufflerTicks;
            Stopwatch stopwatch = new Stopwatch();

            // Time FastShuffleArray
            stopwatch.Start();
            FastShuffleArray fast_array;
            for (int i = 0; i < iterations; i++)
            {
                fast_array = new FastShuffleArray(nSize);
            }
            stopwatch.Stop();
            lFastArrayTicks = stopwatch.ElapsedTicks;

            // Time Fisher-Yates
            stopwatch.Restart();
            for (int i=0; i < iterations; i++)
            {
                int[] arry = new int[nSize];
                for (int j = 0; j < nSize; j++)
                {
                    arry[j] = j + 1;
                }

                Shuffler.FisherYatesShuffle<int>(arry);
            }
            stopwatch.Stop();
            lYatesShufflerTicks = stopwatch.ElapsedTicks;

            // Time Durstenfeld.
            stopwatch.Restart();
            for (int i = 0; i < iterations; i++)
            {
                int[] arry = Shuffler.DurstenfeldShuffle<int>(nSize, IntGenerator.LinearSequence());
            }
            stopwatch.Stop();
            lDurstenfeldShufflerTicks = stopwatch.ElapsedTicks;

            double dAdvantageOverYates = ((double) lYatesShufflerTicks - lFastArrayTicks) / lFastArrayTicks;
            double dAdvantageOverDurstenfeld = ((double) lDurstenfeldShufflerTicks - lFastArrayTicks) / lFastArrayTicks;

            // This is another oddity a performance unit test adds to the system: it really should emit its data so that
            // actual performance over time can be calibrated.
            Console.WriteLine("Adv. over Fisher-Yates = [{0}]", dAdvantageOverYates.ToString("#0.##%"));
            Console.WriteLine("Adv. over Durstenfeld = [{0}]", dAdvantageOverDurstenfeld.ToString("#0.##%"));

            // Assert performance criteria.
            Assert.IsTrue(dAdvantageOverYates > 0.005, 
                "Not faster then Yates by enough.  FastArray is only {0} faster.", 
                dAdvantageOverYates.ToString("#0.##%"));

            Assert.IsTrue(dAdvantageOverDurstenfeld > 0.01, 
                "Not faster then Durstenfeld by enough.  FastArray is only {0} faster.",
                 dAdvantageOverDurstenfeld.ToString("#0.##%"));
        }

    }
}
