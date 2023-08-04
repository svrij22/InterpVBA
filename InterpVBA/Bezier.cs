using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterpVBA
{
    internal class BezierInterpolation
    {
        public static double Bezier(List<double> KnownXs, List<double> KnownYs, double X, int Extrapolate = 0)
        {

            ///
            /// Error trapping
            ///


            int nR = KnownXs.Count;

            // Check if KnownXs and KnownYs have the same count
            if (nR != KnownYs.Count)
            {
                throw new ArgumentException("KnownXs and KnownYs must have the same number of elements.");
            }
            else if (nR < 4)
            {
                throw new ArgumentException("At least 4 values are required for a Bezier curve.");
            }

            // Check if X values are monotonically increasing
            bool bMono = true;
            for (int j = 0; j < nR - 1; j++)
            {
                if (KnownXs[j] > KnownXs[j + 1])
                {
                    bMono = false;
                    break;
                }
            }

            if (!bMono)
            {
                throw new ArgumentException("X values must be monotonically increasing.");
            }

            // Return Y value if X value already exists
            for (int j = 0; j < nR; j++)
            {
                if (X == KnownXs[j])
                {
                    return KnownYs[j];
                }
            }

            ///
            /// Construction 1
            ///

            // First, find which segment the data point is in.
            int S, Segment;

            S = KnownXs.BinarySearch(X);
            if (S < 0) S = ~S - 1; // If X is not found, find the nearest index

            if (S >= KnownYs.Count - 3)
            {
                Segment = 3;
            }
            else if (S < 1)
            {
                Segment = 1;
            }
            else
            {
                Segment = 2;
            }

            // Assign the value to interpolate to the relevant control points.
            double Ax, Bx, Cx, Dx, Ay, By, Cy, Dy;
            Ax = 0;
            Bx = 0;
            Cx = 0;
            Dx = 0;
            Ay = 0;
            By = 0;
            Cy = 0;
            Dy = 0;

            switch (Segment)
            {
                case 1:
                    // This is the first segment
                    Ax = KnownXs[S];
                    Bx = KnownXs[S + 1];
                    Cx = KnownXs[S + 2];
                    Dx = KnownXs[S + 3];
                    Ay = KnownYs[S];
                    By = KnownYs[S + 1];
                    Cy = KnownYs[S + 2];
                    Dy = KnownYs[S + 3];
                    break;

                case 2:
                    // This is a middle segment
                    Ax = KnownXs[S - 1];
                    Bx = KnownXs[S];
                    Cx = KnownXs[S + 1];
                    Dx = KnownXs[S + 2];
                    Ay = KnownYs[S - 1];
                    By = KnownYs[S];
                    Cy = KnownYs[S + 1];
                    Dy = KnownYs[S + 2];
                    break;

                case 3:
                    // This is the last segment
                    Ax = KnownXs[S - 2];
                    Bx = KnownXs[S - 1];
                    Cx = KnownXs[S];
                    Dx = KnownXs[S + 1];
                    Ay = KnownYs[S - 2];
                    By = KnownYs[S - 1];
                    Cy = KnownYs[S];
                    Dy = KnownYs[S + 1];
                    break;
            }

            // Debug info
            Console.WriteLine($"Ax: {Ax}, Bx: {Bx}, Cx: {Cx}, Dx: {Dx}, Ay: {Ay}, By: {By}, Cy: {Cy}, Dy: {Dy}");

            ///
            /// 'Create the distance vectors between the control points.
            ///

            double Zero1 = Math.Sqrt(Math.Pow(Ax - Bx, 2) + Math.Pow(Ay - By, 2));
            double One2 = Math.Sqrt(Math.Pow(Bx - Cx, 2) + Math.Pow(By - Cy, 2));
            double Two3 = Math.Sqrt(Math.Pow(Cx - Dx, 2) + Math.Pow(Cy - Dy, 2));
            double Zero2 = Math.Sqrt(Math.Pow(Ax - Cx, 2) + Math.Pow(Ay - Cy, 2));
            double One3 = Math.Sqrt(Math.Pow(Bx - Dx, 2) + Math.Pow(By - Dy, 2));

            ///
            /// 'Then compute the control points.
            ///

            double P1ABx = Ax + (Bx - Ax) / 6.0;
            double P2ABx = Bx + (Ax - Cx) / 6.0;
            double P1ABy = Ay + (By - Ay) / 6.0;
            double P2ABy = By + (Ay - Cy) / 6.0;
            double P1CDx = Cx + (Dx - Bx) / 6.0;
            double P2CDx = Dx + (Cx - Dx) / 6.0;
            double P1CDy = Cy + (Dy - By) / 6.0;
            double P2CDy = Dy + (Cy - Dy) / 6.0;

            // Debug info
            Console.WriteLine($"P1ABx: {P1ABx}, P2ABx: {P2ABx}, P1ABy: {P1ABy}, P2ABy: {P2ABy}, P1CDx: {P1CDx}, P2CDx: {P2CDx}, P1CDy: {P1CDy}, P2CDy: {P2CDy}");

            ///
            /// 'Adjust the distance between the control points.
            ///

            double P1BCx, P2BCx, P1BCy, P2BCy;

            if ((Zero2 / 6.0 < One2 / 2.0) && (One3 / 6.0 < One2 / 2.0))
            {
                P1BCx = Bx + (Cx - Ax) / 6.0;
                P2BCx = Cx + (Bx - Dx) / 6.0;
                P1BCy = By + (Cy - Ay) / 6.0;
                P2BCy = Cy + (By - Dy) / 6.0;
            }
            else if ((Zero2 / 6.0 >= One2 / 2.0) && (One3 / 6.0 >= One2 / 2.0))
            {
                P1BCx = Bx + (Cx - Ax) * One2 / 2.0 / Zero2;
                P2BCx = Cx + (Bx - Dx) * One2 / 2.0 / One3;
                P1BCy = By + (Cy - Ay) * One2 / 2.0 / Zero2;
                P2BCy = Cy + (By - Dy) * One2 / 2.0 / One3;
            }
            else if (Zero2 / 6.0 >= One2 / 2.0)
            {
                P1BCx = Bx + (Cx - Ax) * One2 / 2.0 / Zero2;
                P2BCx = Cx + (Bx - Dx) * One2 / 2.0 / One3 * (One3 / Zero2);
                P1BCy = By + (Cy - Ay) * One2 / 2.0 / Zero2;
                P2BCy = Cy + (By - Dy) * One2 / 2.0 / One3 * (One3 / Zero2);
            }
            else
            {
                P1BCx = Bx + (Cx - Ax) * One2 / 2.0 / Zero2 * (One2 / One3);
                P2BCx = Cx + (Bx - Dx) * One2 / 2.0 / One3;
                P1BCy = By + (Cy - Ay) * One2 / 2.0 / Zero2 * (One2 / One3);
                P2BCy = Cy + (By - Dy) * One2 / 2.0 / One3;
            }

            // Debug info
            Console.WriteLine($"P1ABx: {P1ABx}, P2ABx: {P2ABx}, P1BCx: {P1BCx}, P2BCx: {P2BCx}, P1CDx: {P1CDx}, P2CDx: {P2CDx}");
            Console.WriteLine($"P1ABy: {P1ABy}, P2ABy: {P2ABy}, P1BCy: {P1BCy}, P2BCy: {P2BCy}, P1CDy: {P1CDy}, P2CDy: {P2CDy}");


            ///
            /// 'Declare an array with the parameter t.
            /// 

            double[] t = { 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1 };


            ///
            /// 'Loop through t and compute the F'x(t) and G'y(t) parametric curves by adding to the array.
            ///

            List<double> ABFx = new List<double>();
            List<double> ABGy = new List<double>();
            List<double> BCFx = new List<double>();
            List<double> BCGy = new List<double>();
            List<double> CDFx = new List<double>();
            List<double> CDGy = new List<double>();

            bool bFound = false;
            int P = 0;

            for (int n = 0; n < t.Length; n++)
            {
                // Construct the parametric Bezier curves F'x(t) and G'y(t) with the Bernstein polynomials.
                // These are for the first segment.
                ABFx.Add(Ax * Math.Pow(1 - t[n], 3) + P1ABx * 3 * t[n] * Math.Pow(1 - t[n], 2) + P2ABx * 3 * Math.Pow(t[n], 2) * (1 - t[n]) + Bx * Math.Pow(t[n], 3));
                ABGy.Add(Ay * Math.Pow(1 - t[n], 3) + P1ABy * 3 * t[n] * Math.Pow(1 - t[n], 2) + P2ABy * 3 * Math.Pow(t[n], 2) * (1 - t[n]) + By * Math.Pow(t[n], 3));

                // These are for middle segments.
                BCFx.Add(Bx * Math.Pow(1 - t[n], 3) + P1BCx * 3 * t[n] * Math.Pow(1 - t[n], 2) + P2BCx * 3 * Math.Pow(t[n], 2) * (1 - t[n]) + Cx * Math.Pow(t[n], 3));
                BCGy.Add(By * Math.Pow(1 - t[n], 3) + P1BCy * 3 * t[n] * Math.Pow(1 - t[n], 2) + P2BCy * 3 * Math.Pow(t[n], 2) * (1 - t[n]) + Cy * Math.Pow(t[n], 3));

                // These are for the last segment.
                CDFx.Add(Cx * Math.Pow(1 - t[n], 3) + P1CDx * 3 * t[n] * Math.Pow(1 - t[n], 2) + P2CDx * 3 * Math.Pow(t[n], 2) * (1 - t[n]) + Dx * Math.Pow(t[n], 3));
                CDGy.Add(Cy * Math.Pow(1 - t[n], 3) + P1CDy * 3 * t[n] * Math.Pow(1 - t[n], 2) + P2CDy * 3 * Math.Pow(t[n], 2) * (1 - t[n]) + Dy * Math.Pow(t[n], 3));

                // Find the closest points on the Bezier curve to interpolate from.
                if (!bFound)
                {
                    switch (Segment)
                    {
                        case 1:
                            if (ABFx[n] > X)
                            {
                                bFound = true;
                                P = n;
                            }
                            break;
                        case 2:
                            if (BCFx[n] > X)
                            {
                                bFound = true;
                                P = n;
                            }
                            break;
                        case 3:
                            if (CDFx[n] > X)
                            {
                                bFound = true;
                                P = n;
                            }
                            break;
                    }
                }
            }

            ///
            /// 'INTERPOLATION
            ///

            double lin = 0;

            // Assuming that Segment and P are previously defined, and X is the value you're working with
            switch (Segment)
            {
                case 1:
                    // This is the first segment.
                    lin = ABGy[P - 1] +
                        (ABGy[P] - ABGy[P - 1]) /
                        (ABFx[P] - ABFx[P - 1]) *
                        (X - ABFx[P - 1]);
                    break;

                case 2:
                    // This is a middle segment.
                    lin = BCGy[P - 1] +
                        (BCGy[P] - BCGy[P - 1]) /
                        (BCFx[P] - BCFx[P - 1]) *
                        (X - BCFx[P - 1]);
                    break;

                case 3:
                    // This is the last segment.
                    lin = CDGy[P - 1] +
                        (CDGy[P] - CDGy[P - 1]) /
                        (CDFx[P] - CDFx[P - 1]) *
                        (X - CDFx[P - 1]);
                    break;
            }

            // This is the result.
            double Bezier = lin;

            // Return the result
            return Bezier;
        }
    }
}
