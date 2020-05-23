using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gaia
{
    /// <summary>
    /// Used to serialise the tree prototypes
    /// </summary>
    [System.Serializable]
    public class ResourceProtoTree
    {
        [Tooltip("Resource name.")]
        public string       m_name;
        [Tooltip("Desktop prefab.")]
        public GameObject   m_desktopPrefab;
        [HideInInspector]
        public string       m_desktopPrefabFileName; // Used for re-association
        [Tooltip("Mobile prefab - future proofing here - not currently used.")]
        public GameObject   m_mobilePrefab;
        [HideInInspector]
        public string       m_mobilePrefabFileName; // Used for re-association
        [Tooltip("How much the tree bends in the wind - only used by unity tree creator trees, ignored by SpeedTree trees.")]
        public float        m_bendFactor;
        [Tooltip("The colour of healthy trees - only used by unity tree creator trees, ignored by SpeedTree trees.")]
        public Color        m_healthyColour = Color.white;
        [Tooltip("The colour of dry trees - only used by unity tree creator trees, ignored by SpeedTree trees.")]
        public Color        m_dryColour = Color.white;
        [Tooltip("DNA - Used by the spawner to control how and where the tree will be spawned.")]
        public ResourceProtoDNA m_dna;
        [Tooltip("SPAWN CRITERIA - Spawn criteria are run against the terrain to assess its fitness in a range of 0..1 for use by this resource. If you add multiple criteria then the fittest one will be selected.")]
        public SpawnCritera[] m_spawnCriteria = new SpawnCritera[0];
        [Tooltip("SPAWN EXTENSIONS - Spawn extensions allow fitness, spawning and post spawning extensions to be made to the spawning system.")]
        public SpawnRuleExtension[] m_spawnExtensions = new SpawnRuleExtension[0];

        /// <summary>
        /// Initialise the tree
        /// </summary>
        /// <param name="spawner">The spawner it belongs to</param>
        public void Initialise(Spawner spawner)
        {
            foreach (SpawnCritera criteria in m_spawnCriteria)
            {
                criteria.Initialise(spawner);
            }
        }

        /// <summary>
        /// Determine whether this has active criteria
        /// </summary>
        /// <returns>True if has actrive criteria</returns>
        public bool HasActiveCriteria()
        {
            for (int idx = 0; idx < m_spawnCriteria.Length; idx++)
            {
                if (m_spawnCriteria[idx].m_isActive)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Set up the asset associations, return true if something changes. Can only be run when the editor is present.
        /// </summary>
        /// <returns>True if something changes</returns>
        public bool SetAssetAssociations()
        {
            bool isModified = false;

            #if UNITY_EDITOR
            if (m_desktopPrefab != null)
            {
                string fileName = Path.GetFileName(AssetDatabase.GetAssetPath(m_desktopPrefab));
                if (fileName != m_desktopPrefabFileName)
                {
                    m_desktopPrefabFileName = fileName;
                    isModified = true;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(m_desktopPrefabFileName))
                {
                    m_desktopPrefabFileName = "";
                    isModified = true;
                }
            }

            if (m_mobilePrefab != null)
            {
                string fileName = Path.GetFileName(AssetDatabase.GetAssetPath(m_mobilePrefab));
                if (fileName != m_mobilePrefabFileName)
                {
                    m_mobilePrefabFileName = fileName;
                    isModified = true;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(m_mobilePrefabFileName))
                {
                    m_mobilePrefabFileName = "";
                    isModified = true;
                }
            }

            #endif
            return isModified;
        }


        /// <summary>
        /// Associate any unallocated assets to this resource. Return true if something changes.
        /// </summary>
        /// <returns>True if the prototype was in some way modified</returns>
        public bool AssociateAssets()
        {
            bool isModified = false;

            #if UNITY_EDITOR
            if (m_desktopPrefab == null)
            {
                if (!string.IsNullOrEmpty(m_desktopPrefabFileName))
                {
                    m_desktopPrefab = GaiaUtils.GetAsset(m_desktopPrefabFileName, typeof(UnityEngine.GameObject)) as GameObject;
                    if (m_desktopPrefab != null)
                    {
                        isModified = true;
                    }
                }
            }

            if (m_mobilePrefab == null)
            {
                if (!string.IsNullOrEmpty(m_mobilePrefabFileName))
                {
                    m_mobilePrefab = GaiaUtils.GetAsset(m_mobilePrefabFileName, typeof(UnityEngine.GameObject)) as GameObject;
                    if (m_mobilePrefab != null)
                    {
                        isModified = true;
                    }
                }
            }
            #endif
            return isModified;
        }


        /// <summary>
        /// Determine whether this has active criteria that checks textures
        /// </summary>
        /// <returns>True if has active criteria that checks textures</returns>
        public bool ChecksTextures()
        {
            for (int idx = 0; idx < m_spawnCriteria.Length; idx++)
            {
                if (m_spawnCriteria[idx].m_isActive && m_spawnCriteria[idx].m_checkTexture)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determine whether this has active criteria that checks proximity
        /// </summary>
        /// <returns>True if has active criteria that checks proximity</returns>
        public bool ChecksProximity()
        {
            for (int idx = 0; idx < m_spawnCriteria.Length; idx++)
            {
                if (m_spawnCriteria[idx].m_isActive && m_spawnCriteria[idx].m_checkProximity)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Add tags to the list if they are not already there
        /// </summary>
        /// <param name="tagList">The list to add the tags to</param>
        public void AddTags(ref List<string> tagList)
        {
            for (int idx = 0; idx < m_spawnCriteria.Length; idx++)
            {
                if (m_spawnCriteria[idx].m_isActive && m_spawnCriteria[idx].m_checkProximity)
                {
                    if (!tagList.Contains(m_spawnCriteria[idx].m_proximityTag))
                    {
                        tagList.Add(m_spawnCriteria[idx].m_proximityTag);
                    }
                }
            }
        }
    }
}