using UnityEngine;
using System.Collections.Generic;
using System;
using Gaia.FullSerializer;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gaia
{
    [System.Serializable]
    public class GaiaResource : ScriptableObject
    {
        [Tooltip("Unique identifier for these resources."), HideInInspector]
        public string m_resourcesID = Guid.NewGuid().ToString();
        [Tooltip("Resource name")]
        public string m_name = "Gaia Resource";
        [Tooltip("The absolute height of the sea or water table in meters. All spawn criteria heights are calculated relative to this. This can also be thought of as the water level. This value is sourced from the defaults file, and managed on a session by session basis.")]
        public float m_seaLevel = 100f;
        [Tooltip("The beach height in meters. Beaches are spawned at sea level and are extended for this height above sea level. This is used when creating default spawn rules in order to create a beach in the zone between water and land.")]
        public float m_beachHeight = 5f;
        [Tooltip("Terrain height.")]
        public float m_terrainHeight = 1000f;
        [Tooltip("Texture prototypes and fitness criteria.")]
        public ResourceProtoTexture[]       m_texturePrototypes = new ResourceProtoTexture[0];
        [Tooltip("Detail prototypes, dna and fitness criteria.")]
        public ResourceProtoDetail[]        m_detailPrototypes = new ResourceProtoDetail[0];
        [Tooltip("Tree prototypes, dna and fitness criteria.")]
        public ResourceProtoTree[]          m_treePrototypes = new ResourceProtoTree[0];
        [Tooltip("Game object prototypes, dna and fitness criteria.")]
        public ResourceProtoGameObject[]    m_gameObjectPrototypes = new ResourceProtoGameObject[0];
        //[Tooltip("Stamp prototypes, dna and fitness criteria.")]
        //public ResourceProtoStamp[]         m_stampPrototypes = new ResourceProtoStamp[0];

        /// <summary>
        /// Set up the asset associations, return true if something changes. Can only be run when the editor is present.
        /// </summary>
        /// <returns>True if something changes</returns>
        public bool SetAssetAssociations()
        {
            int idx = 0;
            bool isChanged = false;

            ResourceProtoTexture texturePrototype;
            for (idx = 0; idx < m_texturePrototypes.GetLength(0); idx++)
            {
                texturePrototype = m_texturePrototypes[idx];
                if (texturePrototype.SetAssetAssociations())
                {
                    isChanged = true;
                }
            }

            ResourceProtoDetail detailPrototype;
            for (idx = 0; idx < m_detailPrototypes.GetLength(0); idx++)
            {
                detailPrototype = m_detailPrototypes[idx];
                if (detailPrototype.SetAssetAssociations())
                {
                    isChanged = true;
                }
            }

            ResourceProtoTree treePrototype;
            for (idx = 0; idx < m_treePrototypes.GetLength(0); idx++)
            {
                treePrototype = m_treePrototypes[idx];
                if (treePrototype.SetAssetAssociations())
                {
                    isChanged = true;
                }
            }

            ResourceProtoGameObject goPrototype;
            for (idx = 0; idx < m_gameObjectPrototypes.GetLength(0); idx++)
            {
                goPrototype = m_gameObjectPrototypes[idx];
                if (goPrototype.SetAssetAssociations())
                {
                    isChanged = true;
                }
            }

            return isChanged;
        }

        /// <summary>
        /// Associate any unallocated assets to this resource. Return true if something changes.
        /// </summary>
        /// <returns>True of the resources were changed in any way</returns>
        public bool AssociateAssets()
        {
            int idx = 0;
            bool isChanged = false;

            ResourceProtoTexture texturePrototype;
            for (idx = 0; idx < m_texturePrototypes.GetLength(0); idx++)
            {
                texturePrototype = m_texturePrototypes[idx];
                if (texturePrototype.AssociateAssets())
                {
                    isChanged = true;
                }
            }

            ResourceProtoDetail detailPrototype;
            for (idx = 0; idx < m_detailPrototypes.GetLength(0); idx++)
            {
                detailPrototype = m_detailPrototypes[idx];
                if (detailPrototype.AssociateAssets())
                {
                    isChanged = true;
                }
            }

            ResourceProtoTree treePrototype;
            for (idx = 0; idx < m_treePrototypes.GetLength(0); idx++)
            {
                treePrototype = m_treePrototypes[idx];
                if (treePrototype.AssociateAssets())
                {
                    isChanged = true;
                }
            }

            ResourceProtoGameObject goPrototype;
            for (idx = 0; idx < m_gameObjectPrototypes.GetLength(0); idx++)
            {
                goPrototype = m_gameObjectPrototypes[idx];
                if (goPrototype.AssociateAssets())
                {
                    isChanged = true;
                }
            }

            return isChanged;
        }

        /// <summary>
        /// Delete all the prototypes
        /// </summary>
        public void DeletePrototypes()
        {
            m_texturePrototypes = new ResourceProtoTexture[0];
            m_detailPrototypes = new ResourceProtoDetail[0];
            m_treePrototypes = new ResourceProtoTree[0];
            m_gameObjectPrototypes = new ResourceProtoGameObject[0];
            //m_stampPrototypes = new ResourceProtoStamp[0];
        }

        /// <summary>
        /// Check if any of the resource prototypes are missing from the terrain
        /// </summary>
        /// <returns>True if any of the prototypes are missing from the terrain</returns>
        public bool PrototypesMissingFromTerrain()
        {
            Terrain terrain = Gaia.TerrainHelper.GetActiveTerrain();
            if (terrain == null)
            {
                Debug.LogWarning("Could not check assets in terrain as no terrain has been supplied.");
                return false;
            }

            int idx = 0;
            for (idx = 0; idx < m_texturePrototypes.GetLength(0); idx++)
            {
                if (PrototypeMissingFromTerrain(GaiaConstants.SpawnerResourceType.TerrainTexture, idx))
                {
                    return true;
                }
            }
            for (idx = 0; idx < m_detailPrototypes.GetLength(0); idx++)
            {
                if (PrototypeMissingFromTerrain(GaiaConstants.SpawnerResourceType.TerrainDetail, idx))
                {
                    return true;
                }
            }
            for (idx = 0; idx < m_treePrototypes.GetLength(0); idx++)
            {
                if (PrototypeMissingFromTerrain(GaiaConstants.SpawnerResourceType.TerrainTree, idx))
                {
                    return true;
                }
            }
            for (idx = 0; idx < m_gameObjectPrototypes.GetLength(0); idx++)
            {
                if (PrototypeMissingFromTerrain(GaiaConstants.SpawnerResourceType.GameObject, idx))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Check if a specific prototype is missing from the terrain
        /// </summary>
        /// <param name="resourceType">Type of resource</param>
        /// <param name="resourceIdx">Index of the resource</param>
        /// <returns>True if it is missing, false otherwise</returns>
        public bool PrototypeMissingFromTerrain(GaiaConstants.SpawnerResourceType resourceType, int resourceIdx)
        {
            if (PrototypeIdxInTerrain(resourceType, resourceIdx) == -1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get the prototype index in the actual terrain and return its index, otherwise -1 signifies not found
        /// </summary>
        /// <param name="resourceType">Type of resource to check</param>
        /// <param name="resourceIdx">Index of the resource to check</param>
        /// <returns>Index in the terrain if found, -1 if not found</returns>
        public int PrototypeIdxInTerrain(GaiaConstants.SpawnerResourceType resourceType, int resourceIdx)
        {
            //Error index -1 = not found
            int errorIdx = -1;
            int localTerrainIdx = 0;

            //Check to see if we have a terrain
            Terrain terrain = Gaia.TerrainHelper.GetActiveTerrain();
            if (terrain == null)
            {
                return errorIdx;
            }

            //Check the resource index
            if (ResourceIdxOutOfBounds(resourceType, resourceIdx))
            {
                return errorIdx;
            }

            //Check to see if its actually available in unity
            if (!ResourceIsInUnity(resourceType, resourceIdx))
            {
                return errorIdx;
            }

            //Now see if the resource exists within that terrain
            switch (resourceType)
            {
                case GaiaConstants.SpawnerResourceType.TerrainTexture:
#if UNITY_2018_4_OR_NEWER
                    ResourceProtoTexture splat = m_texturePrototypes[resourceIdx];
                    foreach (TerrainLayer proto in terrain.terrainData.terrainLayers)
                    {
                        if (GaiaCommon1.Utils.IsSameTexture(splat.m_texture, proto.diffuseTexture, false) == true)
                        {
                            return localTerrainIdx;
                        }
                        localTerrainIdx++;
                    }
#else
                    ResourceProtoTexture splat = m_texturePrototypes[resourceIdx];
                    foreach (GaiaSplatPrototype proto in GaiaSplatPrototype.GetGaiaSplatPrototypes(terrain))
                    {
                        if (GaiaCommon1.Utils.IsSameTexture(splat.m_texture, proto.texture, false) == true)
                        {
                            return localTerrainIdx;
                        }
                        localTerrainIdx++;
                    }
#endif
                    return errorIdx;
                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                    ResourceProtoDetail detail = m_detailPrototypes[resourceIdx];
                    foreach (DetailPrototype proto in terrain.terrainData.detailPrototypes)
                    {
                        if (detail.m_renderMode == proto.renderMode)
                        {
                            if (GaiaCommon1.Utils.IsSameTexture(detail.m_detailTexture, proto.prototypeTexture, false))
                            {
                                return localTerrainIdx;
                            }
                            if (GaiaCommon1.Utils.IsSameGameObject(detail.m_detailProtoype, proto.prototype, false))
                            {
                                return localTerrainIdx;
                            }
                        }
                        localTerrainIdx++;
                    }
                    return errorIdx;
                case GaiaConstants.SpawnerResourceType.TerrainTree:
                    ResourceProtoTree tree = m_treePrototypes[resourceIdx];
                    foreach (TreePrototype proto in terrain.terrainData.treePrototypes)
                    {
                        if (GaiaCommon1.Utils.IsSameGameObject(tree.m_desktopPrefab, proto.prefab, false))
                        {
                            return localTerrainIdx;
                        }
                        localTerrainIdx++;
                    }
                    return errorIdx;
                case GaiaConstants.SpawnerResourceType.GameObject:
                    return resourceIdx;
            }
            return errorIdx;
        }

        /// <summary>
        /// Check to see if a resource index is out of bounds
        /// </summary>
        /// <param name="resourceType">Resource type</param>
        /// <param name="resourceIdx">Resource index</param>
        /// <returns>True if out of bounds, false otherwise</returns>
        public bool ResourceIdxOutOfBounds(GaiaConstants.SpawnerResourceType resourceType, int resourceIdx)
        {
            switch (resourceType)
            {
                case GaiaConstants.SpawnerResourceType.TerrainTexture:
                    if (resourceIdx < 0 || resourceIdx >= m_texturePrototypes.GetLength(0))
                    {
                        return true;
                    }
                    return false;
                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                    if (resourceIdx < 0 || resourceIdx >= m_detailPrototypes.GetLength(0))
                    {
                        return true;
                    }
                    return false;
                case GaiaConstants.SpawnerResourceType.TerrainTree:
                    if (resourceIdx < 0 || resourceIdx >= m_treePrototypes.GetLength(0))
                    {
                        return true;
                    }
                    return false;
                case GaiaConstants.SpawnerResourceType.GameObject:
                    if (resourceIdx < 0 || resourceIdx >= m_gameObjectPrototypes.GetLength(0))
                    {
                        return true;
                    }
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Check to see if the resource is actually available to unity
        /// </summary>
        /// <param name="resourceType">Resource type</param>
        /// <param name="resourceIdx"Resource idx></param>
        /// <returns>True if the resource could be found, false otherwise</returns>
        public bool ResourceIsInUnity(GaiaConstants.SpawnerResourceType resourceType, int resourceIdx)
        {
            if (ResourceIdxOutOfBounds(resourceType, resourceIdx))
            {
                return false;
            }

            //Now see if the resource exists within that terrain
            switch (resourceType)
            {
                case GaiaConstants.SpawnerResourceType.TerrainTexture:
                    ResourceProtoTexture splat = m_texturePrototypes[resourceIdx];
                    if (splat.m_texture == null)
                    {
                        return false;
                    }
                    return true;
                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                    ResourceProtoDetail detail = m_detailPrototypes[resourceIdx];
                    if (detail.m_detailTexture == null && detail.m_detailProtoype == null)
                    {
                        return false;
                    }
                    return true;
                case GaiaConstants.SpawnerResourceType.TerrainTree:
                    ResourceProtoTree tree = m_treePrototypes[resourceIdx];
                    if (tree.m_desktopPrefab == null)
                    {
                        return false;
                    }
                    return true;
                case GaiaConstants.SpawnerResourceType.GameObject:
                    ResourceProtoGameObject go = m_gameObjectPrototypes[resourceIdx];
                    if (go.m_instances[0] == null)
                    {
                        return false;
                    }
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Pick up the prototypes being used in the terrain
        /// </summary>
        public void UpdatePrototypesFromTerrain()
        {
            Terrain terrain = Gaia.TerrainHelper.GetActiveTerrain();
            if (terrain == null)
            {
                Debug.LogWarning("Can not update prototypes from the terrain as there is no terrain currently active in this scene.");
                return;
            }

            //Name storage to stop replicated names
            Dictionary<string, string> names = new Dictionary<string, string>();

            //Create some useful defaults
            m_terrainHeight = terrain.terrainData.size.y;


            int idx;
            SpawnCritera criteria;
            GaiaSplatPrototype terrainTextureProto;
            ResourceProtoTexture resourceTextureProto;
            List<ResourceProtoTexture> resourceTexturePrototypes = new List<ResourceProtoTexture>(m_texturePrototypes);

            var splatPrototypes = GaiaSplatPrototype.GetGaiaSplatPrototypes(terrain);

            while (resourceTexturePrototypes.Count > splatPrototypes.Length)
            {
                resourceTexturePrototypes.RemoveAt(resourceTexturePrototypes.Count - 1);
            }
            for (idx = 0; idx < splatPrototypes.Length; idx++)
            {
                terrainTextureProto = splatPrototypes[idx];
                if (idx < resourceTexturePrototypes.Count)
                {
                    resourceTextureProto = resourceTexturePrototypes[idx];
                }
                else
                {
                    resourceTextureProto = new ResourceProtoTexture();
                    resourceTexturePrototypes.Add(resourceTextureProto);
                }
                resourceTextureProto.m_name = GetUniqueName(terrainTextureProto.texture.name, ref names);
                resourceTextureProto.m_texture = terrainTextureProto.texture;
                resourceTextureProto.m_normal = terrainTextureProto.normalMap;
                resourceTextureProto.m_offsetX = terrainTextureProto.tileOffset.x;
                resourceTextureProto.m_offsetY = terrainTextureProto.tileOffset.y;
                resourceTextureProto.m_sizeX = terrainTextureProto.tileSize.x;
                resourceTextureProto.m_sizeY = terrainTextureProto.tileSize.y;
                resourceTextureProto.m_metalic = terrainTextureProto.metallic;
                resourceTextureProto.m_smoothness = terrainTextureProto.smoothness;

                //Handle empty spawn criteria
                if (resourceTextureProto.m_spawnCriteria.Length == 0)
                {
                    resourceTextureProto.m_spawnCriteria = new SpawnCritera[1];
                    criteria = new SpawnCritera();
                    criteria.m_isActive = true;
                    criteria.m_virginTerrain = false;
                    criteria.m_checkType = Gaia.GaiaConstants.SpawnerLocationCheckType.PointCheck;

                    //Create some reasonable terrain based starting points
                    switch (idx)
                    {
                        case 0: //Base
                            {
                                criteria.m_checkHeight = true;
                                criteria.m_minHeight = m_seaLevel * -1f;
                                criteria.m_maxHeight = m_terrainHeight - m_seaLevel;
                                criteria.m_heightFitness = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));
                                criteria.m_checkSlope = false;
                                criteria.m_minSlope = 0f;
                                criteria.m_maxSlope = 90f;
                                criteria.m_slopeFitness = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f));
                                criteria.m_checkProximity = false;
                                criteria.m_checkTexture = false;
                                break;
                            }

                        case 1: //Grass1
                            {
                                criteria.m_checkHeight = true;
                                criteria.m_minHeight = 1f;
                                criteria.m_maxHeight = m_terrainHeight - m_seaLevel;
                                criteria.m_heightFitness = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.01f, 1f), new Keyframe(1f, 1f));
                                criteria.m_checkSlope = false;
                                criteria.m_minSlope = 0f;
                                criteria.m_maxSlope = 90;
                                criteria.m_slopeFitness = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f));
                                criteria.m_checkProximity = false;
                                criteria.m_checkTexture = false;
                                break;
                            }

                        case 2: //Grass2
                            {
                                criteria.m_checkHeight = true;
                                criteria.m_minHeight = 2f;
                                criteria.m_maxHeight = m_terrainHeight - m_seaLevel;
                                criteria.m_heightFitness = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.02f, 1f), new Keyframe(1f, 1f));
                                criteria.m_checkSlope = true;
                                criteria.m_minSlope = 0f;
                                criteria.m_maxSlope = 90f;
                                criteria.m_slopeFitness = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.1f, 1f), new Keyframe(1f, 1f));
                                criteria.m_checkProximity = false;
                                criteria.m_checkTexture = false;
                                break;
                            }

                        case 3: //Cliffs
                            {
                                criteria.m_checkHeight = false;
                                criteria.m_minHeight = m_seaLevel * -1f;
                                criteria.m_maxHeight = m_terrainHeight - m_seaLevel;
                                criteria.m_heightFitness = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));
                                criteria.m_checkSlope = true;
                                criteria.m_minSlope = 15f;
                                criteria.m_maxSlope = 90f;
                                criteria.m_slopeFitness = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.2f, 1f), new Keyframe(1f, 1f));
                                criteria.m_checkProximity = false;
                                criteria.m_checkTexture = false;
                                break;
                            }

                        default:
                            {
                                criteria.m_isActive = false;
                                criteria.m_checkHeight = false;
                                criteria.m_minHeight = UnityEngine.Random.Range(m_beachHeight - (m_beachHeight / 4f), m_beachHeight * 2f);
                                criteria.m_maxHeight = m_terrainHeight - m_seaLevel;
                                criteria.m_heightFitness = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));
                                criteria.m_checkSlope = false;
                                criteria.m_minSlope = 0f;
                                criteria.m_maxSlope = 90f;
                                criteria.m_slopeFitness = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f));
                                criteria.m_checkProximity = false;
                                criteria.m_checkTexture = false;
                                break;
                            }
                    }
                    resourceTextureProto.m_spawnCriteria[0] = criteria;
                }
            }
            m_texturePrototypes = resourceTexturePrototypes.ToArray();


            //Detail prototypes
            idx = 0;
            names.Clear();
            DetailPrototype terrainDetailProto;
            ResourceProtoDetail resourceDetailProto;
            List<ResourceProtoDetail> resourceDetailPrototypes = new List<ResourceProtoDetail>(m_detailPrototypes);
            while (resourceDetailPrototypes.Count > terrain.terrainData.detailPrototypes.Length)
            {
                resourceDetailPrototypes.RemoveAt(resourceDetailPrototypes.Count - 1);
            }
            for (idx = 0; idx < terrain.terrainData.detailPrototypes.Length; idx++)
            {
                terrainDetailProto = terrain.terrainData.detailPrototypes[idx];
                if (idx < resourceDetailPrototypes.Count)
                {
                    resourceDetailProto = resourceDetailPrototypes[idx];
                }
                else
                {
                    resourceDetailProto = new ResourceProtoDetail();
                    resourceDetailPrototypes.Add(resourceDetailProto);
                }

                resourceDetailProto.m_renderMode = terrainDetailProto.renderMode;
                if (terrainDetailProto.prototype != null)
                {
                    resourceDetailProto.m_name = GetUniqueName(terrainDetailProto.prototype.name, ref names);
                    resourceDetailProto.m_detailProtoype = terrainDetailProto.prototype;
                }
                else
                {
                    resourceDetailProto.m_name = GetUniqueName(terrainDetailProto.prototypeTexture.name, ref names);
                    resourceDetailProto.m_detailTexture = terrainDetailProto.prototypeTexture;
                }

                resourceDetailProto.m_dryColour = terrainDetailProto.dryColor;
                resourceDetailProto.m_healthyColour = terrainDetailProto.healthyColor;
                resourceDetailProto.m_maxHeight = terrainDetailProto.maxHeight;
                resourceDetailProto.m_maxWidth = terrainDetailProto.maxWidth;
                resourceDetailProto.m_minHeight = terrainDetailProto.minHeight;
                resourceDetailProto.m_minWidth = terrainDetailProto.minWidth;
                resourceDetailProto.m_noiseSpread = terrainDetailProto.noiseSpread;
                resourceDetailProto.m_bendFactor = terrainDetailProto.bendFactor;

                //Handle missing dna
                if (resourceDetailProto.m_dna == null)
                {
                    resourceDetailProto.m_dna = new ResourceProtoDNA();
                }

                //Then reinitialise
                resourceDetailProto.m_dna.m_rndScaleInfluence = false;
                resourceDetailProto.m_dna.Update(idx, resourceDetailProto.m_maxWidth, resourceDetailProto.m_maxHeight, 0.1f, 1f);

                //Handle empty spawn criteria
                if (resourceDetailProto.m_spawnCriteria.Length == 0)
                {
                    resourceDetailProto.m_spawnCriteria = new SpawnCritera[1];
                    criteria = new SpawnCritera();
                    criteria.m_isActive = true;
                    criteria.m_virginTerrain = true;
                    criteria.m_checkType = Gaia.GaiaConstants.SpawnerLocationCheckType.PointCheck;

                    //Create some reasonable terrain based starting points
                    criteria.m_checkHeight = true;
                    criteria.m_minHeight = UnityEngine.Random.Range(m_beachHeight * 0.25f, m_beachHeight);
                    criteria.m_maxHeight = m_terrainHeight - m_seaLevel;
                    criteria.m_heightFitness = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.05f, 1f), new Keyframe(1f, 0f));
                    criteria.m_checkSlope = true;
                    criteria.m_minSlope = 0f;
                    criteria.m_maxSlope = UnityEngine.Random.Range(25f, 40f);
                    criteria.m_slopeFitness = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.05f, 1f), new Keyframe(1f, 0f));
                    criteria.m_checkProximity = false;
                    criteria.m_checkTexture = false;
                    resourceDetailProto.m_spawnCriteria[0] = criteria;
                }

            }
            m_detailPrototypes = resourceDetailPrototypes.ToArray();

            //Tree prototypes
            idx = 0;
            names.Clear();
            TreePrototype terrainTreeProto;
            ResourceProtoTree resourceTreeProto;
            List<ResourceProtoTree> resourceTreePrototypes = new List<ResourceProtoTree>(m_treePrototypes);
            while (resourceTreePrototypes.Count > terrain.terrainData.treePrototypes.Length)
            {
                resourceTreePrototypes.RemoveAt(resourceTreePrototypes.Count - 1);
            }
            for (idx = 0; idx < terrain.terrainData.treePrototypes.Length; idx++)
            {
                terrainTreeProto = terrain.terrainData.treePrototypes[idx];
                if (idx < resourceTreePrototypes.Count)
                {
                    resourceTreeProto = resourceTreePrototypes[idx];
                }
                else
                {
                    resourceTreeProto = new ResourceProtoTree();
                    resourceTreePrototypes.Add(resourceTreeProto);
                }

                resourceTreeProto.m_name = GetUniqueName(terrainTreeProto.prefab.name, ref names);
                resourceTreeProto.m_desktopPrefab = resourceTreeProto.m_mobilePrefab = terrainTreeProto.prefab;
                resourceTreeProto.m_bendFactor = terrainTreeProto.bendFactor;

                //DNA
                if (resourceTreeProto.m_dna == null)
                {
                    resourceTreeProto.m_dna = new ResourceProtoDNA();
                    resourceTreeProto.m_dna.Update(idx);
                }
                UpdateDNA(terrainTreeProto.prefab, ref resourceTreeProto.m_dna);
                resourceTreeProto.m_dna.m_boundsRadius = resourceTreeProto.m_dna.m_width * 0.25f;
                resourceTreeProto.m_dna.m_seedThrow = resourceTreeProto.m_dna.m_height * 1.5f;


                //Spawn criteria
                if (resourceTreeProto.m_spawnCriteria.Length == 0)
                {
                    resourceTreeProto.m_spawnCriteria = new SpawnCritera[1];
                    criteria = new SpawnCritera();
                    criteria.m_isActive = true;
                    criteria.m_virginTerrain = true;
                    criteria.m_checkType = GaiaConstants.SpawnerLocationCheckType.PointCheck;

                    //Create some reasonable terrain based starting points
                    criteria.m_checkHeight = true;
                    criteria.m_minHeight = 0f;
                    criteria.m_maxHeight = m_terrainHeight - m_seaLevel;
                    criteria.m_heightFitness = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f));
                    criteria.m_checkSlope = true;
                    criteria.m_minSlope = 0f;
                    criteria.m_maxSlope = UnityEngine.Random.Range(25f, 40f);
                    criteria.m_slopeFitness = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f));
                    criteria.m_checkProximity = false;
                    criteria.m_checkTexture = false;
                    resourceTreeProto.m_spawnCriteria[0] = criteria;
                }
            }
            m_treePrototypes = resourceTreePrototypes.ToArray();

            //Set up the asset associations
            SetAssetAssociations();
        }


        /// <summary>
        /// Get a unique name
        /// </summary>
        /// <param name="name">The original name</param>
        /// <param name="names">The names dictionary</param>
        /// <returns>The new unique name</returns>
        string GetUniqueName(string name, ref Dictionary<string, string> names)
        {
            int idx = 0;
            string newName = name;
            while (names.ContainsKey(newName))
            {
                newName = name + " " + idx.ToString();
                idx++;
            }
            names.Add(newName, newName);
            return newName;
        }

        /// <summary>
        /// Update the DNA based on the physical size of the prefab
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="?"></param>
        void UpdateDNA(GameObject prefab, ref ResourceProtoDNA dna)
        {
            if (prefab != null)
            {
                GameObject go = Instantiate(prefab);
                Bounds bounds = new Bounds(go.transform.position, Vector3.zero);
                foreach (Renderer r in go.GetComponentsInChildren<Renderer>())
                {
                    bounds.Encapsulate(r.bounds);
                }
                foreach (Collider c in go.GetComponentsInChildren<Collider>())
                {
                    bounds.Encapsulate(c.bounds);
                }
                DestroyImmediate(go);

                //Update dna
                dna.Update(dna.m_protoIdx, bounds.size.x, bounds.size.y);
            }
        }

        public void ChangeHeight(float oldHeight, float newHeight)
        {
            SpawnCritera[] criteria;
            SpawnCritera criterion;
            float oldMax = oldHeight - m_seaLevel;
            float newMax = newHeight - m_seaLevel;

            //Adjust textures
            for (int pidx = 0; pidx < m_texturePrototypes.Length; pidx++)
            {
                criteria = m_texturePrototypes[pidx].m_spawnCriteria;
                for (int cidx = 0; cidx < criteria.Length; cidx++)
                {
                    criterion = criteria[cidx];
                    if (criterion.m_maxHeight == oldMax)
                    {
                        criterion.m_maxHeight = newMax;
                    }
                }
            }

            //Adjust details
            for (int pidx = 0; pidx < m_detailPrototypes.Length; pidx++)
            {
                criteria = m_detailPrototypes[pidx].m_spawnCriteria;
                for (int cidx = 0; cidx < criteria.Length; cidx++)
                {
                    criterion = criteria[cidx];
                    if (criterion.m_maxHeight == oldMax)
                    {
                        criterion.m_maxHeight = newMax;
                    }
                }
            }

            //Adjust trees
            for (int pidx = 0; pidx < m_treePrototypes.Length; pidx++)
            {
                criteria = m_treePrototypes[pidx].m_spawnCriteria;
                for (int cidx = 0; cidx < criteria.Length; cidx++)
                {
                    criterion = criteria[cidx];
                    if (criterion.m_maxHeight == oldMax)
                    {
                        criterion.m_maxHeight = newMax;
                    }
                }
            }

            //Adjust gameobjects
            for (int pidx = 0; pidx < m_gameObjectPrototypes.Length; pidx++)
            {
                criteria = m_gameObjectPrototypes[pidx].m_spawnCriteria;
                for (int cidx = 0; cidx < criteria.Length; cidx++)
                {
                    criterion = criteria[cidx];
                    if (criterion.m_maxHeight == oldMax)
                    {
                        criterion.m_maxHeight = newMax;
                    }
                }
            }

            /*
            //Adjust stamps
            for (int pidx = 0; pidx < m_stampPrototypes.Length; pidx++)
            {
                criteria = m_stampPrototypes[pidx].m_spawnCriteria;
                for (int cidx = 0; cidx < criteria.Length; cidx++)
                {
                    criterion = criteria[cidx];
                    if (criterion.m_maxHeight == oldMax)
                    {
                        criterion.m_maxHeight = newMax;
                    }
                }
            }
             */
        }

        /// <summary>
        /// Update the sea level to the sea level provided
        /// </summary>
        /// <param name="newSeaLevel">Value for the new sea level</param>
        public void ChangeSeaLevel(float newSeaLevel)
        { 
            if (newSeaLevel != m_seaLevel)
            {
                ChangeSeaLevel(m_seaLevel, newSeaLevel);
            }
        }

        /// <summary>
        /// Update the sea level - and auto update any terrain criteria - only focus on extremities tho
        /// </summary>
        /// <param name="newSeaLevel">New sea level</param>
        public void ChangeSeaLevel(float oldSeaLevel, float newSeaLevel)
        {
            SpawnCritera[] criteria;
            SpawnCritera criterion;
            float oldMin = oldSeaLevel * -1f;
            float newMin = newSeaLevel * -1f;
            float oldMax = m_terrainHeight - oldSeaLevel;
            float newMax = m_terrainHeight - newSeaLevel;

            //Adjust textures
            for (int pidx = 0; pidx < m_texturePrototypes.Length; pidx++)
            {
                criteria = m_texturePrototypes[pidx].m_spawnCriteria;
                for (int cidx = 0; cidx < criteria.Length; cidx++)
                {
                    criterion = criteria[cidx];
                    if (criterion.m_minHeight == oldMin)
                    {
                        criterion.m_minHeight = newMin;
                    }
                    if (criterion.m_maxHeight == oldMax)
                    {
                        criterion.m_maxHeight = newMax;
                    }
                }
            }

            //Adjust details
            for (int pidx = 0; pidx < m_detailPrototypes.Length; pidx++)
            {
                criteria = m_detailPrototypes[pidx].m_spawnCriteria;
                for (int cidx = 0; cidx < criteria.Length; cidx++)
                {
                    criterion = criteria[cidx];
                    if (criterion.m_minHeight == oldMin)
                    {
                        criterion.m_minHeight = newMin;
                    }
                    if (criterion.m_maxHeight == oldMax)
                    {
                        criterion.m_maxHeight = newMax;
                    }
                }
            }

            //Adjust trees
            for (int pidx = 0; pidx < m_treePrototypes.Length; pidx++)
            {
                criteria = m_treePrototypes[pidx].m_spawnCriteria;
                for (int cidx = 0; cidx < criteria.Length; cidx++)
                {
                    criterion = criteria[cidx];
                    if (criterion.m_minHeight == oldMin)
                    {
                        criterion.m_minHeight = newMin;
                    }
                    if (criterion.m_maxHeight == oldMax)
                    {
                        criterion.m_maxHeight = newMax;
                    }
                }
            }

            //Adjust gameobjects
            for (int pidx = 0; pidx < m_gameObjectPrototypes.Length; pidx++)
            {
                criteria = m_gameObjectPrototypes[pidx].m_spawnCriteria;
                for (int cidx = 0; cidx < criteria.Length; cidx++)
                {
                    criterion = criteria[cidx];
                    if (criterion.m_minHeight == oldMin)
                    {
                        criterion.m_minHeight = newMin;
                    }
                    if (criterion.m_maxHeight == oldMax)
                    {
                        criterion.m_maxHeight = newMax;
                    }
                }
            }

            /*
            //Adjust stamps
            for (int pidx = 0; pidx < m_stampPrototypes.Length; pidx++)
            {
                criteria = m_stampPrototypes[pidx].m_spawnCriteria;
                for (int cidx = 0; cidx < criteria.Length; cidx++)
                {
                    criterion = criteria[cidx];
                    if (criterion.m_minHeight == oldMin)
                    {
                        criterion.m_minHeight = newMin;
                    }
                    if (criterion.m_maxHeight == oldMax)
                    {
                        criterion.m_maxHeight = newMax;
                    }
                }
            }
             */

            //Update to new sea level
            m_seaLevel = newSeaLevel;
        }


        /// <summary>
        /// Set these assets into all terrains
        /// </summary>
        public void ApplyPrototypesToTerrain()
        {
            //Make sure we have located the resources
            AssociateAssets();

            //Now apply to terrain
            foreach (Terrain t in Terrain.activeTerrains)
            {
                ApplyPrototypesToTerrain(t);
            }
        }

        /// <summary>
        /// Set these assets into the terrain provided
        /// </summary>
        /// <param name="terrain"></param>
        public void ApplyPrototypesToTerrain(Terrain terrain)
        {
            // Do a terrain check
            if (terrain == null)
            {
                Debug.LogWarning("Can not apply assets to terrain no terrain has been supplied.");
                return;
            }

            //Alpha splats
            GaiaSplatPrototype newSplat;
            List<GaiaSplatPrototype> terrainSplats = new List<GaiaSplatPrototype>();
            foreach (ResourceProtoTexture splat in m_texturePrototypes)
            {
                if (splat.m_texture != null)
                {
                    newSplat = new GaiaSplatPrototype();
                    newSplat.normalMap = splat.m_normal;
                    newSplat.tileOffset = new Vector2(splat.m_offsetX, splat.m_offsetY);
                    newSplat.tileSize = new Vector2(splat.m_sizeX, splat.m_sizeY);
                    newSplat.texture = splat.m_texture;
                    terrainSplats.Add(newSplat);
                }
                else
                {
                    Debug.LogWarning("Unable to find resource for " + splat.m_name + "... ignoring.");
                }
            }

            GaiaSplatPrototype.SetGaiaSplatPrototypes(terrain, terrainSplats.ToArray(),terrain.name);


            //Detail prototypes
            DetailPrototype newDetail;
            List<DetailPrototype> terrainDetails = new List<DetailPrototype>();
            foreach (ResourceProtoDetail detail in m_detailPrototypes)
            {
                if (detail.m_detailProtoype != null || detail.m_detailTexture != null)
                {
                    newDetail = new DetailPrototype();
                    newDetail.renderMode = detail.m_renderMode;
                    if (detail.m_detailProtoype != null)
                    {
                        newDetail.usePrototypeMesh = true;
                        newDetail.prototype = detail.m_detailProtoype;
                    }
                    else
                    {
                        newDetail.usePrototypeMesh = false;
                        newDetail.prototypeTexture = detail.m_detailTexture;
                    }
                    newDetail.dryColor = detail.m_dryColour;
                    newDetail.healthyColor = detail.m_healthyColour;
                    newDetail.maxHeight = detail.m_maxHeight;
                    newDetail.maxWidth = detail.m_maxWidth;
                    newDetail.minHeight = detail.m_minHeight;
                    newDetail.minWidth = detail.m_minWidth;
                    newDetail.noiseSpread = detail.m_noiseSpread;
                    newDetail.bendFactor = detail.m_bendFactor;
                    terrainDetails.Add(newDetail);
                }
                else
                {
                    Debug.LogWarning("Unable to find resource for " + detail.m_name + "... ignoring.");
                }
            }
            terrain.terrainData.detailPrototypes = terrainDetails.ToArray();

            //Tree prototypes
            TreePrototype newTree;
            List<TreePrototype> terrainTrees = new List<TreePrototype>();
            foreach (ResourceProtoTree tree in m_treePrototypes)
            {
                if (tree.m_desktopPrefab != null)
                {
                    newTree = new TreePrototype();
                    newTree.bendFactor = tree.m_bendFactor;
                    newTree.prefab = tree.m_desktopPrefab;
                    terrainTrees.Add(newTree);
                }
                else
                {
                    Debug.LogWarning("Unable to find resource for " + tree.m_name + "... ignoring.");
                }
            }
            terrain.terrainData.treePrototypes = terrainTrees.ToArray();

            terrain.Flush();
        }

        /// <summary>
        /// Add these assets into all terrains if they arent already there
        /// </summary>
        public void AddMissingPrototypesToTerrain()
        {
            AssociateAssets(); //Make sure everything has been connected up
            foreach (Terrain t in Terrain.activeTerrains)
            {
                AddMissingPrototypesToTerrain(t);
            }
        }

        /// <summary>
        /// Add these assets into the terrain provided if they arent already there
        /// </summary>
        /// <param name="terrain"></param>
        public void AddMissingPrototypesToTerrain(Terrain terrain)
        {
            // Do a terrain check
            if (terrain == null)
            {
                Debug.LogWarning("Can not add resources to the terrain as no terrain has been supplied.");
                return;
            }

            //Alpha splats

            bool found = false;
            GaiaSplatPrototype newSplat;
            List<GaiaSplatPrototype> terrainSplats = new List<GaiaSplatPrototype>(GaiaSplatPrototype.GetGaiaSplatPrototypes(terrain));
            foreach (ResourceProtoTexture splat in m_texturePrototypes)
            {
                //See if we can locate it already
                found = false;
                foreach (GaiaSplatPrototype sp in terrainSplats)
                {
                    if (GaiaCommon1.Utils.IsSameTexture(sp.texture, splat.m_texture, false))
                    {
                        found = true;
                    }
                }
                //Add if necessary
                if (!found)
                {
                    newSplat = new GaiaSplatPrototype();
                    newSplat.normalMap = splat.m_normal;
                    newSplat.tileOffset = new Vector2(splat.m_offsetX, splat.m_offsetY);
                    newSplat.tileSize = new Vector2(splat.m_sizeX, splat.m_sizeY);
                    newSplat.texture = splat.m_texture;
                    terrainSplats.Add(newSplat);
                }
            }

            GaiaSplatPrototype.SetGaiaSplatPrototypes(terrain, terrainSplats.ToArray(), terrain.name);

            //Detail prototypes
            DetailPrototype newDetail;
            List<DetailPrototype> terrainDetails = new List<DetailPrototype>(terrain.terrainData.detailPrototypes);
            foreach (ResourceProtoDetail detail in m_detailPrototypes)
            {
                //See if we can locate it already
                found = false;
                foreach (DetailPrototype dp in terrainDetails)
                {
                    if (dp.renderMode == detail.m_renderMode)
                    {
                        if (GaiaCommon1.Utils.IsSameTexture(dp.prototypeTexture, detail.m_detailTexture, false))
                        {
                            found = true;
                        }
                        if (GaiaCommon1.Utils.IsSameGameObject(dp.prototype, detail.m_detailProtoype, false))
                        {
                            found = true;
                        }
                    }
                }
                //Add if necessary
                if (!found)
                {
                    newDetail = new DetailPrototype();
                    newDetail.renderMode = detail.m_renderMode;
                    if (detail.m_detailProtoype != null)
                    {
                        newDetail.usePrototypeMesh = true;
                        newDetail.prototype = detail.m_detailProtoype;
                    }
                    else
                    {
                        newDetail.usePrototypeMesh = false;
                        newDetail.prototypeTexture = detail.m_detailTexture;
                    }
                    newDetail.dryColor = detail.m_dryColour;
                    newDetail.healthyColor = detail.m_healthyColour;
                    newDetail.maxHeight = detail.m_maxHeight;
                    newDetail.maxWidth = detail.m_maxWidth;
                    newDetail.minHeight = detail.m_minHeight;
                    newDetail.minWidth = detail.m_minWidth;
                    newDetail.noiseSpread = detail.m_noiseSpread;
                    newDetail.bendFactor = detail.m_bendFactor;
                    terrainDetails.Add(newDetail);
                }
            }
            terrain.terrainData.detailPrototypes = terrainDetails.ToArray();

            //Tree prototypes
            TreePrototype newTree;
            List<TreePrototype> terrainTrees = new List<TreePrototype>(terrain.terrainData.treePrototypes);
            foreach (ResourceProtoTree tree in m_treePrototypes)
            {
                //See if we can locate it already
                found = false;
                foreach (TreePrototype tp in terrainTrees)
                {
                    if (GaiaCommon1.Utils.IsSameGameObject(tp.prefab, tree.m_desktopPrefab, false))
                    {
                        found = true;
                    }
                }
                //Add if necessary
                if (!found)
                {
                    newTree = new TreePrototype();
                    newTree.bendFactor = tree.m_bendFactor;
                    newTree.prefab = tree.m_desktopPrefab;
                    terrainTrees.Add(newTree);
                }
            }
            terrain.terrainData.treePrototypes = terrainTrees.ToArray();

            terrain.Flush();
        }

        /// <summary>
        /// Add the resource to the terrains if not already there
        /// </summary>
        /// <param name="resourceType">Resource type</param>
        /// <param name="resourceIdx">Resource idx</param>
        public void AddPrototypeToTerrain(GaiaConstants.SpawnerResourceType resourceType, int resourceIdx)
        {
            foreach (Terrain terrain in Terrain.activeTerrains)
            {
                AddPrototypeToTerrain(resourceType, resourceIdx, terrain);
            }
        }

        /// <summary>
        /// Add the resource to the specified terrain if its not already there
        /// </summary>
        /// <param name="resourceType">Resource type</param>
        /// <param name="resourceIdx">Resource idx</param>
        /// <param name="terrain">Terrain to add it to</param>
        public void AddPrototypeToTerrain(GaiaConstants.SpawnerResourceType resourceType, int resourceIdx, Terrain terrain)
        {
            //Check index
            if (ResourceIdxOutOfBounds(resourceType, resourceIdx))
            {
                return;
            }

            //Exit if its already there - assume if any terrain then in all terrains
            if (!PrototypeMissingFromTerrain(resourceType, resourceIdx))
            {
                return;
            }

            //Now see if the resource exists within that terrain
            switch (resourceType)
            {
                case GaiaConstants.SpawnerResourceType.TerrainTexture:
                    ResourceProtoTexture splat = m_texturePrototypes[resourceIdx];
                    List<GaiaSplatPrototype> terrainSplats = new List<GaiaSplatPrototype>(GaiaSplatPrototype.GetGaiaSplatPrototypes(terrain));
                    GaiaSplatPrototype newSplat = new GaiaSplatPrototype();
                    newSplat.normalMap = splat.m_normal;
                    newSplat.tileOffset = new Vector2(splat.m_offsetX, splat.m_offsetY);
                    newSplat.tileSize = new Vector2(splat.m_sizeX, splat.m_sizeY);
                    newSplat.texture = splat.m_texture;
                    terrainSplats.Add(newSplat);
                    GaiaSplatPrototype.SetGaiaSplatPrototypes(terrain,terrainSplats.ToArray(),terrain.name);
                    break;
                case GaiaConstants.SpawnerResourceType.TerrainDetail:
                    ResourceProtoDetail detail = m_detailPrototypes[resourceIdx];
                    List<DetailPrototype> terrainDetails = new List<DetailPrototype>(terrain.terrainData.detailPrototypes);
                    DetailPrototype newDetail = new DetailPrototype();
                    newDetail.renderMode = detail.m_renderMode;
                    if (detail.m_detailProtoype != null)
                    {
                        newDetail.usePrototypeMesh = true;
                        newDetail.prototype = detail.m_detailProtoype;
                    }
                    else
                    {
                        newDetail.usePrototypeMesh = false;
                        newDetail.prototypeTexture = detail.m_detailTexture;
                    }
                    newDetail.dryColor = detail.m_dryColour;
                    newDetail.healthyColor = detail.m_healthyColour;
                    newDetail.maxHeight = detail.m_maxHeight;
                    newDetail.maxWidth = detail.m_maxWidth;
                    newDetail.minHeight = detail.m_minHeight;
                    newDetail.minWidth = detail.m_minWidth;
                    newDetail.noiseSpread = detail.m_noiseSpread;
                    newDetail.bendFactor = detail.m_bendFactor;
                    terrainDetails.Add(newDetail);
                    terrain.terrainData.detailPrototypes = terrainDetails.ToArray();
                    break;
                case GaiaConstants.SpawnerResourceType.TerrainTree:
                    ResourceProtoTree tree = m_treePrototypes[resourceIdx];
                    List<TreePrototype> terrainTrees = new List<TreePrototype>(terrain.terrainData.treePrototypes);
                    TreePrototype newTree = new TreePrototype();
                    newTree.bendFactor = tree.m_bendFactor;
                    newTree.prefab = tree.m_desktopPrefab;
                    terrainTrees.Add(newTree);
                    terrain.terrainData.treePrototypes = terrainTrees.ToArray();
                    break;
            }
            terrain.Flush();
        }


        /*
        public void UpdateDNA()
        {
            //Bounds
            Bounds bounds;

            //Details
            ResourceProtoDetail detailProto;
            for (int detailIdx = 0; detailIdx < m_detailPrototypes.Length; detailIdx++)
            {
                detailProto = m_detailPrototypes[detailIdx];
                if (detailProto.m_dna == null)
                {
                    detailProto.m_dna = new ResourceProtoDNA();
                    detailProto.m_dna.Initialise();
                }
                detailProto.m_dna.m_width = detailProto.m_maxWidth;
                detailProto.m_dna.m_height = detailProto.m_maxHeight;
                detailProto.m_dna.m_boundsRadius = detailProto.m_dna.m_width;
            }

            ResourceProtoTree treeProto;
            for (int treeIdx = 0; treeIdx < m_treePrototypes.Length; treeIdx++)
            {
                treeProto = m_treePrototypes[treeIdx];
                if (treeProto.m_dna == null)
                {
                    treeProto.m_dna = new ResourceProtoDNA();
                    treeProto.m_dna.Initialise();
                }
                if (treeProto.m_desktopPrefab != null)
                {
                    GameObject go = Instantiate(treeProto.m_desktopPrefab);
                    bounds = new Bounds(go.transform.position, Vector3.zero);
                    foreach (Renderer r in go.GetComponentsInChildren<Renderer>())
                    {
                        bounds.Encapsulate(r.bounds);
                    }
                    foreach (Collider c in go.GetComponentsInChildren<Collider>())
                    {
                        bounds.Encapsulate(c.bounds);
                    }
                    treeProto.m_dna.m_width = bounds.size.x;
                    treeProto.m_dna.m_height = bounds.size.y;
                    treeProto.m_dna.m_boundsRadius = treeProto.m_dna.m_width;
                    DestroyImmediate(go);
                }
            }

            ResourceProtoGameObject gameProto;
            for (int goIdx = 0; goIdx < m_gameObjectPrototypes.Length; goIdx++)
            {
                gameProto = m_gameObjectPrototypes[goIdx];
                if (gameProto.m_dna == null)
                {
                    gameProto.m_dna = new ResourceProtoDNA();
                    gameProto.m_dna.Initialise();
                }
                if (gameProto.m_instances.Length > 0 && gameProto.m_instances[0].m_desktopPrefab != null)
                {
                    GameObject go = Instantiate(gameProto.m_instances[0].m_desktopPrefab);
                    bounds = new Bounds(go.transform.position, Vector3.zero);
                    foreach (Renderer r in go.GetComponentsInChildren<Renderer>())
                    {
                        bounds.Encapsulate(r.bounds);
                    }
                    foreach (Collider c in go.GetComponentsInChildren<Collider>())
                    {
                        bounds.Encapsulate(c.bounds);
                    }
                    gameProto.m_dna.m_width = bounds.size.x;
                    gameProto.m_dna.m_height = bounds.size.y;
                    gameProto.m_dna.m_boundsRadius = gameProto.m_dna.m_width;
                    DestroyImmediate(go);
                }
            }
        }
        */

        #region Cache Helpers

        /// <summary>
        /// Return true if any of these resources do texture based lookups
        /// </summary>
        /// <returns></returns>
        public bool ChecksTextures()
        {
            int idx;
            for (idx = 0; idx < m_texturePrototypes.Length; idx++)
            {
                if (m_texturePrototypes[idx].ChecksTextures())
                {
                    return true;
                }
            }
            for (idx = 0; idx < m_detailPrototypes.Length; idx++)
            {
                if (m_detailPrototypes[idx].ChecksTextures())
                {
                    return true;
                }
            }
            for (idx = 0; idx < m_treePrototypes.Length; idx++)
            {
                if (m_treePrototypes[idx].ChecksTextures())
                {
                    return true;
                }
            }
            for (idx = 0; idx < m_gameObjectPrototypes.Length; idx++)
            {
                if (m_gameObjectPrototypes[idx].ChecksTextures())
                {
                    return true;
                }
            }
            /*
            for (idx = 0; idx < m_stampPrototypes.Length; idx++)
            {
                if (m_stampPrototypes[idx].ChecksTextures())
                {
                    return true;
                }
            }
             */
            return false;
        }

        /// <summary>
        /// Return true if any of these resources do proximity based lookups
        /// </summary>
        /// <returns></returns>
        public bool ChecksProximity()
        {
            int idx;
            for (idx = 0; idx < m_texturePrototypes.Length; idx++)
            {
                if (m_texturePrototypes[idx].ChecksProximity())
                {
                    return true;
                }
            }
            for (idx = 0; idx < m_detailPrototypes.Length; idx++)
            {
                if (m_detailPrototypes[idx].ChecksProximity())
                {
                    return true;
                }
            }
            for (idx = 0; idx < m_treePrototypes.Length; idx++)
            {
                if (m_treePrototypes[idx].ChecksProximity())
                {
                    return true;
                }
            }
            for (idx = 0; idx < m_gameObjectPrototypes.Length; idx++)
            {
                if (m_gameObjectPrototypes[idx].ChecksProximity())
                {
                    return true;
                }
            }
            /*
            for (idx = 0; idx < m_stampPrototypes.Length; idx++)
            {
                if (m_stampPrototypes[idx].ChecksProximity())
                {
                    return true;
                }
            }
             */
            return false;
        }

