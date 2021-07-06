using System;
using System.Collections.Generic;
using System.Text;

namespace Netherite.Physics
{
    public struct Vector3
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public static Vector3 operator *(Vector3 a, double s) => new Vector3(a.X * s, a.Y * s, a.Z * s);

        public static Vector3 operator *(double s, Vector3 a) => new Vector3(a.X * s, a.Y * s, a.Z * s);

        public static Vector3 operator /(Vector3 a, double b) => new Vector3(a.X / b, a.Y / b, a.Z / b);

        /// <summary>
        /// Get the inner product of <see cref="Vector3"/> <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a">The left <see cref="Vector3"/>.</param>
        /// <param name="b">The right <see cref="Vector3"/>.</param>
        /// <returns>The inner product of the two vectors.</returns>
        public static double Product(Vector3 a, Vector3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

        /// <summary>
        /// Get the cross product of <see cref="Vector3"/> <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a">The left <see cref="Vector3"/>.</param>
        /// <param name="b">The right <see cref="Vector3"/>.</param>
        /// <returns>The cross product of the two vectors.</returns>
        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            double x = a.Y * b.Z - a.Z * b.Y;
            double y = a.Z * b.X - a.X * b.Z;
            double z = a.X * b.Y - a.Y * b.X;
            return new Vector3(x, y, z);
        }

        public double Product(Vector3 b) => Product(this, b);

        public Vector3 Cross(Vector3 b) => Cross(this, b);

        public double Magnitude => Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));

        public Vector3 Normalized => this / Magnitude;

        /// <summary>
        /// Get the included angle between this <see cref="Vector3"/> and another <see cref="Vector3"/>.
        /// </summary>
        /// <param name="b">The another vector.</param>
        /// <returns>The included angle in radians.</returns>
        public double GetIncludedAngleWith(Vector3 b)
        {
            double product = Product(b);
            double m1 = Magnitude;
            double m2 = b.Magnitude;

            double c = product / m1 / m2;
            return Math.Acos(c);
        }
    }
}
