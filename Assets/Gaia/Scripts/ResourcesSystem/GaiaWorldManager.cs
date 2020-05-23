using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gaia
{
    /// <summary>
    /// World manager class for Gaia. 
    /// 
    /// Note to self - some of these routines need to be adapted to run at global level to 
    /// avound boundary level issues. Also some routines do excessive conversions. Needs
    /// a little optimisation.
    /// 
    /// TU - Tile units - physical tile units on one to one basis with unity height map
    /// WU - World units - physical world units as unity world unitys, default 1 unit == 1 meter
    /// NU - Normal units - world in normal units
    /// PTI - Physical Tile Index - the index of the physical tile
    /// PTO - Physical Tile Offset - the index of array element within physical a tile
    /// 
    /// </summary>
    public class GaiaWorldManager
    {
        #region Variables

        /// <summary>
        /// Bounds in world units
        /// </summary>
        private Bounds m_worldBoundsWU = new Bounds();
        private Vector3 m_worldBoundsWUMin;
        private Vector3 m_worldBoundsWUMax;
        private Vector3 m_worldBoundsWUSize;

        /// <summary>
        /// Bounds in terrain units
        /// </summary>
        private Bounds m_worldBoundsTU = new Bounds();
        private Vector3 m_worldBoundsTUMin;
        private Vector3 m_worldBoundsTUMax;
        private Vector3 m_worldBoundsTUSize;

        /// <summary>
        /// Bounds in normal units
        /// </summary>
        private Bounds m_worldBoundsNU = new Bounds();
        private Vector3 m_worldBoundsNUMin;
        private Vector3 m_worldBoundsNUMax;

        /// <summary>
        /// Conversion from world units to terrain units
        /// </summary>
        private Vector3 m_WUtoTU = Vector3.one;

        /// <summary>
        /// Conversion from terrain units to world units
        /// </summary>
        private Vector3 m_TUtoWU = Vector3.one;

        /// <summary>
        /// Conversion from terrain units to normal units
        /// </summary>
        private Vector3 m_TUtoNU = Vector3.one;

        /// <summary>
        /// Conversion from normal units to terrain units
        /// </summary>
        private Vector3 m_NUtoTU = Vector3.one;

        /// <summary>
        /// Conversion factor of world units to normal units
        /// </summary>
        private Vector3 m_WUtoNU = Vector3.one;

        /// <summary>
        /// Conversion factor of normal units to world units
        /// </summary>
        private Vector3 m_NUtoWU = Vector3.one;

        /// <summary>
        /// Zero offset for NU, usefult to translate to physical units
        /// </summary>
        private Vector3 m_NUZeroOffset = Vector3.zero;

        /// <summary>
        /// Zero offset for TU, usefult to translate to physical units
        /// </summary>
        private Vector3 m_TUZeroOffset = Vector3.zero;

        /// <summary>
        /// Nmmber of bounds check errors - will be incremented every time something is attempted that is out of bounds
        /// </summary>
        private ulong m_boundsCheckErrors = 0;

        /// <summary>
        /// Array of terrains being managed
        /// </summary>
        private Terrain[,] m_physicalTerrainArray;

        /// <summary>
        /// Array of heightmap terrains being managed - loaded only on demand
        /// </summary>
        private UnityHeightMap[,] m_heightMapTerrainArray;

        /// <summary>
        /// The number of terrain tiles being managed
        /// </summary>
        private int m_tileCount;

        #endregion

        #region Constructors
        public GaiaWorldManager()
        {
        }

        /// <summary>
        /// Constuct a new manager from the existing environment
        /// </summary>
        /// <param name="terrains"></param>
        public GaiaWorldManager(Terrain[] terrains)
        {
            Terrain terrain = null;
            m_worldBoundsWU = new Bounds();
            m_worldBoundsTU = new Bounds();
            m_worldBoundsNU = new Bounds();
            Bounds terrainBoundsWU;
            Bounds terrainBoundsTU;

            string validWorld = IsValidWorld(terrains);
            if (!string.IsNullOrEmpty(validWorld))
            {
                Debug.LogError("GaiaWorldManager(terrains) ERROR" + validWorld);
                return;
            }

            //Calculate bounds
            for (int idx = 0; idx < terrains.Length; idx++)
            {
                terrain = terrains[idx];

                terrainBoundsWU = new Bounds(terrain.transform.position, terrain.terrainData.size);
                terrainBoundsWU.center += terrainBoundsWU.extents;
                if (idx == 0)
                {
                    m_worldBoundsWU = new Bounds(terrainBoundsWU.center, terrainBoundsWU.size);
                }
                else
                {
                    m_worldBoundsWU.Encapsulate(terrainBoundsWU);
                }

                terrainBoundsTU = new Bounds();
                m_WUtoTU = new Vector3(
                    (float)(terrain.terrainData.heightmapResolution) / terrain.terrainData.size.x,
                    (((float)(terrain.terrainData.heightmapResolution - 1) / Mathf.Max(terrain.terrainData.size.x, terrain.terrainData.size.z)) * terrain.terrainData.size.y) / terrain.terrainData.size.y,
                    (float)(terrain.terrainData.heightmapResolution) / terrain.terrainData.size.z
                    );
                m_TUtoWU = new Vector3(1f / m_WUtoTU.x, 1f / m_WUtoTU.y, 1f / m_WUtoTU.z);
                terrainBoundsTU.center = Vector3.Scale(terrainBoundsWU.center, m_WUtoTU);
                terrainBoundsTU.size = Vector3.Scale(terrainBoundsWU.size, m_WUtoTU);
                if (idx == 0)
                {
                    m_worldBoundsTU = new Bounds(terrainBoundsTU.center, terrainBoundsTU.size);
                }
                else
                {
                    m_worldBoundsTU.Encapsulate(terrainBoundsTU);
                }
            }

            //Pick up and calc normal units off last terrain
            if (terrain != null)
            {
                m_TUtoNU = new Vector3(1f / m_worldBoundsTU.size.x, 1f / m_worldBoundsTU.size.y, 1f / m_worldBoundsTU.size.z);
                m_NUtoTU = m_worldBoundsTU.size;
                m_WUtoNU = Vector3.Scale(m_WUtoTU, m_TUtoNU);
                m_NUtoWU = m_worldBoundsWU.size;
            }
            m_worldBoundsNU.center = Vector3.Scale(m_worldBoundsTU.center, m_TUtoNU);
            m_worldBoundsNU.size = Vector3.Scale(m_worldBoundsTU.size, m_TUtoNU);

            //Now work out the NU zero offset
            m_NUZeroOffset = Vector3.zero - m_worldBoundsNU.min;
            m_TUZeroOffset = Vector3.zero - m_worldBoundsTU.min;
            //Vector3 nuZero = m_NUZeroOffset + m_worldBoundsNU.min;

            //Map the terrains to the size of the environment based on terrain bounds
            Vector3 positionPTI;
            m_tileCount = (int)(m_worldBoundsNU.size.x * m_worldBoundsNU.size.z);
            m_physicalTerrainArray = new Terrain[(int)m_worldBoundsNU.size.x, (int)m_worldBoundsNU.size.z];
            m_heightMapTerrainArray = new UnityHeightMap[(int)m_worldBoundsNU.size.x, (int)m_worldBoundsNU.size.z];
            for (int idx = 0; idx < terrains.Length; idx++)
            {
                terrain = terrains[idx];
                positionPTI = WUtoPTI(terrain.transform.position);
                m_physicalTerrainArray[(int)positionPTI.x, (int)positionPTI.z] = terrain;
            }

            m_worldBoundsWUMax = m_worldBoundsWU.max;
            m_worldBoundsWUMin = m_worldBoundsWU.min;
            m_worldBoundsWUSize = m_worldBoundsWU.size;

            m_worldBoundsTUMax = m_worldBoundsTU.max;
            m_worldBoundsTUMin = m_worldBoundsTU.min;
            m_worldBoundsTUSize = m_worldBoundsTU.size;

            m_worldBoundsNUMax = m_worldBoundsNU.max;
            m_worldBoundsNUMin = m_worldBoundsNU.min;
        }

        #endregion

        #region Attributes

        /// <summary>
        /// The number of terrain tiles being managed
        /// </summary>
        public int TileCount
        {
            get { return m_tileCount; }
        }

        /// <summary>
        /// Physical terrain array being managed
        /// </summary>
        public Terrain[,] PhysicalTerrainArray
        {
            get { return m_physicalTerrainArray; }
            set { m_physicalTerrainArray = value; }
        }

        /// <summary>
        /// Unity heightmap based terrain array being managed - loased only on demand
        /// </summary>
        public UnityHeightMap[,] HeightMapTerrainArray
        {
            get { return m_heightMapTerrainArray; }
            set { m_heightMapTerrainArray = value; }
        }

        /// <summary>
        /// Bounds in worlds units
        /// </summary>
        public Bounds WorldBoundsWU
        {
            get { return m_worldBoundsWU; }
        }

        /// <summary>
        /// Bounds in terrain units
        /// </summary>
        public Bounds WorldBoundsTU
        {
            get { return m_worldBoundsTU; }
        }

        /// <summary>
        /// Bounds in normal units
        /// </summary>
        public Bounds WorldBoundsNU
        {
            get { return m_worldBoundsNU; }
        }

        /// <summary>
        /// The conversion factor from world units to terrain units
        /// </summary>
        public Vector3 WUtoTUConversionFactor
        {
            get { return m_WUtoTU; }
        }

        /// <summary>
        /// The conversion factor from world units to normal units
        /// </summary>
        public Vector3 WUtoNUConversionFactor
        {
            get { return m_WUtoNU; }
        }

        /// <summary>
        /// Access to bounds check errors - rather than throwing errors when something is out of bounds, this 
        /// variable will be incremented. Up to user to check for this to detect issues in their code.
        /// </summary>
        public ulong BoundsCheckErrors
        {
            get { return m_boundsCheckErrors; }
            set { m_boundsCheckErrors = value; }
        }

        #endregion

        #region Validation

        /// <summary>
        /// Check all the terrain settings - if they are not right then Gaia wont work as expected
        /// </summary>
        /// <param name="terrains"></param>
        /// <returns></returns>
        public string IsValidWorld(Terrain[] terrains)
        {
            Terrain firstTerrain = null;
            Terrain terrain = null;
            StringBuilder terrainCheck = new StringBuilder();

            //Calculate bounds
            for (int idx = 0; idx < terrains.Length; idx++)
            {
                terrain = terrains[idx];
                if (firstTerrain == null)
                {
                    firstTerrain = terrain;
                }

                //Do a check vs first terrain settings - all settings must be same for Gaia to work
                if (terrain.terrainData.size.x != terrain.terrainData.size.z)
                {
                    terrainCheck.Append(string.Format("\nTerrain {0} is not a square {1} {2}", terrain.name, terrain.terrainData.size.x, terrain.terrainData.size.z));
                }
                if (terrain.terrainData.size != firstTerrain.terrainData.size)
                {
                    terrainCheck.Append(string.Format("\nTerrain {0} - {1} size does not match {2} {3}", terrain.name, firstTerrain.name, terrain.terrainData.size, firstTerrain.terrainData.size));
                }
                if (terrain.terrainData.heightmapResolution != firstTerrain.terrainData.heightmapResolution)
                {
                    terrainCheck.Append(string.Format("\nTerrain {0} - {1} heightmapResolution does not match {2} {3}", terrain.name, firstTerrain.name, terrain.terrainData.heightmapResolution, firstTerrain.terrainData.heightmapResolution));
                }
                if (terrain.terrainData.alphamapResolution != firstTerrain.terrainData.alphamapResolution)
                {
                    terrainCheck.Append(string.Format("\nTerrain {0} - {1} alphamapResolution does not match {2} {3}", terrain.name, firstTerrain.name, terrain.terrainData.alphamapResolution, firstTerrain.terrainData.alphamapResolution));
                }
                if (terrain.terrainData.baseMapResolution != firstTerrain.terrainData.baseMapResolution)
                {
                    terrainCheck.Append(string.Format("\nTerrain {0} - {1} baseMapResolution does not match {2} {3}", terrain.name, firstTerrain.name, terrain.terrainData.baseMapResolution, firstTerrain.terrainData.baseMapResolution));
                }
                if (terrain.terrainData.detailResolution != firstTerrain.terrainData.detailResolution)
                {
                    terrainCheck.Append(string.Format("\nTerrain {0} - {1} detailResolution does not match {2} {3}", terrain.name, firstTerrain.name, terrain.terrainData.detailResolution, firstTerrain.terrainData.detailResolution));
                }
                if (terrain.terrainData.alphamapLayers != firstTerrain.terrainData.alphamapLayers)
                {
                    terrainCheck.Append(string.Format("\nTerrain {0} - {1} alphamapLayers does not match {2} {3}", terrain.name, firstTerrain.name, terrain.terrainData.alphamapLayers, firstTerrain.terrainData.alphamapLayers));
                }
                if (terrain.terrainData.detailPrototypes.Length != firstTerrain.terrainData.detailPrototypes.Length)
                {
                    terrainCheck.Append(string.Format("\nTerrain {0} - {1} detailPrototypes.Length does not match {2} {3}", terrain.name, firstTerrain.name, terrain.terrainData.detailPrototypes.Length, firstTerrain.terrainData.detailPrototypes.Length));
                }
                if (GaiaSplatPrototype.GetGaiaSplatPrototypes(terrain).Length != GaiaSplatPrototype.GetGaiaSplatPrototypes(firstTerrain).Length)
                {
                    terrainCheck.Append(string.Format("\nTerrain {0} - {1} splatPrototypes.Length does not match {2} {3}", terrain.name, firstTerrain.name, GaiaSplatPrototype.GetGaiaSplatPrototypes(terrain).Length, GaiaSplatPrototype.GetGaiaSplatPrototypes(firstTerrain).Length));
                }
                if (terrain.terrainData.treePrototypes.Length != firstTerrain.terrainData.treePrototypes.Length)
                {
                    terrainCheck.Append(string.Format("\nTerrain {0} - {1} treePrototypes.Length does not match {2} {3}", terrain.name, firstTerrain.name, terrain.terrainData.treePrototypes.Length, firstTerrain.terrainData.treePrototypes.Length));
                }
            }

            return terrainCheck.ToString();
        }

        #endregion

        #region Terrain & heightmap object access

        /// <summary>
        /// Get the terrain at this world unit location
        /// </summary>
        /// <param name="positionWU">Position in world units to get the terrain</param>
        /// <returns>The terrain, or null if out of bounds or none there</returns>
        Terrain GetTerrainWU(Vector3 positionWU)
        {
            if (!InBoundsWU(positionWU))
            {
                m_boundsCheckErrors++;
                return null;
            }
            Vector3 positionPTI = WUtoPTI(positionWU);
            return m_physicalTerrainArray[(int)positionPTI.x, (int)positionPTI.z];
        }

        /// <summary>
        /// Get the terrain at this terrain unit location
        /// </summary>
        /// <param name="positionTU">Position in terrain units to get the terrain</param>
        /// <returns>The terrain, or null if out of bounds or none there</returns>
        Terrain GetTerrainTU(Vector3 positionTU)
        {
            if (!InBoundsTU(positionTU))
            {
                m_boundsCheckErrors++;
                return null;
            }
            TUtoPTI(ref positionTU);
            return m_physicalTerrainArray[(int)positionTU.x, (int)positionTU.z];
        }

        /// <summary>
        /// Get the terrain at this normal unit location
        /// </summary>
        /// <param name="positionNU">Position in normal units to get the terrain</param>
        /// <returns>The terrain, or null if out of bounds or none there</returns>
        Terrain GetTerrainNU(Vector3 positionNU)
        {
            if (!InBoundsNU(positionNU))
            {
                m_boundsCheckErrors++;
                return null;
            }
            NUtoPTI(ref positionNU);
            return m_physicalTerrainArray[(int)positionNU.x, (int)positionNU.z];
        }

        /// <summary>
        /// Get the unity height map at this location. If there is none there then create and load it from underlying terrain
        /// </summary>
        /// <param name="positionWU">Position in world units to get the terrain</param>
        /// <returns>The terrain heightmap, or null if out of bounds or none there</returns>
        UnityHeightMap GetHeightMapWU(Vector3 positionWU)
        {
            if (!InBoundsWU(positionWU))
            {
                m_boundsCheckErrors++;
                return null;
            }
            Vector3 positionPTI = WUtoPTI(positionWU);
            UnityHeightMap hm = m_heightMapTerrainArray[(int)positionPTI.x, (int)positionPTI.z];
            if (hm == null)
            {
                Terrain t = GetTerrainWU(positionWU);
                if (t != null)
                {
                    //Create and store a new height map, and load the terrain into it
                    m_heightMapTerrainArray[(int)positionPTI.x, (int)positionPTI.z] = hm = new UnityHeightMap(t);
                }
            }
            return hm;
        }

        /// <summary>
        /// Get the unity height map at this location. If there is none there then create and load it from underlying terrain
        /// </summary>
        /// <param name="positionTU">Position in terrain units to get the terrain</param>
        /// <returns>The terrain heightmap, or null if out of bounds or none there</returns>
        UnityHeightMap GetHeightMapTU(Vector3 positionTU)
        {
            if (!InBoundsTU(positionTU))
            {
                m_boundsCheckErrors++;
                return null;
            }
            TUtoPTI(ref positionTU);
            UnityHeightMap hm = m_heightMapTerrainArray[(int)positionTU.x, (int)positionTU.z];
            if (hm == null)
            {
                Terrain t = GetTerrainTU(positionTU);
                if (t != null)
                {
                    //Create and store a new height map, and load the terrain into it
                    m_heightMapTerrainArray[(int)positionTU.x, (int)positionTU.z] = hm = new UnityHeightMap(t);
                }
            }
            return hm;
        }

        /// <summary>
        /// Get the unity height map at this location. If there is none there then create and load it from underlying terrain
        /// </summary>
        /// <param name="positionNU">Position in terrain units to get the terrain</param>
        /// <returns>The terrain heightmap, or null if out of bounds or none there</returns>
        UnityHeightMap GetHeightMapNU(Vector3 positionNU)
        {
            if (!InBoundsNU(positionNU))
            {
                m_boundsCheckErrors++;
                return null;
            }
            NUtoPTI(ref positionNU);
            UnityHeightMap hm = m_heightMapTerrainArray[(int)positionNU.x, (int)positionNU.z];
            if (hm == null)
            {
                Terrain t = GetTerrainNU(positionNU);
                if (t != null)
                {
                    //Create and store a new height map, and load the terrain into it
                    m_heightMapTerrainArray[(int)positionNU.x, (int)positionNU.z] = hm = new UnityHeightMap(t);
                }
            }
            return hm;
        }


        #endregion

        #region Terrain load and save operations

        /// <summary>
        /// Get all heightmaps from the world and load them - use with care can use a lot of memory
        /// </summary>
        public void LoadFromWorld()
        {
            UnityHeightMap hm;
            for (int x = 0; x < m_heightMapTerrainArray.GetLength(0); x++)
            {
                for (int z = 0; z < m_heightMapTerrainArray.GetLength(1); z++)
                {
                    hm = m_heightMapTerrainArray[x, z];
                    if (hm == null)
                    {
                        Terrain t = m_physicalTerrainArray[x, z];
                        if (t != null)
                        {
                            m_heightMapTerrainArray[x, z] = new UnityHeightMap(t);
                        }
                    }
                    else
                    {
                        hm.LoadFromTerrain(m_physicalTerrainArray[x, z]);
                    }
                }
            }
        }

        /// <summary>
        /// Write all heightmaps back to the world
        /// </summary>
        /// <param name="forceWrite">If true, will aways write, otherwsie will only write changes</param>
        public void SaveToWorld(bool forceWrite = false)
        {
            UnityHeightMap hm;
            for (int x = 0; x < m_heightMapTerrainArray.GetLength(0); x++)
            {
                for (int z = 0; z < m_heightMapTerrainArray.GetLength(1); z++)
                {
                    hm = m_heightMapTerrainArray[x, z];
                    if (hm != null)
                    {
                        if (!forceWrite)
                        {
                            if (hm.IsDirty())
                            {
                                hm.SaveToTerrain(m_physicalTerrainArray[x, z]);
                            }
                        }
                        else
                        {
                            hm.SaveToTerrain(m_physicalTerrainArray[x, z]);
                        }
                    }
                }
            }
        }


        #endregion

        #region Heightmap read and write operations

        /// <summary>
        /// Set the height of all the terrains to this height in world units
        /// </summary>
        /// <param name="heightWU"></param>
        public void SetHeightWU(float heightWU)
        {
            float newHeight = Mathf.Clamp01(heightWU / m_worldBoundsWUSize.y);
            for (int x = 0; x < m_heightMapTerrainArray.GetLength(0); x++)
            {
                for (int z = 0; z < m_heightMapTerrainArray.GetLength(1); z++)
                {
                    m_heightMapTerrainArray[x, z].SetHeight(newHeight);
                }
            }
        }

        /// <summary>
        /// Set height in terrain in world units
        /// </summary>
        /// <param name="positionWU">Position in world Units</param>
        /// <param name="height">Height to set</param>
        public void SetHeightWU(Vector3 positionWU, float height)
        {
            UnityHeightMap uhm = GetHeightMapWU(positionWU);
            if (uhm != null)
            {
                positionWU = WUtoPTO(positionWU);
                uhm[(int)positionWU.x, (int)positionWU.z] = height;
            }
            else
            {
                m_boundsCheckErrors++;
            }
        }

        /// <summary>
        /// Get height from position on terrain in world units
        /// </summary>
        /// <param name="positionWU">Position in world Units</param>
        public float GetHeightWU(Vector3 positionWU)
        {
            UnityHeightMap uhm = GetHeightMapWU(positionWU);
            if (uhm != null)
            {
                positionWU = WUtoPTO(positionWU);
                return uhm[(int)positionWU.x, (int)positionWU.z];
            }
            else
            {
                return float.MinValue;
            }
        }

        /// <summary>
        /// Get interpolated height from position on terrain in world units (NOTE THIS NEEDS TO BE ADAPTED TO RUN AT GLOBAL LEVEL DUE TO GLOBALT TILE ISSUES)
        /// </summary>
        /// <param name="positionWU">Position in world Units</param>
        public float GetHeightInterpolatedWU(Vector3 positionWU)
        {
            UnityHeightMap uhm = GetHeightMapWU(positionWU);
            if (uhm != null)
            {
                positionWU = WUtoPTO(positionWU);
                return uhm[positionWU.x, positionWU.z];
            }
            else
            {
                return float.MinValue;
            }
        }

        /// <summary>
        /// Set height in terrain in terrain units
        /// </summary>
        /// <param name="positionNU">Position in terrain Units</param>
        /// <param name="height">Height to set</param>
        public void SetHeightTU(Vector3 positionTU, float height)
        {
            UnityHeightMap uhm = GetHeightMapTU(positionTU);
            if (uhm != null)
            {
                TUtoPTO(ref positionTU);
                uhm[(int)positionTU.x, (int)positionTU.z] = height;
            }
            else
            {
                m_boundsCheckErrors++;
            }
        }

        /// <summary>
        /// Get height from position on terrain in terrain units
        /// </summary>
        /// <param name="positionTU">Position in terrain Units</param>
        /// <returns>Height at that location or float.MinValue if out of bounds</returns>
        public float GetHeightTU(Vector3 positionTU)
        {
            UnityHeightMap uhm = GetHeightMapTU(positionTU);
            if (uhm != null)
            {
                TUtoPTO(ref positionTU);
                return uhm[(int)positionTU.x, (int)positionTU.z];
            }
            else
            {
                return float.MinValue;
            }
        }

        /// <summary>
        /// Get interpolated height from position on terrain in terrain units (NOTE THIS NEEDS TO BE ADAPTED TO RUN AT GLOBAL LEVEL DUE TO GLOBAL TILE ISSUES)
        /// </summary>
        /// <param name="positionTU">Position in terrain units</param>
        /// <returns>Height at that location or float.MinValue if out of bounds</returns>
        public float GetHeightInterpolatedTU(Vector3 positionTU)
        {
            UnityHeightMap uhm = GetHeightMapTU(positionTU);
            if (uhm != null)
            {
                TUtoPTO(ref positionTU);
                return uhm[positionTU.x, positionTU.z];
            }
            else
            {
                return float.MinValue;
            }
        }

        #endregion

        #region Terrain utilties

        /// <summary>
        /// Flatten all heightmaps - in memory and physically as well
        /// </summary>
        public void FlattenWorld()
        {
            UnityHeightMap hm;
            for (int x = 0; x < m_heightMapTerrainArray.GetLength(0); x++)
            {
                for (int z = 0; z < m_heightMapTerrainArray.GetLength(1); z++)
                {
                    hm = m_heightMapTerrainArray[x, z];
                    if (hm == null)
                    {
                        Terrain t = m_physicalTerrainArray[x, z];
                        if (t != null)
                        {
                            hm = m_heightMapTerrainArray[x, z] = new UnityHeightMap(t);
                        }
                    }
                    if (hm != null)
                    {
                        hm.SetHeight(0f);
                        hm.SaveToTerrain(m_physicalTerrainArray[x, z]);
                    }
                }
            }
        }

        /// <summary>
        /// Smooth all heightmaps - in memory and physically as well
        /// </summary>
        public void SmoothWorld()
        {
            UnityHeightMap hm;
            for (int x = 0; x < m_heightMapTerrainArray.GetLength(0); x++)
            {
                for (int z = 0; z < m_heightMapTerrainArray.GetLength(1); z++)
                {
                    hm = m_heightMapTerrainArray[x, z];
                    if (hm == null)
                    {
                        Terrain t = m_physicalTerrainArray[x, z];
                        if (t != null)
                        {
                            hm = m_heightMapTerrainArray[x, z] = new UnityHeightMap(t);
                        }
                    }
                    if (hm != null)
                    {
                        hm.Smooth(1);
                        hm.SaveToTerrain(m_physicalTerrainArray[x, z]);
                    }
                }
            }
        }

        /// <summary>
        /// Export the world as a PNG
        /// </summary>
        /// <param name="path">Path to save file as</param>
        public void ExportWorldAsPng(string path)
        {
            Vector3 positionTU = m_worldBoundsTU.center;
            HeightMap hm = new HeightMap((int)m_worldBoundsTUSize.z, (int)m_worldBoundsTUSize.x);

            //Grab and flip the world (due to unity flipping terrains)
            for (int x = 0, srcX = (int)m_worldBoundsTUMin.x; srcX < (int)m_worldBoundsTUMax.x; x++, srcX++)
            {
                positionTU.x = srcX;
                for (int z = 0, srcZ = (int)m_worldBoundsTUMin.z; srcZ < (int)m_worldBoundsTUMax.z; z++, srcZ++)
                {
                    positionTU.z = srcZ;
                    hm[z, x] = GetHeightTU(positionTU);
                }
            }

            //Now write it to the path provided
            GaiaUtils.CompressToSingleChannelFileImage(hm.Heights(), path, TextureFormat.RGBA32, true, false);
        }

        /// <summary>
        /// Export the selected splatmap texture as a PNG or all
        /// </summary>
        /// <param name="path">Path to save it as</param>
        /// <param name="textureIdx">The texture to save</param>
        public void ExportSplatmapAsPng(string path, int textureIdx)
        {
            Terrain terrain = Terrain.activeTerrain;
            if (terrain == null)
            {
                Debug.LogError("No active terrain, unable to export splatmaps");
                return;
            }
            int width = terrain.terrainData.alphamapWidth;
            int height = terrain.terrainData.alphamapHeight;
            int layers = terrain.terrainData.alphamapLayers;

            if (textureIdx < layers)
            {
                HeightMap txtMap = new HeightMap(terrain.terrainData.GetAlphamaps(0, 0, width, height), textureIdx);
                txtMap.Flip();
                GaiaUtils.CompressToSingleChannelFileImage(txtMap.Heights(), path, TextureFormat.RGBA32, true, false);
            }
            else
            {
                float[, ,] splatMaps = terrain.terrainData.GetAlphamaps(0, 0, width, height);
                /*
                for (int sm = 0; sm < layers; sm++)
                {
                    //Now flip them
                    for (int x = 0; x < width; x++)
                    {
                        for (int z = 0; z < height; z++)
                        {
                            splatMaps[z, x, sm] = splatMaps[x, z, sm];
                        }
                    }
                }
                 */
                GaiaUtils.CompressToMultiChannelFileImage(splatMaps, path, TextureFormat.RGBA32, true, false);
            }
        }

        /// <summary>
        /// Export the selected grassmap texture as a PNG or all
        /// </summary>
        /// <param name="path">Path to save it as</param>
        /// <param name="detailIdx">The grass to save</param>
        public void ExportGrassmapAsPng(string path)
        {
            Terrain terrain = Terrain.activeTerrain;
            if (terrain == null)
            {
                Debug.LogError("No active terrain, unable to export grassmaps");
                return;
            }
            int width = terrain.terrainData.detailWidth;
            int height = terrain.terrainData.detailHeight;
            int layers = terrain.terrainData.detailPrototypes.Length;

            float [,,] detailMaps = new float[width, height, layers];
            int[,] detailMap;

            for (int dtlIdx = 0; dtlIdx < terrain.terrainData.detailPrototypes.Length; dtlIdx++)
            {
                detailMap = terrain.terrainData.GetDetailLayer(0,0, terrain.terrainData.detailWidth, terrain.terrainData.detailHeight, dtlIdx);
                //Copy the map
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < height; z++)
                    {
                        detailMaps[x, z, dtlIdx] = (float)detailMap[x, z] / 16f;
                    }
                }
                //Flip it
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < height; z++)
                    {
                        detailMaps[z, x, dtlIdx] = detailMaps[x, z, dtlIdx];
                    }
                }
            }
            GaiaUtils.CompressToMultiChannelFileImage(detailMaps, path, TextureFormat.RGBA32, true, false);
        }

        /// <summary>
        /// Export the world normals as a series of PNG files
        /// </summary>
        /// <param name="path">Path to save it as</param>
        public void ExportNormalmapAsPng(string path)
        {
            for (int tileX = 0; tileX < m_physicalTerrainArray.GetLength(0); tileX++)
            {
                for (int tileZ = 0; tileZ < m_physicalTerrainArray.GetLength(1); tileZ++)
                {
                    #if UNITY_EDITOR

                    Texture2D nrmTexture = m_heightMapTerrainArray[tileX, tileZ].CalculateNormals();
                    string spath = path + "_" + tileX + "_" + tileZ + "_N.png";
                    byte[] pngBytes = nrmTexture.EncodeToPNG();
                    GaiaCommon1.Utils.WriteAllBytes(spath, pngBytes);
                    MonoBehaviour.DestroyImmediate(nrmTexture);

                    AssetDatabase.Refresh();
                    Texture2D normalTex = GaiaUtils.GetAsset(spath, typeof(Texture2D)) as Texture2D;
                    GaiaUtils.MakeTextureNormal(normalTex);
                    #endif
                }
            }
        }

        /// <summary>
        /// Export the world normals as a series of PNG files
        /// </summary>
        /// <param name="path">Path to save it as</param>
        public void ExportNormalmapAsPng1(string path)
        {
            Terrain terrain = null;
            int width = 0, height = 0;
            float[,,] nrmMap = null;
            Vector3 normal;

            for (int tileX = 0; tileX < m_physicalTerrainArray.GetLength(0); tileX++)
            {
                for (int tileZ = 0; tileZ < m_physicalTerrainArray.GetLength(1); tileZ++)
                {
                    terrain = m_physicalTerrainArray[tileX, tileZ];
                    if (terrain != null)
                    {
                        width = terrain.terrainData.heightmapResolution;
                        height = terrain.terrainData.heightmapResolution;
                        nrmMap = new float[width, height, 4];

                        for (int x = 0; x < width; x++)
                        {
                            for (int z = 0; z < height; z++)
                            {
                                normal = terrain.terrainData.GetInterpolatedNormal((float)x / (float)width, (float)z / (float)height);
                                nrmMap[x, z, 0] = (normal.x * 0.5f) + 0.5f;
                                nrmMap[x, z, 1] = (normal.y * 0.5f) + 0.5f;
                                nrmMap[x, z, 2] = (normal.z * 0.5f) + 0.5f;
                            }
                        }
                        GaiaUtils.CompressToMultiChannelFileImage(nrmMap, path + "_" + tileX + "_" + tileZ, TextureFormat.RGBA32, true, false);
#if UNITY_EDITOR
                        AssetDatabase.Refresh();
                        Texture2D normalTex = GaiaUtils.GetAsset(path + "_" + tileX + "_" + tileZ + "0.png", typeof(Texture2D)) as Texture2D;
                        GaiaUtils.MakeTextureNormal(normalTex);
#endif
                    }
                }
            }
        }

        /// <summary>
        /// Erode the terrain
        /// </summary>
        /// <param name="path">Path to save it as</param>
        public void ExportWaterflowMapAsPng(int iterations, string path)
        {
            for (int tileX = 0; tileX < m_physicalTerrainArray.GetLength(0); tileX++)
            {
                for (int tileZ = 0; tileZ < m_physicalTerrainArray.GetLength(1); tileZ++)
                {
                    HeightMap hm = m_heightMapTerrainArray[tileX, tileZ].FlowMap(iterations).Normalise();

                    //Now write it to the path provided
                    GaiaUtils.CompressToSingleChannelFileImage(hm.Heights(), path + "_" + tileX + "_" + tileZ + ".png", TextureFormat.RGBA32, true, false);
                }
            }
        }

        /// <summary>
        /// Export the world as a PNG with the shoreline masked out
        /// </summary>
        /// <param name="path">Patht to save file as</param>
        /// <param name="shoreHeightNU"></param>
        public void ExportShorelineMask(string path, float shoreHeightWU, float shoreWidthWU)
        {
            Vector3 positionTU = m_worldBoundsTU.center;
            float shoreHeightNU = shoreHeightWU / m_worldBoundsWUSize.y;
            Vector3 shoreWidthTU = WUtoTU(new Vector3(shoreWidthWU, shoreWidthWU, shoreWidthWU));
            HeightMap shoreMask = new HeightMap((int)m_worldBoundsTUSize.z, (int)m_worldBoundsTUSize.x);

            //Mask the world
            for (float x = 0, srcX = m_worldBoundsTUMin.x; srcX < m_worldBoundsTUMax.x; x += 1f, srcX += 1f)
            {
                positionTU.x = srcX;
                for (float z = 0, srcZ = m_worldBoundsTUMin.z; srcZ < m_worldBoundsTUMax.z; z += 1f, srcZ += 1f)
                {
                    positionTU.z = srcZ;
                    MakeMask(positionTU, shoreHeightNU, shoreWidthTU.x, shoreMask);
                }
            }

            //Flip it
            shoreMask.Flip();

            //Now write it to the path provided
            GaiaUtils.CompressToSingleChannelFileImage(shoreMask.Heights(), path, TextureFormat.RGBA32, true, false);
        }

        /// <summary>
        /// Make a mask by iterating out from this location 
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startZ"></param>
        /// <param name="shoreHeightNU"></param>
        /// <param name="maskSizeTU"></param>
        /// <param name="waterMask"></param>
        /// <param name="maskHm"></param>
        private void MakeMask(Vector3 positionTU, float shoreHeightNU, float maskSizeTU, HeightMap waterMask)
        {
            int   maskX, maskZ;
            float minX = positionTU.x - maskSizeTU;
            float maxX = positionTU.x + maskSizeTU;
            float minZ = positionTU.z - maskSizeTU;
            float maxZ = positionTU.z + maskSizeTU;
            Vector3 checkPos = m_worldBoundsTU.center;
            float strength;

            //Make the mask if height is below sea level
            for (float x = minX; x < maxX; x += 1f)
            {
                checkPos.x = x;
                for (float z = minZ; z < maxZ; z += 1f)
                {
                    checkPos.z = z;
                    if (InBoundsTU(checkPos))
                    {
                        if (GetHeightTU(checkPos) <= shoreHeightNU)
                        {
                            strength = GaiaCommon1.Utils.Math_Distance(x, z, positionTU.x, positionTU.z) / maskSizeTU;
                            if (strength <= 1f)
                            {
                                strength = 1f - strength;

                                maskX = (int)(x + m_TUZeroOffset.x);
                                maskZ = (int)(z + m_TUZeroOffset.z);
                                if (strength > waterMask[maskX, maskZ])
                                {
                                    waterMask[maskX, maskZ] = strength;
                                }
                            }
                        }
                    }
                }
            }
        }


        #endregion

        #region Unit Checks & Conversions

        /// <summary>
        /// Work out if in bounds in world units
        /// </summary>
        /// <param name="positionWU">Position to check</param>
        /// <returns>True if in bounds, false otherwise</returns>
        public bool InBoundsWU(Vector3 positionWU)
        {
            if ( 
                (positionWU.x >= m_worldBoundsWUMin.x) &&
                (positionWU.z >= m_worldBoundsWUMin.z) &&
                (positionWU.x < m_worldBoundsWUMax.x) &&
                (positionWU.z < m_worldBoundsWUMax.z) )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Work out if in bounds in terrain units
        /// </summary>
        /// <param name="positionTU">Position to check</param>
        /// <returns>True if in bounds, false otherwise</returns>
        public bool InBoundsTU(Vector3 positionTU)
        {
            if (
                (positionTU.x >= m_worldBoundsTUMin.x) &&
                (positionTU.z >= m_worldBoundsTUMin.z) &&
                (positionTU.x < m_worldBoundsTUMax.x) &&
                (positionTU.z < m_worldBoundsTUMax.z))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Work out if in bounds in nomral units
        /// </summary>
        /// <param name="positionNU">Position to check</param>
        /// <returns>True if in bounds, false otherwise</returns>
        public bool InBoundsNU(Vector3 positionNU)
        {
            if (
                (positionNU.x >= m_worldBoundsNUMin.x) &&
                (positionNU.z >= m_worldBoundsNUMin.z) &&
                (positionNU.x < m_worldBoundsNUMax.x) &&
                (positionNU.z < m_worldBoundsNUMax.z))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Convert world units to terrain units
        /// </summary>
        /// <param name="positionWU">World units</param>
        /// <returns>Terrain units</returns>
        public Vector3 WUtoTU(Vector3 positionWU)
        {
            return Vector3.Scale(positionWU, m_WUtoTU);
        }

        /// <summary>
        /// Convert world units to normal units
        /// </summary>
        /// <param name="positionWU">World units</param>
        /// <returns>Normal units</returns>
        public Vector3 WUtoNU(Vector3 positionWU)
        {
            return Vector3.Scale(positionWU, m_WUtoNU);
        }

        /// <summary>
        /// Convert world units to physical terrain index
        /// </summary>
        /// <param name="positionWU">World units</param>
        /// <returns>Physical terrain index</returns>
        public Vector3 WUtoPTI(Vector3 positionWU)
        {
            positionWU = WUtoNU(positionWU);
            NUtoPTI(ref positionWU);
            return positionWU;
        }

        /// <summary>
        /// Convert world units to physical terrain offfet
        /// </summary>
        /// <param name="positionWU">Terrain units</param>
        /// <returns>Physical terrain tile offset</returns>
        public Vector3 WUtoPTO(Vector3 positionWU)
        {
            positionWU = WUtoTU(positionWU);
            TUtoPTO(ref positionWU);
            return positionWU;
        }

        /// <summary>
        /// Convert terrain units to world units
        /// </summary>
        /// <param name="positionTU">Terrain units</param>
        /// <returns>World units</returns>
        public Vector3 TUtoWU(Vector3 positionTU)
        {
            return Vector3.Scale(positionTU, m_TUtoWU);
        }

        /// <summary>
        /// Convert terrian units to normal units
        /// </summary>
        /// <param name="positionTU">Terrain units</param>
        /// <returns>Normal units</returns>
        public Vector3 TUtoNU(Vector3 positionTU)
        {
            return Vector3.Scale(positionTU, m_TUtoNU);
        }

        /// <summary>
        /// Convert terrian units to physical terrain index
        /// </summary>
        /// <param name="positionTU">Terrain units</param>
        /// <returns>Physical terrain index</returns>
        public void TUtoPTI(ref Vector3 positionTU)
        {
            positionTU.x = (int)(positionTU.x + m_NUZeroOffset.x) * m_TUtoNU.x;
            positionTU.y = (int)(positionTU.y + m_NUZeroOffset.y) * m_TUtoNU.y;
            positionTU.z = (int)(positionTU.z + m_NUZeroOffset.z) * m_TUtoNU.z;
        }

        /// <summary>
        /// Convert terrian units to physical terrain offfet
        /// </summary>
        /// <param name="positionTU">Terrain units</param>
        /// <returns>Physical terrain tile offset</returns>
        public void TUtoPTO(ref Vector3 positionTU)
        {
            positionTU.x = ((positionTU.x + m_TUZeroOffset.x) % m_worldBoundsTUSize.x);
            positionTU.y = ((positionTU.y + m_TUZeroOffset.y) % m_worldBoundsTUSize.y);
            positionTU.z = ((positionTU.z + m_TUZeroOffset.z) % m_worldBoundsTUSize.z);
        }

        /// <summary>
        /// Convert normal units to world units
        /// </summary>
        /// <param name="positionNU">Normal units</param>
        /// <returns>World units</returns>
        public Vector3 NUtoWU(Vector3 positionNU)
        {
            return Vector3.Scale(positionNU, m_NUtoWU);
        }

        /// <summary>
        /// Convert terrain units to normal units
        /// </summary>
        /// <param name="sourceWU">Normal units</param>
        /// <returns>Terrain units</returns>
        public Vector3 NUtoTU(Vector3 positionNU)
        {
            return Vector3.Scale(positionNU, m_NUtoTU);
        }

        /// <summary>
        /// Convert normal units to physical tile index - used to access terrain tiles
        /// </summary>
        /// <param name="positionNU">Source in normal units</param>
        /// <returns>Terrain tile index</returns>
        public void NUtoPTI(ref Vector3 positionNU)
        {
            positionNU.x = Mathf.Floor(positionNU.x + m_NUZeroOffset.x);
            positionNU.y = Mathf.Floor(positionNU.y + m_NUZeroOffset.y);
            positionNU.z = Mathf.Floor(positionNU.z + m_NUZeroOffset.z);
        }

        /// <summary>
        /// Convert normal units to physical tile offset - used to access array within terrain tiles
        /// </summary>
        /// <param name="positionNU">Source in normal units</param>
        /// <returns>Terrain tile offset</returns>
        public void NUtoPTO(ref Vector3 positionNU)
        {
            positionNU.x = ((positionNU.x + m_NUZeroOffset.x) % 1f) * m_worldBoundsTUSize.x;
            positionNU.y = ((positionNU.y + m_NUZeroOffset.y) % 1f) * m_worldBoundsTUSize.y;
            positionNU.z = ((positionNU.z + m_NUZeroOffset.z) % 1f) * m_worldBoundsTUSize.z;
        }

        /// <summary>
        /// Convert the content values of the supplied vector to ceiling
        /// </summary>
        /// <param name="source">Source vector</param>
        /// <returns>Source as ceiling</returns>
        public Vector3 Ceil(Vector3 source)
        {
            return new Vector3(Mathf.Ceil(source.x), Mathf.Ceil(source.y), Mathf.Ceil(source.z));
        }

        /// <summary>
        /// Convert the content values of the supplied vector to floor
        /// </summary>
        /// <param name="source">Source vector</param>
        /// <returns>Source as ceiling</returns>
        public Vector3 Floor(Vector3 source)
        {
            return new Vector3(Mathf.Floor(source.x), Mathf.Floor(source.y), Mathf.Floor(source.z));
        }

        #endregion

        #region Test

        public void Test()
        {
            Vector3 position;
            StringBuilder sb = new StringBuilder();

            sb.Append("GaiaWorldManagerTest\n");
            sb.Append(string.Format("World Bounds WU : Min {0}, Centre {1}, Max {2}, Size {3}\n", m_worldBoundsWU.min, m_worldBoundsWU.center, m_worldBoundsWU.max, m_worldBoundsWU.size));
            sb.Append(string.Format("World Bounds TU : Min {0}, Centre {1}, Max {2}, Size {3}\n", m_worldBoundsTU.min, m_worldBoundsTU.center, m_worldBoundsTU.max, m_worldBoundsTU.size));
            sb.Append(string.Format("World Bounds NU : Min {0}, Centre {1}, Max {2}, Size {3}\n", m_worldBoundsNU.min, m_worldBoundsNU.center, m_worldBoundsNU.max, m_worldBoundsNU.size));

            sb.Append("\nBounds Tests:");
            position = new Vector3(m_worldBoundsWU.min.x - 1f, m_worldBoundsWU.min.y, m_worldBoundsWU.min.z);
            sb.Append(string.Format("\n<MIN - InBoundsWU({0}) = {1}\n", position, InBoundsWU(position)));
            position = new Vector3(m_worldBoundsWU.min.x, m_worldBoundsWU.min.y, m_worldBoundsWU.min.z);
            sb.Append(string.Format("  MIN - InBoundsWU({0}) = {1}\n", position, InBoundsWU(position)));
            position = new Vector3(m_worldBoundsWU.max.x, m_worldBoundsWU.max.y, m_worldBoundsWU.max.z);
            sb.Append(string.Format("  MAX - InBoundsWU({0}) = {1}\n", position, InBoundsWU(position)));
            position = new Vector3(m_worldBoundsWU.max.x + 1f, m_worldBoundsWU.max.y, m_worldBoundsWU.max.z);
            sb.Append(string.Format(">MAX - InBoundsWU({0}) = {1}\n", position, InBoundsWU(position)));

            position = new Vector3(m_worldBoundsTU.min.x - 1f, m_worldBoundsTU.min.y, m_worldBoundsTU.min.z);
            sb.Append(string.Format("\n<MIN - InBoundsTU({0}) = {1}\n", position, InBoundsTU(position)));
            position = new Vector3(m_worldBoundsTU.min.x, m_worldBoundsTU.min.y, m_worldBoundsTU.min.z);
            sb.Append(string.Format("  MIN - InBoundsTU({0}) = {1}\n", position, InBoundsTU(position)));
            position = new Vector3(m_worldBoundsTU.max.x, m_worldBoundsTU.max.y, m_worldBoundsTU.max.z);
            sb.Append(string.Format("  MAX - InBoundsTU({0}) = {1}\n", position, InBoundsTU(position)));
            position = new Vector3(m_worldBoundsTU.max.x + 1f, m_worldBoundsTU.max.y, m_worldBoundsTU.max.y);
            sb.Append(string.Format(">MAX - InBoundsTU({0}) = {1}\n", position, InBoundsTU(position)));

            position = new Vector3(m_worldBoundsNU.min.x - 0.1f, m_worldBoundsNU.min.y, m_worldBoundsNU.min.z);
            sb.Append(string.Format("\n<MIN - InBoundsNU({0}) = {1}\n", position, InBoundsNU(position)));
            position = new Vector3(m_worldBoundsNU.min.x, m_worldBoundsNU.min.y, m_worldBoundsNU.min.z);
            sb.Append(string.Format("  MIN - InBoundsNU({0}) = {1}\n", position, InBoundsNU(position)));
            position = new Vector3(m_worldBoundsNU.max.x, m_worldBoundsNU.max.y, m_worldBoundsNU.max.z);
            sb.Append(string.Format("  MAX - InBoundsNU({0}) = {1}\n", position, InBoundsNU(position)));
            position = new Vector3(m_worldBoundsNU.max.x + 0.1f, m_worldBoundsNU.max.y, m_worldBoundsNU.max.z);
            sb.Append(string.Format(">MAX - InBoundsNU({0}) = {1}\n", position, InBoundsNU(position)));

            sb.Append("\nPosition Conversion Tests (<MIN, CENTRE, >MAX):");
            position = new Vector3(m_worldBoundsWU.min.x - 1f, m_worldBoundsWU.center.y, m_worldBoundsWU.max.z + 1);
            sb.Append(string.Format("\nInBoundsWU({0}) = {1}\n", position, InBoundsWU(position)));
            sb.Append(string.Format("WUtoTU({0}) = {1:0.000}, {2:0.000}\n", position, WUtoTU(position).x, WUtoTU(position).z));
            sb.Append(string.Format("WUtoNU({0}) = {1:0.000}, {2:0.000}\n", position, WUtoNU(position).x, WUtoNU(position).z));
            sb.Append(string.Format("WUtoPTI({0}) = {1}, {2}\n", position, WUtoPTI(position).x, WUtoPTI(position).z));
            sb.Append(string.Format("WUtoPTO({0}) = {1}, {2}\n", position, WUtoPTO(position).x, WUtoPTO(position).z));

            sb.Append("\nPosition Conversion Tests (MIN, CENTRE, MAX):");
            position = new Vector3(m_worldBoundsWU.min.x, m_worldBoundsWU.center.y, m_worldBoundsWU.max.z);
            sb.Append(string.Format("\nInBoundsWU({0}) = {1}\n", position, InBoundsWU(position)));
            sb.Append(string.Format("WUtoTU({0}) = {1:0.000}, {2:0.000}\n", position, WUtoTU(position).x, WUtoTU(position).z));
            sb.Append(string.Format("WUtoNU({0}) = {1:0.000}, {2:0.000}\n", position, WUtoNU(position).x, WUtoNU(position).z));
            sb.Append(string.Format("WUtoPTI({0}) = {1}, {2}\n", position, WUtoPTI(position).x, WUtoPTI(position).z));
            sb.Append(string.Format("WUtoPTO({0}) = {1}, {2}\n", position, WUtoPTO(position).x, WUtoPTO(position).z));

            position = WUtoTU(position);
            sb.Append(string.Format("\nTUtoWU({0}) = {1}\n", position, TUtoWU(position)));
            sb.Append(string.Format("TUtoNU({0}) = {1}\n", position, TUtoNU(position)));
            position = TUtoNU(position);
            sb.Append(string.Format("\nNUtoWU({0}) = {1}\n", position, NUtoWU(position)));
            sb.Append(string.Format("NUtoTU({0}) = {1}\n", position, NUtoTU(position)));

            sb.Append("\nTerrain Tests:");

            //Clean out any old stuff
            FlattenWorld();

            //Now run up some new stuff
            m_boundsCheckErrors = 0;

            TestBlobWU(m_worldBoundsWU.min, 100, 0.25f);
            TestBlobTU(m_worldBoundsTU.center, 100, 0.5f);
            TestBlobWU(m_worldBoundsWU.max, 100, 1f);


            SaveToWorld();

            sb.Append(string.Format("Bounds check errors : {0}", m_boundsCheckErrors));

            Debug.Log(sb.ToString());
        }

        /// <summary>
        /// Create a cross in the terrain at the location centred on the WU provided
        /// </summary>
        /// <param name="positionWU"></param>
        public void TestBlobWU(Vector3 positionWU, int widthWU, float height)
        {
            Vector3 widthTU = WUtoTU(new Vector3(widthWU, widthWU, widthWU));
            Vector3 sourcePosTU = WUtoTU(positionWU);
            Vector3 posTU;

            for (int x = ((int)(sourcePosTU.x - widthTU.x)); x < (int)(sourcePosTU.x + widthTU.x); x++)
            {
                for (int z = (int)(sourcePosTU.z - widthTU.z); z < (int)(sourcePosTU.z + widthTU.z); z++)
                {
                    posTU = new Vector3(x, m_worldBoundsTU.center.y, z);
                    SetHeightTU(posTU, height);
                }
            }
        }

        /// <summary>
        /// Create a blob in the terrain at the location centred on the TU provided
        /// </summary>
        /// <param name="positionTU"></param>
        public void TestBlobTU(Vector3 positionTU, int widthWU, float height)
        {
            Vector3 widthTU = WUtoTU(new Vector3(widthWU, widthWU, widthWU));
            Vector3 posTU;

            for (int x = (int)(positionTU.x - widthTU.x); x < (int)(positionTU.x + widthTU.x); x++)
            {
                for (int z = (int)(positionTU.z - widthTU.z); z < (int)(positionTU.z + widthTU.z); z++)
                {
                    posTU = new Vector3(x, m_worldBoundsTU.center.y, z);
                    SetHeightTU(posTU, height);
                }
            }
        }

        #endregion
    }
}