#endregion

        #region Add Resources

        /// <summary>
        /// Add a new game object resource, and make some assumptions based on current terrain settings
        /// </summary>
        /// <param name="prefab"></param>
        public void AddGameObject(GameObject prefab)
        {
            if (prefab == null)
            {
                Debug.LogWarning("Can't add null game object");
            }

#if UNITY_2018_3_OR_NEWER && UNITY_EDITOR
            if (PrefabUtility.GetPrefabAssetType(prefab) != PrefabAssetType.NotAPrefab)
            {
                //Create names array
                Dictionary<string, string> names = new Dictionary<string, string>();

                //Create space for larger array
                ResourceProtoGameObject[] pgos = new ResourceProtoGameObject[m_gameObjectPrototypes.Length + 1];

                //Copy existing items across
                for (int idx = 0; idx < m_gameObjectPrototypes.Length; idx++)
                {
                    pgos[idx] = m_gameObjectPrototypes[idx];
                    names.Add(pgos[idx].m_name, pgos[idx].m_name);
                }

                //Create the new game object prototype
                ResourceProtoGameObject pgo = new ResourceProtoGameObject();
                pgo.m_name = GetUniqueName(prefab.name, ref names);

                //Create and store prefab in instances
                ResourceProtoGameObjectInstance pgi = new ResourceProtoGameObjectInstance();
                pgi.m_rotateToSlope = true;
                pgi.m_desktopPrefab = prefab;
                pgi.m_name = prefab.name;
                pgo.m_instances = new ResourceProtoGameObjectInstance[1];
                pgo.m_instances[0] = pgi;

                //Update dna
                pgo.m_dna.Update(m_gameObjectPrototypes.Length);
                UpdateDNA(prefab, ref pgo.m_dna);
                pgo.m_dna.m_minScale = 1f;
                pgo.m_dna.m_maxScale = 1f;


                //Create spawn criteria
                pgo.m_spawnCriteria = new SpawnCritera[1];
                SpawnCritera criteria = new SpawnCritera();
                criteria.m_isActive = true;
                criteria.m_virginTerrain = true;
                criteria.m_checkType = GaiaConstants.SpawnerLocationCheckType.BoundedAreaCheck;
                //Create some reasonable terrain based starting points
                criteria.m_checkHeight = true;
                criteria.m_minHeight = 0f;
                criteria.m_maxHeight = m_terrainHeight - m_seaLevel;
                criteria.m_heightFitness = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f));
                criteria.m_checkSlope = true;
                criteria.m_minSlope = 0f;
                criteria.m_maxSlope = UnityEngine.Random.Range(5f, 15f);
                criteria.m_slopeFitness = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f));
                criteria.m_checkProximity = false;
                criteria.m_checkTexture = false;
                pgo.m_spawnCriteria[0] = criteria;

                pgos[pgos.Length - 1] = pgo;
                m_gameObjectPrototypes = pgos;
            }
