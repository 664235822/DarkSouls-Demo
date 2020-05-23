using UnityEngine;
using System.Collections;
using System;

namespace Gaia
{
    public class ResourceVisualiser : MonoBehaviour {

        [Tooltip("Choose the resources - these are the resources that will be managed.")]
        public GaiaResource m_resources;

        [Tooltip("Visualiser range - controls how far the visualiser extends. Make smaller on lower powered computers.")]
        public float m_range = 200f;

        [Tooltip("Visualiser resolution. Make larger on lower powered computers."), Range(3f, 50f)]
        public float m_resolution = 25f;

        [Tooltip("Minimum fitness - points with fitness less than this value will not be shown."), Range(0f, 1f)]
        public float m_minimumFitness = 0f;

        [Tooltip("Controls which layers are checked for collisions. Must at least include the layer the terrain is on. Add additional layers if other collisions need to be detected as well. Influences terrain detection, tree detection and game object detection.")]
        public LayerMask m_fitnessCollisionLayers;

        [Tooltip("Colour of high fitness locations.")]
        public Color m_fitColour = Color.green;

        [Tooltip("Colour of low fitness locations.")]
        public Color m_unfitColour = Color.red;

        [HideInInspector]
        public Spawner m_spawner;

        [HideInInspector]
        public Vector3 m_lastHitPoint;

        [HideInInspector]
        public string m_lastHitObjectname;

        [HideInInspector]
        public float m_lastHitFitness;

        [HideInInspector]
        public float m_lastHitHeight;

        [HideInInspector]
        public float m_lastHitTerrainHeight;

        [HideInInspector]
        public float m_lastHitTerrainRelativeHeight;

        [HideInInspector]
        public float m_lastHitTerrainSlope;

        [HideInInspector]
        public float m_lastHitAreaSlope;

        [HideInInspector]
        public bool m_lastHitWasVirgin = true;

        [HideInInspector]
        public Gaia.GaiaConstants.SpawnerResourceType m_selectedResourceType;   //The resource type we are showing

        [HideInInspector]
        public int m_selectedResourceIdx;                                       //The actual resouce we are showing

        #pragma warning disable 414
        [HideInInspector]
        private DateTime m_lastUpdateDate = DateTime.Now;
        #pragma warning restore 414

        [HideInInspector]
        private DateTime m_lastCacheUpdateDate = DateTime.Now;

        /// <summary>
        /// Used to visualise resource fitness
        /// </summary>
        private UnityHeightMap m_terrainHeightMap;

        /// <summary>
        /// Knock ourselves out if we happen to be left there in play mode
        /// </summary>
        void Awake()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Std unity enable
        /// </summary>
        void OnEnable()
        {
            Initialise();
        }

        /// <summary>
        /// Initialise as much as we can
        /// </summary>
        public void Initialise()
        {
            m_fitnessCollisionLayers = Gaia.TerrainHelper.GetActiveTerrainLayer();
            m_spawner = GetComponent<Spawner>();
            if (m_spawner == null)
            {
                m_spawner = gameObject.AddComponent<Spawner>();
                m_spawner.m_spawnCollisionLayers = m_fitnessCollisionLayers;
                m_spawner.hideFlags = HideFlags.HideInInspector;
                m_spawner.m_resources = m_resources;
                m_spawner.m_spawnRange = m_range;
                m_spawner.m_showGizmos = false;
                m_spawner.Initialise();
            }
            else
            {
                m_spawner.m_spawnCollisionLayers = m_fitnessCollisionLayers;
                m_spawner.Initialise();
            }
        }

        /// <summary>
        /// Pick up latest terrain information
        /// </summary>
        public void Visualise()
        {
            m_terrainHeightMap = new UnityHeightMap(Gaia.TerrainHelper.GetActiveTerrain());
        }

