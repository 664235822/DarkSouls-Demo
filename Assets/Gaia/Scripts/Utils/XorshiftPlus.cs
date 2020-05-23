using UnityEngine;
using System.Collections;

namespace Gaia
{
    /// <summary>
    /// Simple and fast random number generator which can reset to a specific iteration
    /// </summary>
    public class XorshiftPlus
    {
        //Initialiser magic values
        private const ulong m_A_Init = 181353;
        private const ulong m_B_Init = 7;

        //Seed
        public int m_seed;

        //State
        public ulong m_stateA, m_stateB;

        /// <summary>
        /// Contruct and initialise the RNG
        /// </summary>
        /// <param name="seed"></param>
        public XorshiftPlus(int seed = 1)
	    {
            m_seed = seed;
            if (m_seed == 0)
            {
                m_seed = 1;
            }
            Reset();
	    }

	    /// <summary>
	    /// Reset it to its initial state with the existing seed
	    /// </summary>
        public void Reset()
        {
            m_stateA = m_A_Init * (uint)m_seed;
            m_stateB = m_B_Init * (uint)m_seed;
        }

        /// <summary>
        /// Restet it to a new state with a new seed
        /// </summary>
        /// <param name="seed">New seed</param>
        public void Reset(int seed)
        {
            m_seed = seed;
            if (m_seed == 0)
            {
                m_seed = 1;
            }
            Reset();
        }

        /// <summary>
        /// Reset it to the stade defined by the state variables passed in
        /// </summary>
        public void Reset(ulong stateA, ulong stateB)
        {
            Debug.Log("Resetting RNG State " + stateA + " " + stateB);
            m_stateA = stateA;
            m_stateB = stateB;
        }

        /// <summary>
        /// Return the current state for serialisation
        /// </summary>
        /// <param name="seed">Seed</param>
        /// <param name="stateA">State A</param>
        /// <param name="stateB">State B</param>
        public void GetState(out int seed, out ulong stateA, out ulong stateB)
        {
            seed = m_seed;
            stateA = m_stateA;
            stateB = m_stateB;
        }

        //Check here for wrapper functions
        //https://github.com/tucano/UnityRandom/blob/master/lib/MersenneTwister.cs

        /// <summary>
        /// Get the next value
        /// </summary>
        /// <returns>A value between zero and one inclusive</returns>
	    public float Next()
	    {
		    ulong x = m_stateA;
		    ulong y = m_stateB;
		    m_stateA = y;
		    x ^= x << 23;
		    x ^= x >> 17;
		    x ^= y ^ (y >> 26);
		    m_stateB = x;
            return (float)(x + y) / (float)ulong.MaxValue;
	    }

        /// <summary>
        /// Return the next int
        /// </summary>
        /// <returns></returns>
        public int NextInt()
        {
            return (int)(Next() * int.MaxValue);
        }

        /// <summary>
        /// Get the next value and scale it between the min and max values supplied inclusive
        /// </summary>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns>Next value scaled beteen the range supplied</returns>
        public float Next(float min, float max)
        {
            //float xx = min + (Next() * (max - min));
            //Debug.Log(string.Format("{0:0.0000}", xx));
            //return xx;

            return min + (Next() * (max - min));
        }

        /// <summary>
        /// Get the next value and scale it between the min and max values supplied inclusive
        /// </summary>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns>Next value scaled beteen the range supplied</returns>
        public int Next(int min, int max)
        {
            if (min == max)
            {
                return min;
            }
            return (int)Next((float)min, (float)max+0.999f);
        }

        /// <summary>
        /// Get the next value as a vector
        /// </summary>
        /// <returns>Next value as a vector in ranges 0..1</returns>
        public Vector3 NextVector()
        {
            return new Vector3(Next(), Next(), Next());
        }

        /// <summary>
        /// Get the next value as a vector
        /// </summary>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns></returns>
        public Vector3 NextVector(float min, float max)
        {
            return new Vector3(Next(min, max), Next(min, max), Next(min, max));
        }
    }
}
