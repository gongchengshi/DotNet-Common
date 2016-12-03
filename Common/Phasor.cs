using System;

namespace Gongchengshi
{
    /// <summary>
    /// A phasor value.
    /// </summary>
    public struct Phasor
    {
        public Phasor(float real, float imaginary)
        {
            Real = real;
            Imaginary = imaginary;
        }
        public static Phasor FromPolar(float magnitude, float angle)
        {
            return new Phasor(RealFromPolar(magnitude, angle), ImaginaryFromPolar(magnitude, angle));
        }

        public Single Real;
        public Single Imaginary;

        public Single Magnitude
        {
            get
            {
                return MagnitudeFromRect(Real, Imaginary);
            }
        }
        /// <summary>
        /// Returns the angle in -180.0 to 180.0 degree notation.
        /// </summary>
        public Single AngleDegrees
        {
            get
            {
                return AngleDegreesFromRect(Real, Imaginary);
            }
        }

        /// <summary>
        /// Compute the polar coordinate angle for the given rectangular coordinates.
        /// Angle is returned in -180.0 to 180.0 degree notation.
        /// </summary>
        public static float AngleDegreesFromRect(float real, float imag)
        {
            Single angle = (Single)Math.Atan2(imag, real);
            angle = (Single)(angle * 180 / Math.PI);
            return angle;
        }

        /// <summary>
        /// Compute the polar coordinate magnitude for the given rectangular coordinates.
        /// </summary>
        public static float MagnitudeFromRect(float real, float imag)
        {
            return (Single)Math.Sqrt(real * real + imag * imag);
        }

        /// <summary>
        /// Compute the rectangular coordinate real value for the given polar coordinates.
        /// Angle is expected in radians.
        /// </summary>        
        public static float RealFromPolar(float mag, float angle)
        {
            return (float)(mag * Math.Cos(angle));
        }

        /// <summary>
        /// Compute the rectangular coordinate imaginary value for the given polar coordinates.
        /// Angle is expected in radians.
        /// </summary>        
        public static float ImaginaryFromPolar(float mag, float angle)
        {
            return (float)(mag * Math.Sin(angle));
        }
    }
}