        public SpawnInfo GetSpawnInfo(Vector3 location)
        {
            SpawnInfo spawnInfo = new SpawnInfo();
            spawnInfo.m_textureStrengths = new float[Terrain.activeTerrain.terrainData.alphamapLayers];
            if (m_spawner.CheckLocation(location, ref spawnInfo))
            {
                spawnInfo.m_fitness = GetFitness(ref spawnInfo);
            }
            else
            {
                spawnInfo.m_fitness = 0f;
            }
            return spawnInfo;
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
            //Get the filters
            SpawnCritera[] filters;
            switch (m_selectedResourceType)
            {
                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                    {
                        filters = spawnInfo.m_spawner.m_resources.m_detailPrototypes[m_selectedResourceIdx].m_spawnCriteria;
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainTexture:
                    {
                        filters = spawnInfo.m_spawner.m_resources.m_texturePrototypes[m_selectedResourceIdx].m_spawnCriteria;
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainTree:
                    {
                        filters = spawnInfo.m_spawner.m_resources.m_treePrototypes[m_selectedResourceIdx].m_spawnCriteria;
                        break;
                    }
                default:
                    {
                        filters = spawnInfo.m_spawner.m_resources.m_gameObjectPrototypes[m_selectedResourceIdx].m_spawnCriteria;
                        break;
                    }
            }

            //Drop out if we have no filters
            if (filters == null || filters.Length == 0)
            {
                return 0f;
            }

            //Now calculate fitness
            float maxFitness = float.MinValue;
            int filterIdx;
            SpawnCritera filter;
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
                fitness = filter.GetFitness(ref spawnInfo);
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
        /// Return the value of the least fittest object in the spawn criteria
        /// </summary>
        /// <param name="spawner">The spawner we belong to</param>
        /// <param name="location">The location we are checking</param>
        /// <param name="slope"></param>
        /// <returns>Fitness in range 0..1f</returns>
        public float GetMinFitness(ref SpawnInfo spawnInfo)
        {
            //Get the filters
            SpawnCritera[] filters;
            switch (m_selectedResourceType)
            {
                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                    {
                        if (m_selectedResourceIdx >= spawnInfo.m_spawner.m_resources.m_detailPrototypes.Length)
                        {
                            return 0f;
                        }
                        filters = spawnInfo.m_spawner.m_resources.m_detailPrototypes[m_selectedResourceIdx].m_spawnCriteria;
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainTexture:
                    {
                        if (m_selectedResourceIdx >= spawnInfo.m_spawner.m_resources.m_texturePrototypes.Length)
                        {
                            return 0f;
                        }
                        filters = spawnInfo.m_spawner.m_resources.m_texturePrototypes[m_selectedResourceIdx].m_spawnCriteria;
                        break;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainTree:
                    {
                        if (m_selectedResourceIdx >= spawnInfo.m_spawner.m_resources.m_treePrototypes.Length)
                        {
                            return 0f;
                        }
                        filters = spawnInfo.m_spawner.m_resources.m_treePrototypes[m_selectedResourceIdx].m_spawnCriteria;
                        break;
                    }
                default:
                    {
                        if (m_selectedResourceIdx >= spawnInfo.m_spawner.m_resources.m_gameObjectPrototypes.Length)
                        {
                            return 0f;
                        }
                        filters = spawnInfo.m_spawner.m_resources.m_gameObjectPrototypes[m_selectedResourceIdx].m_spawnCriteria;
                        break;
                    }
            }

            //Drop out if we have no filters
            if (filters == null || filters.Length == 0)
            {
                return 0f;
            }

            //Now calculate fitness
            float minFitness = float.MaxValue;
            int filterIdx;
            SpawnCritera filter;
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
                fitness = filter.GetFitness(ref spawnInfo);
                if (fitness < minFitness)
                {
                    minFitness = fitness;
                    if (minFitness <= 0f)
                    {
                        return minFitness;
                    }
                }
            }

            if (minFitness == float.MaxValue)
            {
                return 0f;
            }
            else
            {
                return minFitness;
            }
        }


        /// <summary>
        /// Return the maximum scaled radius of the thing referred to by the rule
        /// </summary>
        /// <param name="spawnInfo">Spawner information</param>
        /// <returns>Maximum scaled radius</returns>
        public float GetMaxScaledRadius(ref SpawnInfo spawnInfo)
        {
            switch (m_selectedResourceType)
            {
                case GaiaConstants.SpawnerResourceType.TerrainTexture:
                    {
                        return 1f; //Makes no sense, so return arbitrary value
                    }
                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                    {
                        return spawnInfo.m_spawner.m_resources.m_detailPrototypes[m_selectedResourceIdx].m_dna.m_boundsRadius * spawnInfo.m_spawner.m_resources.m_detailPrototypes[m_selectedResourceIdx].m_dna.m_maxScale;
                    }
                case GaiaConstants.SpawnerResourceType.TerrainTree:
                    {
                        return spawnInfo.m_spawner.m_resources.m_treePrototypes[m_selectedResourceIdx].m_dna.m_boundsRadius * spawnInfo.m_spawner.m_resources.m_treePrototypes[m_selectedResourceIdx].m_dna.m_maxScale;
                    }
                default:
                    {
                        return spawnInfo.m_spawner.m_resources.m_gameObjectPrototypes[m_selectedResourceIdx].m_dna.m_boundsRadius * spawnInfo.m_spawner.m_resources.m_gameObjectPrototypes[m_selectedResourceIdx].m_dna.m_maxScale;
                    }
            }
        }

        /// <summary>
        /// Draw gizmos
        /// </summary>
        void OnDrawGizmos()
        {
            if (m_resources == null)
            {
                return;
            }

            if (m_spawner == null)
            {
                return;
            }

            if (m_terrainHeightMap == null)
            {
                return;
            }

            //Lets visualise fitness
            float x, y = transform.position.y, z;
            float xStart = transform.position.x - m_range;
            float xEnd = transform.position.x + m_range;
            float zStart = transform.position.z - m_range;
            float zEnd = transform.position.z + m_range;
            float ballsize = Mathf.Clamp(m_resolution * 0.25f, 0.5f, 5f);

            m_spawner.m_spawnRange = m_range;
            m_spawner.m_spawnerBounds = new Bounds(transform.position, new Vector3(m_range * 2f, m_range * 20f, m_range * 2f));

            SpawnInfo spawnInfo = new SpawnInfo();
            Vector3 location = new Vector3();
            float fitness = 0f;

            //Create caches
            if ((DateTime.Now - m_lastCacheUpdateDate).TotalSeconds > 5)
            {
                m_lastCacheUpdateDate = DateTime.Now;
                m_spawner.DeleteSpawnCaches();
                m_spawner.CreateSpawnCaches(m_selectedResourceType, m_selectedResourceIdx);

                //Also update the location so make moving it easier
                Terrain terrain = TerrainHelper.GetTerrain(transform.position);
                if (terrain != null)
                {
                    transform.position = new Vector3(transform.position.x, terrain.SampleHeight(transform.position) + 5f, transform.position.z );
                }
            }

            //Set up the texture layer array in spawn info
            spawnInfo.m_textureStrengths = new float[Terrain.activeTerrain.terrainData.alphamapLayers];

            //Now visualise fitness
            for (x = xStart; x < xEnd; x += m_resolution)
            {
                for (z = zStart; z < zEnd; z += m_resolution)
                {
                    location.Set(x, y, z);
                    if (m_spawner.CheckLocation(location, ref spawnInfo))
                    {
                        fitness = GetFitness(ref spawnInfo);
                        if (fitness < m_minimumFitness)
                        {
                            continue;
                        }
                        Gizmos.color = Color.Lerp(m_unfitColour, m_fitColour, fitness);
                        Gizmos.DrawSphere(spawnInfo.m_hitLocationWU, ballsize);
                    }
                }
            }

            //Now draw water
            //Water
            if (m_resources != null)
            {
                Bounds bounds = new Bounds();
                if (TerrainHelper.GetTerrainBounds(transform.position, ref bounds) == true)
                {
                    bounds.center = new Vector3(bounds.center.x, m_resources.m_seaLevel, bounds.center.z);
                    bounds.size = new Vector3(bounds.size.x, 0.05f, bounds.size.z);
                    Gizmos.color = new Color(Color.blue.r, Color.blue.g, Color.blue.b, Color.blue.a / 4f);
                    Gizmos.DrawCube(bounds.center, bounds.size);
                }
            }
        }
    }
}