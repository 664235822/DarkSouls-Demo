using System;
using UnityEngine;

namespace Gaia
{
    /// <summary>
    /// Used to control how the terrain world proptotypes work
    /// </summary>

    public struct ProtoDNAInst
    {
        public ResourceProtoDNA dna;
        public Vector3 locationWU;
        public Vector3 locationTU;
        public int age;
    }

    [System.Serializable]
    public class ResourceProtoDNA
    {
        [Tooltip("Width in world units.")]
        public float m_width = 1f;          //Width in world units
        [Tooltip("Height in world units.")]
        public float m_height = 1f;         //Height in world units
        [Tooltip("Radius from centre of object in world units for bounded area checks. Make this larger if you want more free space around your object when it is spawned.")]
        public float m_boundsRadius = 1f;   //The radius of this object - used to help calculate free area
        [Tooltip("The maximum distance a seed can be thrown when a new instance is spawned. Used to control spread area random clustered spawning.")]
        public float m_seedThrow = 12;     //The distance a seed can be thrown
        [Tooltip("The minimum scale that this can be rendered into the world. For terrain details this is the minimum strength that detail will render at.")]
        public float m_minScale = 0.5f;     //The minimum size this can grow as a % scale of original model
        [Tooltip("The maximum scale that this can be rendered into the world. For terrain details this is the maximum strength that detail will render at.")]
        public float m_maxScale = 1.5f;     //The maximum size this can grow as a % scale of original model
        [Tooltip("Randomise the scale somewhere between minimum and maximum scale. If not selected then the scale will be proportionally influenced by the locations fitness.")]
        public bool m_rndScaleInfluence = false; //Allow the scale to be randomly influenced, otherwise pure fitness based
        [Tooltip("Custom parameter to be interpreted by an extension if there is one. Use 'nograss' to exclude grass being grown within the volumne covered by the area bounds.")]
        public string m_extParam = "";

        [HideInInspector]
        public int m_protoIdx = 0;          //The prototype we are describing - internal use only - do not touch

        /// <summary>
        /// Update the prototype index
        /// </summary>
        /// <param name="protoIdx"></param>
        public void Update(int protoIdx)
        {
            m_protoIdx = protoIdx;
        }

        /// <summary>
        /// Update based on what we know
        /// </summary>
        /// <param name="protoIdx">The prototype we refer to</param>
        /// <param name="width">The width in world units</param>
        /// <param name="height">The height in world units</param>
        public void Update(int protoIdx, float width, float height)
        {
            m_protoIdx = protoIdx;
            m_width = width;
            m_height = height;
            m_boundsRadius = m_width;
            m_seedThrow = Mathf.Max(m_width, m_height) * 1.5f;
        }

        /// <summary>
        /// Update based on what we know
        /// </summary>
        /// <param name="protoIdx">The prototype we refer to</param>
        /// <param name="width">The width in world units</param>
        /// <param name="height">The height in world units</param>
        /// <param name="minscale">The minimim scale we will draw at</param>
        /// <param name="maxscale">The maximum scale we will draw at</param>
        public void Update(int protoIdx, float width, float height, float minscale, float maxscale)
        {
            m_protoIdx = protoIdx;
            m_width = width;
            m_height = height;
            m_boundsRadius = m_width;
            m_seedThrow = Mathf.Max(m_width, m_height) * 3;
            m_minScale = minscale;
            m_maxScale = maxscale;
        }

        /// <summary>
        /// Return a scale between min and max that accounts for the fitness of the object
        /// </summary>
        /// <param name="fitness">Fitness - range of 0..1</param>
        /// <returns>Scale</returns>
        public float GetScale(float fitness)
        {
            return m_minScale + ((m_maxScale - m_minScale) * fitness);
        }

        /// <summary>
        /// Return a scale between min and max that accounts for the fitness of the object, plus a random override
        /// </summary>
        /// <param name="fitness">Fitness - range of 0..1</param>
        /// <param name="random">Random value - range of 0..1</param>
        /// <returns>Scale</returns>
        public float GetScale(float fitness, float random)
        {
            return m_minScale + ((m_maxScale - m_minScale) * fitness * random);
        }
    }
}