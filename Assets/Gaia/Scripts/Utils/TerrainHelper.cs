using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gaia
{
    /// <summary>
    /// Terrain utility functions
    /// </summary>
    public class TerrainHelper : MonoBehaviour
    {
        [Range(1, 5), Tooltip("Number of smoothing interations to run. Can be run multiple times.")]
        public int m_smoothIterations = 1;

        //Knock ourselves out if we happen to be left there in play mode
        void Awake()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Flatten all the active terrains
        /// </summary>
        public static void Flatten()
        {
            FlattenTerrain(Terrain.activeTerrains);
        }

        /// <summary>
        /// Flatten the terrain passed in
        /// </summary>
        /// <param name="terrain">Terrain to be flattened</param>
        public static void FlattenTerrain(Terrain terrain)
        {
            int width = terrain.terrainData.heightmapResolution;
            int height = terrain.terrainData.heightmapResolution;
            float[,] heights = new float[width, height];
            terrain.terrainData.SetHeights(0, 0, heights);
        }

        /// <summary>
        /// Flatten all the terrains passed in
        /// </summary>
        /// <param name="terrains">Terrains to be flattened</param>
        public static void FlattenTerrain(Terrain[] terrains)
        {
            foreach (Terrain terrain in terrains)
            {
                int width = terrain.terrainData.heightmapResolution;
                int height = terrain.terrainData.heightmapResolution;
                float[,] heights = new float[width, height];
                terrain.terrainData.SetHeights(0, 0, heights);
            }
        }

        /// <summary>
        /// Stitch the terrains together with unity set neighbors calls
        /// </summary>
        public static void Stitch()
        {
            StitchTerrains(Terrain.activeTerrains);
        }

        /// <summary>
        /// Stitch the terrains together - wont align them although should update this to support that as well.
        /// </summary>
        /// <param name="terrains">Array of terrains to organise as neighbors</param>
        public static void StitchTerrains(Terrain[] terrains)
        {
            Terrain right = null;
            Terrain left = null;
            Terrain bottom = null;
            Terrain top = null;

            foreach (Terrain terrain in terrains)
            {
                right = null;
                left = null;
                bottom = null;
                top = null;

                foreach (Terrain neighbor in terrains)
                {
                    //Check to see if neighbor is above or below
                    if (neighbor.transform.position.x == terrain.transform.position.x)
                    {
                        if ((neighbor.transform.position.z + neighbor.terrainData.size.z) == terrain.transform.position.z)
                        {
                            top = neighbor;
                        }
                        else if ((terrain.transform.position.z + terrain.terrainData.size.z) == neighbor.transform.position.z)
                        {
                            bottom = neighbor;
                        }
                    }
                    else if (neighbor.transform.position.z == terrain.transform.position.z)
                    {
                        if ((neighbor.transform.position.x + neighbor.terrainData.size.z) == terrain.transform.position.z)
                        {
                            left = neighbor;
                        }
                        else if ((terrain.transform.position.x + terrain.terrainData.size.x) == neighbor.transform.position.x)
                        {
                            right = neighbor;
                        }
                    }
                }

                terrain.SetNeighbors(left, top, right, bottom);
            }
        }

        /// <summary>
        /// Smooth the active terrain - needs to be extended to all and to handle edges
        /// </summary>
        /// <param name="iterations">Number of smoothing iterations</param>
        public void Smooth()
        {
            Smooth(m_smoothIterations);
        }

        /// <summary>
        /// Smooth the active terrain - needs to be extended to all and to handle edges
        /// </summary>
        /// <param name="iterations">Number of smoothing iterations</param>
        public static void Smooth(int iterations)
        {
            UnityHeightMap hm = new UnityHeightMap(Terrain.activeTerrain);
            hm.Smooth(iterations);
            hm.SaveToTerrain(Terrain.activeTerrain);
        }

        /// <summary>
        /// Get the vector of the centre of the active terrain, and flush to ground level if asked to
        /// </summary>
        /// <param name="flushToGround">If true set it flush to the ground</param>
        /// <returns>Vector3.zero if no terrain, otherwise the centre of it</returns>
        public static Vector3 GetActiveTerrainCenter(bool flushToGround = true)
        {
            Bounds b = new Bounds();
            Terrain t = GetActiveTerrain();
            if (GetTerrainBounds(t, ref b))
            {
                if (flushToGround == true)
                {
                    return new Vector3(b.center.x, t.SampleHeight(b.center), b.center.z);
                }
                else
                {
                    return b.center;
                }
            }
            return Vector3.zero;
        }


        /// <summary>
        /// Get any active terrain - pref active terrain
        /// </summary>
        /// <returns>Any active terrain or null</returns>
        public static Terrain GetActiveTerrain()
        {
            //Grab active terrain if we can
            Terrain terrain = Terrain.activeTerrain;
            if (terrain != null && terrain.isActiveAndEnabled)
            {
                return terrain;
            }

            //Then check rest of terrains
            for (int idx = 0; idx < Terrain.activeTerrains.Length; idx++)
            {
                terrain = Terrain.activeTerrains[idx];
                if (terrain != null && terrain.isActiveAndEnabled)
                {
                    return terrain;
                }
            }
            return null;
        }

        /// <summary>
        /// Get the layer mask of the active terrain, or default if there isnt one
        /// </summary>
        /// <returns>Layermask of activer terrain or default if there isnt one</returns>
        public static LayerMask GetActiveTerrainLayer()
        {
            LayerMask layer = new LayerMask();
            Terrain terrain = GetActiveTerrain();
            if (terrain != null)
            {
                layer.value = 1 << terrain.gameObject.layer;
                return layer;
            }
            layer.value = 1 << LayerMask.NameToLayer("Default");
            return layer;
        }

        /// <summary>
        /// Get the layer mask of the active terrain, or default if there isnt one
        /// </summary>
        /// <returns>Layermask of activer terrain or default if there isnt one</returns>
        public static LayerMask GetActiveTerrainLayerAsInt()
        {
            LayerMask layerValue = GetActiveTerrainLayer().value;
            for (int layerIdx = 0; layerIdx < 32; layerIdx++)
            {
                if (layerValue == (1 << layerIdx))
                {
                    return layerIdx;
                }
            }
            return LayerMask.NameToLayer("Default");
        }

        /// <summary>
        /// Get the number of active terrain tiles in this scene
        /// </summary>
        /// <returns>Number of terrains in the scene</returns>
        public static int GetActiveTerrainCount()
        {
            Terrain terrain;
            int terrainCount = 0;
            for (int idx = 0; idx < Terrain.activeTerrains.Length; idx++)
            {
                terrain = Terrain.activeTerrains[idx];
                if (terrain != null && terrain.isActiveAndEnabled)
                {
                    terrainCount++;
                }
            }
            return terrainCount;
        }

        /// <summary>
        /// Get the terrain that matches this location, otherwise return null
        /// </summary>
        /// <param name="locationWU">Location to check in world units</param>
        /// <returns>Terrain here or null</returns>
        public static Terrain GetTerrain(Vector3 locationWU)
        {
            Terrain terrain;
            Vector3 terrainMin = new Vector3();
            Vector3 terrainMax = new Vector3();

            //First check active terrain - most likely already selected
            terrain = Terrain.activeTerrain;
            if (terrain != null)
            {
                terrainMin = terrain.GetPosition();
                terrainMax = terrainMin + terrain.terrainData.size;
                if (locationWU.x >= terrainMin.x && locationWU.x <= terrainMax.x)
                {
                    if (locationWU.z >= terrainMin.z && locationWU.z <= terrainMax.z)
                    {
                        return terrain;
                    }
                }
            }

            //Then check rest of terrains
            for (int idx = 0; idx < Terrain.activeTerrains.Length; idx++)
            {
                terrain = Terrain.activeTerrains[idx];
                terrainMin = terrain.GetPosition();
                terrainMax = terrainMin + terrain.terrainData.size;
                if (locationWU.x >= terrainMin.x && locationWU.x <= terrainMax.x)
                {
                    if (locationWU.z >= terrainMin.z && locationWU.z <= terrainMax.z)
                    {
                        return terrain;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Get the bounds of the space encapsulated by the supplied terrain
        /// </summary>
        /// <param name="terrain">Terrain to get bounds for</param>
        /// <param name="bounds">Bounds to update</param>
        /// <returns>True if we got some terrain bounds</returns>
        public static bool GetTerrainBounds(Terrain terrain, ref Bounds bounds)
        {
            if (terrain == null)
            {
                return false;
            }
            bounds.center = terrain.transform.position;
            bounds.size = terrain.terrainData.size;
            bounds.center += bounds.extents;
            return true;
        }


        /// <summary>
        /// Get the bounds of the terrain at this location or fail with a null
        /// </summary>
        /// <param name="locationWU">Location to check and get terrain for</param>
        /// <returns>Bounds of selected terrain or null if invalid for some reason</returns>
        public static bool GetTerrainBounds(Vector3 locationWU, ref Bounds bounds)
        {
            Terrain terrain = GetTerrain(locationWU);
            if (terrain == null)
            {
                return false;
            }
            bounds.center = terrain.transform.position;
            bounds.size = terrain.terrainData.size;
            bounds.center += bounds.extents;
            return true;
        }

        /// <summary>
        /// Get a random location on the terrain supplied
        /// </summary>
        /// <param name="terrain">Terrain to check</param>
        /// <param name="start">Start locaton</param>
        /// <param name="radius">Radius to hunt in</param>
        /// <returns></returns>
        public static Vector3 GetRandomPositionOnTerrain(Terrain terrain, Vector3 start, float radius)
        {
            Vector3 newLocation;
            Vector3 terrainMin = terrain.GetPosition();
            Vector3 terrainMax = terrainMin + terrain.terrainData.size;
            while (true)
            {
                //Get a new location
                newLocation = UnityEngine.Random.insideUnitSphere * radius;
                newLocation = start + newLocation;
                //Make sure the new location is within the terrain bounds
                if (newLocation.x >= terrainMin.x && newLocation.x <= terrainMax.x)
                {
                    if (newLocation.z >= terrainMin.z && newLocation.z <= terrainMax.z)
                    { 
                        //Update it to be on the terrain surface
                        newLocation.y = terrain.SampleHeight(newLocation);
                        return newLocation;
                    }
                }
            }
        }

        /// <summary>
        /// Clear all the trees on all the terrains
        /// </summary>
        public static void ClearTrees()
        {
            Terrain terrain;
            List<TreeInstance> trees = new List<TreeInstance>();
            for (int idx = 0; idx < Terrain.activeTerrains.Length; idx++)
            {
                terrain = Terrain.activeTerrains[idx];
                terrain.terrainData.treeInstances = trees.ToArray();
                terrain.Flush();
            }

            //Call reset on all tree spawners to remove the colliders and reset them to start
            Spawner[] spawners = Object.FindObjectsOfType<Spawner>();
            foreach (Spawner spawner in spawners)
            {
                spawner.SetUpSpawnerTypeFlags();
                if (spawner.IsTreeSpawner())
                {
                    spawner.ResetSpawner();
                }
            }
        }

        /// <summary>
        /// Clear all the details (grass) on all the terrains
        /// </summary>
        public static void ClearDetails()
        {
            Terrain terrain;
            for (int idx = 0; idx < Terrain.activeTerrains.Length; idx++)
            {
                terrain = Terrain.activeTerrains[idx];
                int [,] details = new int[terrain.terrainData.detailWidth, terrain.terrainData.detailHeight];
                for (int dtlIdx = 0; dtlIdx < terrain.terrainData.detailPrototypes.Length; dtlIdx++)
                {
                    terrain.terrainData.SetDetailLayer(0, 0, dtlIdx, details);
                }
                terrain.Flush();
            }

            //Call reset on all tree spawners to remove the colliders and reset them to start
            Spawner[] spawners = Object.FindObjectsOfType<Spawner>();
            foreach (Spawner spawner in spawners)
            {
                if (spawner.IsDetailSpawner())
                {
                    spawner.ResetSpawner();
                }
            }
        }

        /// <summary>
        /// Get the range from the terrain
        /// </summary>
        /// <returns></returns>
        public static float GetRangeFromTerrain()
        {
            Terrain t = Gaia.TerrainHelper.GetActiveTerrain();
            if (t != null)
            {
                return Mathf.Max(t.terrainData.size.x, t.terrainData.size.z) / 2f;
            }
            return 0f;
        }

    }
}