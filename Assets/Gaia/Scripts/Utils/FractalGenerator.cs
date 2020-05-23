using UnityEngine;
using System.Collections;

namespace Gaia
{

    /// <summary>
    /// Fractal generator
    /// </summary>
    public class FractalGenerator
    {
        /// <summary>
        /// The types of fractals we support
        /// </summary>
        public enum Fractals { Perlin, Billow, RidgeMulti };

        /// <summary>
        /// Seed - different seeds generate replicable different fractals
        /// </summary>
        private float m_seed = 0;
        public float Seed
        {
            get { return m_seed; }
            set { m_seed = value; }
        }

        /// <summary>
        /// The amount of detail in the fractal - more octaves mean more detail and longer calc time.
        /// </summary>
        private int m_octaves = 8;
        public int Octaves
        {
            get { return m_octaves; }
            set { m_octaves = value; }
        }

        /// <summary>
        /// The roughness of the fractal noise. Controls how quickly amplitudes diminish for successive octaves. 0..1.
        /// </summary>
        private float m_persistence = 0.65f;
        public float Persistence
        {
            get { return m_persistence; }
            set { m_persistence = value; }
        }

        /// <summary>
        /// The frequency of the first octave
        /// </summary>
        private float m_frequency = 1f;
        public float Frequency
        {
            get { return m_frequency; }
            set { m_frequency = value; }
        }

        /// <summary>
        /// The frequency multiplier between successive octaves. Experiment between 1.5 - 3.5.
        /// </summary>
        private float m_lacunarity = 1.5f;
        public float Lacunarity
        {
            get { return m_lacunarity; }
            set { m_lacunarity = value; }
        }

        /// <summary>
        /// Offset X by a given value, allows local fractal area to be explored.
        /// </summary>
        private float m_XOffset = 0f;
        public float XOffset
        {
            get { return m_XOffset; }
            set { m_XOffset = value; }
        }

        /// <summary>
        /// Offset height by a given value. Decease to get islands, increease to get plateaus.
        /// </summary>
        private float m_ZOffset = 0f;
        public float ZOffset
        {
            get { return m_ZOffset; }
            set { m_ZOffset = value; }
        }

        /// <summary>
        /// Offset Y by a given value, allows local fractal area to be explored.
        /// </summary>
        private float m_YOffset = 0f;
        public float YOffset
        {
            get { return m_YOffset; }
            set { m_YOffset = value; }
        }

        /// <summary>
        /// The type of fractal we are generating
        /// </summary>
        private Fractals m_fractalType = Fractals.Perlin;
        public Fractals FractalType
        {
            get { return m_fractalType; }
            set
            {
                m_fractalType = value;
                switch (m_fractalType)
                {
                    case Fractals.Perlin:
                        {
                            m_noiseCalculator = GetValue_Perlin;
                            break;
                        }
                    case Fractals.Billow:
                        {
                            m_noiseCalculator = GetValue_Billow;
                            break;
                        }
                    case Fractals.RidgeMulti:
                        {
                            CalcSpectralWeights();
                            m_noiseCalculator = GetValue_RidgedMulti;
                            break;
                        }
                }
            }
        }

        private float[] m_spectralWeights = new float[20];      //Used by ridged fractal
        private delegate float GetCalcValue(float x, float z);  //Switch in the relevant fractal algorithm
        private GetCalcValue m_noiseCalculator;                 //Switch in the relevant fractal algorithm

        /// <summary>
        /// Constructor
        /// </summary>
        public FractalGenerator()
        {
            FractalType = Fractals.Perlin;
        }

        /// <summary>
        /// Construct and initialise a fractal generator
        /// </summary>
        /// <param name="frequency">The frequency of the first octave</param>
        /// <param name="lacunarity">The frequency multiplier between successive octaves. Experiment between 1.5 - 3.5.</param>
        /// <param name="octaves">The amount of detail in the fractal - more octaves mean more detail and longer calc time.</param>
        /// <param name="persistance">The roughness of the fractal noise. Controls how quickly amplitudes diminish for successive octaves. 0..1.</param>
        /// <param name="seed">Seed - different seeds generate replicable different fractals</param>
        /// <param name="type">The type of generator being used</param>
        public FractalGenerator(float frequency, float lacunarity, int octaves, float persistance, float seed, Fractals type)
        {
            m_frequency = frequency;
            m_lacunarity = lacunarity;
            m_octaves = octaves;
            m_persistence = persistance;
            m_seed = seed;
            switch (type)
            {
                case Fractals.Perlin:
                    m_noiseCalculator = GetValue_Perlin;
                    break;
                case Fractals.Billow:
                    m_noiseCalculator = GetValue_Billow;
                    break;
                case Fractals.RidgeMulti:
                    CalcSpectralWeights();
                    m_noiseCalculator = GetValue_RidgedMulti;
                    break;
                default:
                    m_noiseCalculator = GetValue_Perlin;
                    break;
            }
        }

        /// <summary>
        /// Set some reasonable defaults for the fractal type
        /// </summary>
        public void SetDefaults()
        {
            switch (m_fractalType)
            {
                case Fractals.Perlin:
                    {
                        m_frequency = 1f;
                        m_lacunarity = 2f;
                        m_octaves = 6;
                        m_persistence = 0.5f;
                        m_seed = 0;
                        m_noiseCalculator = GetValue_Perlin;
                        break;
                    }
                case Fractals.Billow:
                    {
                        m_frequency = 1f;
                        m_lacunarity = 2f;
                        m_octaves = 6;
                        m_persistence = 0.5f;
                        m_seed = 0;
                        m_noiseCalculator = GetValue_Billow;
                        break;
                    }
                case Fractals.RidgeMulti:
                    {
                        m_frequency = 1f;
                        m_lacunarity = 2f;
                        m_octaves = 6;
                        m_seed = 0;
                        CalcSpectralWeights();
                        m_noiseCalculator = GetValue_RidgedMulti;
                        break;
                    }
            }
        }

