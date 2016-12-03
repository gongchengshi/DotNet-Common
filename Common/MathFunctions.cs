using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Gongchengshi
{
    /// <summary>
    /// Miscellaneous math functions.  Essentially this is an extension of the System.Math class.
    /// </summary>
    public static class MathFunctions
    {
        /// <summary>
        /// Calculates the signal to noise ratio from the two digital signals. 
        /// origional and regression must be the same length.
        /// </summary>
        public static double SignalSNR(double[] original, double[] regression)
        {
            return 20.0 * Math.Log10(SumOfSquares(original) / SumOfSquareDifferences(original, regression));
        }

        public static double SumOfSquares(double[] values)
        {
            return values.Select(x => x * x).Sum();
        }

        /// <summary>
        /// Simple function to find the the sum of the squares of the differences of items each array. 
        /// a must be the same length of b.
        /// </summary>
        public static double SumOfSquareDifferences(double[] a, double[] b)
        {
            if (a.Length != b.Length)
            {
                throw new ArgumentException("a and b must be equal length arrays.");
            }

            double sum = 0.0;

            for (int i = 0; i < a.Length; i++)
            {
                var diff = a[i] - b[i];
                sum += (diff * diff);
            }

            return sum;
        }

        /// <summary>
        /// Subtracts the mean of the array from each value in the array in place.
        /// </summary>
        public static void RemoveDcOffset(double[] data)
        {
            double mean = data.Average();

            for (int i = 0; i < data.Length; ++i)
            {
                data[i] -= mean;
            }
        }

        /// <summary>
        /// Scales each value of the array by the max absolute value of all the values.
        /// </summary>
        public static void ScaleByMaxAbs(double[] data)
        {
            // Scale to +/- 1
            Scale(data, data.Max(x => Math.Abs(x)));
        }

        /// <summary>
        /// Scales each value of the array by value in place.
        /// </summary>
        private static void Scale(double[] data, double value)
        {
            if (value == 0.0)
            {
                for (int i = 0; i < data.Length; ++i)
                {
                    data[i] = 0.0;
                }
                return;
            }

            for (int i = 0; i < data.Length; ++i)
            {
                data[i] /= value;
            }
        }

        /// <summary>
        /// Note:  Ignores NaN values (they are excluded from the computation)
        /// </summary>
        public static float MeanSquareError(IList<float> left, IList<float> right)
        {
            Debug.Assert(left.Count == right.Count);

            double sum = 0.0f;
            int count = 0;
            for (int i = 0; i < left.Count; i++)
            {
                var lValue = left[i];
                var rValue = right[i];
                if (!float.IsNaN(lValue) && !float.IsNaN(rValue))
                {
                    var difference = lValue - rValue;
                    sum += difference*difference;
                    count++;
                }
            }
            
            if (count == 0)
            {
                return 0.0f;
            }

            return (float) (sum/count);
        }
    }


    public struct Averager
    {
        public void Add(float value)
        {
            Count++;
            _sum += value;
        }

        public float Average
        {
            get
            {
                if (Count == 0)
                {
                    return float.NaN;
                }
                return (float)(_sum / Count);
            }
        }

        public int Count { get; private set; }
        private double _sum;
    }
}