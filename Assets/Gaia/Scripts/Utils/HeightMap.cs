using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

namespace Gaia
{
    /// <summary>
    /// Utility class for managing unity heightmaps. Height maps have allowable value range of 0..1f.
    /// </summary>
    public class HeightMap
    {
        protected int m_widthX;
        protected int m_depthZ;
        protected float[,] m_heights;
        protected bool m_isPowerOf2;
        protected float m_widthInvX;
        protected float m_depthInvZ;
        protected float m_statMinVal;
        protected float m_statMaxVal;
        protected double m_statSumVals;
        protected bool m_isDirty;
        protected byte[] m_metaData = new byte[0];

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public HeightMap()
        {
            Reset();
        }

        /// <summary>
        /// Construct a heightmap with given width and height
        /// </summary>
        /// <param name="width">Width of constructed heightmap</param>
        /// <param name="height">Height of constructed heightmap</param>
		public HeightMap(int width, int depth) 
		{
            m_widthX = width;
            m_depthZ = depth;
            m_widthInvX = 1f / (float)(m_widthX);
            m_depthInvZ = 1f / (float)(m_depthZ);
            m_heights = new float[m_widthX, m_depthZ];
            m_isPowerOf2 = Gaia.GaiaUtils.Math_IsPowerOf2(m_widthX) && Gaia.GaiaUtils.Math_IsPowerOf2(m_depthZ);
            m_statMinVal = m_statMaxVal = 0f;
            m_statSumVals = 0;
            m_metaData = new byte[0];
            m_isDirty = false;
		}

        /// <summary>
        /// Create a heightmap from a float array
        /// </summary>
        /// <param name="source">Source array</param>
        public HeightMap(float[,] source)
        {
            m_widthX = source.GetLength(0);
            m_depthZ = source.GetLength(1);
            m_widthInvX = 1f / (float)(m_widthX);
            m_depthInvZ = 1f / (float)(m_depthZ);
            m_heights = new float[m_widthX, m_depthZ];
            m_isPowerOf2 = Gaia.GaiaUtils.Math_IsPowerOf2(m_widthX) && Gaia.GaiaUtils.Math_IsPowerOf2(m_depthZ);
            m_statMinVal = m_statMaxVal = 0f;
            m_statSumVals = 0;
            m_metaData = new byte[0];
            Buffer.BlockCopy(source, 0, m_heights, 0, m_widthX * m_depthZ * sizeof(float));
            m_isDirty = false;
        }

        /// <summary>
        /// Create a height mape from the particular slice passed in
        /// </summary>
        /// <param name="source">Height map arrays</param>
        /// <param name="slice">The slice to use</param>
        public HeightMap(float[,,] source, int slice)
        {
            m_widthX = source.GetLength(0);
            m_depthZ = source.GetLength(1);
            m_widthInvX = 1f / (float)(m_widthX);
            m_depthInvZ = 1f / (float)(m_depthZ);
            m_heights = new float[m_widthX, m_depthZ];
            m_isPowerOf2 = Gaia.GaiaUtils.Math_IsPowerOf2(m_widthX) && Gaia.GaiaUtils.Math_IsPowerOf2(m_depthZ);
            m_statMinVal = m_statMaxVal = 0f;
            m_statSumVals = 0;
            m_metaData = new byte[0];

            for (int x = 0; x < m_widthX; x++)
            {
                for (int z = 0; z < m_depthZ; z++)
                {
                    m_heights[x, z] = source[x, z, slice];
                }
            }

            m_isDirty = false;
        }

        /// <summary>
        /// Create a heightmap from an int array - beware of precision issues
        /// </summary>
        /// <param name="source">Source array</param>
        public HeightMap(int[,] source)
        {
            m_widthX = source.GetLength(0);
            m_depthZ = source.GetLength(1);
            m_widthInvX = 1f / (float)(m_widthX);
            m_depthInvZ = 1f / (float)(m_depthZ);
            m_heights = new float[m_widthX, m_depthZ];
            m_isPowerOf2 = Gaia.GaiaUtils.Math_IsPowerOf2(m_widthX) && Gaia.GaiaUtils.Math_IsPowerOf2(m_depthZ);
            m_statMinVal = m_statMaxVal = 0f;
            m_statSumVals = 0;
            m_metaData = new byte[0];

            for (int x = 0; x < m_widthX; x++)
            {
                for (int z = 0; z < m_depthZ; z++)
                {
                    m_heights[x, z] = (float)source[x, z];
                }
            }

            m_isDirty = false;
        }

        /// <summary>
        /// Create a height map that is a copy of another heightmap
        /// </summary>
        /// <param name="source">Source heightmap</param>
        public HeightMap(HeightMap source)
        {
            Reset();
            m_widthX = source.m_widthX;
            m_depthZ = source.m_depthZ;
            m_widthInvX = 1f / (float)(m_widthX);
            m_depthInvZ = 1f / (float)(m_depthZ);
            m_heights = new float[m_widthX, m_depthZ];
            m_isPowerOf2 = source.m_isPowerOf2;
            SetMetaData(source.m_metaData);
            Buffer.BlockCopy(source.m_heights, 0, m_heights, 0, m_widthX * m_depthZ * sizeof(float));
            m_isDirty = false;
        }

        /// <summary>
        /// Create a heightmap by reading in and processing a binary file
        /// </summary>
        /// <param name="sourceFile">File to read in</param>
        public HeightMap(string sourceFile)
        {
            Reset();
            LoadFromBinaryFile(sourceFile);
            m_isDirty = false;
        }

        /// <summary>
        /// Create a heightmap by reading in and processing the byte array
        /// </summary>
        /// <param name="sourceBytes">Source as a byte array</param>
        public HeightMap(byte[] sourceBytes)
        {
            Reset();
            LoadFromByteArray(sourceBytes);
            m_isDirty = false;
        }

        #endregion 

        #region Data access

        /// <summary>
        /// Get width of the height map (x component)
        /// </summary>
        /// <returns>Height map width</returns>
        public int Width()
        {
            return m_widthX;
        }

        /// <summary>
        /// Get depth or height of the height map (z component)
        /// </summary>
        /// <returns>Height map depth</returns>
        public int Depth()
        {
            return m_depthZ;
        }

        /// <summary>
        /// Get min value - need to call update stats before calling this
        /// </summary>
        /// <returns>Min value</returns>
        public float MinVal()
        {
            return m_statMinVal;
        }

        /// <summary>
        /// Get max value - need to call update stats before calling this
        /// </summary>
        /// <returns>Max value</returns>
        public float MaxVal()
        {
            return m_statMaxVal;
        }

        /// <summary>
        /// Get sum of values - need to call update stats before calling this
        /// </summary>
        /// <returns>Sum of values</returns>
        public double SumVal()
        {
            return m_statSumVals;
        }

        /// <summary>
        /// Get metadata
        /// </summary>
        /// <returns></returns>
        public byte[] GetMetaData()
        {
            return m_metaData;
        }

        /// <summary>
        /// Get dirty flag ie we have been modified
        /// </summary>
        /// <returns></returns>
        public bool IsDirty()
        {
            return m_isDirty;
        }

        /// <summary>
        /// Set dirty flag
        /// </summary>
        /// <param name="dirty"></param>
        /// <returns></returns>
        public void SetDirty(bool dirty = true)
        {
            m_isDirty = dirty;
        }

        /// <summary>
        /// Clear the dirty flag
        /// </summary>
        public void ClearDirty()
        {
            m_isDirty = false;
        }

        /// <summary>
        /// Set metadata
        /// </summary>
        /// <param name="metadata">The metadata to set</param>
        public void SetMetaData(byte[] metadata)
        {
            m_metaData = new byte[metadata.Length];
            Buffer.BlockCopy(metadata, 0, m_metaData, 0, metadata.Length);
            m_isDirty = true;
        }

        /// <summary>
        /// Get height map heights
        /// </summary>
        /// <returns>Height map heights</returns>
        public float[,] Heights()
        {
            return  m_heights;
        }

        /// <summary>
        /// Get the heights as a 1D Array
        /// </summary>
        /// <returns>Heights as a 1D array</returns>
        public float[] Heights1D()
        {
            float[] result = new float[m_widthX * m_depthZ];
            Buffer.BlockCopy(m_heights, 0, result, 0, result.Length * sizeof(float));
            return result;
        }

        /// <summary>
        /// Set the array from the content of the supplied 1D array = assume its set up to be correct length
        /// </summary>
        /// <param name="heights"></param>
        public void SetHeights(float[] heights)
        {
            int size = (int)Mathf.Sqrt((float)heights.Length);
            if (size != m_widthX || size != m_depthZ)
            {
                Debug.LogError("SetHeights: Heights do not match. Aborting.");
                return;
            }
            Buffer.BlockCopy(heights, 0, m_heights, 0, heights.Length * sizeof(float));
            m_isDirty = true;
        }

        /// <summary>
        /// Copy the content of the supplied array into this array
        /// </summary>
        /// <param name="heights"></param>
        public void SetHeights(float[,] heights)
        {
            if (m_widthX != heights.GetLength(0) || m_depthZ != heights.GetLength(1))
            {
                Debug.LogError("SetHeights: Sizes do not match. Aborting.");
                return;
            }
            int size = heights.GetLength(0) * heights.GetLength(1);
            Buffer.BlockCopy(heights, 0, m_heights, 0, size * sizeof(float));
            m_isDirty = true;
        }

        /// <summary>
        /// Get height at the given location. If out of bounds will return nearest border.
        /// </summary>
        /// <param name="x">x location</param>
        /// <param name="z">z location</param>
        /// <returns></returns>
        public float GetSafeHeight(int x, int z)
        {
            if (x < 0) x = 0;
            if (z < 0) z = 0;
            if (x >= m_widthX) x = m_widthX - 1;
            if (z >= m_depthZ) z = m_depthZ - 1;
            return m_heights[x, z];
        }

        /// <summary>
        /// Set height at the given location. If out of bounds will set at nearest border.
        /// </summary>
        /// <param name="x">x location</param>
        /// <param name="z">z location</param>
        /// <returns></returns>
        public void SetSafeHeight(int x, int z, float height)
        {
            if (x < 0) x = 0;
            if (z < 0) z = 0;
            if (x >= m_widthX) x = m_widthX - 1;
            if (z >= m_depthZ) z = m_depthZ - 1;
            m_heights[x, z] = height;
            m_isDirty = true;
        }

        /// <summary>
        /// Get the interpolated height at the given location
        /// </summary>
        /// <param name="x">x location, range 0..1</param>
        /// <param name="z">z location, range 0..1</param>
        /// <returns>Interpolated height</returns>
        protected float GetInterpolatedHeight(float x, float z)
        {
            //Scale it
            x *= m_widthX;
            z *= m_depthZ;

            //Convert and handle the '1.0' scenario
            int xR0 = (int)(x);
            if (xR0 == m_widthX)
            {
                xR0 = m_widthX - 1;
            }
            int zR0 = (int)(z);
            if (zR0 == m_depthZ)
            {
                zR0 = m_depthZ - 1;
            }

            int xR1 = xR0 + 1;
            int zR1 = zR0 + 1;
            if (xR1 == m_widthX)
            {
                xR1 = xR0;
            }
            if (zR1 == m_depthZ)
            {
                zR1 = zR0;
            }
            float dx = x - xR0;
            float dz = z - zR0;
            float omdx = 1f - dx;
            float omdz = 1f - dz;
            return omdx * omdz * m_heights[xR0, zR0] +
                      omdx * dz * m_heights[xR0, zR1] +
                      dx * omdz * m_heights[xR1, zR0] +
                      dx * dz * m_heights[xR1, zR1];
        }

        /// <summary>
        /// Get and set the height at the given location
        /// </summary>
        /// <param name="x">x location</param>
        /// <param name="z">z location</param>
        /// <returns>Height at that location</returns>
        public float this[int x, int z]
        {
            get
            {
                return m_heights[x, z];
            }
            set
            {
                m_heights[x, z] = value;
                m_isDirty = true;
            }
        }

        /// <summary>
        /// Get and set the height at the location
        /// </summary>
        /// <param name="x">x location in 0..1f</param>
        /// <param name="z">z location in 0..1f</param>
        /// <returns>Height at that location</returns>
        public float this[float x, float z]
        {
            get
            {
                return GetInterpolatedHeight(x, z);
            }
            set
            {
                //Convert and handle the '1.0' scenario
                x *= m_widthX;
                z *= m_depthZ;

                int xR = (int)(x);
                if (xR == m_widthX)
                {
                    xR = m_widthX-1;
                }
                int zR = (int)(z);
                if (zR == m_depthZ)
                {
                    zR = m_depthZ - 1;
                }
                m_heights[xR, zR] = value;
                m_isDirty = true;
            }
        }

