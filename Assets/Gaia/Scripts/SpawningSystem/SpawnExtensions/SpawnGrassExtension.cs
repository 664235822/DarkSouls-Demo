using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gaia
{
    /// <summary>
    /// A spawner extension that will add grass to an area based on the mask passed in on a spawned object.
    /// </summary>

    public class SpawnGrassExtension : SpawnRuleExtension
    {
        /// <summary>
        /// The zero based grass slot to apply
        /// </summary>
        [Tooltip("The zero based index of the grass texture to be applied.")]
        public int m_grassIndex = 0;

        /// <summary>
        /// Minimum strength
        /// </summary>
        [Tooltip("The minimum strength of the grass to be applied."), Range(0, 15)]
        public int m_minGrassStrenth = 0;

        /// <summary>
        /// Maximum strength
        /// </summary>
        [Tooltip("The maximum strength of the grass to be applied."), Range(0, 15)]
        public int m_maxGrassStrength = 15;

        /// <summary>
        /// The mask to apply
        /// </summary>
        [Tooltip("The mask used to display this grass.")]
        public Texture2D m_grassMask;

        /// <summary>
        /// Normalise the mask
        /// </summary>
        [Tooltip("Whether or not to normalise the mask. Normalisation allows the full dynamic range of the mask to be used.")]
        public bool m_normaliseMask = true;

        /// <summary>
        /// Invert the mask
        /// </summary>
        [Tooltip("Whether or not to invert the mask.")]
        public bool m_invertMask = false;

        /// <summary>
        /// Flip the mask on x, z axis
        /// </summary>
        [Tooltip("Whether or not to flip the mask.")]
        public bool m_flipMask = false;

        /// <summary>
        /// The amount to scale the mask by when applying it
        /// </summary>
        [Tooltip("The mask scale with respect to the areas bounds of this spawn."), Range(0.1f, 10f)]
        public float m_scaleMask = 1f;

        /// <summary>
        /// The texture heightmap
        /// </summary>
        private UnityHeightMap m_textureHM;

        /// <summary>
        /// Initialise the extension
        /// </summary>
        public override void Initialise()
        {
            m_textureHM = new UnityHeightMap(m_grassMask);
            if (m_normaliseMask && m_textureHM.HasData())
            {
                m_textureHM.Normalise();
            }
            if (m_invertMask && m_textureHM.HasData())
            {
                m_textureHM.Invert();
            }
            if (m_flipMask && m_textureHM.HasData())
            {
                m_textureHM.Flip();
            }
        }

        /// <summary>
        /// Whether or not this extension impacts details
        /// </summary>
        /// <returns></returns>
        public override bool AffectsDetails()
        {
            return true;
        }

        /// <summary>
        /// Called after the spawn is complete
        /// </summary>
        /// <param name="spawnRule">The rule that generated this call</param>
        /// <param name="spawnInfo">The spawninfo structure for this call</param>
        public override void PostSpawn(SpawnRule spawnRule, ref SpawnInfo spawnInfo)
        {
            //See if we can load the texture
            if (m_textureHM == null || !m_textureHM.HasData())
            {
                return;
            }

            //Get the terrain
            Terrain t = Gaia.TerrainHelper.GetTerrain(spawnInfo.m_hitLocationWU);
            if (t == null)
            {
                return;
            }

            //Get the cached detail maps
            List<HeightMap> detailMaps = spawnInfo.m_spawner.GetDetailMaps(spawnInfo.m_hitTerrain.GetInstanceID());
            if (detailMaps == null || m_grassIndex >= detailMaps.Count)
            {
                return;
            }

            //Make some speedy calculations
            float widthWU = spawnInfo.m_hitTerrain.terrainData.size.x;
            float depthWU = spawnInfo.m_hitTerrain.terrainData.size.z;
            float radiusWU = spawnRule.GetMaxScaledRadius(ref spawnInfo) * m_scaleMask;
            float xStartWU = spawnInfo.m_hitLocationWU.x - (radiusWU / 2f);
            float zStartWU = spawnInfo.m_hitLocationWU.z - (radiusWU / 2f);
            float xEndWU = xStartWU + radiusWU;
            float zEndWU = zStartWU + radiusWU;
            float stepWU = 0.5f;
            Vector3 locationWU = Vector3.zero;
            float xRotNU = 0f, zRotNU = 0f;
            float grassRange = (float)(m_maxGrassStrength - m_minGrassStrenth);

            for (float x = xStartWU; x < xEndWU; x += stepWU)
            {
                for (float z = zStartWU; z < zEndWU; z += stepWU)
                {
                    //Need to rotate x,z around the pivot by the rotation angle
                    locationWU = new Vector3(x, spawnInfo.m_hitLocationWU.y, z);
                    locationWU = Gaia.GaiaUtils.RotatePointAroundPivot(locationWU, spawnInfo.m_hitLocationWU, new Vector3(0f, spawnInfo.m_spawnRotationY, 0f));

                    //Now normalise the result
                    xRotNU = (locationWU.x / widthWU) + 0.5f;
                    zRotNU = (locationWU.z / depthWU) + 0.5f;

                    //Drop out if out of bounds
                    if (xRotNU < 0f || xRotNU >= 1f || zRotNU < 0f || zRotNU > 1f)
                    {
                        continue;
                    }

                    detailMaps[m_grassIndex][zRotNU, xRotNU] = Mathf.Clamp((m_textureHM[(x - xStartWU) / radiusWU, (z - zStartWU) / radiusWU] * grassRange) + m_minGrassStrenth, 0f, 15f); 
                }
            }
        }
    }
}