        /// <summary>
        /// Get the noise value at this point
        /// </summary>
        /// <param name="x">x location</param>
        /// <param name="z">z location</param>
        /// <returns></returns>
        public float GetValue(float x, float z)
        {
            return m_noiseCalculator(x, z);
        }

        /// <summary>
        /// Get the noise value at this point
        /// </summary>
        /// <param name="x">x location</param>
        /// <param name="z">z location</param>
        /// <returns>Value at this point</returns>
        public double GetValue(double x, double z)
        {
            return GetValue((float)x, (float)z);
        }

        /// <summary>
        /// Get the noise value at this point
        /// </summary>
        /// <param name="x">x location</param>
        /// <param name="z">z location</param>
        /// <returns>Value in range 0..1 at this point</returns>
        public float GetNormalisedValue(float x, float z)
        {
            return Mathf.Clamp01((GetValue(x, z) + 1f) / 2f);
        }

        /// <summary>
        /// Get the noise value at this point
        /// </summary>
        /// <param name="x">x location</param>
        /// <param name="z">z location</param>
        /// <returns>Value in range 0..1 at this point</returns>
        public double GetNormalisedValue(double x, double z)
        {
            return GetNormalisedValue((float)x, (float)z);
        }

        /// <summary>
        /// Calculate a perlin fractal
        /// </summary>
        /// <param name="x">x location</param>
        /// <param name="z">z location</param>
        /// <returns></returns>
        public float GetValue_Perlin(float x, float z)
        {
            float value = 0f;
            float signal = 0f;
            float persistence = 1f;
            float nx, nz;

            x += m_seed;
            z += m_seed;
            x += m_XOffset;
            z += m_ZOffset;
            x *= m_frequency;
            z *= m_frequency;

            for (int octave = 0; octave < m_octaves; octave++)
            {
                nx = x;
                nz = z;
                signal = SimplexNoiseGenerator.Generate(nx, nz);
                value += signal * persistence;
                x *= m_lacunarity;
                z *= m_lacunarity;
                persistence *= m_persistence;
            }

            value += m_YOffset * 2.4f;

            return value;
        }

        /// <summary>
        /// Calculate a billow fractal
        /// </summary>
        /// <param name="x">x location</param>
        /// <param name="z">z location</param>
        /// <returns></returns>
        public float GetValue_Billow(float x, float z)
        {
            float value = 0f;
            float signal = 0f;
            float persistence = 1f;
            float nx, nz;

            x += m_seed;
            z += m_seed;

            x += m_XOffset;
            z += m_ZOffset;
            x *= m_frequency;
            z *= m_frequency;

            for (int octave = 0; octave < m_octaves; octave++)
            {
                nx = x;
                nz = z;
                signal = SimplexNoiseGenerator.Generate(nx, nz);
                signal = 2f * Mathf.Abs(signal) - 1f;
                value += signal * persistence;
                x *= m_lacunarity;
                z *= m_lacunarity;
                persistence *= m_persistence;
            }

            value += m_YOffset * 2.4f;

            return value;
        }

        /// <summary>
        /// Calculate a ridged multi fractal
        /// </summary>
        /// <param name="x">x location</param>
        /// <param name="z">z location</param>
        /// <returns></returns>
        public float GetValue_RidgedMulti(float x, float z)
        {
            float signal = 0.0f;
            float value = 0.0f;
            float weight = 1.0f;
            float offset = 1f;
            float gain = m_persistence;
            float nx, nz;

            x += m_seed;
            z += m_seed;
            x += m_XOffset;
            z += m_ZOffset;
            x *= m_frequency;
            z *= m_frequency;

            for (int octave = 0; octave < m_octaves; octave++)
            {
                nx = x;
                nz = z;

                //Get the coherent-noise value from input value and add it to final result
                signal = SimplexNoiseGenerator.Generate(nx, nz);

                //Make the ridges
                signal = Mathf.Abs(signal);
                signal = offset - signal;

                //Square signal to increase sharpness of ridges.
                signal *= signal;

                //The weighting from previous octave is applied to the signal.
                signal *= weight;

                //Weight successive contributions by previous signal.
                weight = signal * gain;
                if (weight > 1.0)
                {
                    weight = 1.0f;
                }
                if (weight < 0.0f)
                {
                    weight = 0.0f;
                }

                //Add the signal to output value.
                value += signal * m_spectralWeights[octave];

                //Next Octave
                x *= m_lacunarity;
                z *= m_lacunarity;
            }

            value = (value * 1.25f) - 1.0f;

            value += m_YOffset;

            return value;
        }

        /// <summary>
        /// Calculate spectral weights for the ridged fractal
        /// </summary>
        private void CalcSpectralWeights()
        {
            float h = 1.0f;
            float frequency = 1.0f;
            int maxSpectra = m_spectralWeights.GetLength(0);
            for (int i = 0; i < maxSpectra; i++)
            {
                m_spectralWeights[i] = Mathf.Pow(frequency, -h);
                frequency *= m_lacunarity;
            }
        }

    }
}