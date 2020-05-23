using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

namespace Gaia
{
    [CustomEditor(typeof(Spawner))]
    public class SpawnerEditor : Editor
    {
        GUIStyle m_boxStyle;
        GUIStyle m_wrapStyle;
        Spawner m_spawner;
        DateTime m_timeSinceLastUpdate = DateTime.Now;
        bool m_startedUpdates = false;
        private bool m_showTooltips = true;
        private EditorUtilsOLD m_editorUtils = new EditorUtilsOLD();

        void OnEnable()
        {
            //Get the settings and update tooltips
            GaiaSettings settings = GaiaUtils.GetGaiaSettings();
            if (settings != null)
            {
                m_showTooltips = settings.m_showTooltips;
            }

            //Get our spawner
            m_spawner = (Spawner)target;

            //Clean up any rules that relate to missing resources
            CleanUpRules();

            //Do some simple sanity checks
            if (m_spawner.m_rndGenerator == null)
            {
                m_spawner.m_rndGenerator = new Gaia.XorshiftPlus(m_spawner.m_seed);
            }

            if (m_spawner.m_spawnFitnessAttenuator == null)
            {
                m_spawner.m_spawnFitnessAttenuator = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1.0f), new Keyframe(1f, 0.0f));
            }

            if (m_spawner.m_spawnCollisionLayers.value == 0)
            {
                m_spawner.m_spawnCollisionLayers = Gaia.TerrainHelper.GetActiveTerrainLayer();
            }