#endif
#if !UNITY_2018_3_OR_NEWER && UNITY_EDITOR
            if (PrefabUtility.GetPrefabType(prefab) != PrefabType.None)
            {
                //Create names array
                Dictionary<string, string> names = new Dictionary<string, string>();

                //Create space for larger array
                ResourceProtoGameObject[] pgos = new ResourceProtoGameObject[m_gameObjectPrototypes.Length + 1];

                //Copy existing items across
                for (int idx = 0; idx < m_gameObjectPrototypes.Length; idx++)
                {
                    pgos[idx] = m_gameObjectPrototypes[idx];
                    names.Add(pgos[idx].m_name, pgos[idx].m_name);
                }

                //Create the new game object prototype
                ResourceProtoGameObject pgo = new ResourceProtoGameObject();
                pgo.m_name = GetUniqueName(prefab.name, ref names);

                //Create and store prefab in instances
                ResourceProtoGameObjectInstance pgi = new ResourceProtoGameObjectInstance();
                pgi.m_rotateToSlope = true;
                pgi.m_desktopPrefab = prefab;
                pgi.m_name = prefab.name;
                pgo.m_instances = new ResourceProtoGameObjectInstance[1];
                pgo.m_instances[0] = pgi;

                //Update dna
                pgo.m_dna.Update(m_gameObjectPrototypes.Length);
                UpdateDNA(prefab, ref pgo.m_dna);
                pgo.m_dna.m_minScale = 1f;
                pgo.m_dna.m_maxScale = 1f;


                //Create spawn criteria
                pgo.m_spawnCriteria = new SpawnCritera[1];
                SpawnCritera criteria = new SpawnCritera();
                criteria.m_isActive = true;
                criteria.m_virginTerrain = true;
                criteria.m_checkType = GaiaConstants.SpawnerLocationCheckType.BoundedAreaCheck;
                //Create some reasonable terrain based starting points
                criteria.m_checkHeight = true;
                criteria.m_minHeight = 0f;
                criteria.m_maxHeight = m_terrainHeight - m_seaLevel;
                criteria.m_heightFitness = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f));
                criteria.m_checkSlope = true;
                criteria.m_minSlope = 0f;
                criteria.m_maxSlope = UnityEngine.Random.Range(5f, 15f);
                criteria.m_slopeFitness = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f));
                criteria.m_checkProximity = false;
                criteria.m_checkTexture = false;
                pgo.m_spawnCriteria[0] = criteria;

                pgos[pgos.Length - 1] = pgo;
                m_gameObjectPrototypes = pgos;
            }
