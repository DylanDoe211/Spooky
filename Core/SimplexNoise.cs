using Terraria;
using Terraria.ID;
using Terraria.WorldBuilding;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Spooky.Core
{
    //Credits to OpenSimplex2: https://github.com/KdotJPG/OpenSimplex2
    public class SimplexNoise
    {
        private const long PRIME_X = 0x5205402B9270C86FL;
        private const long PRIME_Y = 0x598CD327003817B5L;
        private const long HASH_MULTIPLIER = 0x53A3F72DEEC546F5L;

        private const double SKEW = 0.366025403784439;
        private const double UNSKEW = -0.21132486540518713;

        private const int N_GRADS_EXPONENT = 7;
        private const int N_GRADS = 1 << N_GRADS_EXPONENT;

        private const double NORMALIZER = 0.01001634121365712;
        private const float RSQUARED = 0.5f;
        
        /*
        * Custom fractal method
        * Constant Y : Keeping Y constant (in this case 0) theorically makes the 2D noise be used as 1D noise.
        * Octaves : Low octaves gives a smooth, blurry noise. High octaves gives sharper, organic noise.
        * Frequency / baseScale : Low frequency creates large, wide and smooth structures. High frequency creates thinner, rapidly changing structures.
        * The other values are pretty standard, shouldn't need to change them.
        */
        public static float FractalNoise(long seed, float x, float y, int octaves = 8, float baseScale = 1f, float lacunarity = 2f, float persistence = 0.5f)
        {
            float totalNoise = 0f;
            float frequency = baseScale;
            float amplitude = 1f;
            float maxPossibleAmplitude = 0f; //Used for normalizing the result

            for (int i = 0; i < octaves; i++)
            {
                //Coordinates scaled by current octave's frequency
                float sampleX = x * frequency;
                float sampleY = y * frequency;

                //Fetch standard noise
                float noiseValue = (float)Noise(seed, sampleX, sampleY);

                //Accumulate weighted noise
                totalNoise += noiseValue * amplitude;
                maxPossibleAmplitude += amplitude;

                //Progress parameters for the next octave
                amplitude *= persistence;
                frequency *= lacunarity;
            }

            //Normalize result back to a standard range
            return totalNoise / maxPossibleAmplitude;
        }

        //Standard 2D simplex noise
        public static float Noise(long seed, double x, double y = 0.0)
        {
            //Get points for A2* lattice
            double s = SKEW * (x + y);
            double xs = x + s, ys = y + s;

            return Noise_UnskewedBase(seed, xs, ys);
        }

        private static float Noise_UnskewedBase(long seed, double xs, double ys)
        {
            //Get base points and offsets.
            int xsb = FastFloor(xs), ysb = FastFloor(ys);
            float xi = (float)(xs - xsb), yi = (float)(ys - ysb);

            // Prime pre-multiplication for hash.
            long xsbp = xsb * PRIME_X, ysbp = ysb * PRIME_Y;

            //Unskew.
            float t = (xi + yi) * (float)UNSKEW;
            float dx0 = xi + t, dy0 = yi + t;

            //First vertex.
            float value = 0;
            float a0 = RSQUARED - dx0 * dx0 - dy0 * dy0;
            if (a0 > 0)
            {
                value = a0 * a0 * (a0 * a0) * Grad(seed, xsbp, ysbp, dx0, dy0);
            }

            //Second vertex.
            float a1 = (float)(2 * (1 + 2 * UNSKEW) * (1 / UNSKEW + 2)) * t + ((float)(-2 * (1 + 2 * UNSKEW) * (1 + 2 * UNSKEW)) + a0);
            if (a1 > 0)
            {
                float dx1 = dx0 - (float)(1 + 2 * UNSKEW);
                float dy1 = dy0 - (float)(1 + 2 * UNSKEW);
                value += a1 * a1 * (a1 * a1) * Grad(seed, xsbp + PRIME_X, ysbp + PRIME_Y, dx1, dy1);
            }

            //Third vertex.
            if (dy0 > dx0)
            {
                float dx2 = dx0 - (float)UNSKEW;
                float dy2 = dy0 - (float)(UNSKEW + 1);
                float a2 = RSQUARED - dx2 * dx2 - dy2 * dy2;
                if (a2 > 0)
                {
                    value += a2 * a2 * (a2 * a2) * Grad(seed, xsbp, ysbp + PRIME_Y, dx2, dy2);
                }
            }
            else
            {
                float dx2 = dx0 - (float)(UNSKEW + 1);
                float dy2 = dy0 - (float)UNSKEW;
                float a2 = RSQUARED - dx2 * dx2 - dy2 * dy2;
                if (a2 > 0)
                {
                    value += a2 * a2 * (a2 * a2) * Grad(seed, xsbp + PRIME_X, ysbp, dx2, dy2);
                }
            }

            return value;
        }

        private static int FastFloor(double x)
        {
            int xi = (int)x;
            return x < xi ? xi - 1 : xi;
        }

        private static float Grad(long seed, long xsvp, long ysvp, float dx, float dy)
        {
            long hash = seed ^ xsvp ^ ysvp;
            hash *= HASH_MULTIPLIER;
            hash ^= hash >> (64 - N_GRADS_EXPONENT + 1);
            int gi = (int)hash & ((N_GRADS - 1) << 1);
            return GRADIENTS[gi | 0] * dx + GRADIENTS[gi | 1] * dy;
        }

        private static readonly float[] GRADIENTS;
        static SimplexNoise()
        {
            GRADIENTS = new float[N_GRADS * 2];
            float[] grad2 = [
                0.38268343236509f,   0.923879532511287f,
                0.923879532511287f,  0.38268343236509f,
                0.923879532511287f, -0.38268343236509f,
                0.38268343236509f,  -0.923879532511287f,
                -0.38268343236509f,  -0.923879532511287f,
                -0.923879532511287f, -0.38268343236509f,
                -0.923879532511287f,  0.38268343236509f,
                -0.38268343236509f,   0.923879532511287f,
                //-------------------------------------//
                0.130526192220052f,  0.99144486137381f,
                0.608761429008721f,  0.793353340291235f,
                0.793353340291235f,  0.608761429008721f,
                0.99144486137381f,   0.130526192220051f,
                0.99144486137381f,  -0.130526192220051f,
                0.793353340291235f, -0.60876142900872f,
                0.608761429008721f, -0.793353340291235f,
                0.130526192220052f, -0.99144486137381f,
                -0.130526192220052f, -0.99144486137381f,
                -0.608761429008721f, -0.793353340291235f,
                -0.793353340291235f, -0.608761429008721f,
                -0.99144486137381f,  -0.130526192220052f,
                -0.99144486137381f,   0.130526192220051f,
                -0.793353340291235f,  0.608761429008721f,
                -0.608761429008721f,  0.793353340291235f,
                -0.130526192220052f,  0.99144486137381f,
            ];
            for (int i = 0; i < grad2.Length; i++)
            {
                grad2[i] = (float)(grad2[i] / NORMALIZER);
            }
            for (int i = 0, j = 0; i < GRADIENTS.Length; i++, j++)
            {
                if (j == grad2.Length) j = 0;
                GRADIENTS[i] = grad2[j];
            }
        }
    }
}