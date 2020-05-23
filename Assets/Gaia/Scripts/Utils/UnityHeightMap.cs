using UnityEngine;
using System.Collections;
using System.IO;

namespace Gaia
{
    /// <summary>
    /// Unity integration extension to the heightmap class
    /// </summary>
    public class UnityHeightMap : HeightMap
    {
        public Bounds m_boundsWU = new Bounds();

        /// <summary>
        /// Create a unity heightmap
        /// </summary>
        public UnityHeightMap() : base()
        {
        }

        /// <summary>
        /// Create a unity heightmap by loading from a source file
        /// </summary>
        /// <param name="path">Paht of file to load</param>
        public UnityHeightMap(string path) : base(path)
        {
            m_boundsWU.size = new Vector3(m_widthX, 0f, m_depthZ);
            m_isDirty = false;
        }

        /// <summary>
        /// Create a unity heightmap by loading a TextAsset
        /// </summary>
        /// <param name="source">The text asset to be loaded</param>
        public UnityHeightMap(TextAsset source) : base(source.bytes)
        {
            m_boundsWU.size = new Vector3(m_widthX, 0f, m_depthZ);
            m_isDirty = false;
        }

        /// <summary>
        /// Create a unity heightmap by replicating a source file
        /// </summary>
        /// <param name="source">Source heightmap</param>
        public UnityHeightMap(UnityHeightMap source) : base(source)
        {
            m_boundsWU = source.m_boundsWU;
            m_isDirty = false;
        }

        /// <summary>
        /// Create from terrain
        /// </summary>
        /// <param name="terrain"></param>
        public UnityHeightMap(Terrain terrain) : base()
        {
            LoadFromTerrain(terrain);
        }