#endif
        }


        /// <summary>
        /// Add the game object from a list of prefabs instantiated as game objects
        /// </summary>
        /// <param name="prototypes"></param>
        public void AddGameObject(List<GameObject> prototypes)
        {
            if (prototypes == null || prototypes.Count < 1)
            {
                Debug.LogWarning("Can't add null or empty prototypes list");
                return;
            }

#if UNITY_EDITOR

            //Create names array
            Dictionary<string, string> names = new Dictionary<string, string>();

            //Create space for larger array
            ResourceProtoGameObject[] pgos = new ResourceProtoGameObject[m_gameObjectPrototypes.Length + 1];

            //Copy existing items across
            for (int idx = 0; idx < m_gameObjectPrototypes.Length; idx++)
            {
                pgos[idx] = m_gameObjectPrototypes[idx];
                names.Add(pgos[idx].m_name, pgos[idx].m_name);
            }

            //Create the new game object prototype
            ResourceProtoGameObject pgo = new ResourceProtoGameObject();

            //Now process all of the prototypes
            Bounds localBounds;
            Bounds globalBounds = new Bounds();
            GameObject prefab = null;
            GameObject rootGO = null;
            ResourceProtoGameObjectInstance pgi = null;
            List<ResourceProtoGameObjectInstance> instances = new List<ResourceProtoGameObjectInstance>();

            //First calculate the global bounds - everything will be done relative to this
            foreach (GameObject currentInstance in prototypes)
            {
                //Calculate the bounds
                localBounds = Gaia.GaiaUtils.GetBounds(currentInstance);

                //If first time then set things up
                if (rootGO == null)
                {
                    rootGO = currentInstance;
                    globalBounds = new Bounds(localBounds.center, localBounds.size);
                }
                else
                {
                    globalBounds.Encapsulate(localBounds);
                }
            }

            //Then process each prototype
            rootGO = null;
            foreach (GameObject currentInstance in prototypes)
            {
                //Get the prefab
#if UNITY_2018_3_OR_NEWER
                prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(currentInstance) as GameObject;
#elif UNITY_2018_2_OR_NEWER
                prefab = PrefabUtility.GetCorrespondingObjectFromSource(currentInstance) as GameObject;
#else
                prefab = PrefabUtility.GetPrefabParent(currentInstance) as GameObject;
#endif

                //Calculate the bounds
                localBounds = Gaia.GaiaUtils.GetBounds(currentInstance);

                //If first time then set things up
                if (rootGO == null)
                {
                    rootGO = currentInstance;
                    pgo.m_name = GetUniqueName(prefab.name, ref names);
                }

                pgi = new ResourceProtoGameObjectInstance();
                pgi.m_name = prefab.name;
                pgi.m_desktopPrefab = prefab;
                pgi.m_mobilePrefab = prefab;

                pgi.m_minInstances = 1;
                pgi.m_maxInstances = 1;

                pgi.m_minSpawnOffsetX = pgi.m_maxSpawnOffsetX = currentInstance.transform.position.x - globalBounds.center.x;
                //pgi.m_minSpawnOffsetY = pgi.m_maxSpawnOffsetY = localBounds.size.y * -0.05f;
                pgi.m_minSpawnOffsetY = pgi.m_maxSpawnOffsetY = currentInstance.transform.position.y; // Assume zero is ground
                pgi.m_minSpawnOffsetZ = pgi.m_maxSpawnOffsetZ = currentInstance.transform.position.z - globalBounds.center.z;
                pgi.m_minRotationOffsetX = pgi.m_maxRotationOffsetX = currentInstance.transform.localEulerAngles.x;
                pgi.m_minRotationOffsetY = pgi.m_maxRotationOffsetY = currentInstance.transform.localEulerAngles.y;
                pgi.m_minRotationOffsetZ = pgi.m_maxRotationOffsetZ = currentInstance.transform.localEulerAngles.z;
                pgi.m_useParentScale = false;
                pgi.m_minScale = pgi.m_maxScale = Mathf.Max(currentInstance.transform.localScale.x, currentInstance.transform.localScale.z);
                if (pgi.m_maxScale > 0f)
                {
                    pgi.m_localBounds = Mathf.Max(localBounds.size.x, localBounds.size.z) / pgi.m_maxScale;
                }
                else
                {
                    pgi.m_localBounds = Mathf.Max(localBounds.size.x, localBounds.size.z);
                }
                pgi.m_rotateToSlope = true;
                pgi.m_virginTerrain = false;

                instances.Add(pgi);
            }

            //Update dna
            pgo.m_dna.Update(m_gameObjectPrototypes.Length, Mathf.Max(globalBounds.size.x, globalBounds.size.z), globalBounds.size.y);
            pgo.m_dna.m_minScale = 1f;
            pgo.m_dna.m_maxScale = 1f;

            //Create spawn criteria
            pgo.m_spawnCriteria = new SpawnCritera[1];
            SpawnCritera criteria = new SpawnCritera();
            criteria.m_isActive = true;
            criteria.m_virginTerrain = true;
            criteria.m_checkType = GaiaConstants.SpawnerLocationCheckType.BoundedAreaCheck;

            //Create some reasonable terrain based starting points
            criteria.m_checkHeight = true;
            criteria.m_minHeight = 0f;
            criteria.m_maxHeight = m_terrainHeight - m_seaLevel;
            criteria.m_heightFitness = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f));
            criteria.m_checkSlope = true;
            criteria.m_minSlope = 0f;
            criteria.m_maxSlope = UnityEngine.Random.Range(5f, 10f);
            criteria.m_slopeFitness = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f));
            criteria.m_checkProximity = false;
            criteria.m_checkTexture = false;
            pgo.m_spawnCriteria[0] = criteria;

            pgo.m_instances = instances.ToArray();
            pgos[pgos.Length - 1] = pgo;
            m_gameObjectPrototypes = pgos;


            /*
            if (PrefabUtility.GetPrefabType(prefab) != PrefabType.None)
            {
                //Create names array
                Dictionary<string, string> names = new Dictionary<string, string>();

                //Create space for larger array
                ResourceProtoGameObject[] pgos = new ResourceProtoGameObject[m_gameObjectPrototypes.Length + 1];

                //Copy existing items across
                for (int idx = 0; idx < m_gameObjectPrototypes.Length; idx++)
                {
                    pgos[idx] = m_gameObjectPrototypes[idx];
                    names.Add(pgos[idx].m_name, pgos[idx].m_name);
                }

                //Create the new game object prototype
                ResourceProtoGameObject pgo = new ResourceProtoGameObject();
                pgo.m_name = GetUniqueName(prefab.name, ref names);

                //Create and store prefab in instances
                ResourceProtoGameObjectInstance pgi = new ResourceProtoGameObjectInstance();
                pgi.m_conformToSlope = true;
                pgi.m_desktopPrefab = prefab;
                pgi.m_name = prefab.name;
                pgo.m_instances = new ResourceProtoGameObjectInstance[1];
                pgo.m_instances[0] = pgi;

                //Update dna
                pgo.m_dna.Update(m_gameObjectPrototypes.Length);
                UpdateDNA(prefab, ref pgo.m_dna);

                //Create spawn criteria
                pgo.m_spawnCriteria = new SpawnCritera[1];
                SpawnCritera criteria = new SpawnCritera();
                criteria.m_isActive = true;
                criteria.m_virginTerrain = true;
                criteria.m_checkType = GaiaConstants.SpawnerLocationCheckType.BoundedAreaCheck;
                //Create some reasonable terrain based starting points
                criteria.m_checkHeight = true;
                criteria.m_minHeight = UnityEngine.Random.Range(m_beachHeight - (m_beachHeight / 4f), m_beachHeight * 2f);
                criteria.m_maxHeight = m_terrainHeight - m_seaLevel;
                criteria.m_heightFitness = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f));
                criteria.m_checkSlope = true;
                criteria.m_minSlope = UnityEngine.Random.Range(0f, 1.5f);
                criteria.m_maxSlope = UnityEngine.Random.Range(1.6f, 5f);
                criteria.m_slopeFitness = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f));
                criteria.m_checkProximity = false;
                criteria.m_checkTexture = false;
                pgo.m_spawnCriteria[0] = criteria;

                pgos[pgos.Length - 1] = pgo;
                m_gameObjectPrototypes = pgos;
            }
            */
