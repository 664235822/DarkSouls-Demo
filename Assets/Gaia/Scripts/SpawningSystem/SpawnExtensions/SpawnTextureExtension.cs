using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gaia
{
    /// <summary>
    /// A spawner extension that will retexture an area based on the mask passed in.
    /// </summary>
    public class SpawnTextureExtension : SpawnRuleExtension
    {
        /// <summary>
        /// The zero based texture slot to apply
        /// </summary>
        [Tooltip("The zero based index of the terrain texture to be applied.")]
        public int m_textureIndex = 0;

        /// <summary>
        /// The texure mask to apply
        /// </summary>
        [Tooltip("The mask used to display this texture.")]
        public Texture2D m_textureMask;

        /// <summary>
        /// Normalise the texture mask
        /// </summary>
        [Tooltip("Whether or not to normalise the mask. Normalisation allows the full dynamic range of the mask to be used.")]
        public bool m_normaliseMask = true;

        /// <summary>
        /// Invert the texture mask
        /// </summary>
        [Tooltip("Whether or not to invert the mask.")]
        public bool m_invertMask = false;

        /// <summary>
        /// Flip the x, z of the texture mask
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
            m_textureHM = new UnityHeightMap(m_textureMask);
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
        /// Whether or not this extentsion impacts textures
        /// </summary>
        /// <returns></returns>
        public override bool AffectsTextures()
        {
            return true;
        }

        /// <summary>
        /// Call this to get the degree of completion in range 0..1.
        /// </summary>
        /// <param name="spawner">The spawner this is for</param>
        /// <param name="objSpawned">The object that was spawned in the spawn step (if relevant)</param>
        /// <returns>Return the completion in range of 0 (not started) .. 1 (completed)</returns>
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

            //Get the cached texture maps
            List<HeightMap> txtMaps = spawnInfo.m_spawner.GetTextureMaps(spawnInfo.m_hitTerrain.GetInstanceID());
            if (txtMaps == null || m_textureIndex >= txtMaps.Count)
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
            float currStrength = 0f, newStrength = 1f;

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

                    //Only interested in increasing values
                    currStrength = txtMaps[m_textureIndex][zRotNU, xRotNU];
                    newStrength = m_textureHM[(x - xStartWU) / radiusWU, (z - zStartWU) / radiusWU];
                    if (newStrength > currStrength)
                    {
                        float delta = newStrength - currStrength;
                        float theRest = 1f - currStrength;
                        float adjustment = 0f;
                        if (theRest != 0f)
                        {
                            adjustment = 1f - (delta / theRest);
                        }

                        for (int idx = 0; idx < txtMaps.Count; idx++)
                        {
                            if (idx == m_textureIndex)
                            {
                                txtMaps[idx][zRotNU, xRotNU] = newStrength;
                            }
                            else
                            {
                                txtMaps[idx][zRotNU, xRotNU] *= adjustment;
                            }
                        }
                    }
                }
            }

            spawnInfo.m_spawner.SetTextureMapsDirty();
        }
    }
}