        /// <summary>
        /// Create a heightmap by reading in and processing an image file
        /// </summary>
        /// <param name="bounds">Bounds in world units</param>
        /// <param name="sourceFile">Source file</param>
        /// <param name="width">Width</param>
        /// <param name="depth">Depth</param>
        public UnityHeightMap(Bounds bounds, string sourceFile) : base(sourceFile)
        {
            m_boundsWU = bounds;
            m_isDirty = false;
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="Gaia.UnityHeightMap"/> class from a texture.
		/// </summary>
		/// <param name="texture">Texture.</param>
		public UnityHeightMap(Texture2D texture) : base()
		{
			LoadFromTexture2D(texture);
            m_isDirty = false;
			//TODO - Work out what to do with bounds.
		}

        /// <summary>
        /// Get bounds in world units
        /// </summary>
        /// <returns>Terrain bounds in world units</returns>
        public Bounds GetBoundsWU()
        {
            return m_boundsWU;
        }

        /// <summary>
        /// Get position in world units
        /// </summary>
        /// <returns>Position in world units</returns>
        public Vector3 GetPositionWU()
        {
            Vector3 pos = m_boundsWU.center - m_boundsWU.extents;
            return pos;
        }

        /// <summary>
        /// Set the bounds in world units
        /// </summary>
        /// <param name="bounds"></param>
        public void SetBoundsWU(Bounds bounds)
        {
            m_boundsWU = bounds;
            m_isDirty = true;
        }

        /// <summary>
        /// Set the position in world units
        /// </summary>
        /// <param name="position"></param>
        public void SetPositionWU(Vector3 position)
        {
            m_boundsWU.center = position;
            m_isDirty = true;
        }

        /// <summary>
        /// Load this height map from the supplied terrain
        /// </summary>
        /// <param name="terrain">Terrain to load</param>
        public void LoadFromTerrain(Terrain terrain)
        {
            Reset();
            m_boundsWU.center = terrain.transform.position;
            m_boundsWU.size = terrain.terrainData.size;
            m_boundsWU.center += m_boundsWU.extents;
            m_widthX = terrain.terrainData.heightmapResolution;
            m_depthZ = terrain.terrainData.heightmapResolution;
            m_widthInvX = 1f / (float)(m_widthX);
            m_depthInvZ = 1f / (float)(m_depthZ);
            m_heights = terrain.terrainData.GetHeights(0, 0, m_widthX, m_depthZ);
            m_isPowerOf2 = Gaia.GaiaUtils.Math_IsPowerOf2(m_widthX) && Gaia.GaiaUtils.Math_IsPowerOf2(m_depthZ);
            m_isDirty = false;
        }

        /// <summary>
        /// Update the supplied terrain with the content of this heightmap - will scale if necessary
        /// </summary>
        /// <param name="terrain">Terrain to uppdate</param>
        public void SaveToTerrain(Terrain terrain)
        {
            //Get terrain stats
            int terWidth = terrain.terrainData.heightmapResolution;
            int terDepth = terrain.terrainData.heightmapResolution;

            //Direct one to one mapping
            if (m_widthX == terWidth && m_depthZ == terDepth)
            {
                terrain.terrainData.SetHeights(0, 0, m_heights);
                m_isDirty = false;
                return;
            }

            //Build new array and scale it to the size of the terrain
            float[,] heights = new float[terWidth, terDepth];
            for (int x = 0; x < terWidth; x++)
            {
                for (int z = 0; z < terDepth; z++)
                {
                    heights[x,z] = this[((float)x / (float)terWidth), ((float)z / (float)terDepth)];
                }
            }

            //And apply it
            terrain.terrainData.SetHeights(0, 0, heights);
            m_isDirty = false;
        }

		public void LoadFromTexture2D(Texture2D texture)
		{
            //Check if it is readable - if not then make it readable
            Gaia.GaiaUtils.MakeTextureReadable(texture);

            //Make sure its not commpressed
            Gaia.GaiaUtils.MakeTextureUncompressed(texture);

            //And load
			m_widthX = texture.width;
			m_depthZ = texture.height;
			m_widthInvX = 1f / (float)(m_widthX);
			m_depthInvZ = 1f / (float)(m_depthZ);
			m_heights = new float[m_widthX, m_depthZ];
			m_isPowerOf2 = Gaia.GaiaUtils.Math_IsPowerOf2(m_widthX) && Gaia.GaiaUtils.Math_IsPowerOf2(m_depthZ);

		    for (int x = 0; x < m_widthX; x++)
		    {
		        for (int z = 0; z < m_depthZ; z++)
		        {
		            m_heights[x, z] = texture.GetPixel(x, z).grayscale;
		        }
		    }

            m_isDirty = false;
		}

        /// <summary>
        /// Read heightmap from the supplied RAW file supplied as a text asset
        /// </summary>
        /// <returns>True on success</returns>
        public void ReadRawFromTextAsset(TextAsset asset)
        {
            using (Stream s = new MemoryStream(asset.bytes))
            {
                using (BinaryReader br = new BinaryReader(s))
                {
                    m_widthX = m_depthZ = Mathf.CeilToInt(Mathf.Sqrt(s.Length / 2));
                    m_widthInvX = 1f / (float)(m_widthX);
                    m_depthInvZ = 1f / (float)(m_depthZ);
                    m_heights = new float[m_widthX, m_depthZ];
                    m_isPowerOf2 = Gaia.GaiaUtils.Math_IsPowerOf2(m_widthX) && Gaia.GaiaUtils.Math_IsPowerOf2(m_depthZ);
                    for (int hmX = 0; hmX < m_widthX; hmX++)
                    {
                        for (int hmZ = 0; hmZ < m_depthZ; hmZ++)
                        {
                            m_heights[hmX, hmZ] = (float)br.ReadUInt16() / 65535.0f;
                        }
                    }
                }
                s.Close();
            }
            m_isDirty = false;
        }

        /// <summary>
        /// Calculate the normals for the heightmap
        /// </summary>
        /// <returns>Normals for the heightmap</returns>
        public Texture2D CalculateNormals()
        {
            float terrainHeight = m_widthX / 2f;
            int width = m_widthX;
            int height = m_depthZ;

            float ux = 1.0f / (width - 1.0f);
            float uy = 1.0f / (height - 1.0f);

            float scaleX = terrainHeight / (float)m_widthX;
            float scaleY = terrainHeight / (float)m_depthZ;

            float[] heights = Heights1D();

            Texture2D normalMap = new Texture2D(width, height, TextureFormat.RGBAFloat, false, true);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int xp1 = (x == width - 1) ? x : x + 1;
                    int xn1 = (x == 0) ? x : x - 1;

                    int yp1 = (y == height - 1) ? y : y + 1;
                    int yn1 = (y == 0) ? y : y - 1;

                    float l = heights[xn1 + y * width] * scaleX;
                    float r = heights[xp1 + y * width] * scaleX;

                    float b = heights[x + yn1 * width] * scaleY;
                    float t = heights[x + yp1 * width] * scaleY;

                    float dx = (r - l) / (2.0f * ux);
                    float dy = (t - b) / (2.0f * uy);

                    Vector3 normal;
                    normal.x = -dx;
                    normal.y = -dy;
                    normal.z = 1;
                    normal.Normalize();

                    Color pixel;
                    pixel.r = normal.x * 0.5f + 0.5f;
                    pixel.g = normal.y * 0.5f + 0.5f;
                    pixel.b = normal.z;
                    pixel.a = 1.0f;

                    normalMap.SetPixel(x, y, pixel);
                }

            }
            normalMap.Apply();
            return normalMap;
        }
    }
}