        /// <summary>
        /// Set the level of the entire map to the supplied value
        /// </summary>
        /// <returns>This</returns>
        public HeightMap SetHeight(float height)
        {
            float newLevel = Gaia.GaiaUtils.Math_Clamp(0f, 1f, height);
            for (int hmX = 0; hmX < m_widthX; hmX++)
            {
                for (int hmZ = 0; hmZ < m_depthZ; hmZ++)
                {
                    m_heights[hmX, hmZ] = newLevel;
                }
            }
            m_isDirty = true;
            return this;
        }

        /// <summary>
        /// Get the height rnage for this map
        /// </summary>
        /// <param name="minHeight">Minimum height</param>
        /// <param name="maxHeight">Maximum height</param>
        public void GetHeightRange(ref float minHeight, ref float maxHeight)
        {
            float currHeight;
            maxHeight = float.MinValue;
            minHeight = float.MaxValue;
            for (int hmX = 0; hmX < m_widthX; hmX++)
            {
                for (int hmZ = 0; hmZ < m_depthZ; hmZ++)
                {
                    currHeight = m_heights[hmX, hmZ];
                    if (currHeight > maxHeight)
                    {
                        maxHeight = currHeight;
                    }
                    if (currHeight < minHeight)
                    {
                        minHeight = currHeight;
                    }
                }
            }
        }

