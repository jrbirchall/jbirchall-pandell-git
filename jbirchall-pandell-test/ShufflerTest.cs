/*
 * Copyright (c) James Birchall 2013
*/ 

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using jbirchall_shuffle;

namespace jbirchall_pandell_test
{
    /// <summary>
    /// Unit tests for the Shuffler.
    /// </summary>
    [TestClass]
    public class ShufflerTest
    {
        /// <summary>
        /// Test to confirm that the randomness generated using the Durstenfeld process
        /// is Gaussian.
        /// </summary>
        [TestMethod]
        public void TestDurstenfeldShufflerRandomness()
        {
            TestShufflerRandomness(new DurstenfeldLinearShuffledArrayFactory());
        }

        /// <summary>
        /// Test to confirm that the randomness generated using the Fisher-Yates process
        /// is Gaussian.
        /// </summary>
        [TestMethod]
        public void TestFisherYatesShufflerRandomness()
        {
            TestShufflerRandomness(new FisherYatesLinearShuffledArrayFactory());
        }

        /// <summary>
        /// Test to confirm that the Fisher-Yates shuffle doesn't change the contents.
        /// </summary>
        [TestMethod]
        public void TestFisherYatesShufflerCorrectness()
        {
            int[] aCorrect = new int[100];
            for (int i = 0; i < aCorrect.Length; i++)
            {
                aCorrect[i] = i + 1;
            }

            TestShufflerCorrectness(new FisherYatesLinearShuffledArrayFactory(), aCorrect);
        }

        /// <summary>
        /// Test to confirm that the Durstenfeld shuffle doesn't change the contents.
        /// </summary>
        [TestMethod]
        public void TestDurstenfeldShufflerCorrectness()
        {
            int[] aCorrect = new int[100];
            for (int i = 0; i < aCorrect.Length; i++)
            {
                aCorrect[i] = i + 1;
            }

            TestShufflerCorrectness(new DurstenfeldLinearShuffledArrayFactory(), aCorrect);
        }

        /// <summary>
        /// Test that the underlying shuffle generator produces something that is somewhat Gaussian.
        /// </summary>
        /// <param name="shuffled_array_generator"></param>
        /// <remarks>
        /// This is one of those things that doesn't make a lot of sense to unit test but I'm including it here
        /// for completeness.  And testing for normality is fun!
        /// 
        /// The issue is that any test of probability is also a test that will probably be incorrect.  So any unit
        /// test for randomness may randomly be wrong (either false positive or false negative).
        /// 
        /// Which is irritating in dev shops that integrate unit tests as part of the build process - the build
        /// randomly fails but it's really ok.
        /// 
        /// The better solution is to prove the algorithm correct and control any changes with rigourous review.
        /// 
        /// BTW, we can't just infer that since we're using the built in random number generator that everything
        /// is hunky dorey from a randomness perspective.  We're doing a shuffle and we want to show that the 
        /// shuffle adheres to a normal distribution.
        /// 
        /// Here we're looking at the distribution of a large number of iterations and checking that it tends
        /// towards the sample mean and then putting a test around that boundary.
        /// 
        /// For the purposes of this demonstration, I'm not going any farther into the tests for whether a distribution
        /// is normal.  Nor am I going to specify the probabilty that this test will fail.
        /// </remarks>
        private void TestShufflerRandomness(IShuffledArrayFactory shuffled_array_generator)
        {
            IDictionary<string, FirstOrderStatistic> distribution = new Dictionary<string, FirstOrderStatistic>();

            // You need to tune this parameter based on your particular machine, your randomness specifications 
            // and the number of unit tests you're writing.  
            // More samples = fewer false negatives but takes longer to calculate.
            int samples = 100000;
            int items = 5;

            int distributions = Statistics.factorial(items);
            int expected_distribution_count = samples / distributions;
            double standard_deviation = Math.Sqrt(samples);

            for (int i = 0; i < samples; i++)
            {
                Array arry = shuffled_array_generator.Generate(items);

                // This section creates a unique signature for a given distribution and then
                // increments the count for the distribution.

                // Casting it to a string is a little ugly from an efficiency perspective 
                // but it's simple, correct and clear.

                // The string format is hardcoded to minimise the efficiency penalty.
                string distribution_id = string.Format("{0}|{1}|{2}|{3}|{4}", 
                    arry.GetValue(0), arry.GetValue(1), arry.GetValue(2), arry.GetValue(3), arry.GetValue(4));

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

            // Absence a proper specification, this is the ad hoc definition of acceptable randomness.
            Assert.IsTrue(dStdErr < 1.5);
        }

        /// <summary>
        /// Tests that the passed in shuffler doesn't mess with the items passed in.
        /// </summary>
        /// <param name="shuffled_array_generator"></param>
        /// <param name="correct"></param>
        /// <remarks>
        /// This one is much easier and saner to unit test.  We simply want to check that the random function returns
        /// all the values in the sample specified.
        /// 
        /// The generator is necessary because the difference between Yates and Durstenfeld is that one allocates and
        /// initialises where the other just shuffles.
        /// </remarks>
        private void TestShufflerCorrectness(IShuffledArrayFactory shuffled_array_generator, Array correct)
        {
            Array arry = shuffled_array_generator.Generate(correct.Length);
            Array.Sort(arry);

            CollectionAssert.AreEqual(correct, arry);
        }

    }
}
