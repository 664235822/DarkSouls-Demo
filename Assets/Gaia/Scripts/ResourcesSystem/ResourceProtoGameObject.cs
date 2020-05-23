using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gaia
{
    [System.Serializable]
    public class ResourceProtoGameObjectInstance
    {
        [Tooltip("Instance name.")]
        public string m_name;
        [Tooltip("Desktop prefab.")]
        public GameObject m_desktopPrefab;
        [HideInInspector]
        public string m_desktopPrefabFileName; // Used for re-association
        [Tooltip("Mobile prefab - future proofing here - not currently used.")]
        public GameObject m_mobilePrefab;
        [HideInInspector]
        public string m_mobilePrefabFileName; // Used for re-association

        //V1.5 from here

        [Tooltip("Minimum number of instances to spawn.")]
        public int m_minInstances = 1;
        [Tooltip("Maximum number of instances to spawn.")]
        public int m_maxInstances = 1;

        [Tooltip("The failure chance of each spawn attempt."), Range(0f, 1f)]
        public float m_failureRate = 0f;

        [Tooltip("Minimum X offset from spawn point in meters to intantiate at. Can use this to move objects relative to the spawn point chosen.")]
        public float m_minSpawnOffsetX = 0f;
        [Tooltip("Maximum X offset from spawn point in meters to intantiate at. Can use this to move objects relative to the spawn point chosen.")]
        public float m_maxSpawnOffsetX = 0f;

        [Tooltip("Minimum Y offset from terrain in meters to intantiate at. Can use this to move embed or raise objects from the terrain.")]
        public float m_minSpawnOffsetY = -0.3f;
        [Tooltip("Maximum Y offset from terrain in meters to intantiate at. Can use this to move embed or raise objects from the terrain.")]
        public float m_maxSpawnOffsetY = -0.1f;

        [Tooltip("Minimum Z offset from spawn point in meters to intantiate at. Can use this to move objects relative to the spawn point chosen.")]
        public float m_minSpawnOffsetZ = 0f;
        [Tooltip("Maximum Z offset from spawn point in meters to intantiate at. Can use this to move objects relative to the spawn point chosen.")]
        public float m_maxSpawnOffsetZ = 0f;

        [Tooltip("Rotate the object to the terrain normal. Allows natural slope following. Great for things like trees to give them a little more variation in your scene.")]
        public bool m_rotateToSlope = false;

        [Tooltip("Minimum X rotation from spawned rotation to intantiate at. Can use this to rotate objects relative to spawn point rotation.")]
        [Range(-180f, 180f)]
        public float m_minRotationOffsetX = 0f;
        [Tooltip("Maximum X rotation from spawned rotation to intantiate at. Can use this to rotate objects relative to spawn point rotation."), Range(-180f, 180f)]
        public float m_maxRotationOffsetX = 0f;

        [Tooltip("Minimum Y rotation from spawned rotation to intantiate at. Can use this to rotate objects relative to spawn point rotation."), Range(-180f, 180f)]
        public float m_minRotationOffsetY = -180f;
        [Tooltip("Maximum Y rotation from spawned rotation to intantiate at. Can use this to rotate objects relative to spawn point rotation."), Range(-180f, 180f)]
        public float m_maxRotationOffsetY = 180f;

        [Tooltip("Minimum Z rotation from spawned rotation to intantiate at. Can use this to rotate objects relative to spawn point rotation."), Range(-180f, 180f)]
        public float m_minRotationOffsetZ = 0f;
        [Tooltip("Maximum Z rotation from spawned rotation to intantiate at. Can use this to rotate objects relative to spawn point rotation."), Range(-180f, 180f)]
        public float m_maxRotationOffsetZ = 0f;

        [Tooltip("Get object scale from parent scale.")]
        public bool m_useParentScale = true;

        [Tooltip("Minimum scale."), Range(0f, 20f)]
        public float m_minScale = 1f;
        [Tooltip("Maximum scale."), Range(0f, 20)]
        public float m_maxScale = 1f;
        [Tooltip("Influence scale between min and max scale based on distance from spawn point centre.")]
        public AnimationCurve m_scaleByDistance = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

        [Tooltip("Local bounds radius of this instance.")]
        public float m_localBounds = 5f;

        [Tooltip("Will only spawn on virgin terrain.")]
        public bool m_virginTerrain = true;

        [Tooltip("Custom parameter to be interpreted by an extension if there is one.")]
        public string m_extParam = "";

    }

    [System.Serializable]
    public class ResourceProtoGameObject
    {
        [Tooltip("Resource name.")]
        public string m_name;
        [Tooltip("The game objects that will be instantiated when this is spawned.")]
        public ResourceProtoGameObjectInstance[] m_instances = new ResourceProtoGameObjectInstance[0];
        [Tooltip("DNA - Used by the spawner to control how and where the game objects will be spawned.")]
        public ResourceProtoDNA m_dna = new ResourceProtoDNA();
        [Tooltip("SPAWN CRITERIA - Spawn criteria are run against the terrain to assess its fitness in a range of 0..1 for use by this resource. If you add multiple criteria then the fittest one will be selected.")]
        public SpawnCritera[] m_spawnCriteria = new SpawnCritera[0];
        [Tooltip("SPAWN EXTENSIONS - Spawn extensions allow fitness, spawning and post spawning extensions to be made to the spawning system.")]
        public SpawnRuleExtension[] m_spawnExtensions = new SpawnRuleExtension[0];
        [Tooltip("Set this to true if you want this included in tree based spawner creation.")]
        public bool m_canSpawnAsTree = false;

        /// <summary>
        /// Initialise the game object
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
            ResourceProtoGameObjectInstance goInstance;
            for (int idx = 0; idx < m_instances.GetLength(0); idx++)
            {
                goInstance = m_instances[idx];

                if (goInstance.m_desktopPrefab != null)
                {
                    string fileName = Path.GetFileName(AssetDatabase.GetAssetPath(goInstance.m_desktopPrefab));
                    if (fileName != goInstance.m_desktopPrefabFileName)
                    {
                        goInstance.m_desktopPrefabFileName = fileName;
                        isModified = true;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(goInstance.m_desktopPrefabFileName))
                    {
                        goInstance.m_desktopPrefabFileName = "";
                        isModified = true;
                    }
                }

                if (goInstance.m_mobilePrefab != null)
                {
                    string fileName = Path.GetFileName(AssetDatabase.GetAssetPath(goInstance.m_mobilePrefab));
                    if (fileName != goInstance.m_mobilePrefabFileName)
                    {
                        goInstance.m_mobilePrefabFileName = fileName;
                        isModified = true;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(goInstance.m_mobilePrefabFileName))
                    {
                        goInstance.m_mobilePrefabFileName = "";
                        isModified = true;
                    }
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
            ResourceProtoGameObjectInstance goInstance;
            for (int idx = 0; idx < m_instances.GetLength(0); idx++)
            {
                goInstance = m_instances[idx];

                if (goInstance.m_desktopPrefab == null)
                {
                    if (!string.IsNullOrEmpty(goInstance.m_desktopPrefabFileName))
                    {
                        goInstance.m_desktopPrefab = GaiaUtils.GetAsset(goInstance.m_desktopPrefabFileName, typeof(UnityEngine.GameObject)) as GameObject;
                        if (goInstance.m_desktopPrefab != null)
                        {
                            isModified = true;
                        }
                    }
                }

                if (goInstance.m_mobilePrefab == null)
                {
                    if (!string.IsNullOrEmpty(goInstance.m_mobilePrefabFileName))
                    {
                        goInstance.m_mobilePrefab = GaiaUtils.GetAsset(goInstance.m_mobilePrefabFileName, typeof(UnityEngine.GameObject)) as GameObject;
                        if (goInstance.m_mobilePrefab != null)
                        {
                            isModified = true;
                        }
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