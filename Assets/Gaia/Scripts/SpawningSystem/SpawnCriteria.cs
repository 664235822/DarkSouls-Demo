using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Gaia
{
    [Serializable]
    public class SpawnCritera
    {
        [Tooltip("Criteria name")]
        public string m_name;

        [Tooltip("CHECK TYPE - a single point on the terrain or area based. The size of the area checks is based on the Bounds Radius in the DNA. Area based checks are good for larger structures but substantially slower so use with care.")]
        public Gaia.GaiaConstants.SpawnerLocationCheckType m_checkType = GaiaConstants.SpawnerLocationCheckType.PointCheck;

        [Tooltip("When selected, the criteria will only be valid if the terrain was clear of any other objects at this location. A location is determined to be ‘virgin’ when raycast collision test hits clear terrain. To detect other objects at this location they must have colliders. You can use an invisible collider and this test to stop any resources that require virgin terrain from spawning at that location.")]
        public bool m_virginTerrain = true;

        [Tooltip("Whether or not this location will be checked for height.")]
        public bool m_checkHeight = true;
        [Tooltip("The minimum valid height relative to sea level. Only tested when Check Height is checked.")]
        public float m_minHeight = 0f;
        [Tooltip("The maximum valid height relative to sea level. Only tested when Check Height is checked.")]
        public float m_maxHeight = 1000f;
        [Tooltip("The fitness curve - evaluated between the minimum and the maximum height when Check Height is checked. 0 is low fitness and unlikely to spawn, 1 is high fitness and likely to spawn.")]
        public AnimationCurve m_heightFitness = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0.0f));

        [Tooltip("Whether or not this location will be checked for slope.")]
        public bool m_checkSlope = true;
        [Tooltip("The minimum valid slope. Only tested when Check Slope is checked.")]
        public float m_minSlope = 0f;
        [Tooltip("The maximum valid slope. Only tested when Check Slope is checked.")]
        public float m_maxSlope = 90;
        [Tooltip("The fitness curve - evaluated between the minimum and the maximum slope when Check Slope is checked. 0 is low fitness and unlikely to spawn, 1 is high fitness and likely to spawn.")]
        public AnimationCurve m_slopeFitness = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0.0f));

        [Tooltip("Whether or not to check proximity to other tags near this location.")]
        public bool m_checkProximity = false;
        [Tooltip("The tag that will be checked in proximity to this location e.g. House")]
        public string m_proximityTag = "";
        [Tooltip("The minimum valid proximity. Only tested when Check Proximity is checked.")]
        public float m_minProximity = 0f;
        [Tooltip("The maximum valid proximity. Only tested when Check Proximity is checked.")]
        public float m_maxProximity = 100f;
        [Tooltip("The fitness curve - evaluated between the minimum and the maximum proximity when Check Proximity is checked. 0 is low fitness and unlikely to spawn, 1 is high fitness and likely to spawn.")]
        public AnimationCurve m_proximityFitness = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.3f, 0.2f), new Keyframe(1f, 0.0f));

        [Tooltip("Check textures at this location.")]
        public bool m_checkTexture = false;
        [Tooltip("Texture slots from your terrain (first valid slot is 0). Will select for presence that texture. Use exclamation mark in front of slot to select for absence of that texture. For example 3 selects for presence of texture 3, !3 checks for absence of texture 3. Fitness is based on the strength of the texture at that location in range 0..1. Only tested when Check Texture is checked.")]
        public string m_matchingTextures = "!3";

        [Tooltip("Whether or not this spawn criteria is active. It will be ignored if not active.")]
        public bool m_isActive = true;

        /// <summary>
        /// One off initialisation thing to speed processing
        /// </summary>
        private bool m_isInitialised = false;

        /// <summary>
        /// Initialise the criteria
        /// </summary>
        public void Initialise(Spawner spawner)
        {

            //Check the values in the texture fitness
            string newTextureFilter = "";
            int maxTexture = 4;
            char maxTextureChar = maxTexture.ToString()[0];
            if (Terrain.activeTerrain != null)
            {
                maxTexture = Terrain.activeTerrain.terrainData.alphamapLayers;
                maxTextureChar = maxTexture.ToString()[0];
            }
            for (int idx = 0; idx < m_matchingTextures.Length; idx++)
            {
                if ((m_matchingTextures[idx] >= '0' && m_matchingTextures[idx] <= maxTextureChar) || m_matchingTextures[idx] == '!')
                {
                    newTextureFilter += m_matchingTextures[idx];
                }
            }
            m_matchingTextures = newTextureFilter;

            m_isInitialised = true;
        }


        /// <summary>
        /// Get the fitness of the supplied slope
        /// </summary>
        /// <param name="slope">The slope to be checked</param>
        /// <returns>Fitness at that slope - if fitness not applicable it returns 1f</returns>
        public float GetSlopeFitness(float slope)
        {
            float fitness = 1f;
            if (m_checkSlope)
            {
                if (slope < m_minSlope || slope > m_maxSlope)
                {
                    return 0f;
                }
                fitness = m_maxSlope - m_minSlope;
                if (fitness > 0f)
                {
                    fitness = (slope - m_minSlope) / fitness;
                    fitness = m_slopeFitness.Evaluate(fitness);
                }
                else
                {
                    fitness = 0f;
                }
            }
            return fitness;
        }

        /// <summary>
        /// Get the fitness of the supplied height
        /// </summary>
        /// <param name="height">The height to be checked</param>
        /// <returns>Fitness at that height - if fitness not applicable it returns 1f</returns>
        public float GetHeightFitness(float height, float sealLevel)
        {
            float fitness = 1f;
            if (m_checkHeight)
            {
                height -= sealLevel;
                if (height < m_minHeight || height > m_maxHeight)
                {
                    return 0f;
                }
                fitness = m_maxHeight - m_minHeight;
                if (fitness > 0f)
                {
                    fitness = (height - m_minHeight) / fitness;
                    fitness = m_heightFitness.Evaluate(fitness);
                }
                else
                {
                    fitness = 0f;
                }
            }
            return fitness;
        }

        /// <summary>
        /// Get the fitness of the supplied proximity
        /// </summary>
        /// <param name="proximity">The proximity to be checked</param>
        /// <returns>Fitness at that proximity - if fitness not applicable it returns 1f</returns>
        public float GetProximityFitness(float proximity)
        {
            float fitness = 1f;
            if (m_checkProximity)
            {
                if (proximity < m_minProximity || proximity > m_maxProximity)
                {
                    return 0f;
                }
                fitness = m_maxProximity - m_minProximity;
                if (fitness > 0f)
                {
                    fitness = (proximity - m_minProximity) / fitness;
                    fitness = m_proximityFitness.Evaluate(fitness);
                }
                else
                {
                    fitness = 0f;
                }
            }
            return fitness;
        }

        /// <summary>
        /// Get the fitness of the supplied textures
        /// </summary>
        /// <param name="textures">The textures to be checked</param>
        /// <returns>Fitness of these textures - if fitness not applicable it returns 1f</returns>
        public float GetTextureFitness(float[] textures)
        {
            bool negate = false;
            float fitness = 1f;
            float textureFitness;
            if (m_checkTexture)
            {
                fitness = float.MaxValue;
                for (int checkIdx = 0; checkIdx < m_matchingTextures.Length; checkIdx++)
                {
                    if (m_matchingTextures[checkIdx] == '!')
                    {
                        negate = true;
                    }
                    else
                    {
                        if (negate)
                        {
                            textureFitness = 1f - textures[(int)Char.GetNumericValue(m_matchingTextures[checkIdx])];
                            negate = false;
                            if (fitness == float.MaxValue || (textureFitness < fitness))
                            {
                                fitness = textureFitness;
                            }
                        }
                        else
                        {
                            textureFitness = textures[(int)Char.GetNumericValue(m_matchingTextures[checkIdx])];
                            if (fitness == float.MaxValue || (textureFitness > fitness))
                            {
                                fitness = textureFitness;
                            }
                        }
                    }
                }
            }
            return fitness;
        }

        /// <summary>
        /// Return fitness of the spawninfo object - not ideal as it forces closer coupling
        /// </summary>
        /// <param name="spawnInfo">Spawn infor object</param>
        /// <returns>Fitness at that location 0..1</returns>
        public float GetFitness(ref SpawnInfo spawnInfo)
        {
            //Make sure we are initialised
            if (!m_isInitialised)
            {
                Initialise(spawnInfo.m_spawner);
            }

            //Check for active or not
            if (!m_isActive)
            {
                return 0f;
            }

            //Check for virgin (clear terrain)
            if (m_virginTerrain == true)
            {
                if (spawnInfo.m_wasVirginTerrain != true)
                {
                    return 0f;
                }
            }

            //Set default fitness
            float fitness = 1f;

            //Check height
            if (m_checkHeight)
            {
                fitness = Mathf.Min(fitness, GetHeightFitness(spawnInfo.m_terrainHeightWU, spawnInfo.m_spawner.m_resources.m_seaLevel));
            }

            //Check slope
            if (m_checkSlope && fitness > 0f)
            {
                if (m_checkType == GaiaConstants.SpawnerLocationCheckType.PointCheck)
                {
                    fitness = Mathf.Min(fitness, GetSlopeFitness(spawnInfo.m_terrainSlopeWU));
                }
                else
                {
                    //fitness = Mathf.Min(fitness, GetSlopeFitness(spawnInfo.m_areaHitSlopeWU));
                    fitness = Mathf.Min(fitness, GetSlopeFitness(spawnInfo.m_areaAvgSlopeWU));
                }
            }

            //Check textures
            if (m_checkTexture && fitness > 0f)
            {
                fitness = Mathf.Min(fitness, GetTextureFitness(spawnInfo.m_textureStrengths));
            }

            //Check proximity
            if (m_checkProximity && fitness > 0f)
            {
                Rect area = new Rect(spawnInfo.m_hitLocationWU.x - m_maxProximity, spawnInfo.m_hitLocationWU.z - m_maxProximity, m_maxProximity * 2f, m_maxProximity * 2f);
                GameObject closestObject = spawnInfo.m_spawner.GetClosestObject(m_proximityTag, area);
                if (closestObject != null)
                {
                    fitness = Mathf.Min(fitness, GetProximityFitness(Vector3.Distance(closestObject.transform.position, spawnInfo.m_hitLocationWU)));
                }
                else
                {
                    fitness = 0f;
                }
            }

            //Exit with final fitness
            return fitness;
        }
    }
}