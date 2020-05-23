using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Gaia.FullSerializer;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gaia
{
    [Serializable]
    public class SpawnRule
    {
        public string m_name;
        public bool m_useExtendedSpawn = false;
        public float m_minViableFitness = 0.25f;
        public float m_failureRate = 0f;
        public ulong m_maxInstances = 40000000;
        public bool m_ignoreMaxInstances = false;
        public float m_minDirection = 0f;
        public float m_maxDirection = 359.9f;
        public Gaia.GaiaConstants.SpawnerResourceType m_resourceType;
        public int m_resourceIdx;
        [fsIgnore]
        public int m_resourceIdxPhysical;
        [fsIgnore]
        public Transform m_spawnParent = null;
        [fsIgnore]
        public string m_colliderName = "_GaiaCollider_Grass";

        /// <summary>
        /// Type of noise to use
        /// </summary>
        public Gaia.GaiaConstants.NoiseType m_noiseMask = GaiaConstants.NoiseType.None;

        /// <summary>
        /// Seed for noise based fractal
        /// </summary>
        public float m_noiseMaskSeed = 0;

        /// <summary>
        /// The amount of detail in the fractal - more octaves mean more detail and longer calc time.
        /// </summary>
        public int m_noiseMaskOctaves = 8;

        /// <summary>
        /// The roughness of the fractal noise. Controls how quickly amplitudes diminish for successive octaves. 0..1.
        /// </summary>
        public float m_noiseMaskPersistence = 0.25f;

        /// <summary>
        /// The frequency of the first octave
        /// </summary>
        public float m_noiseMaskFrequency = 1f;

        /// <summary>
        /// The frequency multiplier between successive octaves. Experiment between 1.5 - 3.5.
        /// </summary>
        public float m_noiseMaskLacunarity = 1.5f;

        /// <summary>
        /// The zoom level of the noise
        /// </summary>
        public float m_noiseZoom = 50f;

        /// <summary>
        /// A multiplier to boost the noise
        /// </summary>
        public float m_noiseStrength = 1f;

        /// <summary>
        /// Invert the noise output
        /// </summary>
        public bool m_noiseInvert = false;

        /// <summary>
        /// Our noise generator
        /// </summary>
        private Gaia.FractalGenerator m_noiseGenerator;

        public bool m_isActive = true;
        public bool m_isFoldedOut = false;
        public ulong m_currInstanceCnt = 0;
        public ulong m_activeInstanceCnt = 0;
        public ulong m_inactiveInstanceCnt = 0;

        /// <summary>
        /// Initialise the rule
        /// </summary>
        /// <param name="spawner">Spawner the rule belongs to</param>
        public void Initialise(Spawner spawner)
        {
            //Check if active - if not active then dont bother
            if (m_isActive == false)
            {
                return;
            }

            //Check for valid resource index
            if (spawner.m_resources.ResourceIdxOutOfBounds(m_resourceType, m_resourceIdx))
            {
                Debug.Log(string.Format("Warning: {0} - {1} :: Disabling rule {2} idx {3}, index out of bounds", spawner.gameObject.name, m_name, m_resourceType, m_resourceIdx));
                m_isActive = false;
                return;
            }

            //Check to see if it is Unity
            if (!spawner.m_resources.ResourceIsInUnity(m_resourceType, m_resourceIdx))
            {
                Debug.Log(string.Format("Warning: {0} - {1} :: Disabling rule {2} idx {3}, resource missing from unity", spawner.gameObject.name, m_name, m_resourceType, m_resourceIdx));
                m_isActive = false;
                return;
            }

            //Check for and update the physical index
            m_resourceIdxPhysical = spawner.m_resources.PrototypeIdxInTerrain(m_resourceType, m_resourceIdx);
            if (m_resourceIdxPhysical < 0)
            {
                if (Application.isPlaying) //Only deactivate if we are running and its not in the terrain
                {
                    Debug.Log(string.Format("Warning: {0} - {1} :: Disabling rule as resource is physically missing", spawner.gameObject.name, m_name));
                    m_isActive = false;
                    return;
                }
            }

            //Set the noise generator up
            if (m_noiseGenerator == null)
            {
                m_noiseGenerator = new FractalGenerator();
            }
            m_noiseGenerator.Frequency = m_noiseMaskFrequency;
            m_noiseGenerator.Lacunarity = m_noiseMaskLacunarity;
            m_noiseGenerator.Octaves = m_noiseMaskOctaves;
            m_noiseGenerator.Persistence = m_noiseMaskPersistence;
            m_noiseGenerator.Seed = m_noiseMaskSeed;
            switch (m_noiseMask)
            {
                case GaiaConstants.NoiseType.Billow:
                    {
                        m_noiseGenerator.FractalType = FractalGenerator.Fractals.Billow;
                        break;
                    }
                case GaiaConstants.NoiseType.Ridged:
                    {
                        m_noiseGenerator.FractalType = FractalGenerator.Fractals.RidgeMulti;
                        break;
                    }
                default:
                    {
                        m_noiseGenerator.FractalType = FractalGenerator.Fractals.Perlin;
                        break;
                    }
            }

            //Create the game object parent transform
            if (m_resourceType == GaiaConstants.SpawnerResourceType.GameObject)
            {
                if (spawner.m_goParentGameObject == null)
                {
                    Transform tParent = spawner.transform.Find("Spawned_GameObjects");
                    if (tParent == null)
                    {
                        spawner.m_goParentGameObject = new GameObject("Spawned_GameObjects");
                    }
                    else
                    {
                        spawner.m_goParentGameObject = tParent.gameObject;
                    }
                }

                Transform tChild = spawner.m_goParentGameObject.transform.Find(m_name);
                if (tChild == null)
                {
                    GameObject spawnChild = new GameObject(m_name);
                    spawnChild.transform.parent = spawner.m_goParentGameObject.transform;
                    tChild = spawnChild.transform;
                }
                m_spawnParent = tChild;
            }

            //Set up the collider name
            if (m_resourceType == GaiaConstants.SpawnerResourceType.GameObject)
            {
                if (spawner.m_resources.m_gameObjectPrototypes[m_resourceIdx].m_dna.m_extParam.ToLower().Contains("nograss"))
                {
                    m_colliderName = "_GaiaCollider_NoGrass";
                }
                else
                {
                    m_colliderName = "_GaiaCollider_Grass";
                }
            }
            if (m_resourceType == GaiaConstants.SpawnerResourceType.TerrainTree)
            {
                if (spawner.m_resources.m_treePrototypes[m_resourceIdx].m_dna.m_extParam.ToLower().Contains("nograss"))
                {
                    m_colliderName = "_GaiaCollider_NoGrass";
                }
                else
                {
                    m_colliderName = "_GaiaCollider_Grass";
                }
            }

            //Initialise any extensions
            SpawnRuleExtension[] extensions = null;
            switch (m_resourceType)
            {
                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                    {
                        extensions = spawner.m_resources.m_detailPrototypes[m_resourceIdx].m_spawnExtensions;
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainTexture:
                    {
                        extensions = spawner.m_resources.m_texturePrototypes[m_resourceIdx].m_spawnExtensions;
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainTree:
                    {
                        extensions = spawner.m_resources.m_treePrototypes[m_resourceIdx].m_spawnExtensions;
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.GameObject:
                    {
                        extensions = spawner.m_resources.m_gameObjectPrototypes[m_resourceIdx].m_spawnExtensions;
                        break;
                    }
            }

            //Initialise extensions
            if (extensions != null)
            {
                for (int extensionIdx = 0; extensionIdx < extensions.GetLength(0); extensionIdx++)
                {
                    extensions[extensionIdx].Initialise();
                }
            }

        }

        /// <summary>
        /// Chekc to see if the given resource is available in unity
        /// </summary>
        /// <param name="spawner">Spawner we belong to</param>
        /// <returns>True if the resource is available in unity</returns>
        public bool ResourceIsInUnity(Spawner spawner)
        {
            if (!spawner.m_resources.ResourceIsInUnity(m_resourceType, m_resourceIdx))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Chekc to see if the given resource is loaded in the terrain
        /// </summary>
        /// <param name="spawner">Spawner we belong to</param>
        /// <returns>True if the resource is loaded in the terrain</returns>
        public bool ResourceIsLoadedInTerrain(Spawner spawner)
        {
            m_resourceIdxPhysical = spawner.m_resources.PrototypeIdxInTerrain(m_resourceType, m_resourceIdx);
            if (m_resourceIdxPhysical < 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Add the resource assocated to this rule into the terrain
        /// </summary>
        /// <param name="spawner">Spawner we belong to</param>
        public void AddResourceToTerrain(Spawner spawner)
        {
            spawner.m_resources.AddPrototypeToTerrain(m_resourceType, m_resourceIdx);
        }
        
        /// <summary>
        /// Return the value of the fittest object in the spawn criteria
        /// </summary>
        /// <param name="spawner">The spawner we belong to</param>
        /// <param name="location">The location we are checking</param>
        /// <param name="slope"></param>
        /// <returns>Fitness in range 0..1f</returns>
        public float GetFitness(ref SpawnInfo spawnInfo)
        {
            //Check to see if we are active
            if (!m_isActive)
            {
                return 0f;
            }

            //Get the filters and extensions
            SpawnCritera[] filters = null;
            SpawnRuleExtension[] extensions = null;
            switch (m_resourceType)
            {
                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                    {
                        filters = spawnInfo.m_spawner.m_resources.m_detailPrototypes[m_resourceIdx].m_spawnCriteria;
                        extensions = spawnInfo.m_spawner.m_resources.m_detailPrototypes[m_resourceIdx].m_spawnExtensions;
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainTexture:
                    {
                        filters = spawnInfo.m_spawner.m_resources.m_texturePrototypes[m_resourceIdx].m_spawnCriteria;
                        extensions = spawnInfo.m_spawner.m_resources.m_texturePrototypes[m_resourceIdx].m_spawnExtensions;
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainTree:
                    {
                        filters = spawnInfo.m_spawner.m_resources.m_treePrototypes[m_resourceIdx].m_spawnCriteria;
                        extensions = spawnInfo.m_spawner.m_resources.m_treePrototypes[m_resourceIdx].m_spawnExtensions;
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.GameObject:
                    {
                        filters = spawnInfo.m_spawner.m_resources.m_gameObjectPrototypes[m_resourceIdx].m_spawnCriteria;
                        extensions = spawnInfo.m_spawner.m_resources.m_gameObjectPrototypes[m_resourceIdx].m_spawnExtensions;
                        break;
                    }
                    /*
                default:
                    {
                        filters = spawnInfo.m_spawner.m_resources.m_stampPrototypes[m_resourceIdx].m_spawnCriteria;
                        break;
                    }
                     */
            }

            //Drop out if we have no filters
            if (filters == null || filters.Length == 0)
            {
                return 0f;
            }


            //Get the fitness modifier if we are using fractal based fitness
            float noiseMaskModifier = 1f;
            if (m_noiseMask != GaiaConstants.NoiseType.None && m_noiseGenerator != null)
            {
                if (!m_noiseInvert)
                {
                    noiseMaskModifier = Mathf.Clamp01(m_noiseGenerator.GetNormalisedValue(100000f + (spawnInfo.m_hitLocationWU.x * (1f / m_noiseZoom)), 100000f + (spawnInfo.m_hitLocationWU.z * (1f / m_noiseZoom))) * m_noiseStrength);
                }
                else
                {
                    noiseMaskModifier = Mathf.Clamp01(1f - m_noiseGenerator.GetNormalisedValue(100000f + (spawnInfo.m_hitLocationWU.x * (1f / m_noiseZoom)), 100000f + (spawnInfo.m_hitLocationWU.z * (1f / m_noiseZoom))) * m_noiseStrength); 
                }
            }

            //Now calculate fitness
            float maxFitness = float.MinValue;
            int filterIdx, extensionIdx;
            SpawnCritera filter;
            SpawnRuleExtension extension;
            float fitness = 0f;

            for (filterIdx = 0; filterIdx < filters.Length; filterIdx++)
            {
                filter = filters[filterIdx];
                //Check to see of this filter needs a bounds check
                if (filter.m_checkType == GaiaConstants.SpawnerLocationCheckType.BoundedAreaCheck)
                {
                    if (!spawnInfo.m_spawner.CheckLocationBounds(ref spawnInfo, GetMaxScaledRadius(ref spawnInfo)))
                    {
                        return 0f;
                    }
                }
                //Now calculate and process fitness
                fitness = filter.GetFitness(ref spawnInfo) * noiseMaskModifier;

                //Check to see if any extensions are available and if we can influence it
                if (extensions != null)
                {
                    for (extensionIdx = 0; extensionIdx < extensions.GetLength(0); extensionIdx++)
                    {
                        extension = extensions[extensionIdx];
                        if (extension != null)
                        {
                            fitness = extension.GetFitness(fitness, ref spawnInfo);
                        }
                    }
                }

                //Now process it
                if (fitness > maxFitness)
                {
                    maxFitness = fitness;
                    if (maxFitness >= 1f)
                    {
                        return maxFitness;
                    }
                }
            }
            
            if (maxFitness == float.MinValue)
            {
                return 0f;
            }
            else
            {
                return maxFitness;
            }
        }


        /// <summary>
        /// Return the radius of the thing referred to by the rule
        /// </summary>
        /// <param name="spawnInfo">Spawner information</param>
        /// <returns>Radius</returns>
        public float GetRadius(ref SpawnInfo spawnInfo)
        {
            switch (m_resourceType)
            {
                case GaiaConstants.SpawnerResourceType.TerrainTexture:
                    {
                        return 1f; //Makes no sense, so return arbitrary value
                    }
                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                    {
                        return spawnInfo.m_spawner.m_resources.m_detailPrototypes[m_resourceIdx].m_dna.m_boundsRadius;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainTree:
                    {
                        return spawnInfo.m_spawner.m_resources.m_treePrototypes[m_resourceIdx].m_dna.m_boundsRadius;
                    }
                case GaiaConstants.SpawnerResourceType.GameObject:
                    {
                        return spawnInfo.m_spawner.m_resources.m_gameObjectPrototypes[m_resourceIdx].m_dna.m_boundsRadius;
                    }
                    /*
                default:
                    {
                        return spawnInfo.m_spawner.m_resources.m_stampPrototypes[m_resourceIdx].m_dna.m_boundsRadius;
                    }
                     */
            }
            return 0f;
        }

        /// <summary>
        /// Return the maximum scaled radius of the thing referred to by the rule
        /// </summary>
        /// <param name="spawnInfo">Spawner information</param>
        /// <returns>Maximum scaled radius</returns>
        public float GetMaxScaledRadius(ref SpawnInfo spawnInfo)
        {
            switch (m_resourceType)
            {
                case GaiaConstants.SpawnerResourceType.TerrainTexture:
                    {
                        return 1f; //Makes no sense, so return arbitrary value
                    }
                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                    {
                        return spawnInfo.m_spawner.m_resources.m_detailPrototypes[m_resourceIdx].m_dna.m_boundsRadius * spawnInfo.m_spawner.m_resources.m_detailPrototypes[m_resourceIdx].m_dna.m_maxScale;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainTree:
                    {
                        return spawnInfo.m_spawner.m_resources.m_treePrototypes[m_resourceIdx].m_dna.m_boundsRadius * spawnInfo.m_spawner.m_resources.m_treePrototypes[m_resourceIdx].m_dna.m_maxScale;
                    }
                case GaiaConstants.SpawnerResourceType.GameObject:
                    {
                        return spawnInfo.m_spawner.m_resources.m_gameObjectPrototypes[m_resourceIdx].m_dna.m_boundsRadius * spawnInfo.m_spawner.m_resources.m_gameObjectPrototypes[m_resourceIdx].m_dna.m_maxScale;
                    }
                    /*
                default:
                    {
                        return spawnInfo.m_spawner.m_resources.m_stampPrototypes[m_resourceIdx].m_dna.m_boundsRadius * spawnInfo.m_spawner.m_resources.m_stampPrototypes[m_resourceIdx].m_dna.m_maxScale;
                    } */
            }
            return 0f;
        }


        /// <summary>
        /// Return the seed throw range of the thing referred to by the rule
        /// </summary>
        /// <param name="spawnInfo">Spawner information</param>
        /// <returns>Seed throw range</returns>
        public float GetSeedThrowRange(ref SpawnInfo spawnInfo)
        {
            switch (m_resourceType)
            {
                case GaiaConstants.SpawnerResourceType.TerrainTexture:
                    {
                        return 1f; //Makes no sense, so return arbitrayr value
                    }
                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                    {
                        return spawnInfo.m_spawner.m_resources.m_detailPrototypes[m_resourceIdx].m_dna.m_seedThrow;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainTree:
                    {
                        return spawnInfo.m_spawner.m_resources.m_treePrototypes[m_resourceIdx].m_dna.m_seedThrow;
                    }
                case GaiaConstants.SpawnerResourceType.GameObject:
                    {
                        return spawnInfo.m_spawner.m_resources.m_gameObjectPrototypes[m_resourceIdx].m_dna.m_seedThrow;
                    }
                    /*
                default:
                    {
                        return spawnInfo.m_spawner.m_resources.m_stampPrototypes[m_resourceIdx].m_dna.m_seedThrow;
                    }
                     */
            }
            return 0f;
        }

        /// <summary>
        /// Spawn this rule
        /// </summary>
        /// <param name="spawnInfo"></param>
        public void Spawn(ref SpawnInfo spawnInfo)
        {
            //Check to see if we are active
            if (!m_isActive)
            {
                return;
            }

            //Check to see that we dont exceed our own instance count
            if (!m_ignoreMaxInstances && (m_activeInstanceCnt > m_maxInstances))
            {
                return;
            }

            //Update the instance counter
            m_activeInstanceCnt++;

            //Get the extensions
            SpawnRuleExtension extension = null;
            SpawnRuleExtension[] extensions = null;
            switch (m_resourceType)
            {
                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                    {
                        extensions = spawnInfo.m_spawner.m_resources.m_detailPrototypes[m_resourceIdx].m_spawnExtensions;
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainTexture:
                    {
                        extensions = spawnInfo.m_spawner.m_resources.m_texturePrototypes[m_resourceIdx].m_spawnExtensions;
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainTree:
                    {
                        extensions = spawnInfo.m_spawner.m_resources.m_treePrototypes[m_resourceIdx].m_spawnExtensions;
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.GameObject:
                    {
                        extensions = spawnInfo.m_spawner.m_resources.m_gameObjectPrototypes[m_resourceIdx].m_spawnExtensions;
                        break;
                    }
            }

            //Choose a rotation for the spawn
            if (m_resourceType != GaiaConstants.SpawnerResourceType.GameObject)
            {
                spawnInfo.m_spawnRotationY = spawnInfo.m_spawner.GetRandomFloat(0f, 359.9f);
            }
            else
            {
                spawnInfo.m_spawnRotationY = spawnInfo.m_spawner.GetRandomFloat(m_minDirection, m_maxDirection);
            }

            int xtnIdx;
            bool overrideSpawn = false;
            if (extensions != null)
            {
                for (xtnIdx = 0; xtnIdx < extensions.GetLength(0); xtnIdx++)
                {
                    extension = extensions[xtnIdx];
                    if (extension != null)
                    {
                        if (extension.OverridesSpawn(this, ref spawnInfo))
                        {
                            overrideSpawn = true;
                            extension.Spawn(this, ref spawnInfo);
                        }
                    }
                }
            }

            //Now call the normal spawn process
            if (!overrideSpawn)
            {
                //Now spawn it baby!
                switch (m_resourceType)
                {
                    case GaiaConstants.SpawnerResourceType.TerrainTexture:
                        {
                            //Only interested in increasing values
                            if (spawnInfo.m_fitness > spawnInfo.m_textureStrengths[m_resourceIdxPhysical])
                            {
                                float delta = spawnInfo.m_fitness - spawnInfo.m_textureStrengths[m_resourceIdxPhysical];
                                float theRest = 1f - spawnInfo.m_textureStrengths[m_resourceIdxPhysical];
                                float adjustment = 0f;
                                if (theRest != 0f)
                                {
                                    adjustment = 1f - (delta / theRest);
                                }

                                for (int idx = 0; idx < spawnInfo.m_textureStrengths.Length; idx++)
                                {
                                    if (idx == m_resourceIdx)
                                    {
                                        spawnInfo.m_textureStrengths[idx] = spawnInfo.m_fitness;
                                    }
                                    else
                                    {
                                        spawnInfo.m_textureStrengths[idx] *= adjustment;
                                    }
                                }

                                spawnInfo.m_spawner.SetTextureMapsDirty();
                            }
                            break;
                        }
                    case GaiaConstants.SpawnerResourceType.TerrainDetail:
                        {
                            HeightMap detailMap = spawnInfo.m_spawner.GetDetailMap(spawnInfo.m_hitTerrain.GetInstanceID(), m_resourceIdxPhysical);
                            int newStrength = 1;
                            if (spawnInfo.m_spawner.m_resources.m_detailPrototypes[m_resourceIdx].m_dna.m_rndScaleInfluence == true)
                            {
                                newStrength = (int)Mathf.Clamp(15f * spawnInfo.m_spawner.m_resources.m_detailPrototypes[m_resourceIdx].m_dna.GetScale(spawnInfo.m_fitness, spawnInfo.m_spawner.GetRandomFloat(0f, 1f)), 1f, 15f);
                            }
                            else
                            {
                                newStrength = (int)Mathf.Clamp(15f * spawnInfo.m_spawner.m_resources.m_detailPrototypes[m_resourceIdx].m_dna.GetScale(spawnInfo.m_fitness), 1f, 15f);
                            }

                            //Handle non cached scenario
                            if (detailMap == null)
                            {
                                int x = (int)(spawnInfo.m_hitLocationNU.x * spawnInfo.m_hitTerrain.terrainData.detailWidth);
                                int z = (int)(spawnInfo.m_hitLocationNU.z * spawnInfo.m_hitTerrain.terrainData.detailHeight);
                                int[,] detail = spawnInfo.m_hitTerrain.terrainData.GetDetailLayer(x, z, 1, 1, m_resourceIdxPhysical);
                                if (detail[0, 0] < newStrength)
                                {
                                    detail[0, 0] = newStrength;
                                    spawnInfo.m_hitTerrain.terrainData.SetDetailLayer(x, z, m_resourceIdxPhysical, detail);
                                }
                            }
                            else
                            //Handle cached scenario
                            {
                                if (detailMap[spawnInfo.m_hitLocationNU.z, spawnInfo.m_hitLocationNU.x] < newStrength)
                                {
                                    detailMap[spawnInfo.m_hitLocationNU.z, spawnInfo.m_hitLocationNU.x] = newStrength;
                                }
                            }

                            break;
                        }
                    case GaiaConstants.SpawnerResourceType.TerrainTree:
                        {
                            ResourceProtoTree treeProto = spawnInfo.m_spawner.m_resources.m_treePrototypes[m_resourceIdx];
                            TreeInstance t = new TreeInstance();
                            t.prototypeIndex = m_resourceIdxPhysical;
                            t.position = spawnInfo.m_hitLocationNU;

                            if (treeProto.m_dna.m_rndScaleInfluence == true)
                            {
                                t.widthScale = treeProto.m_dna.GetScale(spawnInfo.m_fitness, spawnInfo.m_spawner.GetRandomFloat(0f, 1f));
                            }
                            else
                            {
                                t.widthScale = treeProto.m_dna.GetScale(spawnInfo.m_fitness);
                            }
                            t.heightScale = t.widthScale;
                            t.rotation = spawnInfo.m_spawnRotationY * (Mathf.PI / 180f); //In radians
                            t.color = treeProto.m_healthyColour;
                            t.lightmapColor = Color.white;

                            spawnInfo.m_hitTerrain.AddTreeInstance(t);
                            spawnInfo.m_spawner.m_treeCache.AddTree(spawnInfo.m_hitLocationWU, t.prototypeIndex);

                            float boundsRadius = treeProto.m_dna.m_boundsRadius * t.widthScale;
                            GameObject colliderObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                            colliderObj.name = m_colliderName;
                            colliderObj.transform.position = new Vector3(spawnInfo.m_hitLocationWU.x, spawnInfo.m_hitLocationWU.y + boundsRadius, spawnInfo.m_hitLocationWU.z); 
                            colliderObj.GetComponent<MeshRenderer>().enabled = false;
                            colliderObj.transform.localScale = new Vector3(boundsRadius, boundsRadius, boundsRadius);
                            colliderObj.AddComponent<CapsuleCollider>();
                            colliderObj.layer = spawnInfo.m_spawner.m_spawnColliderLayer;
                            #if UNITY_EDITOR
                            GameObjectUtility.SetStaticEditorFlags(colliderObj, StaticEditorFlags.NavigationStatic);
                            #endif
                            if (spawnInfo.m_spawner.m_areaBoundsColliderCache == null)
                            {
                                spawnInfo.m_spawner.m_areaBoundsColliderCache = new GameObject("Bounds_ColliderCache");
                                spawnInfo.m_spawner.m_areaBoundsColliderCache.transform.parent = spawnInfo.m_spawner.transform;

                            }
                            colliderObj.transform.parent = spawnInfo.m_spawner.m_areaBoundsColliderCache.transform;
                            
                            break;
                        }
                    case GaiaConstants.SpawnerResourceType.GameObject:
                        {
                            ResourceProtoGameObject gameProto = spawnInfo.m_spawner.m_resources.m_gameObjectPrototypes[m_resourceIdx];
                            float scale = 1f;
                            float localScale = 1f;
                            float localDist = 0f;
                            if (gameProto.m_dna.m_rndScaleInfluence == true)
                            {
                                scale = gameProto.m_dna.GetScale(spawnInfo.m_fitness, spawnInfo.m_spawner.GetRandomFloat(0f, 1f));
                            }
                            else
                            {
                                scale = gameProto.m_dna.GetScale(spawnInfo.m_fitness);
                            }
                            int spawnedInstances = 0;
                            float boundsRadius = gameProto.m_dna.m_boundsRadius * scale;
                            Vector3 scaleVect = new Vector3(scale, scale, scale);
                            Vector3 location = spawnInfo.m_hitLocationWU;
                            ResourceProtoGameObjectInstance gpi;
                            SpawnInfo gpSpawnInfo = new SpawnInfo();
                            gpSpawnInfo.m_spawner = spawnInfo.m_spawner;
                            gpSpawnInfo.m_textureStrengths = new float[Terrain.activeTerrain.terrainData.alphamapLayers];
                            for (int idx = 0; idx < gameProto.m_instances.Length; idx++)
                            {
                                gpi = gameProto.m_instances[idx];
                                spawnedInstances = spawnInfo.m_spawner.GetRandomInt(gpi.m_minInstances, gpi.m_maxInstances); //Randomly choose how many instances to spawn
                                for (int inst = 0; inst < spawnedInstances; inst++) //For each instance
                                {
                                    if (spawnInfo.m_spawner.GetRandomFloat(0f, 1f) >= gpi.m_failureRate) //Handle failure override
                                    {
                                        location = spawnInfo.m_hitLocationWU;
                                        location.x += (spawnInfo.m_spawner.GetRandomFloat(gpi.m_minSpawnOffsetX, gpi.m_maxSpawnOffsetX) * scale);
                                        location.z += (spawnInfo.m_spawner.GetRandomFloat(gpi.m_minSpawnOffsetZ, gpi.m_maxSpawnOffsetZ) * scale);
                                        location = Gaia.GaiaUtils.RotatePointAroundPivot(location, spawnInfo.m_hitLocationWU, new Vector3(0f, spawnInfo.m_spawnRotationY, 0f));
                                        location.y += 500f;

                                        //Now learn about this new location
                                        if (spawnInfo.m_spawner.CheckLocation(location, ref gpSpawnInfo))
                                        {
                                            if (!gpi.m_virginTerrain || gpSpawnInfo.m_wasVirginTerrain)
                                            {
                                                GameObject go;

                                                #if UNITY_EDITOR
                                                    go = PrefabUtility.InstantiatePrefab(gpi.m_desktopPrefab) as GameObject;
                                                #else
                                                    go = GameObject.Instantiate(gpi.m_desktopPrefab) as GameObject;
                                                #endif

                                                go.name = "_Sp_" + go.name;

                                                location = gpSpawnInfo.m_hitLocationWU;
                                                location.y = gpSpawnInfo.m_terrainHeightWU;
                                                location.y += (spawnInfo.m_spawner.GetRandomFloat(gpi.m_minSpawnOffsetY, gpi.m_maxSpawnOffsetY) * scale);

                                                go.transform.position = location;

                                                if (gpi.m_useParentScale)
                                                {
                                                    localScale = scale;
                                                    go.transform.localScale = scaleVect;
                                                }
                                                else
                                                {
                                                    localDist = Vector3.Distance(spawnInfo.m_hitLocationWU, gpSpawnInfo.m_hitLocationWU);
                                                    localScale = gpi.m_minScale + (gpi.m_scaleByDistance.Evaluate(localDist / boundsRadius) * spawnInfo.m_spawner.GetRandomFloat(0f, gpi.m_maxScale - gpi.m_minScale));
                                                    go.transform.localScale = new Vector3(localScale, localScale, localScale);
                                                }

                                                go.transform.rotation = Quaternion.Euler(
                                                    new Vector3(
                                                        spawnInfo.m_spawner.GetRandomFloat(gpi.m_minRotationOffsetX, gpi.m_maxRotationOffsetX),
                                                        spawnInfo.m_spawner.GetRandomFloat(gpi.m_minRotationOffsetY + spawnInfo.m_spawnRotationY, gpi.m_maxRotationOffsetY + spawnInfo.m_spawnRotationY),
                                                        spawnInfo.m_spawner.GetRandomFloat(gpi.m_minRotationOffsetZ, gpi.m_maxRotationOffsetZ)));

                                                if (gameProto.m_instances[idx].m_rotateToSlope == true)
                                                {
                                                    go.transform.rotation = Quaternion.FromToRotation(go.transform.up, gpSpawnInfo.m_terrainNormalWU) * go.transform.rotation;
                                                }

                                                //Set the parent
                                                if (m_spawnParent != null)
                                                {
                                                    go.transform.parent = m_spawnParent;
                                                }

                                                //Also add a sphere collider to the cache - this enables bounds radius to be honoured
                                                GameObject localSphereColliderObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);

                                                if (gpi.m_extParam.ToLower().Contains("nograss"))
                                                {
                                                    localSphereColliderObj.name = "_GaiaCollider_NoGrass";
                                                }
                                                else
                                                {
                                                    localSphereColliderObj.name = "_GaiaCollider_Grass";
                                                }
                                                localSphereColliderObj.transform.position = location;
                                                localSphereColliderObj.GetComponent<MeshRenderer>().enabled = false;
                                                float localBoundsRadius = gpi.m_localBounds * localScale;
                                                localSphereColliderObj.transform.localScale = new Vector3(localBoundsRadius, localBoundsRadius, localBoundsRadius);
                                                localSphereColliderObj.AddComponent<SphereCollider>();
                                                localSphereColliderObj.layer = spawnInfo.m_spawner.m_spawnColliderLayer;
                                                #if UNITY_EDITOR
                                                GameObjectUtility.SetStaticEditorFlags(localSphereColliderObj, StaticEditorFlags.NavigationStatic);
                                                #endif
                                                if (spawnInfo.m_spawner.m_areaBoundsColliderCache == null)
                                                {
                                                    spawnInfo.m_spawner.m_areaBoundsColliderCache = new GameObject("Bounds_ColliderCache");
                                                    spawnInfo.m_spawner.m_areaBoundsColliderCache.transform.parent = spawnInfo.m_spawner.transform;

                                                }
                                                localSphereColliderObj.transform.parent = spawnInfo.m_spawner.m_areaBoundsColliderCache.transform;
                                            }
                                        }

                                    }
                                }
                            }

                            //Also add a sphere collider to the cache - this enables bounds radius to be honoured
                            GameObject sphereColliderObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            sphereColliderObj.name = m_colliderName;
                            sphereColliderObj.transform.position = spawnInfo.m_hitLocationWU;
                            sphereColliderObj.GetComponent<MeshRenderer>().enabled = false;
                            sphereColliderObj.transform.localScale = new Vector3(boundsRadius, boundsRadius, boundsRadius);
                            sphereColliderObj.AddComponent<SphereCollider>();
                            sphereColliderObj.layer = spawnInfo.m_spawner.m_spawnColliderLayer;
                            #if UNITY_EDITOR
                            GameObjectUtility.SetStaticEditorFlags(sphereColliderObj, StaticEditorFlags.NavigationStatic);
                            #endif
                            if (spawnInfo.m_spawner.m_areaBoundsColliderCache == null)
                            {
                                spawnInfo.m_spawner.m_areaBoundsColliderCache = new GameObject("Bounds_ColliderCache");
                                spawnInfo.m_spawner.m_areaBoundsColliderCache.transform.parent = spawnInfo.m_spawner.transform;

                            }
                            sphereColliderObj.transform.parent = spawnInfo.m_spawner.m_areaBoundsColliderCache.transform;

                            break;
                        }
                    /*
                case Constants.SpawnerResourceType.Stamp:
                    {
                        //We will stamp into the cached terrain height map
                        break;
                    }
                     */
                }

                if (extensions != null)
                {
                    for (xtnIdx = 0; xtnIdx < extensions.GetLength(0); xtnIdx++)
                    {
                        extension = extensions[xtnIdx];
                        if (extension != null)
                        {
                            extension.PostSpawn(this, ref spawnInfo);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Whether or not this rule needs a heightmap cache
        /// </summary>
        /// <returns>True if the rule needs a heightmap cache</returns>
        public bool CacheHeightMaps(Spawner spawner)
        {
            //Exit if not active
            if (!m_isActive)
            {
                return false;
            }

            /*
            //Always cache if we are a stamp
            if (m_resourceType == Constants.SpawnerResourceType.Stamp)
            {
                return true;
            }
            */

            return false;
        }


        /// <summary>
        /// Whether or not this rule needs a texture cache
        /// </summary>
        /// <returns>True if the rule needs a texture cache</returns>
        public bool CacheTextures(Spawner spawner)
        {
            //Exit if not active
            if (!m_isActive)
            {
                return false;
            }

            //Always cache if we are a texture
            if (m_resourceType == GaiaConstants.SpawnerResourceType.TerrainTexture)
            {
                return true;
            }

            //Cache if this rule refers to a texture
            SpawnRuleExtension extension = null;
            SpawnRuleExtension[] extensions = null;
            switch (m_resourceType)
            {
                case GaiaConstants.SpawnerResourceType.TerrainTexture:
                    {
                        extensions = spawner.m_resources.m_texturePrototypes[m_resourceIdx].m_spawnExtensions;
                        if (extensions != null && extensions.GetLength(0) > 0)
                        {
                            for (int xtnIdx = 0; xtnIdx < extensions.GetLength(0); xtnIdx++)
                            {
                                extension = extensions[xtnIdx];
                                if (extension != null && extension.AffectsTextures())
                                {
                                    return true;
                                }
                            }
                        }
                        return spawner.m_resources.m_texturePrototypes[m_resourceIdx].ChecksTextures();
                    }
                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                    {
                        extensions = spawner.m_resources.m_detailPrototypes[m_resourceIdx].m_spawnExtensions;
                        if (extensions != null && extensions.GetLength(0) > 0)
                        {
                            for (int xtnIdx = 0; xtnIdx < extensions.GetLength(0); xtnIdx++)
                            {
                                extension = extensions[xtnIdx];
                                if (extension != null && extension.AffectsTextures())
                                {
                                    return true;
                                }
                            }
                        }
                        return spawner.m_resources.m_detailPrototypes[m_resourceIdx].ChecksTextures();
                    }
                case GaiaConstants.SpawnerResourceType.TerrainTree:
                    {
                        extensions = spawner.m_resources.m_treePrototypes[m_resourceIdx].m_spawnExtensions;
                        if (extensions != null && extensions.GetLength(0) > 0)
                        {
                            for (int xtnIdx = 0; xtnIdx < extensions.GetLength(0); xtnIdx++)
                            {
                                extension = extensions[xtnIdx];
                                if (extension != null && extension.AffectsTextures())
                                {
                                    return true;
                                }
                            }
                        }
                        return spawner.m_resources.m_treePrototypes[m_resourceIdx].ChecksTextures();
                    }
                case GaiaConstants.SpawnerResourceType.GameObject:
                    {
                        extensions = spawner.m_resources.m_gameObjectPrototypes[m_resourceIdx].m_spawnExtensions;
                        if (extensions != null && extensions.GetLength(0) > 0)
                        {
                            for (int xtnIdx = 0; xtnIdx < extensions.GetLength(0); xtnIdx++)
                            {
                                extension = extensions[xtnIdx];
                                if (extension != null && extension.AffectsTextures())
                                {
                                    return true;
                                }
                            }
                        }
                        return spawner.m_resources.m_gameObjectPrototypes[m_resourceIdx].ChecksTextures();
                    }
                    /*
                case Constants.SpawnerResourceType.Stamp:
                    {
                        return spawner.m_resources.m_stampPrototypes[m_resourceIdx].ChecksTextures();
                    } */
            }

            //Exit nothing found
            return false;
        }

        /// <summary>
        /// Whether or not this rule needs a detail cache
        /// </summary>
        /// <returns>True if the rule needs a detail cache</returns>
        public bool CacheDetails(Spawner spawner)
        {
            //Exit if not active
            if (!m_isActive)
            {
                return false;
            }

            if (m_resourceType == GaiaConstants.SpawnerResourceType.TerrainDetail)
            {
                return true;
            }

            //Cache if this rule refers to a texture
            SpawnRuleExtension extension = null;
            SpawnRuleExtension[] extensions = null;
            switch (m_resourceType)
            {
                case GaiaConstants.SpawnerResourceType.TerrainTexture:
                    {
                        extensions = spawner.m_resources.m_texturePrototypes[m_resourceIdx].m_spawnExtensions;
                        if (extensions != null && extensions.GetLength(0) > 0)
                        {
                            for (int xtnIdx = 0; xtnIdx < extensions.GetLength(0); xtnIdx++)
                            {
                                extension = extensions[xtnIdx];
                                if (extension != null && extension.AffectsDetails())
                                {
                                    return true;
                                }
                            }
                        }
                        return spawner.m_resources.m_texturePrototypes[m_resourceIdx].ChecksTextures();
                    }
                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                    {
                        extensions = spawner.m_resources.m_detailPrototypes[m_resourceIdx].m_spawnExtensions;
                        if (extensions != null && extensions.GetLength(0) > 0)
                        {
                            for (int xtnIdx = 0; xtnIdx < extensions.GetLength(0); xtnIdx++)
                            {
                                extension = extensions[xtnIdx];
                                if (extension != null && extension.AffectsDetails())
                                {
                                    return true;
                                }
                            }
                        }
                        return spawner.m_resources.m_detailPrototypes[m_resourceIdx].ChecksTextures();
                    }
                case GaiaConstants.SpawnerResourceType.TerrainTree:
                    {
                        extensions = spawner.m_resources.m_treePrototypes[m_resourceIdx].m_spawnExtensions;
                        if (extensions != null && extensions.GetLength(0) > 0)
                        {
                            for (int xtnIdx = 0; xtnIdx < extensions.GetLength(0); xtnIdx++)
                            {
                                extension = extensions[xtnIdx];
                                if (extension != null && extension.AffectsDetails())
                                {
                                    return true;
                                }
                            }
                        }
                        return spawner.m_resources.m_treePrototypes[m_resourceIdx].ChecksTextures();
                    }
                case GaiaConstants.SpawnerResourceType.GameObject:
                    {
                        extensions = spawner.m_resources.m_gameObjectPrototypes[m_resourceIdx].m_spawnExtensions;
                        if (extensions != null && extensions.GetLength(0) > 0)
                        {
                            for (int xtnIdx = 0; xtnIdx < extensions.GetLength(0); xtnIdx++)
                            {
                                extension = extensions[xtnIdx];
                                if (extension != null && extension.AffectsDetails())
                                {
                                    return true;
                                }
                            }
                        }
                        return spawner.m_resources.m_gameObjectPrototypes[m_resourceIdx].ChecksTextures();
                    }
                /*
            case Constants.SpawnerResourceType.Stamp:
                {
                    return spawner.m_resources.m_stampPrototypes[m_resourceIdx].ChecksTextures();
                } */
            }

            return false;
        }

        /// <summary>
        /// Whether or not this rule needs a proximity tag cache
        /// </summary>
        /// <returns>True if the rule needs a proximity tag cache</returns>
        public bool CacheProximity(Spawner spawner)
        {
            //Exit if not active
            if (!m_isActive)
            {
                return false;
            }

            //Cache if this rule refers to a texture
            switch (m_resourceType)
            {
                case GaiaConstants.SpawnerResourceType.TerrainTexture:
                    {
                        return spawner.m_resources.m_texturePrototypes[m_resourceIdx].ChecksProximity();
                    }
                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                    {
                        return spawner.m_resources.m_detailPrototypes[m_resourceIdx].ChecksProximity();
                    }
                case GaiaConstants.SpawnerResourceType.TerrainTree:
                    {
                        return spawner.m_resources.m_treePrototypes[m_resourceIdx].ChecksProximity();
                    }
                case GaiaConstants.SpawnerResourceType.GameObject:
                    {
                        return spawner.m_resources.m_gameObjectPrototypes[m_resourceIdx].ChecksProximity();
                    }
                    /*
                case Constants.SpawnerResourceType.Stamp:
                    {
                        return spawner.m_resources.m_stampPrototypes[m_resourceIdx].ChecksProximity();
                    }
                     */
            }

            //Exit nothing found
            return false;
        }

        /// <summary>
        /// Add proximity tags to the tag list if they are not already on there
        /// </summary>
        /// <param name="spawner">Spawner that has the resources</param>
        /// <param name="tagList">Proximity tag list being added to</param>
        /// <returns></returns>
        public void AddProximityTags(Spawner spawner, ref List<string> tagList)
        {
            //Exit if not active
            if (!m_isActive)
            {
                return;
            }

            //Add tags
            switch (m_resourceType)
            {
                case GaiaConstants.SpawnerResourceType.TerrainTexture:
                    {
                        spawner.m_resources.m_texturePrototypes[m_resourceIdx].AddTags(ref tagList);
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                    {
                        spawner.m_resources.m_detailPrototypes[m_resourceIdx].AddTags(ref tagList);
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainTree:
                    {
                        spawner.m_resources.m_treePrototypes[m_resourceIdx].AddTags(ref tagList);
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.GameObject:
                    {
                        spawner.m_resources.m_gameObjectPrototypes[m_resourceIdx].AddTags(ref tagList);
                        break;
                    }
                    /*
                case Constants.SpawnerResourceType.Stamp:
                    {
                        spawner.m_resources.m_stampPrototypes[m_resourceIdx].AddTags(ref tagList);
                        break;
                    } */
            }
        }

        /*
        /// <summary>
        /// Add stamp paths to the stamp list if they are not already on there
        /// </summary>
        /// <param name="spawner">Spawner that has the resources</param>
        /// <param name="stampList">Stamp list being added to</param>
        /// <returns></returns>
        public void AddStamps(Spawner spawner, ref List<string> stampList)
        {
            //Exit if not active
            if (!m_isActive)
            {
                return;
            }

            //Add stamps
            if (m_resourceType == Constants.SpawnerResourceType.Stamp)
            {
                spawner.m_resources.m_stampPrototypes[m_resourceIdx].AddStamps(ref stampList);
            }
        }
        */
    }
}