using UnityEngine;
using System.Collections;

namespace Gaia
{
    public class WaterFlowMap
    {
        public float m_dropletVolume = 0.3f;                  //How much water is deposited in each droplet   
        public float m_dropletAbsorbtionRate = 0.05f;        //The amount of water absorbed by the terrain as the droplet traverses it
        public int m_waterflowSmoothIterations = 1;         //The number of post process smooth iterations to apply
        private UnityHeightMap m_heightMap;
        private HeightMap m_waterFlowMap;

        /// <summary>
        /// Create a waterflow map from the terrain
        /// </summary>
        /// <param name="terrain">Terrain to create the waterflow map from</param>
        public void CreateWaterFlowMap(Terrain terrain)
        {
            m_heightMap = new UnityHeightMap(terrain);
            int width = m_heightMap.Width();
            int depth = m_heightMap.Depth();
            m_waterFlowMap = new HeightMap(width, depth);

            //Avoid edges for now
            for (int x = 1; x < (width - 1); x++)
            {
                for (int z = 1; z < (depth - 1); z++)
                {
                    TraceWaterFlow(x, z, width, depth);
                }
            }

            //Flip it so it matches the terrain
            m_waterFlowMap.Flip();

            //Smooth the water map
            m_waterFlowMap.Smooth(m_waterflowSmoothIterations);
        }

        /// <summary>
        /// Trace the water flow of one droplet
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startZ"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        void TraceWaterFlow(int startX, int startZ, int width, int height)
        {
            float dropletVolume = m_dropletVolume;
            int x = startX;
            int z = startZ;
            float currHeight, nextHeight;
            int nX, nZ, testX, testZ, nextX, nextZ;

            while (dropletVolume > 0f)
            {
                m_waterFlowMap[x, z] += m_dropletAbsorbtionRate;
                dropletVolume -= m_dropletAbsorbtionRate;

                //Work out next place to flow to
                nextHeight = currHeight = m_heightMap[x, z];
                nextX = x;
                nextZ = z;

                //Search around the droplet for the next lowest height
                for (nX = -1; nX < 2; nX++)
                {
                    for (nZ = -1; nZ < 2; nZ++)
                    {
                        testX = x + nX;
                        testZ = z + nZ;
                        if ((testX >= 0) && (testX < width)) //Check valid xcoordinate
                        {
                            if ((testZ >= 0) && (testZ < height))
                            {
                                if (m_heightMap[testX, testZ] < nextHeight)
                                {
                                    nextX = testX;
                                    nextZ = testZ;
                                    nextHeight = m_heightMap[testX, testZ];
                                }
                            }
                        }
                    }
                }

                //If same height, then pool - pooling affects the underlying height by raising it
                if (currHeight == nextHeight)
                {
                    m_heightMap[x, z] += m_dropletAbsorbtionRate;
                }

                //Otherwise move onto next location
                else
                {
                    x = nextX;
                    z = nextZ;
                }
            }
        }  

        /// <summary>
        /// Export the water map to the supplied path
        /// </summary>
        /// <param name="path"></param>
        public void ExportWaterMapToPath(string path)
        {
            Gaia.GaiaUtils.CompressToSingleChannelFileImage(m_waterFlowMap.Heights(), path, Gaia.GaiaConstants.fmtHmTextureFormat, true, false);
        }

        /*
        void AdjustTerrainTexture(int mountainIdx, int flowIdx, float[,] waterMap)
        {
            Terrain terrain = Terrain.activeTerrain;
            int waterMapSize = waterMap.GetLength(0);
            int splatSize = terrain.terrainData.alphamapResolution;
            int splatChannels = terrain.terrainData.splatPrototypes.Length;
            float[, ,] splatMaps = terrain.terrainData.GetAlphamaps(0, 0, splatSize, splatSize);

            //Check mountain index
            if (mountainIdx < 0 || mountainIdx >= splatChannels)
            {
                Debug.LogError(string.Format("Invalid mountain index {0} in {1} terrain", mountainIdx, splatChannels));
                return;
            }

            //Check flow index
            if (flowIdx < 0 || flowIdx >= splatChannels)
            {
                Debug.LogError(string.Format("Invalid flow index {0} in {1} terrain", flowIdx, splatChannels));
                return;
            }

            //Some handy params
            Debug.Log(string.Format("Splat size {0}, water map size {1}", splatSize, waterMapSize));
            float scaleFactor = (float)(waterMapSize - 1f) / (float)splatSize;
            float waterStrength, adjustment;

            //Apply the effect of water to mountains - this will give striations
            for (int x = 0; x < splatSize; x++)
            {
                for (int z = 0; z < splatSize; z++)
                {
                    waterStrength = waterMap[(int)(x * scaleFactor), (int)(z * scaleFactor)];
                    if (waterStrength > 1f)
                    {
                        waterStrength = 1f;
                    }

                    adjustment = waterStrength * splatMaps[x, z, mountainIdx];
                    splatMaps[x, z, mountainIdx] -= adjustment;
                    splatMaps[x, z, flowIdx] += adjustment;
                }
            }

            terrain.terrainData.SetAlphamaps(0, 0, splatMaps);
        }
        */
    }
}