            StartEditorUpdates();
        }

        void OnDisable()
        {
        }


        /// <summary>
        /// Start editor updates
        /// </summary>
        public void StartEditorUpdates()
        {
            if (!m_startedUpdates)
            {
                m_startedUpdates = true;
                EditorApplication.update += EditorUpdate;
            }
        }

        /// <summary>
        /// Stop editor updates
        /// </summary>
        public void StopEditorUpdates()
        {
            if (m_startedUpdates)
            {
                m_startedUpdates = false;
                EditorApplication.update -= EditorUpdate;
            }
        }

        /// <summary>
        /// This is used just to force the editor to repaint itself
        /// </summary>
        void EditorUpdate()
        {
            if (m_spawner != null)
            {
                if (m_spawner.m_updateCoroutine != null)
                {
                    if ((DateTime.Now - m_timeSinceLastUpdate).TotalMilliseconds > 500)
                    {
                        //Debug.Log("Active repainting spawner " + m_spawner.gameObject.name);
                        m_timeSinceLastUpdate = DateTime.Now;
                        Repaint();
                    }
                }
                else
                {
                    if ((DateTime.Now - m_timeSinceLastUpdate).TotalSeconds > 5)
                    {
                        //Debug.Log("Inactive repainting spawner " + m_spawner.gameObject.name);
                        m_timeSinceLastUpdate = DateTime.Now;
                        Repaint();
                    }
                }
            }
        }

        /// <summary>
        /// Draw the UI
        /// </summary>
        public override void OnInspectorGUI()
        {
            //Get our spawner
            m_spawner = (Spawner)target;

            //Init editor utils
            m_editorUtils.Initialize();

            //Set up the box style
            if (m_boxStyle == null)
            {
                m_boxStyle = new GUIStyle(GUI.skin.box);
                m_boxStyle.normal.textColor = GUI.skin.label.normal.textColor;
                m_boxStyle.fontStyle = FontStyle.Bold;
                m_boxStyle.alignment = TextAnchor.UpperLeft;
            }

            //Setup the wrap style
            if (m_wrapStyle == null)
            {
                m_wrapStyle = new GUIStyle(GUI.skin.label);
                m_wrapStyle.wordWrap = true;
            }

            //Hide the transform
            //spawner.transform.hideFlags = HideFlags.HideInInspector;

            //Draw the intro
            m_editorUtils.DrawIntro("Spawner", "The Spawner allows you to place features into your terrain. Click here to see a tutorial.", "http://www.procedural-worlds.com/gaia/tutorials/spawner-overview/");

            //Disable if spawning
            if (m_spawner.m_spawnProgress > 0f)
            {
                GUI.enabled = false;
            }

            GaiaResource resources = m_spawner.m_resources;
            if (resources == null)
            {
                GUILayout.BeginVertical("Spawner Settings", m_boxStyle);
                GUILayout.Space(20);
                m_spawner.m_resources = (GaiaResource)EditorGUILayout.ObjectField(GetLabel("Resources"), m_spawner.m_resources, typeof(GaiaResource), false);
                GUILayout.EndVertical();
                return;
            }

            EditorGUI.BeginChangeCheck();

            GUILayout.BeginVertical("Spawner Settings", m_boxStyle);
                GUILayout.Space(20);
                resources = (GaiaResource)EditorGUILayout.ObjectField(GetLabel("Resources"), m_spawner.m_resources, typeof(GaiaResource), false);

                //We only want to go further if we have resources
                int seed = EditorGUILayout.IntField(GetLabel("Seed"), m_spawner.m_seed);
                float spawnRange = EditorGUILayout.FloatField(GetLabel("Range"), m_spawner.m_spawnRange);
                Gaia.GaiaConstants.SpawnerShape spawnerShape = (Gaia.GaiaConstants.SpawnerShape)EditorGUILayout.EnumPopup(GetLabel("Shape"), m_spawner.m_spawnerShape);
                LayerMask spawnerLayerMask = LayerMaskField(GetLabel("Collision Layers"), m_spawner.m_spawnCollisionLayers);
                Gaia.GaiaConstants.OperationMode mode = (Gaia.GaiaConstants.OperationMode)EditorGUILayout.EnumPopup(GetLabel("Execution Mode"), m_spawner.m_mode);

                float spawnerInterval;
                if (mode == GaiaConstants.OperationMode.DesignTime)
                {
                    spawnerInterval = m_spawner.m_spawnInterval;
                }
                else
                {
                    spawnerInterval = EditorGUILayout.FloatField(GetLabel("Spawn Interval"), m_spawner.m_spawnInterval);
                }

                float triggerRange = 0f;
                string triggerTags;
                if (mode != GaiaConstants.OperationMode.RuntimeTriggeredInterval)
                {
                    triggerRange = m_spawner.m_triggerRange;
                    triggerTags = m_spawner.m_triggerTags;
                }
                else
                {
                    triggerRange = EditorGUILayout.FloatField(GetLabel("Trigger Range"), m_spawner.m_triggerRange);
                    triggerTags = EditorGUILayout.TextField(GetLabel("Trigger Tags"), m_spawner.m_triggerTags);
                }

                Gaia.GaiaConstants.SpawnerRuleSelector spawnRuleSelector = (Gaia.GaiaConstants.SpawnerRuleSelector)EditorGUILayout.EnumPopup(GetLabel("Rule Selector"), m_spawner.m_spawnRuleSelector);
                Gaia.GaiaConstants.SpawnerLocation spawnLocationAlgorithm = (Gaia.GaiaConstants.SpawnerLocation)EditorGUILayout.EnumPopup(GetLabel("Location Selector"), m_spawner.m_spawnLocationAlgorithm);
                float locationIncrement = m_spawner.m_locationIncrement;
                float maxJitteredLocationOffsetPct = m_spawner.m_maxJitteredLocationOffsetPct;
                int locationChecksPerInt = m_spawner.m_locationChecksPerInt;
                int maxSeededClusterSize = m_spawner.m_maxRandomClusterSize;
                if (spawnLocationAlgorithm == GaiaConstants.SpawnerLocation.EveryLocation || spawnLocationAlgorithm == GaiaConstants.SpawnerLocation.EveryLocationJittered)
                {
                    locationIncrement = EditorGUILayout.FloatField(GetLabel("Location Increment"), m_spawner.m_locationIncrement);
                    if (spawnLocationAlgorithm == GaiaConstants.SpawnerLocation.EveryLocationJittered)
                    {
                        maxJitteredLocationOffsetPct = EditorGUILayout.Slider(GetLabel("Max Jitter Percent"), m_spawner.m_maxJitteredLocationOffsetPct, 0f, 1f);
                    }
                }
                else
                {
                    locationChecksPerInt = EditorGUILayout.IntSlider(GetLabel("Locations Per Spawn"), m_spawner.m_locationChecksPerInt, 1, 10000);
                    if (spawnLocationAlgorithm == GaiaConstants.SpawnerLocation.RandomLocationClustered)
                    {
                        maxSeededClusterSize = EditorGUILayout.IntSlider(GetLabel("Max Cluster Size"), m_spawner.m_maxRandomClusterSize, 1, 1000);
                    }
                }

                AnimationCurve spawnRangeAttenuator = EditorGUILayout.CurveField(GetLabel("Distance Mask"), m_spawner.m_spawnFitnessAttenuator);
                Gaia.GaiaConstants.ImageFitnessFilterMode areaMaskMode = (Gaia.GaiaConstants.ImageFitnessFilterMode)EditorGUILayout.EnumPopup(GetLabel("Area Mask"), m_spawner.m_areaMaskMode);
                Texture2D imageMask = m_spawner.m_imageMask;
                bool imageMaskInvert = m_spawner.m_imageMaskInvert;
                bool imageMaskNormalise = m_spawner.m_imageMaskNormalise;
                bool imageMaskFlip = m_spawner.m_imageMaskFlip;
                int imageMaskSmoothIterations = m_spawner.m_imageMaskSmoothIterations;

                float noiseMaskSeed = m_spawner.m_noiseMaskSeed;
                int noiseMaskOctaves = m_spawner.m_noiseMaskOctaves;
                float noiseMaskPersistence = m_spawner.m_noiseMaskPersistence;
                float noiseMaskFrequency = m_spawner.m_noiseMaskFrequency;
                float noiseMaskLacunarity = m_spawner.m_noiseMaskLacunarity;
                float noiseZoom = m_spawner.m_noiseZoom;
                bool noiseInvert = m_spawner.m_noiseInvert;

                if (areaMaskMode == GaiaConstants.ImageFitnessFilterMode.ImageAlphaChannel ||
                    areaMaskMode == GaiaConstants.ImageFitnessFilterMode.ImageBlueChannel ||
                    areaMaskMode == GaiaConstants.ImageFitnessFilterMode.ImageGreenChannel ||
                    areaMaskMode == GaiaConstants.ImageFitnessFilterMode.ImageGreyScale ||
                    areaMaskMode == GaiaConstants.ImageFitnessFilterMode.ImageRedChannel)
                {
                    imageMask = (Texture2D)EditorGUILayout.ObjectField(GetLabel("Image Mask"), m_spawner.m_imageMask, typeof(Texture2D), false);
                }
                if (areaMaskMode == GaiaConstants.ImageFitnessFilterMode.None)
                {
                }
                else if (areaMaskMode == GaiaConstants.ImageFitnessFilterMode.PerlinNoise)
                {
                    noiseMaskSeed = EditorGUILayout.Slider(GetLabel("Noise Seed"), noiseMaskSeed, 0f, 65000f);
                    noiseMaskOctaves = EditorGUILayout.IntSlider(GetLabel("Octaves"), noiseMaskOctaves, 1, 12);
                    noiseMaskPersistence = EditorGUILayout.Slider(GetLabel("Persistence"), noiseMaskPersistence, 0f, 1f);
                    noiseMaskFrequency = EditorGUILayout.Slider(GetLabel("Frequency"), noiseMaskFrequency, 0f, 1f);
                    noiseMaskLacunarity = EditorGUILayout.Slider(GetLabel("Lacunarity"), noiseMaskLacunarity, 1.5f, 3.5f);
                    noiseZoom = EditorGUILayout.Slider(GetLabel("Zoom"), noiseZoom, 1f, 1000f);
                    noiseInvert = EditorGUILayout.Toggle(GetLabel("Invert"), noiseInvert);
                }
                else
                {
                    imageMaskSmoothIterations = EditorGUILayout.IntSlider(GetLabel("Smooth Mask"), m_spawner.m_imageMaskSmoothIterations, 0, 20);
                    imageMaskNormalise = EditorGUILayout.Toggle(GetLabel("Normalise Mask"), m_spawner.m_imageMaskNormalise);
                    imageMaskInvert = EditorGUILayout.Toggle(GetLabel("Invert Mask"), m_spawner.m_imageMaskInvert);
                    imageMaskFlip = EditorGUILayout.Toggle(GetLabel("Flip Mask"), m_spawner.m_imageMaskFlip);
                }

                bool enableColliderCacheAtRuntime = m_spawner.m_enableColliderCacheAtRuntime;
                if (m_spawner.IsGameObjectSpawner())
                {
                    enableColliderCacheAtRuntime = EditorGUILayout.Toggle(GetLabel("Enable RT Collider Cache"), enableColliderCacheAtRuntime);
                }

            GUILayout.EndVertical();

            //Back the rules up
            SpawnRule rule, newRule;
            List<SpawnRule> ruleBackup = new List<SpawnRule>();
            for (int idx = 0; idx < m_spawner.m_spawnerRules.Count; idx++)
            {
                rule = m_spawner.m_spawnerRules[idx];
                newRule = new SpawnRule();
                newRule.m_activeInstanceCnt = rule.m_activeInstanceCnt;
                newRule.m_currInstanceCnt = rule.m_currInstanceCnt;
                newRule.m_failureRate = rule.m_failureRate;
                newRule.m_inactiveInstanceCnt = rule.m_inactiveInstanceCnt;
                newRule.m_isActive = rule.m_isActive;
                newRule.m_isFoldedOut = rule.m_isFoldedOut;
                newRule.m_maxInstances = rule.m_maxInstances;
                newRule.m_ignoreMaxInstances = rule.m_ignoreMaxInstances;
                newRule.m_minViableFitness = rule.m_minViableFitness;
                newRule.m_name = rule.m_name;

                newRule.m_minDirection = rule.m_minDirection;
                newRule.m_maxDirection = rule.m_maxDirection;

                newRule.m_noiseMask = rule.m_noiseMask;
                newRule.m_noiseMaskFrequency = rule.m_noiseMaskFrequency;
                newRule.m_noiseMaskLacunarity = rule.m_noiseMaskLacunarity;
                newRule.m_noiseMaskOctaves = rule.m_noiseMaskOctaves;
                newRule.m_noiseMaskPersistence = rule.m_noiseMaskPersistence;
                newRule.m_noiseMaskSeed = rule.m_noiseMaskSeed;
                newRule.m_noiseZoom = rule.m_noiseZoom;
                newRule.m_noiseInvert = rule.m_noiseInvert;
                newRule.m_noiseStrength = rule.m_noiseStrength;

                newRule.m_resourceType = rule.m_resourceType;
                newRule.m_resourceIdx = rule.m_resourceIdx;
                ruleBackup.Add(newRule);
            }

            //Display the rules
            GUILayout.BeginVertical(m_boxStyle);
            GUILayout.Space(3);
            //GUILayout.BeginVertical("Spawner Rules", m_boxStyle);
            EditorGUILayout.LabelField("Spawner Rules");
            //GUILayout.Space(21);

            Rect addRect = EditorGUILayout.BeginVertical();
            addRect.y -= 20f;
            addRect.x = addRect.width - 10;
            addRect.width = 25;
            addRect.height = 20;
            if (GUI.Button(addRect, GetLabel("+")))
            {
                ruleBackup.Add(new SpawnRule());
            }

            addRect.x -= 27f;
            if (GUI.Button(addRect, GetLabel("A")))
            {
                for (int idx = 0; idx < ruleBackup.Count; idx++)
                {
                    ruleBackup[idx].m_isActive = true;
                }
            }

            addRect.x -= 27f;
            if (GUI.Button(addRect, GetLabel("I")))
            {
                for (int idx=0; idx < ruleBackup.Count; idx++)
                {
                    ruleBackup[idx].m_isActive = false;
                }
            }

            addRect.x -= 27f;
            if (GUI.Button(addRect, GetLabel("X")))
            {
                if (EditorUtility.DisplayDialog("Delete All Rules ?", "Are you sure you want to delete all rules - this can not be undone ?", "Yes", "No"))
                {
                    ruleBackup.Clear();
                }
            }

            EditorGUILayout.EndVertical();

            EditorGUI.indentLevel++;
            for (int ruleIdx = 0; ruleIdx < ruleBackup.Count; ruleIdx++)
            {
                rule = ruleBackup[ruleIdx];
                if (rule.m_isActive)
                {
                    rule.m_isFoldedOut = EditorGUILayout.Foldout(rule.m_isFoldedOut, rule.m_name, true);
                }
                else
                {
                    rule.m_isFoldedOut = EditorGUILayout.Foldout(rule.m_isFoldedOut, rule.m_name + " [inactive]", true);
                }
                if (rule.m_isFoldedOut)
                {
                    rule.m_resourceType = (Gaia.GaiaConstants.SpawnerResourceType)EditorGUILayout.EnumPopup(GetLabel("Resource Type"), rule.m_resourceType);

                    GUIContent[] assetChoices = null;
                    switch (rule.m_resourceType)
                    {
                        case GaiaConstants.SpawnerResourceType.TerrainTexture:
                            {
                                assetChoices = new GUIContent[m_spawner.m_resources.m_texturePrototypes.Length];
                                for (int assetIdx = 0; assetIdx < m_spawner.m_resources.m_texturePrototypes.Length; assetIdx++)
                                {
                                    assetChoices[assetIdx] = new GUIContent(m_spawner.m_resources.m_texturePrototypes[assetIdx].m_name);
                                }
                                break;
                            }
                        case GaiaConstants.SpawnerResourceType.TerrainDetail:
                            {
                                assetChoices = new GUIContent[m_spawner.m_resources.m_detailPrototypes.Length];
                                for (int assetIdx = 0; assetIdx < m_spawner.m_resources.m_detailPrototypes.Length; assetIdx++)
                                {
                                    assetChoices[assetIdx] = new GUIContent(m_spawner.m_resources.m_detailPrototypes[assetIdx].m_name);
                                }
                                break;
                            }
                        case GaiaConstants.SpawnerResourceType.TerrainTree:
                            {
                                assetChoices = new GUIContent[m_spawner.m_resources.m_treePrototypes.Length];
                                for (int assetIdx = 0; assetIdx < m_spawner.m_resources.m_treePrototypes.Length; assetIdx++)
                                {
                                    assetChoices[assetIdx] = new GUIContent(m_spawner.m_resources.m_treePrototypes[assetIdx].m_name);
                                }
                                break;
                            }
                        case GaiaConstants.SpawnerResourceType.GameObject:
                            {
                                assetChoices = new GUIContent[m_spawner.m_resources.m_gameObjectPrototypes.Length];
                                for (int assetIdx = 0; assetIdx < m_spawner.m_resources.m_gameObjectPrototypes.Length; assetIdx++)
                                {
                                    assetChoices[assetIdx] = new GUIContent(m_spawner.m_resources.m_gameObjectPrototypes[assetIdx].m_name);
                                }
                                break;
                            }
                        /*
                    default:
                        {
                            assetChoices = new GUIContent[m_spawner.m_resources.m_stampPrototypes.Length];
                            for (int assetIdx = 0; assetIdx < m_spawner.m_resources.m_stampPrototypes.Length; assetIdx++)
                            {
                                assetChoices[assetIdx] = new GUIContent(m_spawner.m_resources.m_stampPrototypes[assetIdx].m_name);
                            }
                            break;
                        } */
                    }

                    if (assetChoices.Length <= 0)
                    {

                    }
                    else
                    {
                        rule.m_resourceIdx = EditorGUILayout.Popup(GetLabel("Selected Resource"), rule.m_resourceIdx, assetChoices);
                        switch (rule.m_resourceType)
                        {
                            case GaiaConstants.SpawnerResourceType.TerrainTexture:
                                {
                                    rule.m_name = m_spawner.m_resources.m_texturePrototypes[rule.m_resourceIdx].m_name;
                                    break;
                                }
                            case GaiaConstants.SpawnerResourceType.TerrainDetail:
                                {
                                    rule.m_name = m_spawner.m_resources.m_detailPrototypes[rule.m_resourceIdx].m_name;
                                    break;
                                }
                            case GaiaConstants.SpawnerResourceType.TerrainTree:
                                {
                                    rule.m_name = m_spawner.m_resources.m_treePrototypes[rule.m_resourceIdx].m_name;
                                    break;
                                }
                            case GaiaConstants.SpawnerResourceType.GameObject:
                                {
                                    rule.m_name = m_spawner.m_resources.m_gameObjectPrototypes[rule.m_resourceIdx].m_name;

                                    //See if we can find a custom fitness
                                    if (m_spawner.m_resources.m_gameObjectPrototypes[rule.m_resourceIdx].m_instances.Length > 0)
                                    {
                                        GameObject go = m_spawner.m_resources.m_gameObjectPrototypes[rule.m_resourceIdx].m_instances[0].m_desktopPrefab;
                                        bool gotExtension = false;
                                        //if (go.GetComponent<ISpawnRuleExtension>() != null)
                                        //{
                                        //    gotExtension = true;
                                        //}
                                        //else
                                        //{
                                        //    if (go.GetComponentInChildren<ISpawnRuleExtension>() != null)
                                        //    {
                                        //        gotExtension = true;
                                        //    }
                                        //}
                                        if (gotExtension)
                                        {
                                            Debug.Log("Got a spawn rule extension on " + go.name);
                                        }
                                    }
                                    break;
                                }
                            /*
                        default:
                            {
                                rule.m_name = m_spawner.m_resources.m_stampPrototypes[rule.m_resourceIdx].m_name;
                                break;
                            } */
                        }

                        //Check to see if we can use extended fitness and spawner

                        rule.m_minViableFitness = EditorGUILayout.Slider(GetLabel("Min Viable Fitness"), rule.m_minViableFitness, 0f, 1f);
                        rule.m_failureRate = EditorGUILayout.Slider(GetLabel("Failure Rate"), rule.m_failureRate, 0f, 1f);
                        rule.m_maxInstances = (ulong)EditorGUILayout.LongField(GetLabel("Max Instances"), (long)rule.m_maxInstances);
                        rule.m_ignoreMaxInstances = EditorGUILayout.Toggle(GetLabel("Ignore Max Instances"), rule.m_ignoreMaxInstances);

                        rule.m_noiseMask = (Gaia.GaiaConstants.NoiseType)EditorGUILayout.EnumPopup(GetLabel("Noise Mask"), rule.m_noiseMask);
                        if (rule.m_noiseMask != GaiaConstants.NoiseType.None)
                        {
                            rule.m_noiseMaskSeed = EditorGUILayout.Slider(GetLabel("Seed"), rule.m_noiseMaskSeed, 0f, 65000f);
                            rule.m_noiseMaskOctaves = EditorGUILayout.IntSlider(GetLabel("Octaves"), rule.m_noiseMaskOctaves, 1, 12);
                            rule.m_noiseMaskPersistence = EditorGUILayout.Slider(GetLabel("Persistence"), rule.m_noiseMaskPersistence, 0f, 1f);
                            rule.m_noiseMaskFrequency = EditorGUILayout.Slider(GetLabel("Frequency"), rule.m_noiseMaskFrequency, 0f, 32f);
                            rule.m_noiseMaskLacunarity = EditorGUILayout.Slider(GetLabel("Lacunarity"), rule.m_noiseMaskLacunarity, 1.5f, 3.5f);
                            rule.m_noiseStrength = EditorGUILayout.Slider(GetLabel("Strength"), rule.m_noiseStrength, 0f, 2f);
                            rule.m_noiseZoom = EditorGUILayout.Slider(GetLabel("Zoom"), rule.m_noiseZoom, 1f, 1000f);
                            rule.m_noiseInvert = EditorGUILayout.Toggle(GetLabel("Invert"), rule.m_noiseInvert);
                        }


                        //Direction control for spawned POI
                        if (rule.m_resourceType == GaiaConstants.SpawnerResourceType.GameObject)
                        {
                            rule.m_minDirection = EditorGUILayout.Slider(GetLabel("Min Direction"), rule.m_minDirection, 0f, 359.9f);
                            rule.m_maxDirection = EditorGUILayout.Slider(GetLabel("Max Direction"), rule.m_maxDirection, 0f, 359.9f);
                            if (rule.m_minDirection > rule.m_maxDirection)
                            {
                                rule.m_minDirection = rule.m_maxDirection;
                            }
                            else if (rule.m_maxDirection < rule.m_minDirection)
                            {
                                rule.m_maxDirection = rule.m_minDirection;
                            }
                        }

                        rule.m_isActive = EditorGUILayout.Toggle(GetLabel("Active"), rule.m_isActive);

                        if (m_spawner.m_showStatistics)
                        {
                            EditorGUILayout.LabelField(GetLabel("Instances Spawned"), new GUIContent(rule.m_activeInstanceCnt.ToString()));
                        }
                    }

                    GUILayout.Space(20);
                    Rect delRect = EditorGUILayout.BeginHorizontal();
                    delRect.x += 17;
                    delRect.y -= 20;
                    delRect.width = 17;
                    delRect.height += 18;
                    if (GUI.Button(delRect, GetLabel("-")))
                    {
                        ruleBackup.Remove(rule);
                    }

                    delRect.x += 25f;
                    delRect.width += 50f;
                    if (GUI.Button(delRect, GetLabel("Visualise")))
                    {
                        GameObject gaiaObj = GameObject.Find("Gaia");
                        if (gaiaObj == null)
                        {
                            gaiaObj = new GameObject("Gaia");
                        }
                        GameObject visualiserObj = GameObject.Find("Visualiser");
                        if (visualiserObj == null)
                        {
                            visualiserObj = new GameObject("Visualiser");
                            visualiserObj.AddComponent<ResourceVisualiser>();
                            visualiserObj.transform.parent = gaiaObj.transform;
                            visualiserObj.transform.position = Gaia.TerrainHelper.GetActiveTerrainCenter();
                        }
                        ResourceVisualiser visualiser = visualiserObj.GetComponent<ResourceVisualiser>();
                        visualiser.m_resources = m_spawner.m_resources;
                        visualiser.m_selectedResourceType = rule.m_resourceType;
                        visualiser.m_selectedResourceIdx = rule.m_resourceIdx;
                        Selection.activeGameObject = visualiserObj;
                    }
                    EditorGUILayout.EndHorizontal();

                }
            }
            EditorGUI.indentLevel--;
            GUILayout.EndVertical();

            //Show statistics or not
            if (m_spawner.m_showStatistics)
            {
                GUILayout.BeginVertical("Statistics", m_boxStyle);
                GUILayout.Space(20);
                EditorGUILayout.LabelField(GetLabel("Active Rules"), GetLabel(m_spawner.m_activeRuleCnt.ToString()));
                EditorGUILayout.LabelField(GetLabel("Inactive Rules"), GetLabel(m_spawner.m_inactiveRuleCnt.ToString()));
                //EditorGUILayout.LabelField(GetLabel("TOTAL Rules"), GetLabel(m_spawner.m_totalRuleCnt.ToString()));
                GUILayout.Space(8);
                //EditorGUILayout.LabelField(GetLabel("Active Instances"), GetLabel(m_spawner.m_activeInstanceCnt.ToString()));
                //EditorGUILayout.LabelField(GetLabel("Inactive Instances"), GetLabel(m_spawner.m_inactiveInstanceCnt.ToString()));
                EditorGUILayout.LabelField(GetLabel("TOTAL Instances"), GetLabel(m_spawner.m_totalInstanceCnt.ToString()));
                EditorGUILayout.LabelField(GetLabel("MAX INSTANCES"), GetLabel(m_spawner.m_maxInstanceCnt.ToString()));
                GUILayout.EndVertical();
            }

            GUILayout.BeginVertical("Layout Helpers", m_boxStyle);
            GUILayout.Space(20);
            bool showGizmos = EditorGUILayout.Toggle(GetLabel("Show Gizmos"), m_spawner.m_showGizmos);
            bool showStatistics = m_spawner.m_showStatistics = EditorGUILayout.Toggle(GetLabel("Show Statistics"), m_spawner.m_showStatistics);
            bool showTerrainHelper = m_spawner.m_showTerrainHelper = EditorGUILayout.Toggle(GetLabel("Show Terrain Helper"), m_spawner.m_showTerrainHelper);
            GUILayout.EndVertical();

            //Check for changes, make undo record, make changes and let editor know we are dirty
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_spawner, "Made changes");
                m_spawner.m_mode = mode;
                m_spawner.m_seed = seed;
                m_spawner.m_spawnerShape = spawnerShape;
                m_spawner.m_spawnRuleSelector = spawnRuleSelector;
                m_spawner.m_spawnLocationAlgorithm = spawnLocationAlgorithm;
                m_spawner.m_spawnCollisionLayers = spawnerLayerMask;
                m_spawner.m_locationIncrement = locationIncrement;
                m_spawner.m_maxJitteredLocationOffsetPct = maxJitteredLocationOffsetPct;
                m_spawner.m_locationChecksPerInt = locationChecksPerInt;
                m_spawner.m_maxRandomClusterSize = maxSeededClusterSize;
                m_spawner.m_spawnRange = spawnRange;
                m_spawner.m_spawnFitnessAttenuator = spawnRangeAttenuator;
                m_spawner.m_areaMaskMode = areaMaskMode;
                m_spawner.m_imageMask = imageMask;
                m_spawner.m_imageMaskInvert = imageMaskInvert;
                m_spawner.m_imageMaskFlip = imageMaskFlip;
                m_spawner.m_imageMaskSmoothIterations = imageMaskSmoothIterations;
                m_spawner.m_imageMaskNormalise = imageMaskNormalise;

                m_spawner.m_noiseMaskSeed = noiseMaskSeed;
                m_spawner.m_noiseMaskOctaves = noiseMaskOctaves;
                m_spawner.m_noiseMaskPersistence = noiseMaskPersistence;
                m_spawner.m_noiseMaskFrequency = noiseMaskFrequency;
                m_spawner.m_noiseMaskLacunarity = noiseMaskLacunarity;
                m_spawner.m_noiseZoom = noiseZoom;
                m_spawner.m_noiseInvert = noiseInvert;

                m_spawner.m_triggerRange = triggerRange;
                m_spawner.m_triggerTags = triggerTags;
                m_spawner.m_spawnInterval = spawnerInterval;
                m_spawner.m_resources = resources;
                m_spawner.m_spawnerRules = ruleBackup;
                m_spawner.m_showGizmos = showGizmos;
                m_spawner.m_showStatistics = showStatistics;
                m_spawner.m_showTerrainHelper = showTerrainHelper;

                m_spawner.m_enableColliderCacheAtRuntime = enableColliderCacheAtRuntime;

                if (m_spawner.m_imageMask != null)
                {
                    GaiaUtils.MakeTextureReadable(m_spawner.m_imageMask);
                    GaiaUtils.MakeTextureUncompressed(m_spawner.m_imageMask);
                }

                EditorUtility.SetDirty(m_spawner);
            }

            //Terrain control
            if (showTerrainHelper)
            {
                GUILayout.BeginVertical("Terrain Helper", m_boxStyle);
                GUILayout.Space(20);
                GUILayout.BeginHorizontal();

                if (GUILayout.Button(GetLabel("Flatten")))
                {
                    if (EditorUtility.DisplayDialog("Flatten Terrain tiles ?", "Are you sure you want to flatten all terrain tiles - this can not be undone ?", "Yes", "No"))
                    {
                        m_spawner.FlattenTerrain();
                    }
                }
                if (GUILayout.Button(GetLabel("Smooth")))
                {
                    if (EditorUtility.DisplayDialog("Smooth Terrain tiles ?", "Are you sure you want to smooth all terrain tiles - this can not be undone ?", "Yes", "No"))
                    {
                        m_spawner.SmoothTerrain();
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button(GetLabel("Clear Trees")))
                {
                    if (EditorUtility.DisplayDialog("Clear Terrain trees ?", "Are you sure you want to clear all terrain trees - this can not be undone ?", "Yes", "No"))
                    {
                        m_spawner.ClearTrees();
                    }
                }
                if (GUILayout.Button(GetLabel("Clear Details")))
                {
                    if (EditorUtility.DisplayDialog("Clear Terrain details ?", "Are you sure you want to clear all terrain details - this can not be undone ?", "Yes", "No"))
                    {
                        m_spawner.ClearDetails();
                    }
                }

                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                GUILayout.EndVertical();
            }

            //Ragardless, re-enable the spawner controls
            GUI.enabled = true;
            
            //Display progress
            if (m_spawner.m_spawnProgress > 0f && m_spawner.m_spawnProgress < 1f)
            {
                GUILayout.BeginVertical("Spawn Controller", m_boxStyle);
                GUILayout.Space(20);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(GetLabel("Cancel")))
                {
                    m_spawner.CancelSpawn();
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                GUILayout.EndVertical();
                ProgressBar(string.Format("Progress ({0:0.0}%)", m_spawner.m_spawnProgress * 100f), m_spawner.m_spawnProgress);
            }
            else
            {
                GUILayout.BeginVertical("Spawn Controller", m_boxStyle);
                GUILayout.Space(20);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(GetLabel("Ground")))
                {
                    m_spawner.GroundToTerrain();
                }
                if (GUILayout.Button(GetLabel("Fit To Terrain")))
                {
                    m_spawner.FitToTerrain();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button(GetLabel("Reset")))
                {
                    m_spawner.ResetSpawner();
                }
                if (GUILayout.Button(GetLabel("Spawn")))
                {
                    //Check that they have a single selected terrain
                    if (Gaia.TerrainHelper.GetActiveTerrainCount() != 1)
                    {
                        EditorUtility.DisplayDialog("OOPS!", "You must have only one active terrain in order to use a Spawner. Please either add a terrain, or deactivate all but one terrain.", "OK");
                    }
                    else
                    {
                        bool cancel = false;

                        //Check that the terrain layer is selected
                        if (!GaiaCommon1.Utils.IsInLayerMask(Gaia.TerrainHelper.GetActiveTerrain().gameObject, m_spawner.m_spawnCollisionLayers))
                        {
                            if (EditorUtility.DisplayDialog("WARNING!", "This feature requires your Spawner to have a layer mask which includes the Terrain layer.", "Add Terrain Layer To Spawner", "Cancel"))
                            {
                                m_spawner.m_spawnCollisionLayers.value |= Gaia.TerrainHelper.GetActiveTerrainLayer().value;
                            }
                            else
                            {
                                Debug.Log("Spawn cancelled");
                                cancel = true;
                            }
                        }

                        //Check that the sea level matches the sea level of the session
                        GaiaSessionManager sessionMgr = GaiaSessionManager.GetSessionManager();
                        if (sessionMgr != null && sessionMgr.m_session != null)
                        {
                            if (m_spawner.m_resources.m_seaLevel != sessionMgr.m_session.m_seaLevel)
                            {
                                if (EditorUtility.DisplayDialog("WARNING!", "Your resources sea level does not match your session sea level. To ensure consistent treatment of heights while spawning your resources sea level should match your session sea level.", "Update Resources Sea Level ?", "Ignore"))
                                {
                                    m_spawner.m_resources.ChangeSeaLevel(m_spawner.m_resources.m_seaLevel, sessionMgr.m_session.m_seaLevel);
                                }
                            }
                        }
                        
                        //Check that the resources are in the terrain
                        if (!cancel)
                        {
                            m_spawner.AssociateAssets();
                            int[] missingResources = m_spawner.GetMissingResources();
                            if (missingResources.GetLength(0) > 0)
                            {
                                SpawnRule missingRule;
                                StringBuilder sb = new StringBuilder();
                                for (int idx = 0; idx < missingResources.GetLength(0); idx++)
                                {
                                    missingRule = m_spawner.m_spawnerRules[missingResources[idx]];
                                    if (idx != 0)
                                    {
                                        sb.Append(", ");
                                    }
                                    sb.Append(missingRule.m_name);
                                }

                                if (EditorUtility.DisplayDialog("WARNING!", "The following resources are missing from the terrain! " + sb.ToString(), "Add Them Now ?", "Cancel"))
                                {
                                    m_spawner.AddResourcesToTerrain(missingResources);
                                }
                                else
                                {
                                    Debug.Log("Spawn cancelled");
                                    cancel = true;
                                }
                            }
                        }

                        if (!cancel)
                        {
                            //Check that they are not using terrain based mask - this can give unexpected results
                            if (areaMaskMode == GaiaConstants.ImageFitnessFilterMode.TerrainTexture0 ||
                                areaMaskMode == GaiaConstants.ImageFitnessFilterMode.TerrainTexture1 ||
                                areaMaskMode == GaiaConstants.ImageFitnessFilterMode.TerrainTexture2 ||
                                areaMaskMode == GaiaConstants.ImageFitnessFilterMode.TerrainTexture3 ||
                                areaMaskMode == GaiaConstants.ImageFitnessFilterMode.TerrainTexture4 ||
                                areaMaskMode == GaiaConstants.ImageFitnessFilterMode.TerrainTexture5 ||
                                areaMaskMode == GaiaConstants.ImageFitnessFilterMode.TerrainTexture6 ||
                                areaMaskMode == GaiaConstants.ImageFitnessFilterMode.TerrainTexture7
                                )
                            {
                                //Do an alert and fix if necessary
                                if (!m_spawner.IsFitToTerrain())
                                {
                                    if (EditorUtility.DisplayDialog("WARNING!", "This feature requires your Spawner to be Fit To Terrain in order to guarantee correct placement.", "Spawn Anyway", "Cancel"))
                                    {
                                        m_spawner.RunSpawnerIteration();
                                    }
                                    else
                                    {
                                        Debug.Log("Spawn cancelled");
                                    }
                                }
                                else
                                {
                                    m_spawner.RunSpawnerIteration();
                                }
                            }
                            else
                            {
                                m_spawner.RunSpawnerIteration();
                            }
                        }
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                GUILayout.EndVertical();
            }

            //if (GUILayout.Button("Deactivate"))
            //{
            //    spawner.DeactivateAllInstances();
            //}

            GUI.enabled = true;

            GUILayout.Space(5f);
        }

        /// <summary>
        /// Delete any old rules left over from previous resources / changes to resources
        /// </summary>
        void CleanUpRules()
        {
            //Drop out if no spawner or resources
            if (m_spawner == null || m_spawner.m_resources == null)
            {
                return;
            }

            //Drop out if spawner doesnt have resources
            int idx = 0;
            SpawnRule rule;
            bool dirty = false;
            while (idx < m_spawner.m_spawnerRules.Count)
            {
                rule = m_spawner.m_spawnerRules[idx];

                switch (rule.m_resourceType)
                {
                    case GaiaConstants.SpawnerResourceType.TerrainTexture:
                        {
                            if (rule.m_resourceIdx >= m_spawner.m_resources.m_texturePrototypes.Length)
                            {
                                m_spawner.m_spawnerRules.RemoveAt(idx);
                                dirty = true;
                            }
                            else
                            {
                                if (rule.m_name != m_spawner.m_resources.m_texturePrototypes[rule.m_resourceIdx].m_name)
                                {
                                    rule.m_name = m_spawner.m_resources.m_texturePrototypes[rule.m_resourceIdx].m_name;
                                    dirty = true;
                                }
                                idx++;
                            }
                            break;
                        }
                    case GaiaConstants.SpawnerResourceType.TerrainDetail:
                        {
                            if (rule.m_resourceIdx >= m_spawner.m_resources.m_detailPrototypes.Length)
                            {
                                m_spawner.m_spawnerRules.RemoveAt(idx);
                                dirty = true;
                            }
                            else
                            {
                                if (rule.m_name != m_spawner.m_resources.m_detailPrototypes[rule.m_resourceIdx].m_name)
                                {
                                    rule.m_name = m_spawner.m_resources.m_detailPrototypes[rule.m_resourceIdx].m_name;
                                    dirty = true;
                                }
                                idx++;
                            }
                            break;
                        }
                    case GaiaConstants.SpawnerResourceType.TerrainTree:
                        {
                            if (rule.m_resourceIdx >= m_spawner.m_resources.m_treePrototypes.Length)
                            {
                                m_spawner.m_spawnerRules.RemoveAt(idx);
                                dirty = true;
                            }
                            else
                            {
                                if (rule.m_name != m_spawner.m_resources.m_treePrototypes[rule.m_resourceIdx].m_name)
                                {
                                    rule.m_name = m_spawner.m_resources.m_treePrototypes[rule.m_resourceIdx].m_name;
                                    dirty = true;
                                }
                                idx++;
                            }
                            break;
                        }
                    case GaiaConstants.SpawnerResourceType.GameObject:
                        {
                            if (rule.m_resourceIdx >= m_spawner.m_resources.m_gameObjectPrototypes.Length)
                            {
                                m_spawner.m_spawnerRules.RemoveAt(idx);
                                dirty = true;
                            }
                            else
                            {
                                if (rule.m_name != m_spawner.m_resources.m_gameObjectPrototypes[rule.m_resourceIdx].m_name)
                                {
                                    rule.m_name = m_spawner.m_resources.m_gameObjectPrototypes[rule.m_resourceIdx].m_name;
                                    dirty = true;
                                }
                                idx++;
                            }
                            break;
                        }
                        /*
                    default:
                        {
                            if (rule.m_resourceIdx >= m_spawner.m_resources.m_stampPrototypes.Length)
                            {
                                m_spawner.m_spawnerRules.RemoveAt(idx);
                                deleted = true;
                            }
                            else
                            {
                                idx++;
                            }
                            break;
                        } */
                }
            }
            //Mark it as dirty if we deleted something
            if (dirty)
            {
                Debug.LogWarning(string.Format("{0} : There was a mismatch between your spawner settings and your resources file. Spawner settings have been updated to match resources.", m_spawner.name));
                EditorUtility.SetDirty(m_spawner);
            }
        }

        /// <summary>
        /// Draw a progress bar
        /// </summary>
        /// <param name="label"></param>
        /// <param name="value"></param>

        void ProgressBar(string label, float value)
        {
            // Get a rect for the progress bar using the same margins as a textfield:
            Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
            EditorGUI.ProgressBar(rect, value, label);
            EditorGUILayout.Space();
        }

        /// <summary>
        /// Handy layer mask interface
        /// </summary>
        /// <param name="label"></param>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        static LayerMask LayerMaskField(GUIContent label, LayerMask layerMask)
        {
            List<string> layers = new List<string>();
            List<int> layerNumbers = new List<int>();

            for (int i = 0; i < 32; i++)
            {
                string layerName = LayerMask.LayerToName(i);
                if (layerName != "")
                {
                    layers.Add(layerName);
                    layerNumbers.Add(i);
                }
            }
            int maskWithoutEmpty = 0;
            for (int i = 0; i < layerNumbers.Count; i++)
            {
                if (((1 << layerNumbers[i]) & layerMask.value) > 0)
                    maskWithoutEmpty |= (1 << i);
            }
            maskWithoutEmpty = EditorGUILayout.MaskField(label, maskWithoutEmpty, layers.ToArray());
            int mask = 0;
            for (int i = 0; i < layerNumbers.Count; i++)
            {
                if ((maskWithoutEmpty & (1 << i)) > 0)
                    mask |= (1 << layerNumbers[i]);
            }
            layerMask.value = mask;
            return layerMask;
        }

        /// <summary>
        /// Get a content label - look the tooltip up if possible
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        GUIContent GetLabel(string name)
        {
            string tooltip = "";
            if (m_showTooltips && m_tooltips.TryGetValue(name, out tooltip))
            {
                return new GUIContent(name, tooltip);
            }
            else
            {
                return new GUIContent(name);
            }
        }

        /// <summary>
        /// The tooltips
        /// </summary>
        static Dictionary<string, string> m_tooltips = new Dictionary<string, string>
        {
            { "Resources", "The object that contains the resources that these rules will apply to." },
            { "Execution Mode", "The way this spawner runs. Design time : At design time only. Runtime Interval : At run time on a timed interval. Runtime Triggered Interval : At run time on a timed interval, and only when the tagged game object is closer than the trigger range from the center of the spawner." },
            { "Shape", "The shape of the spawn area. The spawner will only spawn within this area." },
            { "Range","Distance in meters from the centre of the spawner that the spawner can spawn in. Shown as a red box or sphere in the gizmos." },
            { "Spawn Interval", "The time in seconds between spawn iterations." },
            { "Trigger Range","Distance in meters from the centre of the spawner that the trigger will activate." },
            { "Trigger Tags","The tags of the game objects that will set the spawner off. Multiple tags can be separated by commas eg Player,Minion etc." },
            { "Rule Selector", "The way a rule is selected to be spawned. \nAll : All rules are selected. \nFittest : Only the rule with the fittest spawn criteria is selected. If multiple rules have the same fitness then one will be randomly selected.\nWeighted Fittest : The chance of a rule being selected is directly proportional to its fitness. Fitter rules have more chance of selection. Use this to create more natural blends between objects.\nRandom : Rule selection is random." },
            { "Collision Layers", "Controls which layers are checked for collisions when spawning. Must at least include the layer the terrain is on. Add additional layers if other collisions need to be detected as well. Influences terrain detection, tree detection and game object detection." },
            { "Location Selector", "How the spawner selects locations to spawn in. \nEvery Location: The spawner will attempt to spawn at every location. \nEvery Location Jittered: The spawner will attempt to spawn at every location, but will offset the location by a random jitter factor. Use this to break up lines.\nRandom Location: The spawner will attempt to spawn at random locations.\nRandom Location Clustered: The spawner will attempt to spawn clusters at random locations." },
            { "Location Increment", "The distance from the last location that every new location will be incremented in meters." },
            { "Max Jitter Percent", "Every new location will be offset by a random distance up to a maximum of the jitter percentage multiplied by the location increment." },
            { "Locations Per Spawn", "The number of locations that will be checked every Spawn interval. This does not guarantee that something will be spawned at that location, because lack of fitness may preclude that location from being used." },
            { "Max Cluster Size", "The maximum individuals in a cluster before a new cluster is started." },

            { "X", "Delete all rules."},
            { "I", "Inavtivate all rules."},
            { "A", "Activate all rules."},
            { "+", "Add a rule."},
            { "-", "Delete the rule."},
            { "Visualise", "Visualise this rule in the visualiser."},

            { "Distance Mask", "Mask fitness over distance. Left hand side of curve represents the centre of the spawner. Use this to alter spawn success away from centre e.g. peter out towards edges."},
            { "Area Mask", "Mask fitness over area. None - Don't apply image filter. Grey Scale - apply image filter using greys scale. R - Apply filter from red channel. G - Apply filter from green channel. B - Apply filter from blue channel. A - Apply filter from alpha channel. Terrain Texture Slot - apply mask from texture painted on terrain."},
            { "Image Mask", "The texure to use as the source of the area mask."},
            { "Smooth Mask", "Smooth the mask before applying it. This is a nice way to clean noise up in the mask, or to soften the edges of the mask."},
            { "Normalise Mask", "Normalise the mask before applying it. Ensures that the full dynamic range of the mask is used."},
            { "Invert Mask", "Invert the mask before applying it."},
            { "Flip Mask", "Flip the mask on its x and y axis mask before applying it. Useful sometimes to match the unity terrain as this is flipped internally."},
            { "Seed", "The unique seed for this spawner. If the environment, resources or rules dont change, then hitting Reset and respawning will always regenerate the same result." },

            { "Noise Mask", "Mask fitness with a noise value."},
            { "Noise Seed", "The seed value for the noise function - the same seed will always generate the same noise for a given set of parameters."},
            { "Octaves", "The amount of detail in the noise - more octaves mean more detail and longer calculation time."},
            { "Persistence", "The roughness of the noise. Controls how quickly amplitudes diminish for successive octaves. 0..1."},
            { "Frequency", "The frequency of the first octave."},
            { "Lacunarity", "The frequency multiplier between successive octaves. Experiment between 1.5 - 3.5."},
            { "Zoom", "The zoom level of the noise. Larger zooms display the noise over larger areas."},
            { "Invert", "Invert the noise."},


            { "Name", "Rule name - purely for convenience" },
            { "Resource Type", "The type of resource this rule will apply to." },
            { "Selected Resource", "The resource this rule applies to. To modify how the resource interprets terrain fitness change its spawn criteria." },
            { "Min Viable Fitness", "The minimum fitness needed to be considered viable to spawn." },
            { "Failure Rate", "The amount of the time that the rule will fail even if fit enough. 0 means never fail, and 1 means always fail. Use this to thin things out." },
            { "Max Instances", "The maximum number of resource instances this rule can spawn. Use this to stop over population." },
            { "Ignore Max Instances", "Ignores the max instances criteria. Useful for texturing very large terrains." },
            { "Active", "Whether this rule is active or not. Use this to disable the rule."},
            { "Curr Inst Count", "The number of instances of this rule that have been spawned."},
            { "Instances Spawned", "The number of times this resource has been spawned." },
            { "Inactive Inst Count", "The number of inactive instances that have been spawned, but are now inactive and in the pool for re-use. Only relevant when game objects have been spawned" },
         
            { "Active Rules", "The number of active rules being managed by the spawner."},
            { "Inactive Rules", "The number of inactive rules being managed by the spawner."},
            { "TOTAL Rules", "The total number of rules being managed by the spawner."},
            { "MAX INSTANCES", "The maximum number of instances that can be managed by the spawner."},
            { "Active Instances", "The number of active instances being managed by the spawner."},
            { "Inactive Instances", "The number inactive instances being managed by the spawner."},
            { "TOTAL Instances", "The total number of active and inactive instances being managed by the spawner."},

            { "Min Direction", "Minimum rotation in degrees for this game object spawner." },
            { "Max Direction", "Maximum rotation in degrees fpr this game object spawner." },

            { "Ground Level", "Ground level for this feature, used to make positioning easier." },
            { "Show Ground Level", "Show ground level." },
            { "Stick To Ground", "Stick to ground level." },
            { "Show Gizmos", "Show the spawners gizmos." },
            { "Show Rulers", "Show rulers." },
            { "Show Statistics", "Show spawner statistics." },
            { "Flatten", "Flatten the entire terrain - use with care!" },
            { "Smooth", "Smooth the entire terrain - removes jaggies and increases frame rate - run multiple times to increase effect - use with care!" },
            { "Clear Trees", "Clear trees from entire terrain - use with care!" },
            { "Clear Details", "Clear details / grass from entire terrain - use with care!" },
            { "Ground", "Position the spawner at ground level on the terrain." },
            { "Fit To Terrain", "Fits and aligns the spawner to the terrain." },
            { "Reset", "Resets the spawner, deletes any spawned game objects, and resets the random number generator." },
            { "Spawn", "Run a single spawn iteration. You can run as many spawn iterations as you like." },
        };
    }
}