#endif
        }


#endregion

        #region Create Spawners

        public GameObject CreateCoverageTextureSpawner(float range, float increment)
        {
            CreateOrFindSessionManager();
            GameObject gaiaObj = CreateOrFindGaia();
            //SpawnerGroup sg = CreateOrFindGroupSpawner();

            GameObject spawnerObj = new GameObject("Coverage Texture Spawner");
            spawnerObj.AddComponent<Spawner>();
            Spawner spawner = spawnerObj.GetComponent<Spawner>();
            spawner.m_resources = this;
            spawnerObj.transform.parent = gaiaObj.transform;
            spawnerObj.transform.position = Gaia.TerrainHelper.GetActiveTerrainCenter();

            //Do basic setup
            spawner.m_resources = this;
            spawner.m_mode = GaiaConstants.OperationMode.DesignTime;
            spawner.m_spawnerShape = GaiaConstants.SpawnerShape.Box;
            spawner.m_rndGenerator = new Gaia.XorshiftPlus(spawner.m_seed);
            spawner.m_spawnRange = range;
            spawner.m_spawnFitnessAttenuator = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));
            spawner.m_spawnRuleSelector = GaiaConstants.SpawnerRuleSelector.All;
            spawner.m_spawnLocationAlgorithm = GaiaConstants.SpawnerLocation.EveryLocation;
            spawner.m_spawnCollisionLayers = Gaia.TerrainHelper.GetActiveTerrainLayer();
            spawner.m_locationIncrement = Mathf.Clamp(increment, 0.2f, 64f);

            //Iterate thru all the textures and add them. Assume the first one is the base.
            SpawnRule rule;
            for (int resIdx = 0; resIdx < m_texturePrototypes.Length; resIdx++ )
            {
                rule = new SpawnRule();
                rule.m_name = m_texturePrototypes[resIdx].m_name;
                rule.m_resourceType = GaiaConstants.SpawnerResourceType.TerrainTexture;
                rule.m_resourceIdx = resIdx;
                rule.m_minViableFitness = 0f;
                rule.m_failureRate = 0f;
                rule.m_maxInstances = (ulong)((range * 2f) * (range * 2f));
                rule.m_isActive = true;
                rule.m_isFoldedOut = false;
                rule.m_ignoreMaxInstances = true;
                rule.m_useExtendedSpawn = false;
                spawner.m_activeRuleCnt++;
                spawner.m_spawnerRules.Add(rule);

                //Break the 3rd texture up with some perlin
                if (resIdx == 2)
                {
                    rule.m_noiseMask = GaiaConstants.NoiseType.Perlin;
                    rule.m_noiseMaskFrequency = 1f;
                    rule.m_noiseMaskLacunarity = 1.5f;
                    rule.m_noiseMaskOctaves = 8;
                    rule.m_noiseMaskPersistence = 0.25f;
                    rule.m_noiseMaskSeed = (float)UnityEngine.Random.Range(0, 5000);
                    rule.m_noiseStrength = 1f;
                    rule.m_noiseZoom = 150f;
                }
            }

            //And it to the group spawner
            SpawnerGroup.SpawnerInstance si;
            si = new SpawnerGroup.SpawnerInstance();
            si.m_name = spawnerObj.name;
            si.m_interationsPerSpawn = 1;
            si.m_spawner = spawner;
            //sg.m_spawners.Add(si);

            return spawnerObj;
        }

        /// <summary>
        /// Create a detail spawner with the specified range
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public GameObject CreateCoverageDetailSpawner(float range, float increment)
        {
            CreateOrFindSessionManager();
            GameObject gaiaObj = CreateOrFindGaia();
            //SpawnerGroup sg = CreateOrFindGroupSpawner();

            GameObject spawnerObj = new GameObject("Coverage Detail Spawner");
            spawnerObj.AddComponent<Spawner>();
            Spawner spawner = spawnerObj.GetComponent<Spawner>();
            spawner.m_resources = this;
            spawnerObj.transform.parent = gaiaObj.transform;
            spawnerObj.transform.position = Gaia.TerrainHelper.GetActiveTerrainCenter();

            //Do basic setup
            spawner.m_resources = this;
            spawner.m_mode = GaiaConstants.OperationMode.DesignTime;
            spawner.m_spawnerShape = GaiaConstants.SpawnerShape.Box;
            spawner.m_rndGenerator = new Gaia.XorshiftPlus(spawner.m_seed);
            spawner.m_spawnRange = range;
            spawner.m_spawnFitnessAttenuator = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));
            spawner.m_spawnRuleSelector = GaiaConstants.SpawnerRuleSelector.All;
            spawner.m_spawnLocationAlgorithm = GaiaConstants.SpawnerLocation.EveryLocationJittered;
            spawner.m_spawnCollisionLayers = Gaia.TerrainHelper.GetActiveTerrainLayer();
            spawner.m_locationIncrement = increment;

            //Iterate thru all the details and add them.
            SpawnRule rule;
            for (int resIdx = 0; resIdx < m_detailPrototypes.Length; resIdx++)
            {
                rule = new SpawnRule();
                rule.m_name = m_detailPrototypes[resIdx].m_name;
                rule.m_resourceType = GaiaConstants.SpawnerResourceType.TerrainDetail;
                rule.m_resourceIdx = resIdx;
                rule.m_minViableFitness = UnityEngine.Random.Range(0.2f, 0.5f);
                rule.m_failureRate = UnityEngine.Random.Range(0.7f, 0.95f);
                rule.m_maxInstances = (ulong)((range * 2f) * (range * 2f));
                rule.m_ignoreMaxInstances = true;
                rule.m_isActive = true;
                rule.m_isFoldedOut = false;
                rule.m_useExtendedSpawn = false;

                //Create some nice defaults for the new grasses
                switch (resIdx)
                {
                    case 0:
                        {
                            rule.m_minViableFitness = 0.1f;
                            rule.m_failureRate = 0f;
                            rule.m_noiseMask = GaiaConstants.NoiseType.Perlin;
                            rule.m_noiseMaskFrequency = 1f;
                            rule.m_noiseMaskLacunarity = 2f;
                            rule.m_noiseMaskOctaves = 8;
                            rule.m_noiseMaskPersistence = 0.25f;
                            rule.m_noiseMaskSeed = 0f;
                            rule.m_noiseStrength = 1.5f;
                            rule.m_noiseZoom = 50f;
                            rule.m_noiseInvert = true;
                        }
                        break;
                    case 1:
                        {
                            rule.m_minViableFitness = 0.1f;
                            rule.m_failureRate = 0.8f;
                            rule.m_noiseMask = GaiaConstants.NoiseType.Perlin;
                            rule.m_noiseMaskFrequency = 1f;
                            rule.m_noiseMaskLacunarity = 2f;
                            rule.m_noiseMaskOctaves = 8;
                            rule.m_noiseMaskPersistence = 0.25f;
                            rule.m_noiseMaskSeed = 0f;
                            rule.m_noiseStrength = 1.5f;
                            rule.m_noiseZoom = 50f;
                            rule.m_noiseInvert = true;
                        }
                        break;
                    case 2:
                        {
                            rule.m_minViableFitness = 0.4f;
                            rule.m_failureRate = 0f;
                            rule.m_noiseMask = GaiaConstants.NoiseType.Perlin;
                            rule.m_noiseMaskFrequency = 1f;
                            rule.m_noiseMaskLacunarity = 2f;
                            rule.m_noiseMaskOctaves = 8;
                            rule.m_noiseMaskPersistence = 0.25f;
                            rule.m_noiseMaskSeed = 0f;
                            rule.m_noiseStrength = 1.5f;
                            rule.m_noiseZoom = 50f;
                            rule.m_noiseInvert = false;
                        }
                        break;
                    case 3:
                        {
                            rule.m_minViableFitness = 0.2f;
                            rule.m_failureRate = 0f;
                            rule.m_noiseMask = GaiaConstants.NoiseType.Perlin;
                            rule.m_noiseMaskFrequency = 1f;
                            rule.m_noiseMaskLacunarity = 2f;
                            rule.m_noiseMaskOctaves = 8;
                            rule.m_noiseMaskPersistence = 0.25f;
                            rule.m_noiseMaskSeed = 0f;
                            rule.m_noiseStrength = 1.5f;
                            rule.m_noiseZoom = 30f;
                            rule.m_noiseInvert = true;
                        }
                        break;
                    case 4:
                        {
                            rule.m_minViableFitness = 0.5f;
                            rule.m_failureRate = 0.65f;
                            rule.m_noiseMask = GaiaConstants.NoiseType.Perlin;
                            rule.m_noiseMaskFrequency = 1f;
                            rule.m_noiseMaskLacunarity = 2f;
                            rule.m_noiseMaskOctaves = 8;
                            rule.m_noiseMaskPersistence = 0.25f;
                            rule.m_noiseMaskSeed = 13390f;
                            rule.m_noiseStrength = 1.5f;
                            rule.m_noiseZoom = 50f;
                            rule.m_noiseInvert = true;
                        }
                        break;
                    case 5:
                        {
                            rule.m_minViableFitness = 0.4f;
                            rule.m_failureRate = 0.3f;
                            rule.m_noiseMask = GaiaConstants.NoiseType.Perlin;
                            rule.m_noiseMaskFrequency = 1f;
                            rule.m_noiseMaskLacunarity = 2f;
                            rule.m_noiseMaskOctaves = 8;
                            rule.m_noiseMaskPersistence = 0.25f;
                            rule.m_noiseMaskSeed = 13390f;
                            rule.m_noiseStrength = 1.5f;
                            rule.m_noiseZoom = 30f;
                            rule.m_noiseInvert = false;
                        }
                        break;
                    case 6:
                        {
                            rule.m_minViableFitness = 0.5f;
                            rule.m_failureRate = 0.9f;
                            rule.m_noiseMask = GaiaConstants.NoiseType.Perlin;
                            rule.m_noiseMaskFrequency = 1f;
                            rule.m_noiseMaskLacunarity = 1.5f;
                            rule.m_noiseMaskOctaves = 8;
                            rule.m_noiseMaskPersistence = 0.25f;
                            rule.m_noiseMaskSeed = 6886f;
                            rule.m_noiseStrength = 1f;
                            rule.m_noiseZoom = 90f;
                            rule.m_noiseInvert = false;
                        }
                        break;
                    default:
                        {
                            rule.m_isActive = false;
                        }
                        break;
                }

                spawner.m_activeRuleCnt++;
                spawner.m_spawnerRules.Add(rule);
            }

            //And it to the group spawner
            SpawnerGroup.SpawnerInstance si;
            si = new SpawnerGroup.SpawnerInstance();
            si.m_name = spawnerObj.name;
            si.m_interationsPerSpawn = 1;
            si.m_spawner = spawner;
            //sg.m_spawners.Add(si);

            return spawnerObj;
        }

        /// <summary>
        /// Find or create Gaia in the scene
        /// </summary>
        /// <returns></returns>
        public GameObject CreateOrFindGaia()
        {
            //Find or create gaia
            GameObject gaiaObj = GameObject.Find("Gaia");
            if (gaiaObj == null)
            {
                gaiaObj = new GameObject("Gaia");
            }
            return gaiaObj;
        }

        /// <summary>
        /// Create / show the session manager
        /// </summary>
        public GameObject CreateOrFindSessionManager()
        {
            GaiaSessionManager sessMgr = GaiaSessionManager.GetSessionManager();
            ChangeSeaLevel(sessMgr.m_session.m_seaLevel);
            return sessMgr.gameObject;
        }

        /// <summary>
        /// Find or create the Spawner Group in the scene
        /// </summary>
        /// <returns></returns>