        /// <summary>
        /// Get the slope at the designated location
        /// </summary>
        /// <param name="x">x location</param>
        /// <param name="z">z location</param>
        /// <returns>Steepness at tha location</returns>
        public float GetSlope(int x, int z)
        {
            float height = m_heights[x, z];

            // Compute the differentials by stepping 1 in both directions.
            float dx = m_heights[x + 1, z] - height;
            float dy = m_heights[x, z + 1] - height;

            // The "steepness" is the magnitude of the gradient vector, 
            // For a faster but not as accurate computation, you can just use abs(dx) + abs(dy)
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Get the slope at the designated location
        /// </summary>
        /// <param name="x">x location in range 0..1</param>
        /// <param name="z">z location in range 0..1</param>
        /// <returns>Steepness</returns>
        public float GetSlope(float x, float z)
        {
            float dX = (GetInterpolatedHeight(x + (m_widthInvX * 0.9f), z) - GetInterpolatedHeight(x - (m_widthInvX * 0.9f), z));
            float dZ = (GetInterpolatedHeight(x, z + (m_depthInvZ * 0.9f)) - GetInterpolatedHeight(x, (z - m_depthInvZ * 0.9f)));
            //float direction = (float)Math.Atan2(deltaZ, deltaX);
            return Gaia.GaiaUtils.Math_Clamp(0f, 90f, (float)(Math.Sqrt((dX * dX) + (dZ * dZ)) * 10000));
        }

        /// <summary>
        /// Get the slope at the designated location
        /// </summary>
        /// <param name="x">x location in range 0..1</param>
        /// <param name="z">z location in range 0..1</param>
        /// <returns>Steepness</returns>
        public float GetSlope_a(float x, float z)
        {
            float center = GetInterpolatedHeight(x, z);
            float dTop = Math.Abs(GetInterpolatedHeight(x - m_widthInvX, z) - center);
            float dBot = Math.Abs(GetInterpolatedHeight(x + m_widthInvX, z) - center);
            float dLeft = Math.Abs(GetInterpolatedHeight(x, z - m_depthInvZ) - center);
            float dRight = Math.Abs(GetInterpolatedHeight(x, z + m_depthInvZ) - center);
            return ((dTop + dBot + dLeft + dRight) / 4f) * 400f;
        }

        /// <summary>
        /// Get the highest point around the edges of the heightmap - this is used as base level by scanner
        /// </summary>
        /// <returns>Base level</returns>
        public float GetBaseLevel()
        {
            float baseLevel = 0f;

            for (int x = 0; x < m_widthX; x++)
            {
                if (m_heights[x, 0] > baseLevel)
                {
                    baseLevel = m_heights[x, 0];
                }
                if (m_heights[x, m_depthZ-1] > baseLevel)
                {
                    baseLevel = m_heights[x, m_depthZ - 1];
                }
            }

            for (int z = 0; z < m_depthZ; z++)
            {
                if (m_heights[0, z] > baseLevel)
                {
                    baseLevel = m_heights[0, z];
                }
                if (m_heights[m_widthX-1, z] > baseLevel)
                {
                    baseLevel = m_heights[m_widthX - 1, z];
                }
            }

            return baseLevel;
        }

        /// <summary>
        /// Return true if we have data, false otherwise
        /// </summary>
        /// <returns>True if we have data, false otehrwise</returns>
        public bool HasData()
        {
            if (m_widthX <= 0 || m_depthZ <= 0)
            {
                return false;
            }
            if (m_heights == null)
            {
                return false;
            }
            if (m_heights.GetLength(0) != m_widthX || m_heights.GetLength(1) != m_depthZ)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get a specific row
        /// </summary>
        /// <param name="rowX">Row to get</param>
        /// <returns>Content of the row</returns>
        public float[] GetRow(int rowX)
        {
            float [] row = new float[m_depthZ];
            for (int z = 0; z < m_depthZ; z++)
            {
                row[z] = m_heights[rowX, z];
            }
            return row;
        }

        /// <summary>
        /// Set the content of a specific row
        /// </summary>
        /// <param name="rowX">Row to set</param>
        /// <param name="values">Values to set</param>
        public void SetRow(int rowX, float [] values)
        {
            for (int z = 0; z < m_depthZ; z++)
            {
                m_heights[rowX, z] = values[z];
            }
        }

        /// <summary>
        /// Get a specific column
        /// </summary>
        /// <param name="columnZ">Column to get</param>
        /// <returns>Content of the column</returns>
        public float[] GetColumn(int columnZ)
        {
            float[] col = new float[m_widthX];
            for (int x = 0; x < m_widthX; x++)
            {
                col[x] = m_heights[x, columnZ];
            }
            return col;
        }

        /// <summary>
        /// Set the content of a specific column
        /// </summary>
        /// <param name="columnZ">Column to set</param>
        /// <param name="values">Values to set</param>
        public void SetColumn(int columnZ, float[] values)
        {
            for (int x = 0; x< m_widthX; x++)
            {
                m_heights[x, columnZ] = values[x];
            }
        }

        #endregion

        #region Operations

        /// <summary>
        /// Reset the heightmap including all stats
        /// </summary>
        public void Reset()
        {
            m_widthX = m_depthZ = 0;
            m_widthInvX = m_depthInvZ = 0f;
            m_heights = null;
            m_statMinVal = m_statMaxVal = 0f;
            m_statSumVals = 0;
            m_metaData = new byte[0];
            m_heights = new float[0, 0];
            m_isDirty = false;
        }

        /// <summary>
        /// Update heightmap stats
        /// </summary>
        public void UpdateStats()
        {
            m_statMinVal = 1f;
            m_statMaxVal = 0f;
            m_statSumVals = 0;
            float height = 0f;
            for (int hmX = 0; hmX < m_widthX; hmX++)
            {
                for (int hmZ = 0; hmZ < m_depthZ; hmZ++)
                {
                    height = m_heights[hmX, hmZ];
                    if (height < m_statMinVal)
                    {
                        m_statMinVal = height;
                    }
                    if (height > m_statMaxVal)
                    {
                        m_statMaxVal = height;
                    }
                    m_statSumVals += height;
                }
            }
        }

        /// <summary>
        /// Smooth the height map
        /// </summary>
        /// <param name="iterations">Number of iterations of smoothing to run</param>
        /// <returns>This</returns>
        public HeightMap Smooth(int iterations)
        {
            for (int i = 0; i < iterations; i++ )
            {
                for (int x = 0; x < m_widthX; x++)
                {
                    for (int z = 0; z < m_depthZ; z++)
                    {
                        m_heights[x, z] = Gaia.GaiaUtils.Math_Clamp(0f, 1f, (GetSafeHeight(x - 1, z) + GetSafeHeight(x + 1, z) + GetSafeHeight(x, z - 1) + GetSafeHeight(x, z + 1)) / 4f);
                    }
                }
            }
            m_isDirty = true;
            return this;
        }

        /// <summary>
        /// Smooth in a given radius
        /// </summary>
        /// <param name="radius">Smoothing radius</param>
        /// <returns>This</returns>
        public HeightMap SmoothRadius(int radius)
        {
            radius = Mathf.Max(5, radius);
            HeightMap filter = new HeightMap(m_widthX, m_depthZ);
            float factor = 1f / ((2 * radius + 1) * (2 * radius + 1));
            for (int y = 0; y < m_depthZ; y++)
            {
                for (int x = 0; x < m_widthX; x++)
                {
                    filter[x, y] = factor * m_heights[x,y];
                }
            }
            for (int x = radius; x < m_widthX - radius; x++)
            {
                int y = radius;
                float sum = 0f;
                for (int i = -radius; i < radius + 1; i++)
                {
                    for (int j = -radius; j < radius + 1; j++)
                    {
                        sum += filter[x + j, y + i];
                    }
                }
                for (y++; y < m_depthZ - radius; y++)
                {
                    for (int j = -radius; j < radius + 1; j++)
                    {
                        sum -= filter[x + j, y - radius - 1];
                        sum += filter[x + j, y + radius];
                    }
                    m_heights[x, y] = sum;
                }
            }
            m_isDirty = true;
            return this;
        }

        /// <summary>
        /// Applies the kernel to the array
        /// </summary>
        /// <param name="kernel">The kernel to apply</param>
        /// <returns>Convolved array</returns>
        public HeightMap Convolve(float[,] kernel)
        {
            int xr, yj;
            float a, d, divisor = 0f;
            int radius = Mathf.FloorToInt(kernel.GetLength(0) / 2f);
            int kernelWidth = kernel.GetLength(0);
            int kernelHeight = kernel.GetLength(1);
            int kernelSize = kernelWidth * kernelHeight;
            int processedKernelSize = 0;

            //Calculate the divisor
            for (int r = 0; r < kernelWidth; r++)
            {
                for (int j = 0; j < kernelHeight; j++)
                {
                    divisor += kernel[r, j];
                }
            }
            if (Gaia.GaiaUtils.Math_ApproximatelyEqual(divisor, 0f))
            {
                divisor = 1f;
            }

            for (int x = 0; x < m_widthX; x++)
            {
                for (int y = 0; y < m_depthZ; y++)
                {
                    a = 0f;
                    d = 0f;
                    processedKernelSize = 0;
                    for (int r = -radius; r <= radius; r++)
                    {
                        xr = x + r;
                        if (xr < 0 || xr >= m_widthX)
                        {
                            continue;
                        }

                        for (int j = -radius; j <= radius; j++)
                        {
                            yj = y + j;
                            if (yj < 0 || yj >= m_depthZ)
                            {
                                continue;
                            }

                            float k = kernel[r + radius, j + radius];
                            d += k;
                            a += (m_heights[xr, yj] * k);
                            processedKernelSize++;
                        }
                    }

                    if (processedKernelSize != kernelSize)
                    {
//                        if (!PWCommon.Utils.Math_ApproximatelyEqual(d, 0f))
//                        {
//                            m_heights[x, y] = a / d;
//                        }
//                        else
//                        {
//                            m_heights[x, y] = a / divisor;
//                        }
                    }
                    else
                    {
                        m_heights[x, y] = Mathf.Clamp01(a / divisor);
                    }
                }
            }
            m_isDirty = true;
            return this;
        }

        /// <summary>
        /// Denoises the array - removes small lows and highs - becomes less sensitive the greater 
        /// the radius is - 1 is generlly good usage.
        /// </summary>
        /// <param name="radius">Radius of the denoise check</param>
        /// <returns>Denoised array</returns>
        public HeightMap DeNoise(int radius)
        {
            float min, max, v = 0f;
            for (int x = radius; x < m_widthX - radius; x++)
            {
                for (int y = radius; y < m_depthZ - radius; y++)
                {
                    min = float.MaxValue;
                    max = float.MinValue;
                    for (int r = -radius; r <= radius; r++)
                    {
                        for (int j = -radius; j <= radius; j++)
                        {
                            if (!(r == 0 && j == 0))
                            {
                                v = m_heights[x + r, y + j];
                                if (v < min)
                                {
                                    min = v;
                                }
                                if (v > max)
                                {
                                    max = v;
                                }
                            }
                        }
                    }
                    v = m_heights[x, y];
                    if (v > max)
                    {
                        m_heights[x, y] = max;
                    }
                    else if (v < min)
                    {
                        m_heights[x, y] = min;
                    }
                }
            }
            m_isDirty = true;
            return this;
        }

        /// <summary>
        /// Grows the features in the array
        /// </summary>
        /// <param name="radius">Radius of the growth check</param>
        /// <returns>Grown array</returns>
        public HeightMap GrowEdges(int radius)
        {
            int xr, yj;
            float min, max, v = 0f;
            for (int x = 0; x < m_widthX; x++)
            {
                for (int y = 0; y < m_depthZ; y++)
                {
                    min = float.MaxValue;
                    max = float.MinValue;
                    for (int r = -radius; r <= radius; r++)
                    {
                        for (int j = -radius; j <= radius; j++)
                        {
                            xr = x + r;
                            if (xr < 0 || xr >= m_widthX)
                            {
                                continue;
                            }

                            if (!(r == 0 && j == 0))
                            {
                                yj = y + j;
                                if (yj < 0 || yj >= m_depthZ)
                                {
                                    continue;
                                }

                                v = m_heights[xr, yj];
                                if (v < min)
                                {
                                    min = v;
                                }
                                if (v > max)
                                {
                                    max = v;
                                }
                            }
                        }
                    }
                    v = m_heights[x, y];
                    if (max > v)
                    {
                        m_heights[x, y] = (max + v) / 2f;
                    }
                }
            }
            m_isDirty = true;
            return this;
        }

        /// <summary>
        /// Shrinks the features in the array - also makes nice erosion effect
        /// </summary>
        /// <param name="radius">Radius of the shrink check</param>
        /// <returns>Shrunk array</returns>
        public HeightMap ShrinkEdges(int radius)
        {
            int xr, yj;
            float min, max, v = 0f;
            for (int x = 0; x < m_widthX; x++)
            {
                for (int y = 0; y < m_depthZ; y++)
                {
                    min = float.MaxValue;
                    max = float.MinValue;
                    for (int r = -radius; r <= radius; r++)
                    {
                        xr = x + r;
                        if (xr < 0 || xr == m_widthX)
                        {
                            continue;
                        }
                        for (int j = -radius; j <= radius; j++)
                        {
                            if (!(r == 0 && j == 0))
                            {
                                yj = y + j;
                                if (yj < 0 || yj >= m_depthZ)
                                {
                                    continue;
                                }

                                v = m_heights[xr, yj];
                                if (v < min)
                                {
                                    min = v;
                                }
                                if (v > max)
                                {
                                    max = v;
                                }
                            }
                        }
                    }
                    v = m_heights[x, y];
                    if (min < v)
                    {
                        m_heights[x, y] = (min + v) / 2f;
                    }
                }
            }
            m_isDirty = true;
            return this;
        }

        /// <summary>
        /// Return a new heightmap where each point at contains the slopes of this heightmap at that point
        /// </summary>
        /// <returns></returns>
        public HeightMap GetSlopeMap()
        {
            HeightMap slopeMap = new HeightMap(this);

            for (int x = 0; x < m_widthX; x++)
            {
                for (int z = 0; z < m_depthZ; z++)
                {
                    slopeMap[x, z] = GetSlope(x, z);
                }
            }

            return slopeMap;
        }

        public enum CopyType {  AlwaysCopy, CopyIfLessThan, CopyIfGreaterThan}

        /// <summary>
        /// Copy the supplied heightmap
        /// </summary>
        /// <param name="heightMap">Heightmap to copy</param>
        /// <param name="copyType">Always - always copy, CopyIfLessThan - copy new value if less than current value, CopyIfGreaterThan - copy new value if greater than current value.</param>
        /// /// <returns>This</returns>
        public HeightMap Copy(HeightMap heightMap, CopyType copyType = CopyType.AlwaysCopy)
        {
            if (copyType == CopyType.AlwaysCopy)
            {
                if (m_widthX != heightMap.m_widthX || m_depthZ != heightMap.m_depthZ)
                {
                    for (int x = 0; x < m_widthX; x++)
                    {
                        for (int z = 0; z < m_depthZ; z++)
                        {
                            m_heights[x, z] = heightMap[m_widthInvX * x, m_depthInvZ * z];
                        }
                    }
                }
                else
                {
                    for (int x = 0; x < m_widthX; x++)
                    {
                        for (int z = 0; z < m_depthZ; z++)
                        {
                            m_heights[x, z] = heightMap.m_heights[x, z];
                        }
                    }
                }
            }
            else if (copyType == CopyType.CopyIfLessThan)
            {
                if (m_widthX != heightMap.m_widthX || m_depthZ != heightMap.m_depthZ)
                {
                    for (int x = 0; x < m_widthX; x++)
                    {
                        for (int z = 0; z < m_depthZ; z++)
                        {
                            float h0 = m_heights[x, z];
                            float h1 = heightMap[m_widthInvX * x, m_depthInvZ * z];
                            if (h1 < h0)
                            {
                                m_heights[x, z] = h1;
                            }
                        }
                    }
                }
                else
                {
                    for (int x = 0; x < m_widthX; x++)
                    {
                        for (int z = 0; z < m_depthZ; z++)
                        {
                            float h0 = m_heights[x, z];
                            float h1 = heightMap[x, z];
                            if (h1 < h0)
                            {
                                m_heights[x, z] = h1;
                            }
                        }
                    }
                }
            }
            else if (copyType == CopyType.CopyIfGreaterThan)
            {
                if (m_widthX != heightMap.m_widthX || m_depthZ != heightMap.m_depthZ)
                {
                    for (int x = 0; x < m_widthX; x++)
                    {
                        for (int z = 0; z < m_depthZ; z++)
                        {
                            float h0 = m_heights[x, z];
                            float h1 = heightMap[m_widthInvX * x, m_depthInvZ * z];
                            if (h1 > h0)
                            {
                                m_heights[x, z] = h1;
                            }
                        }
                    }
                }
                else
                {
                    for (int x = 0; x < m_widthX; x++)
                    {
                        for (int z = 0; z < m_depthZ; z++)
                        {
                            float h0 = m_heights[x, z];
                            float h1 = heightMap[x, z];
                            if (h1 > h0)
                            {
                                m_heights[x, z] = h1;
                            }
                        }
                    }
                }
            }
            m_isDirty = true;
            return this;
        }

        /// <summary>
        /// Copy the source heightmap and clamp it
        /// </summary>
        /// <param name="heightMap">Heightmap to copy</param>
        /// <param name="min">Min value to clamp it to</param>
        /// <param name="max">Max value to clamp it to</param>
        /// <returns>This</returns>
        public HeightMap CopyClamped(HeightMap heightMap, float min, float max)
        {
            if (m_widthX != heightMap.m_widthX || m_depthZ != heightMap.m_depthZ)
            {
                for (int x = 0; x < m_widthX; x++)
                {
                    for (int z = 0; z < m_depthZ; z++)
                    {
                        float newValue = heightMap[m_widthInvX * x, m_depthInvZ * z];
                        if (newValue < min)
                        {
                            newValue = min;
                        }
                        else if (newValue > max)
                        {
                            newValue = max;
                        }
                        m_heights[x, z] = newValue;
                    }
                }
            }
            else
            {
                for (int x = 0; x < m_widthX; x++)
                {
                    for (int z = 0; z < m_depthZ; z++)
                    {
                        float newValue = heightMap.m_heights[x, z];
                        if (newValue < min)
                        {
                            newValue = min;
                        }
                        else if (newValue > max)
                        {
                            newValue = max;
                        }
                        m_heights[x, z] = newValue;
                    }
                }
            }
            m_isDirty = true;
            return this;
        }

        /// <summary>
        /// Duplicate this heightmap and return a new onw
        /// </summary>
        /// <returns>A new heightmap which is a direct duplicate of this one</returns>
        public HeightMap Duplicate()
        {
            return new HeightMap(this);
        }

        /// <summary>
        /// Invert the heightmap
        /// </summary>
        /// <returns>This</returns>
		public HeightMap Invert()
		{
			for (int x = 0; x < m_widthX; x++)
			{
				for (int z = 0; z < m_depthZ; z++)
				{
					m_heights[x, z] = 1f - m_heights[x, z];
				}
			}
            m_isDirty = true;
		    return this;
		}

        /// <summary>
        /// Flip the heightmap
        /// </summary>
        /// <returns>This</returns>
        public HeightMap Flip()
        {
            float[,] heights = new float[m_depthZ, m_widthX];
            for (int x = 0; x < m_widthX; x++)
            {
                for (int z = 0; z < m_depthZ; z++)
                {
                    heights[z, x] = m_heights[x, z];
                }
            }
            m_heights = heights;
            m_widthX = heights.GetLength(0);
            m_depthZ = heights.GetLength(1);
            m_widthInvX = 1f / (float)(m_widthX);
            m_depthInvZ = 1f / (float)(m_depthZ);
            m_isPowerOf2 = Gaia.GaiaUtils.Math_IsPowerOf2(m_widthX) && Gaia.GaiaUtils.Math_IsPowerOf2(m_depthZ);
            m_statMinVal = m_statMaxVal = 0f;
            m_statSumVals = 0;
            m_isDirty = true;
            return this;
        }

        /// <summary>
        /// Normalise the heightmap
        /// </summary>
        /// <returns>This</returns>
        public HeightMap Normalise()
        {
            float height;
            float maxHeight = float.MinValue;
            float minHeight = float.MaxValue;
            for (int x = 0; x < m_widthX; x++)
            {
                for (int z = 0; z < m_depthZ; z++)
                {
                    height = m_heights[x, z];
                    if (height > maxHeight)
                    {
                        maxHeight = height;
                    }
                    if (height < minHeight)
                    {
                        minHeight = height;
                    }
                }
            }
            float heightRange = maxHeight - minHeight;
            if (heightRange > 0f)
            {
                for (int x = 0; x < m_widthX; x++)
                {
                    for (int z = 0; z < m_depthZ; z++)
                    {
                        m_heights[x, z] = (m_heights[x, z] - minHeight) / heightRange;
                    }
                }
                m_isDirty = true;
            }
            return this;
        }

        /*
        /// <summary>
        /// Thermally erode the heightmap
        /// </summary>
        /// <param name="min_threshold"></param>
        /// <param name="max_threshold"></param>
        /// <param name="iterations"></param>
        public HeightMap Erode(float min_threshold, float max_threshold, int iterations)
        {
            if (min_threshold < 0f || min_threshold >= 1f || max_threshold <= 0f && max_threshold > 1f || min_threshold >= max_threshold)
            {
                Debug.Log("Invalid erosion values");
                return this;
            }

            HeightMap diff = new HeightMap(m_widthX, m_depthZ);
            for (int i = 0; i < iterations; i++)
            {
                // material transport
                for (int x = 1; x < m_widthX - 1; x++)
                {
                    for (int z = 1; z < m_depthZ - 1; z++)
                    {
                        // calculate height differences
                        float h = m_heights[x, z];
                        float h1 = m_heights[x, z + 1];
                        float h2 = m_heights[x - 1, z];
                        float h3 = m_heights[x + 1, z];
                        float h4 = m_heights[x, z - 1];
                        float d1 = h - h1;
                        float d2 = h - h2;
                        float d3 = h - h3;
                        float d4 = h - h4;
                        // find greatest height difference
                        float max_diff = float.MinValue;
                        float total_diff = 0f;
                        if (d1 > 0f)
                        {
                            total_diff += d1;
                            if (d1 > max_diff)
                            {
                                max_diff = d1;
                            }
                        }
                        if (d2 > 0f)
                        {
                            total_diff += d2;
                            if (d2 > max_diff)
                            {
                                max_diff = d2;
                            }
                        }
                        if (d3 > 0f)
                        {
                            total_diff += d3;
                            if (d3 > max_diff)
                            {
                                max_diff = d3;
                            }
                        }
                        if (d4 > 0f)
                        {
                            total_diff += d4;
                            if (d4 > max_diff)
                            {
                                max_diff = d4;
                            }
                        }
                        // skip if outside talus threshold
                        if (max_diff < min_threshold || max_diff > max_threshold)
                        {
                            continue;
                        }
                        max_diff = max_diff / 2f;
                        float factor = max_diff / total_diff;
                        diff[x, z] = diff[x, z] - max_diff;
                        // transport material
                        if (d1 > 0)
                            diff[x, z + 1] = diff[x, z + 1] + factor * d1;
                        if (d2 > 0)
                            diff[x - 1, z] = diff[x - 1, z] + factor * d2;
                        if (d3 > 0)
                            diff[x + 1, z] = diff[x + 1, z] + factor * d3;
                        if (d4 > 0)
                            diff[x, z - 1] = diff[x, z - 1] + factor * d4;
                    }
                }
                // apply changes
                Add(diff);
                diff.SetHeight(0f);
                //Set dirty
                m_isDirty = true;
            }
            return this;
        }

        /// <summary>
        /// Thermally erode the heightmap
        /// </summary>
        /// <param name="iterations">Number of iterations</param>
        /// <param name="min_threshold">Minimum erosion threshold</param>
        /// <param name="max_threshold">Maximum erosion threshold</param>
        public HeightMap ErodeThermal_1(int iterations, float min_threshold, float max_threshold)
        {
            //Check thresholds
            if (min_threshold < 0f || min_threshold > 1f || max_threshold < 0f && max_threshold > 1f || min_threshold >= max_threshold)
            {
                Debug.Log("Invalid erosion thresholds");
                return this;
            }

            HeightMap diff = new HeightMap(m_widthX, m_depthZ);
            for (int i = 0; i < iterations; i++)
            {
                // material transport
                for (int x = 1; x < m_widthX - 1; x++)
                {
                    for (int z = 1; z < m_depthZ - 1; z++)
                    {
                        // calculate height differences
                        float h = m_heights[x, z];
                        float h1 = m_heights[x, z + 1];
                        float h2 = m_heights[x - 1, z];
                        float h3 = m_heights[x + 1, z];
                        float h4 = m_heights[x, z - 1];
                        float d1 = h - h1;
                        float d2 = h - h2;
                        float d3 = h - h3;
                        float d4 = h - h4;
                        // find greatest height difference
                        float max_diff = float.MinValue;
                        float total_diff = 0f;
                        if (d1 > 0f)
                        {
                            total_diff += d1;
                            if (d1 > max_diff)
                            {
                                max_diff = d1;
                            }
                        }
                        if (d2 > 0f)
                        {
                            total_diff += d2;
                            if (d2 > max_diff)
                            {
                                max_diff = d2;
                            }
                        }
                        if (d3 > 0f)
                        {
                            total_diff += d3;
                            if (d3 > max_diff)
                            {
                                max_diff = d3;
                            }
                        }
                        if (d4 > 0f)
                        {
                            total_diff += d4;
                            if (d4 > max_diff)
                            {
                                max_diff = d4;
                            }
                        }
                        // skip if outside talus threshold
                        if (max_diff < min_threshold || max_diff > max_threshold)
                        {
                            continue;
                        }
                        max_diff = max_diff / 2f;
                        float factor = max_diff / total_diff;
                        diff[x, z] = diff[x, z] - max_diff;
                        // transport material
                        if (d1 > 0)
                            diff[x, z + 1] = diff[x, z + 1] + factor * d1;
                        if (d2 > 0)
                            diff[x - 1, z] = diff[x - 1, z] + factor * d2;
                        if (d3 > 0)
                            diff[x + 1, z] = diff[x + 1, z] + factor * d3;
                        if (d4 > 0)
                            diff[x, z - 1] = diff[x, z - 1] + factor * d4;
                    }
                }
                // apply changes
                AddClamped(diff, 0f, 1f);
                // reset diff
                diff.SetHeight(0f);
                //Set dirty
                m_isDirty = true;
            }
            return this;
        }
        */

        /// <summary>
        /// Run a thermal erosion pass
        /// </summary>
        /// <param name="iterations"></param>
        /// <param name="talusMin"></param>
        /// <param name="talusMax"></param>
        /// <param name="hardnessMask"></param>
        /// <returns></returns>
        public HeightMap ErodeThermal(int iterations, float talusMin, float talusMax, HeightMap hardnessMask)
        {
            float h, h1, h2, h3, h4, d1, d2, d3, d4, max_d;
            int i, j;
            for (int iter = 0; iter < iterations; iter++)
            {
                for (int x = 1; x < m_widthX - 1; x++)
                {
                    for (int z = 1; z < m_depthZ - 1; z++)
                    {
                        h = m_heights[x, z];
                        h1 = m_heights[x, z + 1];
                        h2 = m_heights[x - 1, z];
                        h3 = m_heights[x + 1, z];
                        h4 = m_heights[x, z - 1];
                        d1 = h - h1;
                        d2 = h - h2;
                        d3 = h - h3;
                        d4 = h - h4;
                        i = 0;
                        j = 0;
                        max_d = 0f;
                        if (d1 > max_d)
                        {
                            max_d = d1;
                            j = 1;
                        }
                        if (d2 > max_d)
                        {
                            max_d = d2;
                            i = -1;
                            j = 0;
                        }
                        if (d3 > max_d)
                        {
                            max_d = d3;
                            i = 1;
                            j = 0;
                        }
                        if (d4 > max_d)
                        {
                            max_d = d4;
                            i = 0;
                            j = -1;
                        }
                        if (max_d < talusMin || max_d > talusMax)
                        {
                            continue;
                        }
                        max_d *= (1f - hardnessMask[m_widthInvX * x, m_depthInvZ * z]);
                        max_d *= 0.5f;
                        m_heights[x, z] = m_heights[x, z] - max_d;
                        m_heights[x + i, z + j] = m_heights[x + i, z + j] + max_d;
                        m_isDirty = true;
                    }
                }
            }
            return this;
        }

        /*
        /// <summary>
        /// Hydraulic erosion
        /// </summary>
        /// <param name="iterations">Number of iterations to run</param>
        /// <param name="rainMap">Rain map - added every rain freq intervc</param>
        /// <param name="rainFrequency">Frequency between iterations that rain will fall</param>
        /// <param name="sedimentDisolveRate">Max amount of sediment disolved from underlying heightmap every iteration - 0f..1f = 1f = all height moved</param>
        /// <returns>This</returns>
        public HeightMap ErodeHydraulic_0(int iterations, HeightMap rainMap, int rainFrequency, float sedimentDisolveRate)
        {
            HeightMap waterMap = new HeightMap(m_widthX, m_depthZ);
            HeightMap waterMapDiff = new HeightMap(m_widthX, m_depthZ);
            HeightMap sedimentMap = new HeightMap(m_widthX, m_depthZ);
            HeightMap sedimentMapDiff = new HeightMap(m_widthX, m_depthZ);

            //Consume all water over the duration of the rain fall
            float transferRate = 1f / (float)rainFrequency;

            for (int i = 0; i < iterations; i++)
            {
                // simulate rain - add water to water map
                if (i % rainFrequency == 0)
                {
                    waterMap.Add(rainMap);
                }

                //Sediment and rain - energy must be conserved - so rain in == rain out, hm new = hm orig + sediment map-> therefore move sediment in 1 step based on erosion and height

                // calculate water and sediment 
                for (int x = 1; x < m_widthX - 1; x++)
                {
                    for (int z = 1; z < m_depthZ - 1; z++)
                    {
                        // get relative values at this location
                        float cellHeight = m_heights[x, z];
                        float cellWater = waterMap[x, z];
                        //float cellSediment = sedimentMap[x, z];
                        //float totalCellHeight = cellHeight + cellWater + cellSediment;

                        // determine the magnitude of the drop around the location
                        float deltaHeight = 0;
                        int affectedCells = 0;
                        for (int dX = x-1; dX <= x+1; dX++)
                        {
                            for (int dZ = z-1; dZ <= z+1; dZ++)
                            {
                                float localDelta = cellHeight - m_heights[dX, dZ];
                                if (localDelta > 0f)
                                {
                                    deltaHeight += localDelta;
                                    affectedCells++;
                                }
                            }
                        }

                        //Move water and sediment if we can
                        if (affectedCells > 0)
                        {
                            // add in a batch of newly dissolved sediment - theory - greater height difference = greater water flow = more sediment generation
                            float newSediment = cellWater * deltaHeight * sedimentDisolveRate;

                            // remove sediment from current location
                            sedimentMapDiff[x, z] -= newSediment;

                            // remove water from current location
                            waterMapDiff[x, z] -= cellWater * transferRate;

                            // push sediment and water to new locations
                            for (int dX = x - 1; dX <= x + 1; dX++)
                            {
                                for (int dZ = z - 1; dZ <= z + 1; dZ++)
                                {
                                    float localDelta = cellHeight - m_heights[dX, dZ];
                                    if (localDelta > 0f)
                                    {
                                        float flowStrength = localDelta/deltaHeight;

                                        //Move a percentge of water from current location to new location
                                        waterMapDiff[dX, dZ] += cellWater * flowStrength * transferRate;

                                        //Move the sediment to its new location
                                        sedimentMapDiff[dX, dZ] += newSediment * flowStrength;
                                    }
                                }
                            }
                        }
                    }
                }

                // apply changes to water map
                waterMap.Add(waterMapDiff);

                // apply changes to sediment map
                sedimentMap.Add(sedimentMapDiff);

                // apply changes to height map
                AddClamped(sedimentMapDiff, 0f, 1f);

                // water vaporization
                waterMap.SubtractClamped(transferRate, 0f, 1f);

                // clear diff maps
                waterMapDiff.SetHeight(0f);
                sedimentMapDiff.SetHeight(0f);

                //Set dirty
                m_isDirty = true;
            }

            return this;
        }

        /// <summary>
        /// Hydraulic erosion 
        /// </summary>
        /// <param name="iterations">Number of iterations to run</param>
        /// <param name="rainMap">Rain map - added every rain freq intervc</param>
        /// <param name="rainFrequency">Frequency between iterations that rain will fall</param>
        /// <param name="sedimentDisolveRate">Max amount of sediment disolved from underlying heightmap every iteration - 0f..1f = 1f = all height moved</param>
        /// <returns>This</returns>
        public HeightMap ErodeHydraulic_1(int iterations, HeightMap rainMap, int rainFrequency, float sedimentDisolveRate)
        {
            HeightMap waterMap = new HeightMap(m_widthX, m_depthZ);
            HeightMap velocityMap = new HeightMap(m_widthX, m_depthZ);
            HeightMap hardnessMap = new HeightMap(m_widthX, m_depthZ);
            HeightMap waterMapDiff = new HeightMap(m_widthX, m_depthZ);
            HeightMap sedimentMap = new HeightMap(m_widthX, m_depthZ);
            HeightMap sedimentMapDiff = new HeightMap(m_widthX, m_depthZ);

            //Consume all water over the duration of the rain fall
            float transferRate = 1f / (float)rainFrequency;

            for (int i = 0; i < iterations; i++)
            {
                // simulate rain - add water to water map
                if (i % rainFrequency == 0)
                {
                    waterMap.Add(rainMap);
                }

                //Sediment and rain - energy must be conserved - so rain in == rain out, 
                //hm new = hm orig + sediment map-> therefore move sediment in 1 step based on erosion and height

                //caclulate velocity


                // calculate water and sediment 
                for (int x = 1; x < m_widthX - 1; x++)
                {
                    for (int z = 1; z < m_depthZ - 1; z++)
                    {
                        // get relative values at this location
                        float cellHeight = m_heights[x, z];
                        float cellWater = waterMap[x, z];
                        //float cellSediment = sedimentMap[x, z];
                        //float totalCellHeight = cellHeight + cellWater + cellSediment;

                        // determine the magnitude of the drop around the location
                        float deltaHeight = 0;
                        int affectedCells = 0;
                        for (int dX = x - 1; dX <= x + 1; dX++)
                        {
                            for (int dZ = z - 1; dZ <= z + 1; dZ++)
                            {
                                float localDelta = cellHeight - m_heights[dX, dZ];
                                if (localDelta > 0f)
                                {
                                    deltaHeight += localDelta;
                                    affectedCells++;
                                }
                            }
                        }

                        //Move water and sediment if we can
                        if (affectedCells > 0)
                        {
                            // add in a batch of newly dissolved sediment - theory - greater height difference = greater water flow = more sediment generation
                            float newSediment = cellWater * deltaHeight * sedimentDisolveRate;

                            // remove sediment from current location
                            sedimentMapDiff[x, z] -= newSediment;

                            // remove water from current location
                            waterMapDiff[x, z] -= cellWater * transferRate;

                            // push sediment and water to new locations
                            for (int dX = x - 1; dX <= x + 1; dX++)
                            {
                                for (int dZ = z - 1; dZ <= z + 1; dZ++)
                                {
                                    float localDelta = cellHeight - m_heights[dX, dZ];
                                    if (localDelta > 0f)
                                    {
                                        float flowStrength = localDelta / deltaHeight;

                                        //Move a percentge of water from current location to new location
                                        waterMapDiff[dX, dZ] += cellWater * flowStrength * transferRate;

                                        //Move the sediment to its new location
                                        sedimentMapDiff[dX, dZ] += newSediment * flowStrength;
                                    }
                                }
                            }
                        }
                    }
                }

                // apply changes to water map
                waterMap.Add(waterMapDiff);

                // apply changes to sediment map
                sedimentMap.Add(sedimentMapDiff);

                // apply changes to height map
                AddClamped(sedimentMapDiff, 0f, 1f);

                // water vaporization
                waterMap.SubtractClamped(transferRate, 0f, 1f);

                // clear diff maps
                waterMapDiff.SetHeight(0f);
                sedimentMapDiff.SetHeight(0f);

                //Set dirty
                m_isDirty = true;
            }

            return this;
        }
        */

        /// <summary>
        /// Quantize the heightmap to mod's of this value
        /// </summary>
        /// <param name="divisor"></param>
        /// <returns></returns>
        public HeightMap Quantize(float divisor)
        {
            for (int x = 0; x < m_widthX; x++)
            {
                for (int z = 0; z < m_depthZ; z++)
                {
                    m_heights[x, z] = Mathf.Round(m_heights[x, z] / divisor) * divisor;
                }
            }
            m_isDirty = true;
            return this;
        }

        /// <summary>
        /// Quantize the heightmap to mod's of this value
        /// </summary>
        /// <param name="divisor"></param>
        /// <param name="curve"></param>
        /// <returns></returns>
        public HeightMap Quantize(float [] startHeights, AnimationCurve [] curves)
        {
            int numTerraces = startHeights.GetLength(0);
            if (numTerraces == 0)
            {
                Debug.LogWarning("Quantize : must supply heights!");
                return this;
            }
            if (curves.GetLength(0) != numTerraces)
            {
                Debug.LogWarning("Quantize : startHeights and curves do not match!");
                return this;
            }

            for (int x = 0; x < m_widthX; x++)
            {
                for (int z = 0; z < m_depthZ; z++)
                {
                    int terraceIdx = 0;
                    float startHeight = 0f;
                    float nextHeight = 1f;
                    float currHeight = m_heights[x, z];

                    for (terraceIdx = numTerraces-1; terraceIdx >= 0; terraceIdx--)
                    {
                        startHeight = startHeights[terraceIdx];
                        if (terraceIdx == numTerraces-1)
                        {
                            nextHeight = 1f;
                        }
                        else
                        {
                            nextHeight = startHeights[terraceIdx+1];
                        }
                        if (startHeight <= currHeight && currHeight <= nextHeight)
                        {
                            break;
                        }
                    }

                    m_heights[x, z] = startHeight + ((currHeight-startHeight) * curves[terraceIdx].Evaluate((currHeight - startHeight) / (nextHeight - startHeight)));
                }
            }
            m_isDirty = true;
            return this;
        }

        /// <summary>
        /// Calculate the curvature of this heightmap
        /// </summary>
        /// <param name="curvatureType">Type of curvature to calculate</param>
        /// <returns>Curvature map</returns>
        public HeightMap CurvatureMap(GaiaConstants.CurvatureType curvatureType)
        {
            float limit = 10000;
            int height = m_depthZ;
            int width = m_widthX;
            float ux = 1.0f / (width - 1.0f);
            float uy = 1.0f / (height - 1.0f);
            float[] heights = this.Heights1D();
            HeightMap cMap = this.Duplicate();

            for (int z = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {

                    int xp1 = (x == width - 1) ? x : x + 1;
                    int xn1 = (x == 0) ? x : x - 1;

                    int yp1 = (z == height - 1) ? z : z + 1;
                    int yn1 = (z == 0) ? z : z - 1;

                    float v = heights[x + z * width];

                    float l = heights[xn1 + z * width];
                    float r = heights[xp1 + z * width];

                    float b = heights[x + yn1 * width];
                    float t = heights[x + yp1 * width];

                    float lb = heights[xn1 + yn1 * width];
                    float lt = heights[xn1 + yp1 * width];

                    float rb = heights[xp1 + yn1 * width];
                    float rt = heights[xp1 + yp1 * width];

                    float dx = (r - l) / (2.0f * ux);
                    float dy = (t - b) / (2.0f * uy);

                    float dxx = (r - 2.0f * v + l) / (ux * ux);
                    float dyy = (t - 2.0f * v + b) / (uy * uy);

                    float dxy = (rt - rb - lt + lb) / (4.0f * ux * uy);

                    float curve = 0.0f;

                    switch (curvatureType)
                    {
                        case GaiaConstants.CurvatureType.Horizontal:
                            curve = HorizontalCurve(limit, dx, dy, dxx, dyy, dxy);
                            break;

                        case GaiaConstants.CurvatureType.Vertical:
                            curve = VerticalCurve(limit, dx, dy, dxx, dyy, dxy);
                            break;

                        case GaiaConstants.CurvatureType.Average:
                            curve = AverageCurve(limit, dx, dy, dxx, dyy, dxy);
                            break;
                    }

                    cMap[x, z] = curve;
                }
            }
            return cMap;
        }

        /// <summary>
        /// Horizontal curvature
        /// </summary>
        private float HorizontalCurve(float limit, float dx, float dy, float dxx, float dyy, float dxy)
        {
            float kh = -2.0f * (dy * dy * dxx + dx * dx * dyy - dx * dy * dxy);
            kh /= dx * dx + dy * dy;

            if (float.IsInfinity(kh) || float.IsNaN(kh)) kh = 0.0f;

            if (kh < -limit) kh = -limit;
            if (kh > limit) kh = limit;

            kh /= limit;
            kh = kh * 0.5f + 0.5f;

            return kh;
        }

        /// <summary>
        /// Vertical curvature
        /// </summary>
        private float VerticalCurve(float limit, float dx, float dy, float dxx, float dyy, float dxy)
        {
            float kv = -2.0f * (dx * dx * dxx + dy * dy * dyy + dx * dy * dxy);
            kv /= dx * dx + dy * dy;

            if (float.IsInfinity(kv) || float.IsNaN(kv)) kv = 0.0f;

            if (kv < -limit) kv = -limit;
            if (kv > limit) kv = limit;

            kv /= limit;
            kv = kv * 0.5f + 0.5f;

            return kv;
        }

        /// <summary>
        /// Average curvature
        /// </summary>
        private float AverageCurve(float limit, float dx, float dy, float dxx, float dyy, float dxy)
        {
            float kh = HorizontalCurve(limit, dx, dy, dxx, dyy, dxy);
            float kv = VerticalCurve(limit, dx, dy, dxx, dyy, dxy);
            return (kh + kv) * 0.5f;
        }

        /// <summary>
        /// Create an aspect map
        /// </summary>
        /// <param name="aspectType">Type of aspect to create</param>
        /// <returns>Aspect map</returns>
        public HeightMap Aspect(GaiaConstants.AspectType aspectType)
        {
            int height = m_depthZ;
            int width = m_widthX;
            float[] heights = this.Heights1D();
            float ux = 1.0f / (width - 1.0f);
            float uy = 1.0f / (height - 1.0f);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {

                    int xp1 = (x == width - 1) ? x : x + 1;
                    int xn1 = (x == 0) ? x : x - 1;

                    int yp1 = (y == height - 1) ? y : y + 1;
                    int yn1 = (y == 0) ? y : y - 1;

                    float l = heights[xn1 + y * width];
                    float r = heights[xp1 + y * width];

                    float b = heights[x + yn1 * width];
                    float t = heights[x + yp1 * width];

                    float dx = (r - l) / (2.0f * ux);
                    float dy = (t - b) / (2.0f * uy);

                    float m = Mathf.Sqrt(dx * dx + dy * dy);
                    float a = Mathf.Acos(-dy / m) * Mathf.Rad2Deg;

                    if (float.IsInfinity(a) || float.IsNaN(a))
                        a = 0.0f;

                    float aspect = 180.0f * (1.0f + Sign(dx)) - Sign(dx) * a;

                    switch (aspectType)
                    {
                        case GaiaConstants.AspectType.Northerness:
                            aspect = Mathf.Cos(aspect * Mathf.Deg2Rad);
                            aspect = aspect * 0.5f + 0.5f;
                            break;

                        case GaiaConstants.AspectType.Easterness:
                            aspect = Mathf.Sin(aspect * Mathf.Deg2Rad);
                            aspect = aspect * 0.5f + 0.5f;
                            break;

                        default:
                            aspect /= 360.0f;
                            break;
                    }

                    m_heights[x, y] = aspect;
                }
            }
            m_isDirty = true;
            return this;
        }

        /// <summary>
        /// Value based on sign
        /// </summary>
        /// <param name="v">Value to check</param>
        /// <returns></returns>
        private float Sign(float v)
        {
            if (v > 0) return 1;
            if (v < 0) return -1;
            return 0;
        }

        /// <summary>
        /// </summary>
        /// <param name="iterations">Number of iterations to run</param>
        /// <param name="hardnessMap">Hardness map - influences rate of erosion</param>
        /// <param name="rainMap">Rain map - added every rain freq intervc</param>
        /// <param name="rainFrequency">Frequency between iterations that rain will fall</param>
        /// <param name="sedimentDisolveRate">Max amount of sediment disolved from underlying heightmap every iteration - 0f..1f = 1f = all height moved</param>
        /// <param name="sedimentMap">Sediment passed back to caller</param>
        /// <returns>This</returns>
        public HeightMap ErodeHydraulic(int iterations, HeightMap hardnessMap, HeightMap rainMap, int rainFrequency, float sedimentDisolveRate, ref HeightMap sedimentMap)
        {
			HeightMap waterMap = new HeightMap(m_widthX, m_depthZ);
            float[,,] waterOutFlow = new float[m_widthX, m_depthZ, 4];
            HeightMap waterMapDiff = new HeightMap(m_widthX, m_depthZ);
            HeightMap sedimentMapDiff = new HeightMap(m_widthX, m_depthZ);

            //Consume all water over the duration of the rain fall
            float transferRate = 1f / (float)rainFrequency;

			for (int i = 0; i < iterations; i++)
            {
                // simulate rain - add water to water map
                if (i % rainFrequency == 0)
                {
                    waterMap.Add(rainMap);
                }

                // caculate outflow
                CalculateWaterOutflow(this, waterMap, waterOutFlow);

                // update the water map
                UpdateWaterMap(waterMap, waterOutFlow);

                // erode and move sediment 
                for (int x = 0; x < m_widthX; x++)
                {
                    for (int z = 0; z < m_depthZ; z++)
                    {
                        // get relative values at this location
                        float height = m_heights[x, z];
                        float water = waterMap[x, z];
                        float hardness = hardnessMap[x, z];

                        // determine the magnitude of the drop around the location
                        float deltaHeight = 0;
                        int affectedCells = 0;
                        for (int dX = x - 1; dX <= x + 1; dX++)
                        {
                            if (dX < 0 || dX == m_widthX)
                            {
                                continue;
                            }
                            for (int dZ = z - 1; dZ <= z + 1; dZ++)
                            {
                                if (dZ < 0 || dZ == m_depthZ)
                                {
                                    continue;
                                }
                                float localDelta = height - m_heights[dX, dZ];
                                if (localDelta > 0f)
                                {
                                    deltaHeight += localDelta;
                                    affectedCells++;
                                }
                            }
                        }

                        //Move water and sediment if we can
                        if (affectedCells > 0)
                        {
                            // add in a batch of newly dissolved sediment - theory - greater height difference = greater water flow = more sediment generation
                            float newSediment = water * deltaHeight * sedimentDisolveRate * (1f - hardness);

                            // remove sediment from current location
                            sedimentMapDiff[x, z] -= newSediment;

                            // push sediment to new locations
                            for (int dX = x - 1; dX <= x + 1; dX++)
                            {
                                if (dX < 0 || dX == m_widthX)
                                {
                                    continue;
                                }
                                for (int dZ = z - 1; dZ <= z + 1; dZ++)
                                {
                                    if (dZ < 0 || dZ == m_depthZ)
                                    {
                                        continue;
                                    }

                                    float localDelta = height - m_heights[dX, dZ];
                                    if (localDelta > 0f)
                                    {
                                        float flow = newSediment * (localDelta / deltaHeight);
                                        sedimentMapDiff[dX, dZ] += flow;
                                        //sedimentMap[dX, dZ] += flow;
                                    }
                                }
                            }
                        }
					}
				}

                // apply changes to sediment map
                sedimentMap.Add(sedimentMapDiff);

                // apply changes to height map
                AddClamped(sedimentMapDiff, 0f, 1f);

                // water vaporization
                waterMap.SubtractClamped(transferRate, 0f, 1f);

                // clear diff maps
                waterMapDiff.SetHeight(0f);
                sedimentMapDiff.SetHeight(0f);

                //Set dirty
                m_isDirty = true;
			}

			return this;
        }

        /// <summary>
        /// Calculate outflow
        /// </summary>
        /// <param name="waterMap">Water map</param>
        /// <param name="outFlow">Outflow</param>
        /// <param name="heightMap">Heights</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        private void CalculateWaterOutflow(HeightMap heightMap, HeightMap waterMap, float[,,] outFlow)
        {
            int width = heightMap.Width();
            int height = heightMap.Depth();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int xLeft = (x == 0) ? 0 : x - 1;
                    int xRight = (x == width - 1) ? width - 1 : x + 1;
                    int yUp = (y == 0) ? 0 : y - 1;
                    int yDown = (y == height - 1) ? height - 1 : y + 1;

                    float waterHt = waterMap[x, y];
                    float waterHts0 = waterMap[xLeft, y];
                    float waterHts1 = waterMap[xRight, y];
                    float waterHts2 = waterMap[x, yUp];
                    float waterHts3 = waterMap[x, yDown];

                    float landHt = heightMap[x, y];
                    float landHts0 = heightMap[xLeft, y];
                    float landHts1 = heightMap[xRight, y];
                    float landHts2 = heightMap[x, yUp];
                    float landHts3 = heightMap[x, yDown];

                    float diff0 = (waterHt + landHt) - (waterHts0 + landHts0);
                    float diff1 = (waterHt + landHt) - (waterHts1 + landHts1);
                    float diff2 = (waterHt + landHt) - (waterHts2 + landHts2);
                    float diff3 = (waterHt + landHt) - (waterHts3 + landHts3);

                    //out flow is previous flow plus flow for this time step.
                    float flow0 = Mathf.Max(0, outFlow[x, y, 0] + diff0);
                    float flow1 = Mathf.Max(0, outFlow[x, y, 1] + diff1);
                    float flow2 = Mathf.Max(0, outFlow[x, y, 2] + diff2);
                    float flow3 = Mathf.Max(0, outFlow[x, y, 3] + diff3);

                    float sum = flow0 + flow1 + flow2 + flow3;

                    if (sum > 0.0f)
                    {
                        //If the sum of the outflow flux exceeds the amount in the cell
                        //flow value will be scaled down by a factor K to avoid negative update.
                        float K = waterHt / (sum * TIME);
                        if (K > 1.0f)
                        {
                            K = 1.0f;
                        }
                        if (K < 0.0f)
                        {
                            K = 0.0f;
                        }

                        outFlow[x, y, 0] = flow0 * K;
                        outFlow[x, y, 1] = flow1 * K;
                        outFlow[x, y, 2] = flow2 * K;
                        outFlow[x, y, 3] = flow3 * K;
                    }
                    else
                    {
                        outFlow[x, y, 0] = 0.0f;
                        outFlow[x, y, 1] = 0.0f;
                        outFlow[x, y, 2] = 0.0f;
                        outFlow[x, y, 3] = 0.0f;
                    }
                }
            }
        }

