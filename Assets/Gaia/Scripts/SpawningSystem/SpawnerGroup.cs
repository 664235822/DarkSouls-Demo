using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gaia
{
    /// <summary>
    /// Allow grouped spawners and spawner control
    /// </summary>
    [ExecuteInEditMode]
    public class SpawnerGroup : MonoBehaviour
    {
        /// <summary>
        /// Spawner instances
        /// </summary>
        [Serializable]
        public class SpawnerInstance
        {
            public string m_name;                   //The name of the spawner
            public Spawner m_spawner;              //The spawner instance we are managing
            public int m_interationsPerSpawn = 1;   //The number of spawner iterations to call per spawn
        }

        /// <summary>
        /// The spawner instances
        /// </summary>
        public List<SpawnerInstance> m_spawners = new List<SpawnerInstance>();

        /// <summary>
        /// The spawner group instances
        /// </summary>
        [HideInInspector]
        public List<SpawnerGroup> m_spawnerGroups = new List<SpawnerGroup>();

        /// <summary>
        /// Use for co-routine simulation
        /// </summary>
        public IEnumerator m_updateCoroutine;

        /// <summary>
        /// Used to signal a cancelled spawn
        /// </summary>
        private bool m_cancelSpawn = false;

        /// <summary>
        /// Used to force an editor update so we can see progress
        /// </summary>
        [HideInInspector]
        public int m_progress = 0;

        /// <summary>
        /// Called by unity in editor when this is enabled - unity initialisation is quite opaque!
        /// </summary>
        void OnEnable()
        {
        }

        void OnDisable()
        {
        }

        /// <summary>
        /// Start editor updates
        /// </summary>
        public void StartEditorUpdates()
        {
            #if UNITY_EDITOR
            EditorApplication.update += EditorUpdate;
            #endif
        }

        //Stop editor updates
        public void StopEditorUpdates()
        {
            #if UNITY_EDITOR
            EditorApplication.update -= EditorUpdate;
            #endif
        }

        /// <summary>
        /// This is executed only in the editor - using it to simulate co-routine execution and update execution
        /// </summary>
        void EditorUpdate()
        {
            if (m_updateCoroutine == null)
            {
                StopEditorUpdates();
                return;
            }
            else
            {
                m_updateCoroutine.MoveNext();
            }
        }


        /// <summary>
        /// Run a spawner iteration
        /// </summary>
        public void RunSpawnerIteration()
        {
            m_cancelSpawn = false;
            m_progress = 0;

            #if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    m_updateCoroutine = RunSpawnerIterationCoRoutine();
                    StartEditorUpdates();
                }
                else
                {
                    StartCoroutine(RunSpawnerIterationCoRoutine());
                }
            #else
                StartCoroutine(RunSpawnerIterationCoRoutine());
            #endif
        }


        /// <summary>
        /// Run a spawner iteration as a coroutine
        /// </summary>
        public IEnumerator RunSpawnerIterationCoRoutine()
        {
            SpawnerInstance si;
            for (int idx = 0; idx < m_spawners.Count; idx++)
            {
                si = m_spawners[idx];
                if (si != null && si.m_spawner != null)
                {
                    for (int iter=0; iter < si.m_interationsPerSpawn; iter++)
                    {
                        if (!m_cancelSpawn)
                        {
                            //si.m_spawner.m_showDebug = true;
                            si.m_spawner.RunSpawnerIteration();
                            yield return new WaitForSeconds(0.2f);
                            while (si.m_spawner.m_spawnComplete != true)
                            {
                                m_progress++; //Forces an editor update
                                yield return new WaitForSeconds(0.5f);
                            }
                            m_progress++;
                            #if UNITY_EDITOR
                            SceneView.RepaintAll();
                            #endif
                        }
                    }
                }
            }
            m_progress = 0;
            m_updateCoroutine = null;
        }

        /// <summary>
        /// Cancel the spawn
        /// </summary>
        public void CancelSpawn()
        {
            //This
            m_cancelSpawn = true;
            for (int idx = 0; idx < m_spawners.Count; idx++)
            {
                m_spawners[idx].m_spawner.CancelSpawn();
            }

            //Groups
            for (int idx = 0; idx < m_spawnerGroups.Count; idx++)
            {
                m_spawnerGroups[idx].CancelSpawn();
            }
        }

        /// <summary>
        /// Update the names from the prefabs - return true if something changed
        /// </summary>
        /// <returns></returns>
        public bool FixNames()
        {
            bool changed = false;
            SpawnerInstance si;
            for (int idx = 0; idx < m_spawners.Count; idx++)
            {
                si = m_spawners[idx];
                if (si != null && si.m_spawner != null)
                {
                    if (si.m_name != si.m_spawner.name)
                    {
                        si.m_name = si.m_spawner.name;
                        changed = true;
                    }
                }
            }
            return changed;
        }

        /// <summary>
        /// Reset the spawners 
        /// </summary>
        public void ResetSpawner()
        {
            //This
            SpawnerInstance si;
            for (int idx = 0; idx < m_spawners.Count; idx++)
            {
                si = m_spawners[idx];
                if (si != null && si.m_spawner != null)
                {
                    si.m_spawner.ResetSpawner();
                }
            }

            //Groups
            SpawnerGroup sg;
            for (int idx = 0; idx < m_spawnerGroups.Count; idx++)
            {
                sg = m_spawnerGroups[idx];
                if (sg != null)
                {
                    sg.ResetSpawner();
                }
            }
        }
    }
}