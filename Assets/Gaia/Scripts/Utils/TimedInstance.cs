using UnityEngine;
using System.Collections;

namespace Gaia
{

    public class TimedInstance : System.Diagnostics.Stopwatch
    {
        //Timer frequency - will depend on systems its on
        public long nanosecPerTick = (1000L*1000L*1000L) / Frequency;

        //Name of the instance
        public string m_name;

        //Number of iterations
        public int m_iterations = 0;

        public TimedInstance(string name, bool start = true)
            : base()
        {
            m_name = name;
            if (start)
            {
                Start();
            }
        }

        new public void Start()
        {
            m_iterations++;
            base.Start();
        }

        new public void Reset()
        {
            m_iterations = 0;
            base.Reset();
        }

        public void IncIterations()
        {
            m_iterations++;
        }

        public float GetAvgMs() 
        {
            return ((float)ElapsedMilliseconds / (float)m_iterations);
        }

        public float GetAvgS()
        {
            return (((float)ElapsedMilliseconds / (float)m_iterations)) / 1000f;
        }

        public override string ToString()
        {
            // the overall frame time and frames per second:
            return string.Format("{0}: Avg: {1:0.000}s Last: {1:0.000}s", m_name, GetAvgMs() / 1000f, ElapsedMilliseconds / 1000f);
        }
    }
}