        /// <summary>
        /// Update the water map
        /// </summary>
        /// <param name="waterMap">Water map</param>
        /// <param name="outFlow">Outflow</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        private void UpdateWaterMap(HeightMap waterMap, float[,,] outFlow)
        {
            int width = waterMap.Width();
            int height = waterMap.Depth();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float flowOUT = outFlow[x, y, 0] + outFlow[x, y, 1] + outFlow[x, y, 2] + outFlow[x, y, 3];
                    float flowIN = 0.0f;

                    //Flow in is inflow from neighour cells. Note for the cell on the left you need 
                    //thats cells flow to the right (ie it flows into this cell)
                    flowIN += (x == 0) ? 0.0f : outFlow[x - 1, y, RIGHT];
                    flowIN += (x == width - 1) ? 0.0f : outFlow[x + 1, y, LEFT];
                    flowIN += (y == 0) ? 0.0f : outFlow[x, y - 1, TOP];
                    flowIN += (y == height - 1) ? 0.0f : outFlow[x, y + 1, BOTTOM];

                    float ht = waterMap[x, y] + (flowIN - flowOUT) * TIME;
                    if (ht < 0.0f)
                    {
                        ht = 0.0f;
                    }

                    //Result is net volume change over time
                    waterMap[x, y] = ht;
                }
            }
        }



        private const int LEFT = 0;
        private const int RIGHT = 1;
        private const int BOTTOM = 2;
        private const int TOP = 3;
        private const float TIME = 0.2f;

        /// <summary>
        /// Calculate the a flow map for the heightmap
        /// </summary>
        /// <param name="iterations">Number of iterations to run</param>
        /// <returns>Flowmap</returns>
        public HeightMap FlowMap(int iterations)
        {
            int height = m_depthZ;
            int width = m_widthX;
            float[] heights = this.Heights1D();
            float[,] waterMap = new float[width, height];
            float[,,] outFlow = new float[width, height, 4];

            FillWaterMap(0.0001f, waterMap, width, height);

            for (int i = 0; i < iterations; i++)
            {
                ComputeOutflow(waterMap, outFlow, heights, width, height);
                UpdateWaterMap(waterMap, outFlow, width, height);
            }

            float[,] velocityMap = new float[width, height];

            CalculateVelocityField(velocityMap, outFlow, width, height);
            NormalizeMap(velocityMap, width, height);

            return new HeightMap(velocityMap);
        }

        /// <summary>
        /// Fill the water map
        /// </summary>
        /// <param name="amount">Amount to fill it</param>
        /// <param name="waterMap">Watermap</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        private void FillWaterMap(float amount, float[,] waterMap, int width, int height)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    waterMap[x, y] = amount;
                }
            }
        }

        /// <summary>
        /// Compute outflow
        /// </summary>
        /// <param name="waterMap">Water map</param>
        /// <param name="outFlow">Outflow</param>
        /// <param name="heightMap">Heights</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        private void ComputeOutflow(float[,] waterMap, float[,,] outFlow, float[] heightMap, int width, int height)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int xn1 = (x == 0) ? 0 : x - 1;
                    int xp1 = (x == width - 1) ? width - 1 : x + 1;
                    int yn1 = (y == 0) ? 0 : y - 1;
                    int yp1 = (y == height - 1) ? height - 1 : y + 1;

                    float waterHt = waterMap[x, y];
                    float waterHts0 = waterMap[xn1, y];
                    float waterHts1 = waterMap[xp1, y];
                    float waterHts2 = waterMap[x, yn1];
                    float waterHts3 = waterMap[x, yp1];

                    float landHt = heightMap[x + y * width];
                    float landHts0 = heightMap[xn1 + y * width];
                    float landHts1 = heightMap[xp1 + y * width];
                    float landHts2 = heightMap[x + yn1 * width];
                    float landHts3 = heightMap[x + yp1 * width];

                    float diff0 = (waterHt + landHt) - (waterHts0 + landHts0);
                    float diff1 = (waterHt + landHt) - (waterHts1 + landHts1);
                    float diff2 = (waterHt + landHt) - (waterHts2 + landHts2);
                    float diff3 = (waterHt + landHt) - (waterHts3 + landHts3);

                    //out flow is previous flow plus flow for this time step.
                    float flow0 = Mathf.Max(0, outFlow[x, y, 0] + diff0);
                    float flow1 = Mathf.Max(0, outFlow[x, y, 1] + diff1);
                    float flow2 = Mathf.Max(0, outFlow[x, y, 2] + diff2);
                    float flow3 = Mathf.Max(0, outFlow[x, y, 3] + diff3);

                    float sum = flow0 + flow1 + flow2 + flow3;

                    if (sum > 0.0f)
                    {
                        //If the sum of the outflow flux exceeds the amount in the cell
                        //flow value will be scaled down by a factor K to avoid negative update.
                        float K = waterHt / (sum * TIME);
                        if (K > 1.0f) K = 1.0f;
                        if (K < 0.0f) K = 0.0f;

                        outFlow[x, y, 0] = flow0 * K;
                        outFlow[x, y, 1] = flow1 * K;
                        outFlow[x, y, 2] = flow2 * K;
                        outFlow[x, y, 3] = flow3 * K;
                    }
                    else
                    {
                        outFlow[x, y, 0] = 0.0f;
                        outFlow[x, y, 1] = 0.0f;
                        outFlow[x, y, 2] = 0.0f;
                        outFlow[x, y, 3] = 0.0f;
                    }

                }
            }
        }

        /// <summary>
        /// Update the water map
        /// </summary>
        /// <param name="waterMap">Water map</param>
        /// <param name="outFlow">Outflow</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        private void UpdateWaterMap(float[,] waterMap, float[,,] outFlow, int width, int height)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float flowOUT = outFlow[x, y, 0] + outFlow[x, y, 1] + outFlow[x, y, 2] + outFlow[x, y, 3];
                    float flowIN = 0.0f;

                    //Flow in is inflow from neighour cells. Note for the cell on the left you need 
                    //thats cells flow to the right (ie it flows into this cell)
                    flowIN += (x == 0) ? 0.0f : outFlow[x - 1, y, RIGHT];
                    flowIN += (x == width - 1) ? 0.0f : outFlow[x + 1, y, LEFT];
                    flowIN += (y == 0) ? 0.0f : outFlow[x, y - 1, TOP];
                    flowIN += (y == height - 1) ? 0.0f : outFlow[x, y + 1, BOTTOM];

                    float ht = waterMap[x, y] + (flowIN - flowOUT) * TIME;
                    if (ht < 0.0f) ht = 0.0f;

                    //Result is net volume change over time
                    waterMap[x, y] = ht;
                }
            }
        }

        /// <summary>
        /// Calculate water flow velocity field
        /// </summary>
        /// <param name="velocityMap">Velocity map</param>
        /// <param name="outFlow">Outlflow map</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        private void CalculateVelocityField(float[,] velocityMap, float[,,] outFlow, int width, int height)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float dl = (x == 0) ? 0.0f : outFlow[x - 1, y, RIGHT] - outFlow[x, y, LEFT];
                    float dr = (x == width - 1) ? 0.0f : outFlow[x, y, RIGHT] - outFlow[x + 1, y, LEFT];
                    float dt = (y == height - 1) ? 0.0f : outFlow[x, y + 1, BOTTOM] - outFlow[x, y, TOP];
                    float db = (y == 0) ? 0.0f : outFlow[x, y, BOTTOM] - outFlow[x, y - 1, TOP];
                    float vx = (dl + dr) * 0.5f;
                    float vy = (db + dt) * 0.5f;
                    velocityMap[x, y] = Mathf.Sqrt(vx * vx + vy * vy);
                }
            }
        }

        /// <summary>
        /// Normalize array of floats
        /// </summary>
        /// <param name="map">Array to normalize</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        private void NormalizeMap(float[,] map, int width, int height)
        {
            float min = float.PositiveInfinity;
            float max = float.NegativeInfinity;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float v = map[x, y];
                    if (v < min) min = v;
                    if (v > max) max = v;
                }
            }

            float size = max - min;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float v = map[x, y];

                    if (size < 1e-12f)
                        v = 0;
                    else
                        v = (v - min) / size;

                    map[x, y] = v;
                }
            }
        }

        /// <summary>
        /// Calculate slope map for this height map
        /// </summary>
        /// <returns></returns>
        public HeightMap SlopeMap()
        {
            int height = m_depthZ;
            int width = m_widthX;
            float[] heights = this.Heights1D();
            float ux = 1.0f / (width - 1.0f);
            float uy = 1.0f / (height - 1.0f);

            float scaleX = 0.5f;
            float scaleY = 0.5f;

            HeightMap sMap = new HeightMap(width, height);

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

                    float g = Mathf.Sqrt(dx * dx + dy * dy);
                    float slope = g / Mathf.Sqrt(1.0f + g * g);

                    sMap[x, y] = slope;
                }
            }

            return sMap;
        }

        /// <summary>
        /// Add value
        /// </summary>
        /// <param name="value">Value to add</param>
        public HeightMap Add(float value)
        {
            for (int x = 0; x < m_widthX; x++)
            {
                for (int z = 0; z < m_depthZ; z++)
                {
                    m_heights[x, z] += value;
                }
            }
            m_isDirty = true;
            return this;
        }

        /// <summary>
        /// Add heightmap        
        /// </summary>
        /// <param name="heightMap">Heightmap to add</param>
        public HeightMap Add(HeightMap heightMap)
        {
            if (m_widthX != heightMap.m_widthX || m_depthZ != heightMap.m_depthZ)
            {
                for (int x = 0; x < m_widthX; x++)
                {
                    for (int z = 0; z < m_depthZ; z++)
                    {
                        m_heights[x, z] += heightMap[m_widthInvX * x, m_depthInvZ * z];
                    }
                }
            }
            else
            {
                for (int x = 0; x < m_widthX; x++)
                {
                    for (int z = 0; z < m_depthZ; z++)
                    {
                        m_heights[x, z] += heightMap.m_heights[x, z];
                    }
                }
            }
            m_isDirty = true;
            return this;
        }

        /// <summary>
        /// Add value and clamp result
        /// </summary>
        /// <param name="value">Value to add</param>
        /// <param name="min">Min value to clamp it to</param>
        /// <param name="max">Max value to clamp it to</param>
        public HeightMap AddClamped(float value, float min, float max)
        {
            for (int x = 0; x < m_widthX; x++)
            {
                for (int z = 0; z < m_depthZ; z++)
                {
                    float newValue = m_heights[x, z] + value;
                    if (newValue < min)
                    {
                        newValue = min;
                    }
                    else if (newValue > max)
                    {
                        newValue = max;
                    }
                    m_heights[x, z] = newValue;
                }
            }
            m_isDirty = true;
            return this;
        }

        /// <summary>
        /// Add heightmap and clamp result
        /// </summary>
        /// <param name="heightMap">Heightmap to add</param>
        /// <param name="min">Min value to clamp it to</param>
        /// <param name="max">Max value to clamp it to</param>
        public HeightMap AddClamped(HeightMap heightMap, float min, float max)
        {
            if (m_widthX != heightMap.m_widthX || m_depthZ != heightMap.m_depthZ)
            {
                for (int x = 0; x < m_widthX; x++)
                {
                    for (int z = 0; z < m_depthZ; z++)
                    {
                        float newValue = m_heights[x, z] + heightMap[m_widthInvX * x, m_depthInvZ * z];
                        if (newValue < min)
                        {
                            newValue = min;
                        }
                        else if (newValue > max)
                        {
                            newValue = max;
                        }
                        m_heights[x, z] = newValue;
                    }
                }
            }
            else
            {
                for (int x = 0; x < m_widthX; x++)
                {
                    for (int z = 0; z < m_depthZ; z++)
                    {
                        float newValue = m_heights[x, z] + heightMap.m_heights[x, z];
                        if (newValue < min)
                        {
                            newValue = min;
                        }
                        else if (newValue > max)
                        {
                            newValue = max;
                        }
                        m_heights[x, z] = newValue;
                    }
                }
            }
            m_isDirty = true;
            return this;
        }

        /// <summary>
        /// Subtract value
        /// </summary>
        /// <param name="value">Value to subtract</param>
        public HeightMap Subtract(float value)
        {
            for (int x = 0; x < m_widthX; x++)
            {
                for (int z = 0; z < m_depthZ; z++)
                {
                    m_heights[x, z] -= value;
                }
            }
            m_isDirty = true;
            return this;
        }

        /// <summary>
        /// Subtract heightmap
        /// </summary>
        /// <param name="heightMap">Heightmap to subtract</param>
        public HeightMap Subtract(HeightMap heightMap)
        {
            if (m_widthX != heightMap.m_widthX || m_depthZ != heightMap.m_depthZ)
            {
                for (int x = 0; x < m_widthX; x++)
                {
                    for (int z = 0; z < m_depthZ; z++)
                    {
                        m_heights[x, z] -= heightMap[m_widthInvX * x, m_depthInvZ * z];
                    }
                }
            }
            else
            {
                for (int x = 0; x < m_widthX; x++)
                {
                    for (int z = 0; z < m_depthZ; z++)
                    {
                        m_heights[x, z] -= heightMap.m_heights[x, z];
                    }
                }
            }
            m_isDirty = true;
            return this;
        }

        /// <summary>
        /// Subtract value and clamp result
        /// </summary>
        /// <param name="value">Value to subtract</param>
        /// <param name="min">Min value to clamp it to</param>
        /// <param name="max">Max value to clamp it to</param>
        public HeightMap SubtractClamped(float value, float min, float max)
        {
            for (int x = 0; x < m_widthX; x++)
            {
                for (int z = 0; z < m_depthZ; z++)
                {
                    float newValue = m_heights[x, z] - value;
                    if (newValue < min)
                    {
                        newValue = min;
                    }
                    else if (newValue > max)
                    {
                        newValue = max;
                    }
                    m_heights[x, z] = newValue;
                }
            }
            m_isDirty = true;
            return this;
        }

        /// <summary>
        /// Subtract heightmap and clamp result
        /// </summary>
        /// <param name="heightMap">Heightmap to subtract</param>
        /// <param name="min">Min value to clamp it to</param>
        /// <param name="max">Max value to clamp it to</param>
        public HeightMap SubtractClamped(HeightMap heightMap, float min, float max)
        {
            if (m_widthX != heightMap.m_widthX || m_depthZ != heightMap.m_depthZ)
            {
                for (int x = 0; x < m_widthX; x++)
                {
                    for (int z = 0; z < m_depthZ; z++)
                    {
                        float newValue = m_heights[x, z] - heightMap[m_widthInvX * x, m_depthInvZ * z];
                        if (newValue < min)
                        {
                            newValue = min;
                        }
                        else if (newValue > max)
                        {
                            newValue = max;
                        }
                        m_heights[x, z] = newValue;
                    }
                }
            }
            else
            {
                for (int x = 0; x < m_widthX; x++)
                {
                    for (int z = 0; z < m_depthZ; z++)
                    {
                        float newValue = m_heights[x, z] - heightMap.m_heights[x, z];
                        if (newValue < min)
                        {
                            newValue = min;
                        }
                        else if (newValue > max)
                        {
                            newValue = max;
                        }
                        m_heights[x, z] = newValue;
                    }
                }
            }
            m_isDirty = true;
            return this;
        }

        /// <summary>
        /// Multiply by value
        /// </summary>
        /// <param name="value">Value to multiply</param>
        public HeightMap Multiply(float value)
        {
            for (int x = 0; x < m_widthX; x++)
            {
                for (int z = 0; z < m_depthZ; z++)
                {
                    m_heights[x, z] *= value;
                }
            }
            m_isDirty = true;
            return this;
        }

        /// <summary>
        /// Multiply by heightmap
        /// </summary>
        /// <param name="heightMap">Heightmap to multiply</param>
        public HeightMap Multiply(HeightMap heightMap)
        {
            if (m_widthX != heightMap.m_widthX || m_depthZ != heightMap.m_depthZ)
            {
                for (int x = 0; x < m_widthX; x++)
                {
                    for (int z = 0; z < m_depthZ; z++)
                    {
                        m_heights[x, z] *= heightMap[m_widthInvX * x, m_depthInvZ * z];
                    }
                }
            }
            else
            {
                for (int x = 0; x < m_widthX; x++)
                {
                    for (int z = 0; z < m_depthZ; z++)
                    {
                        m_heights[x, z] *= heightMap.m_heights[x, z];
                    }
                }
            }
            m_isDirty = true;
            return this;
        }

        /// <summary>
        /// Multiply by value and clamp result
        /// </summary>
        /// <param name="value">Value to multiply it by</param>
        /// <param name="min">Min value to clamp it to</param>
        /// <param name="max">Max value to clamp it to</param>
        public HeightMap MultiplyClamped(float value, float min, float max)
        {
            for (int x = 0; x < m_widthX; x++)
            {
                for (int z = 0; z < m_depthZ; z++)
                {
                    float newValue = m_heights[x, z] * value;
                    if (newValue < min)
                    {
                        newValue = min;
                    }
                    else if (newValue > max)
                    {
                        newValue = max;
                    }
                    m_heights[x, z] = newValue;
                }
            }
            m_isDirty = true;
            return this;
        }

        /// <summary>
        /// Multiply by heightmap and clamp result
        /// </summary>
        /// <param name="heightMap">Heightmap to multiply</param>
        /// <param name="min">Min value to clamp it to</param>
        /// <param name="max">Max value to clamp it to</param>
        public HeightMap MultiplyClamped(HeightMap heightMap, float min, float max)
        {
            if (m_widthX != heightMap.m_widthX || m_depthZ != heightMap.m_depthZ)
            {
                for (int x = 0; x < m_widthX; x++)
                {
                    for (int z = 0; z < m_depthZ; z++)
                    {
                        float newValue = m_heights[x, z] * heightMap[m_widthInvX * x, m_depthInvZ * z];
                        if (newValue < min)
                        {
                            newValue = min;
                        }
                        else if (newValue > max)
                        {
                            newValue = max;
                        }
                        m_heights[x, z] = newValue;
                    }
                }
            }
            else
            {
                for (int x = 0; x < m_widthX; x++)
                {
                    for (int z = 0; z < m_depthZ; z++)
                    {
                        float newValue = m_heights[x, z] * heightMap.m_heights[x, z];
                        if (newValue < min)
                        {
                            newValue = min;
                        }
                        else if (newValue > max)
                        {
                            newValue = max;
                        }
                        m_heights[x, z] = newValue;
                    }
                }
            }
            m_isDirty = true;
            return this;
        }

        /// <summary>
        /// Divide by value
        /// </summary>
        /// <param name="value">Value to divide</param>
        public HeightMap Divide(float value)
        {
            for (int x = 0; x < m_widthX; x++)
            {
                for (int z = 0; z < m_depthZ; z++)
                {
                    m_heights[x, z] /= value;
                }
            }
            m_isDirty = true;
            return this;
        }

        /// <summary>
        /// Divide by heightmap
        /// </summary>
        /// <param name="heightMap">Heightmap to divide</param>
        public HeightMap Divide(HeightMap heightMap)
        {
            if (m_widthX != heightMap.m_widthX || m_depthZ != heightMap.m_depthZ)
            {
                for (int x = 0; x < m_widthX; x++)
                {
                    for (int z = 0; z < m_depthZ; z++)
                    {
                        m_heights[x, z] /= heightMap[m_widthInvX * x, m_depthInvZ * z];
                    }
                }
            }
            else
            {
                for (int x = 0; x < m_widthX; x++)
                {
                    for (int z = 0; z < m_depthZ; z++)
                    {
                        m_heights[x, z] /= heightMap.m_heights[x, z];
                    }
                }
            }
            m_isDirty = true;
            return this;
        }

        /// <summary>
        /// Divide by value and clamp result
        /// </summary>
        /// <param name="value">Value to divide</param>
        /// <param name="min">Min value to clamp it to</param>
        /// <param name="max">Max value to clamp it to</param>
        public HeightMap DivideClamped(float value, float min, float max)
        {
            for (int x = 0; x < m_widthX; x++)
            {
                for (int z = 0; z < m_depthZ; z++)
                {
                    float newValue = m_heights[x, z] / value;
                    if (newValue < min)
                    {
                        newValue = min;
                    }
                    else if (newValue > max)
                    {
                        newValue = max;
                    }
                    m_heights[x, z] = newValue;
                }
            }
            m_isDirty = true;
            return this;
        }

        /// <summary>
        /// Divide by heightmap and clamp result
        /// </summary>
        /// <param name="heightMap">Heightmap to multiply</param>
        /// <param name="min">Min value to clamp it to</param>
        /// <param name="max">Max value to clamp it to</param>
        public HeightMap DivideClamped(HeightMap heightMap, float min, float max)
        {
            if (m_widthX != heightMap.m_widthX || m_depthZ != heightMap.m_depthZ)
            {
                for (int x = 0; x < m_widthX; x++)
                {
                    for (int z = 0; z < m_depthZ; z++)
                    {
                        float newValue = m_heights[x, z] / heightMap[m_widthInvX * x, m_depthInvZ * z];
                        if (newValue < min)
                        {
                            newValue = min;
                        }
                        else if (newValue > max)
                        {
                            newValue = max;
                        }
                        m_heights[x, z] = newValue;
                    }
                }
            }
            else
            {
                for (int x = 0; x < m_widthX; x++)
                {
                    for (int z = 0; z < m_depthZ; z++)
                    {
                        float newValue = m_heights[x, z] / heightMap.m_heights[x, z];
                        if (newValue < min)
                        {
                            newValue = min;
                        }
                        else if (newValue > max)
                        {
                            newValue = max;
                        }
                        m_heights[x, z] = newValue;
                    }
                }
            }
            m_isDirty = true;
            return this;
        }

        /// <summary>
        /// Lerp to new values supplied based on mask
        /// </summary>
        /// <param name="hmNewValues">New values to lerp to</param>
        /// <param name="hmLerpMask">Mask that controls degree of interpolation - 1 = take target, 0 = keep original</param>
        public HeightMap Lerp(HeightMap hmNewValues, HeightMap hmLerpMask)
        {
            if (m_widthX != hmNewValues.m_widthX || m_depthZ != hmNewValues.m_depthZ)
            {
                if (m_widthX != hmLerpMask.m_widthX || m_depthZ != hmLerpMask.m_depthZ)
                {
                    for (int x = 0; x < m_widthX; x++)
                    {
                        for (int z = 0; z < m_depthZ; z++)
                        {
                            m_heights[x, z] = Mathf.Lerp(m_heights[x, z], hmNewValues[m_widthInvX * x, m_depthInvZ * z], hmLerpMask[m_widthInvX * x, m_depthInvZ * z]);
                        }
                    }
                }
                else
                {
                    for (int x = 0; x < m_widthX; x++)
                    {
                        for (int z = 0; z < m_depthZ; z++)
                        {
                            m_heights[x, z] = Mathf.Lerp(m_heights[x, z], hmNewValues[m_widthInvX * x, m_depthInvZ * z], hmLerpMask[x, z]);
                        }
                    }
                }
            }
            else
            {
                if (m_widthX != hmLerpMask.m_widthX || m_depthZ != hmLerpMask.m_depthZ)
                {
                    for (int x = 0; x < m_widthX; x++)
                    {
                        for (int z = 0; z < m_depthZ; z++)
                        {
                            m_heights[x, z] = Mathf.Lerp(m_heights[x, z], hmNewValues[x, z], hmLerpMask[m_widthInvX * x, m_depthInvZ * z]);
                        }
                    }
                }
                else
                {
                    for (int x = 0; x < m_widthX; x++)
                    {
                        for (int z = 0; z < m_depthZ; z++)
                        {
                            m_heights[x, z] = Mathf.Lerp(m_heights[x, z], hmNewValues[x, z], hmLerpMask[x, z]);
                        }
                    }
                }
            }
            m_isDirty = true;
            return this;
        }

        /// <summary>
        /// Sum the content of the heightmap
        /// </summary>
        /// <returns>Sum of heightmap content</returns>
        public float Sum()
        {
            float sum = 0f;
            for (int x = 0; x < m_widthX; x++)
            {
                for (int z = 0; z < m_depthZ; z++)
                {
                    sum += m_heights[x, z];
                }
            }
            return sum;
        }

        /// <summary>
        /// Average the content of the heightmap
        /// </summary>
        /// <returns>Average of heightmap content</returns>
        public float Average()
        {
            return Sum() / (m_widthX * m_depthZ);
        }

        /// <summary>
        /// Adjust to the power of the exponent provided
        /// </summary>
        /// <param name="exponent">Exponent to power of</param>
        public HeightMap Power(float exponent)
        {
            for (int x = 0; x < m_widthX; x++)
            {
                for (int z = 0; z < m_depthZ; z++)
                {
                    m_heights[x, z] = Mathf.Pow(m_heights[x, z], exponent);
                }
            }
            m_isDirty = true;
            return this;
        }

        /// <summary>
        /// Adjust contrast by the value provided
        /// </summary>
        /// <param name="contrast">Contrast value</param>
        public HeightMap Contrast(float contrast)
        {
            for (int x = 0; x < m_widthX; x++)
            {
                for (int z = 0; z < m_depthZ; z++)
                {
                    m_heights[x, z] = ((m_heights[x, z] - 0.5f) * contrast) + 0.5f;
                }
            }
            m_isDirty = true;
            return this;
        }

        #endregion

        #region File Operations

        /// <summary>
        /// Save ourselves into the file provided
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="input"></param>
        public void SaveToBinaryFile(string fileName)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, m_widthX);
            formatter.Serialize(stream, m_depthZ);
            formatter.Serialize(stream, m_metaData);
            formatter.Serialize(stream, m_heights);
            stream.Close();
            m_isDirty = false;
        }

        /// <summary>
        /// Load ourselves from the file provided
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadFromBinaryFile(string fileName)
        {
            if (!System.IO.File.Exists(fileName))
            {
                Debug.LogError("Could not locate file : " + fileName);
                return;
            }

            Reset();
            BinaryFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            m_widthX = (int)formatter.Deserialize(stream);
            m_depthZ = (int)formatter.Deserialize(stream);
            m_metaData = (byte[])formatter.Deserialize(stream);
            m_heights = (float[,])formatter.Deserialize(stream);
            stream.Close();
            m_widthInvX = 1f / (float)(m_widthX);
            m_depthInvZ = 1f / (float)(m_depthZ);
            m_isPowerOf2 = Gaia.GaiaUtils.Math_IsPowerOf2(m_widthX) && Gaia.GaiaUtils.Math_IsPowerOf2(m_depthZ);
            m_isDirty = false;
        }

