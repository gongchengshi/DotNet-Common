using System.Linq;

namespace Gongchengshi
{
    /// <summary>
    /// Implementation of the Infinite Impulse Reponse filter.
    /// </summary>
    public class IirFilter
    {
        protected readonly double[] _a;
        protected readonly double[] _b;

        /// <summary>
        /// Uses the nonseeded version of the filter and the passed in a, b
        /// </summary>
        public IirFilter(double[] a, double[] b)
        {
            _a = a;
            _b = b;
        }

        public virtual double[] Filter(double[] data)
        {
            var filtered = new double[data.Length];

            for (int n = 0; n < filtered.Length; ++n)
            {
                // filtered[n] = b0*data(n) + b1*data(n-1) + b2*data(n-2) + b3*data(n-3) + b4*data(n-4)
                //             - a1*filtered(n-1) - a2*filtered(n-2) - a3*filtered(n-3) - a4*filtered(n-4)
                for (int i = 0; i < _b.Length && i <= n; ++i)
                {
                    filtered[n] += _b[i] * data[n - i];
                }
                for (int i = 1; i < _a.Length && i <= n; ++i)
                {
                    filtered[n] -= _a[i] * filtered[n - i];
                }
            }

            return filtered;
        }
    }

    /// <summary>
    /// Implementation of the Infinite Impulse Reponse filter that saves its state each time Filter is run so that 
    /// the current run starts off where the previous run left off.
    /// </summary>
    public sealed class BlockIirFilter : IirFilter
    {
        private readonly double[] _x;
        private readonly double[] _y;

        public BlockIirFilter(double[] a, double[] b)
            : base(a, b)
        {
            _x = Enumerable.Repeat(0.0, b.Length - 1).ToArray();
            _y = Enumerable.Repeat(0.0, a.Length - 1).ToArray();
        }

        public override double[] Filter(double[] data)
        {
            var filtered = new double[data.Length];

            for (int n = 0; n < data.Length; ++n)
            {
                // y[n] = b0*x(n) + b1*x(n-1) + b2*x(n-2) + b3*x(n-3) + b4*x(n-4)
                //      - a1*y(n-1) - a2*y(n-2) - a3*y(n-3) - a4*y(n-4)                
                for (int i = 0; i < _b.Length; ++i)
                {
                    // x = _x for the first _b.Length - 1 values
                    double x = i <= n ? data[n - i] : _x[i - n - 1];

                    filtered[n] += _b[i] * x;
                }
                for (int i = 1; i < _a.Length; ++i)
                {
                    // y = _y for the first _a.Length - 1 values
                    double y = i <= n ? filtered[n - i] : _y[i - n - 1];

                    filtered[n] -= _a[i] * y;
                }
            }

            // Save state
            for (int i = 0, j = data.Length - 1; i < _x.Length; ++i, --j)
            {
                _x[i] = data[j];
            }
            for (int i = 0, j = filtered.Length - 1; i < _y.Length; ++i, --j)
            {
                _y[i] = filtered[j];
            }
            
            return filtered;
        }
    }
}