/*
        public SpawnerGroup CreateOrFindGroupSpawner()
        {
            CreateOrFindSessionManager();
            GameObject gaiaObj = CreateOrFindGaia();

            //Find or create the spawner group
            GameObject sgObj = GameObject.Find("Group Spawner");
            if (sgObj == null)
            {
                sgObj = new GameObject("Group Spawner");
                sgObj.AddComponent<SpawnerGroup>();
                sgObj.transform.parent = gaiaObj.transform;
            }
            return sgObj.GetComponent<SpawnerGroup>();
        }
*/

        /// <summary>
        /// Create a clustered detail spawner with the specified range
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public GameObject CreateClusteredDetailSpawner(float range, float increment)
        {
            CreateOrFindSessionManager();
            GameObject gaiaObj = CreateOrFindGaia();
            //SpawnerGroup sg = CreateOrFindGroupSpawner();

            GameObject spawnerObj = new GameObject("Clustered Detail Spawner");
            spawnerObj.AddComponent<Spawner>();
            Spawner spawner = spawnerObj.GetComponent<Spawner>();
            spawner.m_resources = this;
            spawnerObj.transform.parent = gaiaObj.transform;
            spawnerObj.transform.position = Gaia.TerrainHelper.GetActiveTerrainCenter();

            //Do basic setup
            spawner.m_resources = this;
            spawner.m_mode = GaiaConstants.OperationMode.DesignTime;
            spawner.m_spawnerShape = GaiaConstants.SpawnerShape.Box;
            spawner.m_rndGenerator = new Gaia.XorshiftPlus(spawner.m_seed);
            spawner.m_spawnRange = range;
            spawner.m_spawnFitnessAttenuator = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));
            spawner.m_spawnRuleSelector = GaiaConstants.SpawnerRuleSelector.Random;
            spawner.m_spawnLocationAlgorithm = GaiaConstants.SpawnerLocation.RandomLocationClustered;
            spawner.m_locationChecksPerInt = UnityEngine.Random.Range((int)range * 7, (int)range * 10);
            spawner.m_maxRandomClusterSize = UnityEngine.Random.Range(10, 100);
            spawner.m_spawnCollisionLayers = Gaia.TerrainHelper.GetActiveTerrainLayer();

            spawner.m_locationIncrement = increment * 1.5f;

            //Iterate thru all the details and add them.
            SpawnRule rule;
            for (int resIdx = 0; resIdx < m_detailPrototypes.Length; resIdx++)
            {
                rule = new SpawnRule();
                rule.m_name = m_detailPrototypes[resIdx].m_name;
                rule.m_resourceType = GaiaConstants.SpawnerResourceType.TerrainDetail;
                rule.m_resourceIdx = resIdx;
                rule.m_minViableFitness = UnityEngine.Random.Range(0.3f, 0.6f);
                rule.m_failureRate = UnityEngine.Random.Range(0.1f, 0.3f); ;
                rule.m_maxInstances = (ulong)((range * 2f) * (range * 2f));
                rule.m_ignoreMaxInstances = true;
                rule.m_isActive = false;
                rule.m_isFoldedOut = false;
                rule.m_useExtendedSpawn = false;
                spawner.m_activeRuleCnt++;
                spawner.m_spawnerRules.Add(rule);

                if (resIdx > 2)
                {
                    rule.m_isActive = true;
                }
            }

            //And it to the group spawner
            SpawnerGroup.SpawnerInstance si;
            si = new SpawnerGroup.SpawnerInstance();
            si.m_name = spawnerObj.name;
            si.m_interationsPerSpawn = 1;
            si.m_spawner = spawner;
            //sg.m_spawners.Add(si);

            //And return it
            return spawnerObj;
        }

        public GameObject CreateClusteredTreeSpawner(float range)
        {
            CreateOrFindSessionManager();
            GameObject gaiaObj = CreateOrFindGaia();

            GameObject spawnerObj = new GameObject("Clustered Tree Spawner");
            spawnerObj.AddComponent<Spawner>();
            Spawner spawner = spawnerObj.GetComponent<Spawner>();
            spawner.m_resources = this;
            spawnerObj.transform.parent = gaiaObj.transform;
            spawnerObj.transform.position = Gaia.TerrainHelper.GetActiveTerrainCenter();

            //Do basic setup
            spawner.m_resources = this;
            spawner.m_mode = GaiaConstants.OperationMode.DesignTime;
            spawner.m_spawnerShape = GaiaConstants.SpawnerShape.Box;
            spawner.m_rndGenerator = new Gaia.XorshiftPlus(spawner.m_seed);
            spawner.m_spawnRange = range;
            spawner.m_spawnFitnessAttenuator = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));
            spawner.m_spawnRuleSelector = GaiaConstants.SpawnerRuleSelector.Random;
            spawner.m_spawnLocationAlgorithm = GaiaConstants.SpawnerLocation.RandomLocationClustered;
            spawner.m_spawnCollisionLayers = Gaia.TerrainHelper.GetActiveTerrainLayer();
            spawner.m_locationChecksPerInt = (int)range * 5;
            spawner.m_maxRandomClusterSize = 30;

            //Iterate thru all the trees and add them.
            SpawnRule rule;
            for (int resIdx = 0; resIdx < m_treePrototypes.Length; resIdx++)
            {
                rule = new SpawnRule();
                rule.m_name = m_treePrototypes[resIdx].m_name;
                rule.m_resourceType = GaiaConstants.SpawnerResourceType.TerrainTree;
                rule.m_resourceIdx = resIdx;
                rule.m_minViableFitness = 0.25f;
                rule.m_failureRate = 0f;
                rule.m_maxInstances = (ulong)((range * range) / 5f);
                rule.m_isActive = true;
                rule.m_isFoldedOut = false;
                rule.m_useExtendedSpawn = false;
                spawner.m_activeRuleCnt++;
                spawner.m_spawnerRules.Add(rule);

                //Create some nice defaults for the default trees
                switch (resIdx)
                {
                    case 0: //Broadleaf desktop
                        {
                            rule.m_minViableFitness = 0.2f;
                            rule.m_failureRate = 0f;
                            rule.m_noiseMask = GaiaConstants.NoiseType.Perlin;
                            rule.m_noiseMaskFrequency = 1f;
                            rule.m_noiseMaskLacunarity = 2f;
                            rule.m_noiseMaskOctaves = 8;
                            rule.m_noiseMaskPersistence = 0.25f;
                            rule.m_noiseMaskSeed = 0f;
                            rule.m_noiseStrength = 1.5f;
                            rule.m_noiseZoom = 50f;
                            rule.m_noiseInvert = false;
                        }
                        break;
                    case 1: //Conifer desktop
                        {
                            rule.m_minViableFitness = 0.2f;
                            rule.m_failureRate = 0f;
                            rule.m_noiseMask = GaiaConstants.NoiseType.Perlin;
                            rule.m_noiseMaskFrequency = 1f;
                            rule.m_noiseMaskLacunarity = 2f;
                            rule.m_noiseMaskOctaves = 8;
                            rule.m_noiseMaskPersistence = 0.25f;
                            rule.m_noiseMaskSeed = 0f;
                            rule.m_noiseStrength = 1.5f;
                            rule.m_noiseZoom = 50f;
                            rule.m_noiseInvert = true;
                        }
                        break;
                }
            }

            //And it to the group spawner
            SpawnerGroup.SpawnerInstance si;
            si = new SpawnerGroup.SpawnerInstance();
            si.m_name = spawnerObj.name;
            si.m_interationsPerSpawn = 1;
            si.m_spawner = spawner;
            //sg.m_spawners.Add(si);

            return spawnerObj;
        }

        public GameObject CreateCoverageTreeSpawner(float range)
        {
            CreateOrFindSessionManager();
            GameObject gaiaObj = CreateOrFindGaia();
            //SpawnerGroup sg = CreateOrFindGroupSpawner();

            GameObject spawnerObj = new GameObject("Coverage Tree Spawner");
            spawnerObj.AddComponent<Spawner>();
            Spawner spawner = spawnerObj.GetComponent<Spawner>();
            spawner.m_resources = this;
            spawnerObj.transform.parent = gaiaObj.transform;
            spawnerObj.transform.position = Gaia.TerrainHelper.GetActiveTerrainCenter();

            //Do basic setup
            spawner.m_resources = this;
            spawner.m_mode = GaiaConstants.OperationMode.DesignTime;
            spawner.m_spawnerShape = GaiaConstants.SpawnerShape.Box;
            spawner.m_rndGenerator = new Gaia.XorshiftPlus(spawner.m_seed);
            spawner.m_spawnRange = range;
            spawner.m_spawnFitnessAttenuator = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));
            spawner.m_spawnRuleSelector = GaiaConstants.SpawnerRuleSelector.Random;
            spawner.m_spawnLocationAlgorithm = GaiaConstants.SpawnerLocation.EveryLocationJittered;
            spawner.m_locationIncrement = 45f;
            spawner.m_maxJitteredLocationOffsetPct = 0.85f;
            spawner.m_spawnCollisionLayers = Gaia.TerrainHelper.GetActiveTerrainLayer();
            spawner.m_locationChecksPerInt = (int)range * 5;

            //Iterate thru all the trees and add them.
            SpawnRule rule;
            for (int resIdx = 0; resIdx < m_treePrototypes.Length; resIdx++)
            {
                rule = new SpawnRule();
                rule.m_name = m_treePrototypes[resIdx].m_name;
                rule.m_resourceType = GaiaConstants.SpawnerResourceType.TerrainTree;
                rule.m_resourceIdx = resIdx;
                rule.m_minViableFitness = 0.25f;
                rule.m_failureRate = 0f;
                rule.m_maxInstances = (ulong)((range * range) / 5f);
                rule.m_isActive = true;
                rule.m_isFoldedOut = false;
                rule.m_useExtendedSpawn = false;
                spawner.m_activeRuleCnt++;
                spawner.m_spawnerRules.Add(rule);

                //Create some nice defaults for the default trees
                switch (resIdx)
                {
                    case 0: //Broadleaf desktop
                        {
                            rule.m_minViableFitness = 0.2f;
                            rule.m_failureRate = 0f;
                            rule.m_noiseMask = GaiaConstants.NoiseType.Perlin;
                            rule.m_noiseMaskFrequency = 1f;
                            rule.m_noiseMaskLacunarity = 2f;
                            rule.m_noiseMaskOctaves = 8;
                            rule.m_noiseMaskPersistence = 0.25f;
                            rule.m_noiseMaskSeed = 0f;
                            rule.m_noiseStrength = 1.5f;
                            rule.m_noiseZoom = 50f;
                            rule.m_noiseInvert = false;
                        }
                        break;
                    case 1: //Conifer desktop
                        {
                            rule.m_minViableFitness = 0.2f;
                            rule.m_failureRate = 0f;
                            rule.m_noiseMask = GaiaConstants.NoiseType.Perlin;
                            rule.m_noiseMaskFrequency = 1f;
                            rule.m_noiseMaskLacunarity = 2f;
                            rule.m_noiseMaskOctaves = 8;
                            rule.m_noiseMaskPersistence = 0.25f;
                            rule.m_noiseMaskSeed = 0f;
                            rule.m_noiseStrength = 1.5f;
                            rule.m_noiseZoom = 50f;
                            rule.m_noiseInvert = true;
                        }
                        break;
                }
            }

            //And it to the group spawner
            SpawnerGroup.SpawnerInstance si;
            si = new SpawnerGroup.SpawnerInstance();
            si.m_name = spawnerObj.name;
            si.m_interationsPerSpawn = 1;
            si.m_spawner = spawner;
            //sg.m_spawners.Add(si);

            return spawnerObj;
        }

        public GameObject CreateCoverageGameObjectSpawner(float range)
        {
            CreateOrFindSessionManager();
            GameObject gaiaObj = CreateOrFindGaia();
            //SpawnerGroup sg = CreateOrFindGroupSpawner();

            GameObject spawnerObj = new GameObject("Coverage GameObject Spawner");
            spawnerObj.AddComponent<Spawner>();
            Spawner spawner = spawnerObj.GetComponent<Spawner>();
            spawner.m_resources = this;
            spawnerObj.transform.parent = gaiaObj.transform;
            spawnerObj.transform.position = Gaia.TerrainHelper.GetActiveTerrainCenter();

            //Do basic setup
            spawner.m_resources = this;
            spawner.m_mode = GaiaConstants.OperationMode.DesignTime;
            spawner.m_spawnerShape = GaiaConstants.SpawnerShape.Box;
            spawner.m_rndGenerator = new Gaia.XorshiftPlus(spawner.m_seed);
            spawner.m_spawnRange = range;
            spawner.m_spawnFitnessAttenuator = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));
            spawner.m_spawnRuleSelector = GaiaConstants.SpawnerRuleSelector.Random;
            spawner.m_spawnLocationAlgorithm = GaiaConstants.SpawnerLocation.EveryLocationJittered;
            spawner.m_locationIncrement = 45f;
            spawner.m_maxJitteredLocationOffsetPct = 0.85f;
            spawner.m_spawnCollisionLayers = Gaia.TerrainHelper.GetActiveTerrainLayer();
            spawner.m_locationChecksPerInt = (int)range * 5;
            
            //Iterate thru all the trees and add them.
            SpawnRule rule;
            for (int resIdx = 0; resIdx < m_gameObjectPrototypes.Length; resIdx++)
            {
                rule = new SpawnRule();
                rule.m_name = m_gameObjectPrototypes[resIdx].m_name;
                rule.m_resourceType = GaiaConstants.SpawnerResourceType.GameObject;
                rule.m_resourceIdx = resIdx;
                rule.m_minViableFitness = 0f;
                rule.m_failureRate = 0f;
                rule.m_maxInstances = (ulong)((range * range) / 5f);
                rule.m_isActive = !m_gameObjectPrototypes[resIdx].m_canSpawnAsTree;
                rule.m_isFoldedOut = false;
                rule.m_useExtendedSpawn = false;
                spawner.m_activeRuleCnt++;
                spawner.m_spawnerRules.Add(rule);
            }

            //And it to the group spawner
            SpawnerGroup.SpawnerInstance si;
            si = new SpawnerGroup.SpawnerInstance();
            si.m_name = spawnerObj.name;
            si.m_interationsPerSpawn = 1;
            si.m_spawner = spawner;
            //sg.m_spawners.Add(si);

            //And return it
            return spawnerObj;
        }

        public GameObject CreateCoverageGameObjectSpawnerForTrees(float range)
        {
            CreateOrFindSessionManager();
            GameObject gaiaObj = CreateOrFindGaia();
            //SpawnerGroup sg = CreateOrFindGroupSpawner();

            GameObject spawnerObj = new GameObject("Coverage GO Tree Spawner");
            spawnerObj.AddComponent<Spawner>();
            Spawner spawner = spawnerObj.GetComponent<Spawner>();
            spawner.m_resources = this;
            spawnerObj.transform.parent = gaiaObj.transform;
            spawnerObj.transform.position = Gaia.TerrainHelper.GetActiveTerrainCenter();

            //Do basic setup
            spawner.m_resources = this;
            spawner.m_mode = GaiaConstants.OperationMode.DesignTime;
            spawner.m_spawnerShape = GaiaConstants.SpawnerShape.Box;
            spawner.m_rndGenerator = new Gaia.XorshiftPlus(spawner.m_seed);
            spawner.m_spawnRange = range;
            spawner.m_spawnFitnessAttenuator = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));
            spawner.m_spawnRuleSelector = GaiaConstants.SpawnerRuleSelector.Random;
            spawner.m_spawnLocationAlgorithm = GaiaConstants.SpawnerLocation.EveryLocationJittered;
            spawner.m_locationIncrement = 45f;
            spawner.m_maxJitteredLocationOffsetPct = 0.85f;
            spawner.m_spawnCollisionLayers = Gaia.TerrainHelper.GetActiveTerrainLayer();
            spawner.m_locationChecksPerInt = (int)range * 5;

            //Iterate thru all the trees and add them.
            SpawnRule rule;
            for (int resIdx = 0; resIdx < m_gameObjectPrototypes.Length; resIdx++)
            {
                if (m_gameObjectPrototypes[resIdx].m_canSpawnAsTree)
                {
                    rule = new SpawnRule();
                    rule.m_name = m_gameObjectPrototypes[resIdx].m_name;
                    rule.m_resourceType = GaiaConstants.SpawnerResourceType.GameObject;
                    rule.m_resourceIdx = resIdx;
                    rule.m_minViableFitness = 0.25f;
                    rule.m_failureRate = 0f;
                    rule.m_maxInstances = (ulong)((range * range) / 5f);
                    rule.m_isActive = true;
                    rule.m_isFoldedOut = false;
                    rule.m_useExtendedSpawn = false;
                    spawner.m_activeRuleCnt++;
                    spawner.m_spawnerRules.Add(rule);

                    //Create some nice defaults for the default trees
                    switch (resIdx)
                    {
                        case 0: //Broadleaf desktop
                        {
                            rule.m_minViableFitness = 0.2f;
                            rule.m_failureRate = 0f;
                            rule.m_noiseMask = GaiaConstants.NoiseType.Perlin;
                            rule.m_noiseMaskFrequency = 1f;
                            rule.m_noiseMaskLacunarity = 2f;
                            rule.m_noiseMaskOctaves = 8;
                            rule.m_noiseMaskPersistence = 0.25f;
                            rule.m_noiseMaskSeed = 0f;
                            rule.m_noiseStrength = 1.5f;
                            rule.m_noiseZoom = 50f;
                            rule.m_noiseInvert = false;
                        }
                            break;
                        case 1: //Conifer desktop
                        {
                            rule.m_minViableFitness = 0.2f;
                            rule.m_failureRate = 0f;
                            rule.m_noiseMask = GaiaConstants.NoiseType.Perlin;
                            rule.m_noiseMaskFrequency = 1f;
                            rule.m_noiseMaskLacunarity = 2f;
                            rule.m_noiseMaskOctaves = 8;
                            rule.m_noiseMaskPersistence = 0.25f;
                            rule.m_noiseMaskSeed = 0f;
                            rule.m_noiseStrength = 1.5f;
                            rule.m_noiseZoom = 50f;
                            rule.m_noiseInvert = true;
                        }
                            break;
                    }
                }
            }

            //And it to the group spawner
            SpawnerGroup.SpawnerInstance si;
            si = new SpawnerGroup.SpawnerInstance();
            si.m_name = spawnerObj.name;
            si.m_interationsPerSpawn = 1;
            si.m_spawner = spawner;

            //And return it
            return spawnerObj;
        }


        public GameObject CreateClusteredGameObjectSpawner(float range)
        {
            CreateOrFindSessionManager();
            GameObject gaiaObj = CreateOrFindGaia();
            //SpawnerGroup sg = CreateOrFindGroupSpawner();

            GameObject spawnerObj = new GameObject("Clustered GameObject Spawner");
            spawnerObj.AddComponent<Spawner>();
            Spawner spawner = spawnerObj.GetComponent<Spawner>();
            spawner.m_resources = this;
            spawnerObj.transform.parent = gaiaObj.transform;
            spawnerObj.transform.position = Gaia.TerrainHelper.GetActiveTerrainCenter();

            //Do basic setup
            spawner.m_resources = this;
            spawner.m_mode = GaiaConstants.OperationMode.DesignTime;
            spawner.m_spawnerShape = GaiaConstants.SpawnerShape.Box;
            spawner.m_rndGenerator = new Gaia.XorshiftPlus(spawner.m_seed);
            spawner.m_spawnRange = range;
            spawner.m_spawnFitnessAttenuator = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));
            spawner.m_spawnRuleSelector = GaiaConstants.SpawnerRuleSelector.Random;
            spawner.m_spawnLocationAlgorithm = GaiaConstants.SpawnerLocation.RandomLocationClustered;
            spawner.m_spawnCollisionLayers = Gaia.TerrainHelper.GetActiveTerrainLayer();
            spawner.m_locationChecksPerInt = 2000;
            spawner.m_maxRandomClusterSize = 20;
            spawner.m_locationIncrement = 45;

            //Iterate thru all the trees and add them.
            SpawnRule rule;
            for (int resIdx = 0; resIdx < m_gameObjectPrototypes.Length; resIdx++)
            {
                rule = new SpawnRule();
                rule.m_name = m_gameObjectPrototypes[resIdx].m_name;
                rule.m_resourceType = GaiaConstants.SpawnerResourceType.GameObject;
                rule.m_resourceIdx = resIdx;
                rule.m_minViableFitness = 0f;
                rule.m_failureRate = 0f;
                rule.m_maxInstances = (ulong)range * 2;
                rule.m_isActive = true;
                rule.m_isFoldedOut = false;
                rule.m_useExtendedSpawn = false;
                spawner.m_activeRuleCnt++;
                spawner.m_spawnerRules.Add(rule);
            }

            //And it to the group spawner
            SpawnerGroup.SpawnerInstance si;
            si = new SpawnerGroup.SpawnerInstance();
            si.m_name = spawnerObj.name;
            si.m_interationsPerSpawn = 1;
            si.m_spawner = spawner;
            //sg.m_spawners.Add(si);

            //And return it
            return spawnerObj;
        }

        public GameObject CreateClusteredGameObjectSpawnerForTrees(float range)
        {
            CreateOrFindSessionManager();
            GameObject gaiaObj = CreateOrFindGaia();
            GameObject spawnerObj = new GameObject("Clustered GO Tree Spawner");
            spawnerObj.AddComponent<Spawner>();
            Spawner spawner = spawnerObj.GetComponent<Spawner>();
            spawner.m_resources = this;
            spawnerObj.transform.parent = gaiaObj.transform;
            spawnerObj.transform.position = Gaia.TerrainHelper.GetActiveTerrainCenter();

            //Do basic setup
            spawner.m_resources = this;
            spawner.m_mode = GaiaConstants.OperationMode.DesignTime;
            spawner.m_spawnerShape = GaiaConstants.SpawnerShape.Box;
            spawner.m_rndGenerator = new Gaia.XorshiftPlus(spawner.m_seed);
            spawner.m_spawnRange = range;
            spawner.m_spawnFitnessAttenuator = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));
            spawner.m_spawnRuleSelector = GaiaConstants.SpawnerRuleSelector.Random;
            spawner.m_spawnLocationAlgorithm = GaiaConstants.SpawnerLocation.RandomLocationClustered;
            spawner.m_spawnCollisionLayers = Gaia.TerrainHelper.GetActiveTerrainLayer();
            spawner.m_locationChecksPerInt = (int)range * 5;
            spawner.m_maxRandomClusterSize = 30;

            //Iterate thru all the trees and add them.
            SpawnRule rule;
            for (int resIdx = 0; resIdx < m_gameObjectPrototypes.Length; resIdx++)
            {
                if (m_gameObjectPrototypes[resIdx].m_canSpawnAsTree)
                {
                    rule = new SpawnRule();
                    rule.m_name = m_gameObjectPrototypes[resIdx].m_name;
                    rule.m_resourceType = GaiaConstants.SpawnerResourceType.GameObject;
                    rule.m_resourceIdx = resIdx;
                    rule.m_minViableFitness = 0.25f;
                    rule.m_failureRate = 0f;
                    rule.m_maxInstances = (ulong)range * 2;
                    rule.m_isActive = true;
                    rule.m_isFoldedOut = false;
                    rule.m_useExtendedSpawn = false;
                    spawner.m_activeRuleCnt++;
                    spawner.m_spawnerRules.Add(rule);
                }
            }

            //And it to the group spawner
            SpawnerGroup.SpawnerInstance si;
            si = new SpawnerGroup.SpawnerInstance();
            si.m_name = spawnerObj.name;
            si.m_interationsPerSpawn = 1;
            si.m_spawner = spawner;

            //And return it
            return spawnerObj;
        }


        /*
        public GameObject CreateStampSpawner(float range)
        {
            GameObject gaiaObj = GameObject.Find("Gaia");
            if (gaiaObj == null)
            {
                gaiaObj = new GameObject("Gaia");
            }
            GameObject spawnerObj = new GameObject("Stamp Spawner");
            spawnerObj.AddComponent<Spawner>();
            Spawner spawner = spawnerObj.GetComponent<Spawner>();
            spawner.m_resources = this;
            spawnerObj.transform.parent = gaiaObj.transform;

            //Do basic setup
            spawner.m_resources = this;
            spawner.m_mode = Constants.OperationMode.DesignTime;
            spawner.m_spawnerShape = Constants.SpawnerShape.Box;
            spawner.m_rndGenerator = new System.Random(spawner.m_seed);
            spawner.m_spawnRange = range;
            spawner.m_spawnRangeAttenuator = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));
            spawner.m_spawnRuleSelector = Constants.SpawnerRuleSelector.Random;
            spawner.m_spawnLocationAlgorithm = Constants.SpawnerLocation.RandomLocation;
            spawner.m_spawnerLayerMask = 1 << LayerMask.NameToLayer("Default");
            spawner.m_locationChecksPerInt = 5;
            spawner.m_maxRandomClusterSize = 1;

            //Iterate thru all the stamps and add them.
            SpawnRule rule;
            for (int resIdx = 0; resIdx < m_stampPrototypes.Length; resIdx++)
            {
                rule = new SpawnRule();
                rule.m_name = m_stampPrototypes[resIdx].m_name;
                rule.m_resourceType = Constants.SpawnerResourceType.Stamp;
                rule.m_resourceIdx = resIdx;
                rule.m_minViableFitness = 0.25f;
                rule.m_failureRate = 0f;
                rule.m_maxInstances = (int)range / 5;
                rule.m_isActive = true;
                rule.m_isFoldedOut = false;
                rule.m_useExtendedFitness = false;
                rule.m_useExtendedSpawn = false;
                spawner.m_activeRuleCnt++;
                spawner.m_spawnerRules.Add(rule);
            }

            return spawnerObj;
        }
        */

