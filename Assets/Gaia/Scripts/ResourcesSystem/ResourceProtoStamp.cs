using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gaia
{
    /// <summary>
    /// Prototype for stamps and their fitness
    /// </summary>
    [System.Serializable]
    public class ResourceProtoStamp
    {
        [Tooltip("Stamp name.")]
        public string m_name;
        [Tooltip("Stamp texture.")]
        public Texture2D m_texture;
        [Tooltip("How far from the terrain’s anchor point the tiling will start.")]
        public bool m_stickToGround = true;
        [Tooltip("DNA - Used by the spawner to control how and where the tree will be spawned.")]
        public ResourceProtoDNA m_dna;
        [Tooltip("SPAWN CRITERIA - Spawn criteria are run against the terrain to assess its fitness in a range of 0..1 for use by this resource. If you add multiple criteria then the fittest one will be selected.")]
        public SpawnCritera[] m_spawnCriteria = new SpawnCritera[0];

        /// <summary>
        /// Initialise the texture
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
        /// <param name="stampList">The list to add the tags to</param>
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

        /// <summary>
        /// Add stamp path to the list if they are not already there
        /// </summary>
        /// <param name="stampList">The list to add the stamp path to</param>
        public void AddStamps(ref List<string> stampList)
        {
            if (m_texture != null)
            {
                #if UNITY_EDITOR
                string stampPath = UnityEditor.AssetDatabase.GetAssetPath(m_texture);
                if (!stampList.Contains(stampPath))
                {
                    stampList.Add(stampPath);
                }
                #endif
            }
        }

    }
}