/*
        /// <summary>
        /// Save to a system bitmap - note - you will lose precision as we are going to from 32 bit to 8 bit values
        /// </summary>
        /// <returns>System bitmap</returns>
        public System.Drawing.Bitmap SaveToBitmap(PixelFormat format)
        {
            System.Drawing.Bitmap bm = new Bitmap(m_widthX, m_depthZ, format);
            if (format == PixelFormat.Format8bppIndexed)
            {
                BitmapData data = bm.LockBits(new Rectangle(0, 0, m_widthX, m_depthZ), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
                byte[] bytes = new byte[data.Height * data.Stride];
                Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
                for (int x = 0; x < m_widthX; x++)
                {
                    for (int z = 0; z < m_depthZ; z++)
                    {
                        bytes[x * data.Stride + z] = (byte)(m_heights[x, z] * 255f);
                    }
                }
                // Copy the bytes from the byte array into the image
                Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);
                bm.UnlockBits(data);
            }
            else
            {
                for (int x = 0; x < m_widthX; x++)
                {
                    for (int z = 0; z < m_depthZ; z++)
                    {
                        int value = (int)(m_heights[x, z] * 255f);
                        bm.SetPixel(x, z, System.Drawing.Color.FromArgb(value, value, value, value));
                    }
                }
            }
            return bm;
        }

        public void LoadFromBitmap(System.Drawing.Bitmap bitmap)
        {
            if (bitmap == null)
            {
                Debug.LogError("No data provided");
                return;
            }
            Reset();
            m_widthX = bitmap.Width;
            m_depthZ = bitmap.Height;
            m_widthXOpt = m_widthX - 1f;
            m_depthZOpt = m_depthZ - 1f;
            m_widthInvX = 1f / (float)(m_widthX);
            m_depthInvZ = 1f / (float)(m_depthZ);
            m_heights = new float[m_widthX, m_depthZ];
            m_isPowerOf2 = PWCommon.Utils.Math_IsPowerOf2(m_widthX) && PWCommon.Utils.Math_IsPowerOf2(m_depthZ);
            m_statMinVal = m_statMaxVal = 0f;
            m_statSumVals = 0;
            m_metaData = new byte[0];
            m_isDirty = false;

            for (int x = 0; x < m_widthX; x++)
            {
                for (int z = 0; z < m_depthZ; z++)
                {
                    System.Drawing.Color c = bitmap.GetPixel(x, z);
                    //Convert to greyscale
                    m_heights[x, z] = (float)(0.29899999499321 * (double)((float)c.R/255f) + 0.587000012397766 * (double)((float)c.G/255f) + 57.0 / 500.0 * (double)((float)c.B/255f));
                }
            }
        }
*/

        /// <summary>
        /// Load ourselves from the byte array provided
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadFromByteArray(byte[] source)
        {
            if (source == null)
            {
                Debug.LogError("No data provided");
                return;
            }

            Reset();
            BinaryFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream(source);
            m_widthX = (int)formatter.Deserialize(stream);
            m_depthZ = (int)formatter.Deserialize(stream);
            m_metaData = (byte[])formatter.Deserialize(stream);
            m_heights = (float[,])formatter.Deserialize(stream);
            stream.Close();
            m_widthInvX = 1f / (float)(m_widthX);
            m_depthInvZ = 1f / (float)(m_depthZ);
            m_isPowerOf2 = Gaia.GaiaUtils.Math_IsPowerOf2(m_widthX) && Gaia.GaiaUtils.Math_IsPowerOf2(m_depthZ);
            m_isDirty = false;
        }

        /// <summary>
        /// Load ourselves from the raw file provided
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadFromRawFile(string fileName, GaiaConstants.RawByteOrder byteOrder, ref GaiaConstants.RawBitDepth bitDepth, ref int resolution)
        {
            if (!System.IO.File.Exists(fileName))
            {
                Debug.LogError("Could not locate raw file : " + fileName);
                return;
            }

            Reset();
            using (FileStream fileStream = File.OpenRead(fileName))
            {
                using (BinaryReader br = (byteOrder == GaiaConstants.RawByteOrder.IBM) ? new BinaryReader(fileStream) : new BinaryReaderMac(fileStream))
				{
					if (bitDepth == GaiaConstants.RawBitDepth.Sixteen)
					{
						if (fileStream.Length % 2 == 0)
						{
							resolution = m_widthX = m_depthZ = Mathf.CeilToInt(Mathf.Sqrt(fileStream.Length / 2));
							m_heights = new float[m_widthX, m_depthZ];
							for (int x = 0; x < m_widthX; x++)
							{
								for (int z = 0; z < m_depthZ; z++)
								{
									m_heights[x, z] = (float)br.ReadUInt16() / 65535.0f; //Should consider doing the unity HM switch here							
								}
							}
						}
						// Can't be 16 bit raw, try to process as 8bit
						else
						{
							m_widthX = m_depthZ = Mathf.CeilToInt(Mathf.Sqrt(fileStream.Length));
#if UNITY_EDITOR
							if (UnityEditor.EditorUtility.DisplayDialog("OOPS!",
								"The file received is not 16-bit RAW.\n" +
								"If processed as an 8-bit RAW it's resolution would be " + m_widthX + " x " + m_widthX + ".\n" +
								"Do you want to attempt to scan it as an 8-bit RAW?\n" +
								"(Tip: Check if the resolution matches the resolution of what you are trying to scan)\n\n" +
								"WARNING: 8-bit RAW files have very poor precision and result in terraced stamps.\n\n",
								"YES", "NO"))
							{
								bitDepth = GaiaConstants.RawBitDepth.Eight;
								resolution = m_widthX;
								m_heights = new float[m_widthX, m_depthZ];
								for (int x = 0; x < m_widthX; x++)
								{
									for (int z = 0; z < m_depthZ; z++)
									{
										m_heights[x, z] = (float)br.ReadByte() / 255.0f; //Should consider doing the unity HM switch here							
									}
								}
							}
#endif
						}
					}
					else
					{
						resolution = m_widthX = m_depthZ = Mathf.CeilToInt(Mathf.Sqrt(fileStream.Length));
						m_heights = new float[m_widthX, m_depthZ];
						for (int x = 0; x < m_widthX; x++)
						{
							for (int z = 0; z < m_depthZ; z++)
							{
								m_heights[x, z] = (float)br.ReadByte() / 255.0f; //Should consider doing the unity HM switch here							
							}
						}
					}
                }
                fileStream.Close();
            }
            m_widthInvX = 1f / (float)(m_widthX);
            m_depthInvZ = 1f / (float)(m_depthZ);
            m_isPowerOf2 = Gaia.GaiaUtils.Math_IsPowerOf2(m_widthX) && Gaia.GaiaUtils.Math_IsPowerOf2(m_depthZ);
            m_isDirty = false;
        }

        #endregion

        #region Debug

        /// <summary>
        /// A handy utility to dump the content of a heightmap.
        /// Example: for unity terrain heightmaps use DumpMap(9f, 0, "", true).
        /// </summary>
        /// <param name="scaleValue">Amount to scale the value by</param>
        /// <param name="precision">The number of decimal points to show</param>
        /// <param name="spacer">The spacer to show (or not)</param>
        /// <param name="flip">Whether or not to flip the lookup</param>
        public void DumpMap(float scaleValue, int precision, string spacer, bool flip)
        {
            StringBuilder debugStr = new StringBuilder();
            string format = "";
            if (precision == 0)
            {
                format = "{0:0}";
            }
            else
            {
                format = "{0:0.";
                for (int p = 0; p < precision; p++)
                {
                    format += "0";
                }
                format += "}";
            }
            if (!string.IsNullOrEmpty(spacer))
            {
                format += spacer;
            }

            for (int x = 0; x < m_widthX; x++)
            {
                for (int z = 0; z < m_depthZ; z++)
                {
                    if (!flip)
                    {
                        debugStr.AppendFormat(format, m_heights[x, z] * scaleValue);
                    }
                    else
                    {
                        debugStr.AppendFormat(format, m_heights[z, x] * scaleValue);
                    }
                }
                debugStr.AppendLine();
            }

            Debug.Log(debugStr.ToString());
        }

        /// <summary>
        /// Dump a specific row
        /// </summary>
        /// <param name="rowX">The row to dump</param>
        /// <param name="scaleValue">Amount to scale the value by</param>
        /// <param name="precision">The number of decimal points to show</param>
        /// <param name="spacer">The spacer to show (or not)</param>
        public void DumpRow(int rowX, float scaleValue, int precision, string spacer)
        {
            StringBuilder debugStr = new StringBuilder();
            string format = "";
            if (precision == 0)
            {
                format = "{0:0}";
            }
            else
            {
                format = "{0:0.";
                for (int p = 0; p < precision; p++)
                {
                    format += "0";
                }
                format += "}";
            }
            if (!string.IsNullOrEmpty(spacer))
            {
                format += spacer;
            }

            float [] values = GetRow(rowX);
            for (int v = 0; v < values.Length; v++)
            {
                debugStr.AppendFormat(format, values[v] * scaleValue);
            }

            Debug.Log(debugStr.ToString());
        }

        /// <summary>
        /// Dump a specific column
        /// </summary>
        /// <param name="columnZ">The column to dump</param>
        /// <param name="scaleValue">Amount to scale the value by</param>
        /// <param name="precision">The number of decimal points to show</param>
        /// <param name="spacer">The spacer to show (or not)</param>
        public void DumpColumn(int columnZ, float scaleValue, int precision, string spacer)
        {
            StringBuilder debugStr = new StringBuilder();
            string format = "";
            if (precision == 0)
            {
                format = "{0:0}";
            }
            else
            {
                format = "{0:0.";
                for (int p = 0; p < precision; p++)
                {
                    format += "0";
                }
                format += "}";
            }
            if (!string.IsNullOrEmpty(spacer))
            {
                format += spacer;
            }

            float[] values = GetColumn(columnZ);
            for (int v = 0; v < values.Length; v++)
            {
                debugStr.AppendFormat(format, values[v] * scaleValue);
            }

            Debug.Log(debugStr.ToString());
        }

        #endregion
    }
}