#endregion

        #region Exporters

        /// <summary>
        /// This routine will export a texture from the current terrain - experimental only at moment and not supported.
        /// </summary>
        public void ExportTexture()
        {
            Terrain terrain;
            Texture2D exportTexture;
            Color pixel;
            int width, height, layers;
            float aR, aG, aB, aA;

            //Now iterate through the terrains and export them
            for (int terrIdx = 0; terrIdx < Terrain.activeTerrains.Length; terrIdx++)
            {
                terrain = Terrain.activeTerrains[terrIdx];
                width = terrain.terrainData.alphamapWidth;
                height = terrain.terrainData.alphamapHeight;
                layers = terrain.terrainData.alphamapLayers;
                exportTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
                float[, ,] splatMaps = terrain.terrainData.GetAlphamaps(0, 0, width, height);

                //Iterate thru the terrain
                for (int x = 0; x < width; x++)
                {
                    for (int z = 0; z < height; z++)
                    {
                        aR = aG = aB = aA = 0f;
                        for (int t = 0; t < layers; t++)
                        {
                            pixel = m_texturePrototypes[t].m_texture.GetPixel(
                                x % ((int)m_texturePrototypes[t].m_sizeX / m_texturePrototypes[t].m_texture.width),
                                z % ((int)m_texturePrototypes[t].m_sizeY / m_texturePrototypes[t].m_texture.height)
                                );
                            aR += splatMaps[x, z, t] * pixel.r;
                            aG += splatMaps[x, z, t] * pixel.g;
                            aB += splatMaps[x, z, t] * pixel.b;
                            aA += splatMaps[x, z, t] * pixel.a;
                        }
                        exportTexture.SetPixel(x, z, new Color(aR, aG, aB, aA));
                    }
                }

                //Now export / save the texture
                Gaia.GaiaUtils.ExportPNG(terrain.name + " - Export", exportTexture);

                //And destroy it
                DestroyImmediate(exportTexture);
            }
            Debug.LogError("Attempted to export textures on terrain that does not exist!");
        }

#endregion

        #region Serialisation

        /// <summary>
        /// Serialise this as json
        /// </summary>
        /// <returns></returns>
        public string SerialiseJson()
        {
            fsData data;
            fsSerializer serializer = new fsSerializer();
            serializer.TrySerialize(this, out data);
            return fsJsonPrinter.CompressedJson(data);
        }

        /// <summary>
        /// Deserialise the suplied json into this object
        /// </summary>
        /// <param name="json">Source json</param>
        public void DeSerialiseJson(string json)
        {
            fsData data = fsJsonParser.Parse(json);
            fsSerializer serializer = new fsSerializer();
            var defaults = this;
            serializer.TryDeserialize<GaiaResource>(data, ref defaults);
        }

#endregion
    }
}