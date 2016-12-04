using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ZPG_Server
{
    class Noise
    {
        public class PerlinNoise
        {
            private const int GradientSizeTable = 256;
            private readonly Random _random;
            private readonly double[] _gradients = new double[GradientSizeTable * 3];
            /* Borrowed from Darwyn Peachey (see references above).
               The gradient table is indexed with an XYZ triplet, which is first turned
               into a single random index using a lookup in this table. The table simply
               contains all numbers in [0..255] in random order. */
            private byte[] _perm = new byte[256];

            private byte[] RandomPerm()
            {
                byte[] _perm = new byte[256];
                for (int i = 0; i < 256; i++)
                    _perm[i] = (byte)i;
                byte Z;
                for (int i = 0; i < 256; i++)
                    for (int j = 0; j < 256; j++)
                    {
                        if (i == j) j++;
                        Z = _perm[i];
                        _perm[i] = _perm[j];
                        _perm[j] = Z;
                    }
                        return _perm;
            }

            public PerlinNoise(int seed)
            {
                _random = new Random(seed);
                InitGradients();
            }

            public double Noise(double x, double y, double z)
            {
                /* The main noise function. Looks up the pseudorandom gradients at the nearest
                   lattice points, dots them with the input vector, and interpolates the
                   results to produce a single output value in [0, 1] range. */

                int ix = (int)Math.Floor(x);
                double fx0 = x - ix;
                double fx1 = fx0 - 1;
                double wx = Smooth(fx0);

                int iy = (int)Math.Floor(y);
                double fy0 = y - iy;
                double fy1 = fy0 - 1;
                double wy = Smooth(fy0);

                int iz = (int)Math.Floor(z);
                double fz0 = z - iz;
                double fz1 = fz0 - 1;
                double wz = Smooth(fz0);

                double vx0 = Lattice(ix, iy, iz, fx0, fy0, fz0);
                double vx1 = Lattice(ix + 1, iy, iz, fx1, fy0, fz0);
                double vy0 = Lerp(wx, vx0, vx1);

                vx0 = Lattice(ix, iy + 1, iz, fx0, fy1, fz0);
                vx1 = Lattice(ix + 1, iy + 1, iz, fx1, fy1, fz0);
                double vy1 = Lerp(wx, vx0, vx1);

                double vz0 = Lerp(wy, vy0, vy1);

                vx0 = Lattice(ix, iy, iz + 1, fx0, fy0, fz1);
                vx1 = Lattice(ix + 1, iy, iz + 1, fx1, fy0, fz1);
                vy0 = Lerp(wx, vx0, vx1);

                vx0 = Lattice(ix, iy + 1, iz + 1, fx0, fy1, fz1);
                vx1 = Lattice(ix + 1, iy + 1, iz + 1, fx1, fy1, fz1);
                vy1 = Lerp(wx, vx0, vx1);

                double vz1 = Lerp(wy, vy0, vy1);
                return Lerp(wz, vz0, vz1);
            }

            private void InitGradients()
            {
                for (int i = 0; i < GradientSizeTable; i++)
                {
                    double z = 1f - 2f * _random.NextDouble();
                    double r = Math.Sqrt(1f - z * z);
                    double theta = 2 * Math.PI * _random.NextDouble();
                    _gradients[i * 3] = r * Math.Cos(theta);
                    _gradients[i * 3 + 1] = r * Math.Sin(theta);
                    _gradients[i * 3 + 2] = z;
                }
            }

            private int Permutate(int x)
            {
                const int mask = GradientSizeTable - 1;
                return _perm[x & mask];
            }

            private int Index(int ix, int iy, int iz)
            {
                // Turn an XYZ triplet into a single gradient table index.
                return Permutate(ix + Permutate(iy + Permutate(iz)));
            }

            private double Lattice(int ix, int iy, int iz, double fx, double fy, double fz)
            {
                // Look up a random gradient at [ix,iy,iz] and dot it with the [fx,fy,fz] vector.
                int index = Index(ix, iy, iz);
                int g = index * 3;
                return _gradients[g] * fx + _gradients[g + 1] * fy + _gradients[g + 2] * fz;
            }

            private double Lerp(double t, double value0, double value1)
            {
                // Simple linear interpolation.
                return value0 + t * (value1 - value0);
            }

            private double Smooth(double x)
            {
                /* Smoothing curve. This is used to calculate interpolants so that the noise
                  doesn't look blocky when the frequency is low. */
                return x * x * (3 - 2 * x);
            }
        }
    }
}
