using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
//using PWCommon;

namespace Gaia
{
    [CustomEditor(typeof(Stamper))]
    public class StamperEditor : Editor
    {
        GUIStyle m_boxStyle;
        GUIStyle m_wrapStyle;
        Stamper m_stamper;
        float m_minX = -2000f;
        float m_maxX = 2000f;
        float m_minY = -2000f;
        float m_maxY = 2000f;
        float m_minZ = -2000f;
        float m_maxZ = 2000f;
        DateTime m_timeSinceLastUpdate = DateTime.Now;
        bool m_startedUpdates = false;
        private bool m_showTooltips = true;
        private EditorUtilsOLD m_editorUtils = new EditorUtilsOLD();

        private void OnDestroy()
        {
            //if (m_editorUtils != null)
            //{
            //    m_editorUtils.Dispose();
            //}
        }

        /// <summary>
        /// Called when object selected
        /// </summary>
        void OnEnable()
        {
            //if (m_editorUtils == null)
            //{
            //    m_editorUtils = new EditorUtils(this);
            //}
            //Get the settings and update tooltips
            GaiaSettings settings = Gaia.GaiaUtils.GetGaiaSettings();
            if (settings != null)
            {
                m_showTooltips = settings.m_showTooltips;
            }

            m_stamper = (Stamper)target;
            if (m_stamper != null)
            {
                m_stamper.ShowPreview();
            }

            if (Terrain.activeTerrain != false)
            {
                float height = Terrain.activeTerrain.terrainData.size.y;
                float maxWidth = Mathf.Max(Terrain.activeTerrain.terrainData.size.x, Terrain.activeTerrain.terrainData.size.z);
                m_minX = Terrain.activeTerrain.GetPosition().x - (0.5f * maxWidth);
                m_maxX = m_minX + (2f * maxWidth);
                m_minY = Terrain.activeTerrain.GetPosition().y - (height);
                m_maxY = m_minY + (2f * height);
                m_minZ = Terrain.activeTerrain.GetPosition().z - (0.5f * maxWidth);
                m_maxZ = m_minZ + (2f * maxWidth);
            }

            StartEditorUpdates();
        }

        /// <summary>
        /// Called when object deselected
        /// </summary>
        void OnDisable()
        {
            m_stamper = (Stamper)target;
            if (m_stamper != null)
            {
                if (!m_stamper.m_alwaysShow)
                {
                    m_stamper.HidePreview();
                }
            }
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
            if (m_stamper != null)
            {
                if (m_stamper.m_updateCoroutine != null)
                {
                    if ((DateTime.Now - m_timeSinceLastUpdate).TotalMilliseconds > 500)
                    {
                        m_timeSinceLastUpdate = DateTime.Now;
                        Repaint();
                    }
                }
                else
                {
                    if ((DateTime.Now - m_timeSinceLastUpdate).TotalSeconds > 5)
                    {
                        m_timeSinceLastUpdate = DateTime.Now;
                        Repaint();
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            //Get our stamper
            m_stamper = (Stamper)target;

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

            //Draw the intro
            m_editorUtils.DrawIntro("Stamper", "The stamper allows you to stamp features into your terrain. Click here to see a tutorial.", "http://www.procedural-worlds.com/gaia/tutorials/stamper-introduction/");

            //Disable if spawning
            if (m_stamper.m_stampComplete != true && !m_stamper.m_cancelStamp)
            {
                GUI.enabled = false;
            }

            EditorGUI.BeginChangeCheck();

            GUILayout.BeginVertical("Operations:", m_boxStyle);
            GUILayout.Space(20);
            Texture2D feature = (Texture2D)EditorGUILayout.ObjectField(GetLabel("Stamp Preview"), m_stamper.m_stampPreviewImage, typeof(Texture2D), false);
            GaiaResource resources = (GaiaResource)EditorGUILayout.ObjectField(GetLabel("Resources"), m_stamper.m_resources, typeof(GaiaResource), false);
            Gaia.GaiaConstants.FeatureOperation operation = (Gaia.GaiaConstants.FeatureOperation)EditorGUILayout.EnumPopup(GetLabel("Operation Type"), m_stamper.m_stampOperation);
            float stencilHeight = m_stamper.m_stencilHeight;
            if (operation == GaiaConstants.FeatureOperation.StencilHeight)
            {
                stencilHeight = EditorGUILayout.Slider(GetLabel("Stencil Height"), m_stamper.m_stencilHeight, -1000f, 1000f);
            }
            float blendStrength = m_stamper.m_blendStrength;
            if (operation == GaiaConstants.FeatureOperation.BlendHeight)
            {
                blendStrength = EditorGUILayout.Slider(GetLabel("Blend Strength"), m_stamper.m_blendStrength, 0f, 1f);
            }
            //GUILayout.Label(stamper.m_feature, GUILayout.Width(200f), GUILayout.Height(200f) );
            AnimationCurve heightModifier = EditorGUILayout.CurveField(GetLabel("Transform Height"), m_stamper.m_heightModifier);
            int smoothIterations = m_stamper.m_smoothIterations = EditorGUILayout.IntSlider(GetLabel("Smooth Stamp"), m_stamper.m_smoothIterations, 0, 10);
            bool normaliseStamp = EditorGUILayout.Toggle(GetLabel("Normalise Stamp"), m_stamper.m_normaliseStamp);
            bool invertStamp = EditorGUILayout.Toggle(GetLabel("Invert Stamp"), m_stamper.m_invertStamp);
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Position, Rotate and Scale:", m_boxStyle);
            GUILayout.Space(20);
            //GUILayout.Label("Operation:", EditorStyles.boldLabel);
            float x = EditorGUILayout.Slider(GetLabel("Position X"), m_stamper.m_x, m_minX, m_maxX);
            float y = m_stamper.m_y;
            if (!m_stamper.m_stickBaseToGround)
            {
                y = EditorGUILayout.Slider(GetLabel("Position Y"), m_stamper.m_y, m_minY, m_maxY);
            }
            float z = EditorGUILayout.Slider(GetLabel("Position Z"), m_stamper.m_z, m_minZ, m_maxZ);
            float rotation = EditorGUILayout.Slider(GetLabel("Rotation"), m_stamper.m_rotation, -180f, 180f);
            float width = EditorGUILayout.Slider(GetLabel("Width"), m_stamper.m_width, 0.1f, 200f);
            float height = EditorGUILayout.Slider(GetLabel("Height"), m_stamper.m_height, 0.1f, 100f);
            float baseLevel = EditorGUILayout.Slider(GetLabel("Base Level"), m_stamper.m_baseLevel, 0f, 1f);
            bool stickBaseToGround = EditorGUILayout.Toggle(GetLabel("Ground Base"), m_stamper.m_stickBaseToGround);
            bool stampBase = EditorGUILayout.Toggle(GetLabel("Stamp Base"), m_stamper.m_drawStampBase);
            bool showBase = EditorGUILayout.Toggle(GetLabel("Show Base"), m_stamper.m_showBase);
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Masks:", m_boxStyle);
            GUILayout.Space(20);
            //GUILayout.Label("Masks:", EditorStyles.boldLabel);
            AnimationCurve distanceMask = EditorGUILayout.CurveField(GetLabel("Distance Mask"), m_stamper.m_distanceMask);
            Gaia.GaiaConstants.ImageFitnessFilterMode areaMaskMode = (Gaia.GaiaConstants.ImageFitnessFilterMode)EditorGUILayout.EnumPopup(GetLabel("Area Mask"), m_stamper.m_areaMaskMode);
            Texture2D imageMask = m_stamper.m_imageMask;
            bool imageMaskInvert = m_stamper.m_imageMaskInvert;
            bool imageMaskNormalise = m_stamper.m_imageMaskNormalise;
            bool imageMaskFlip = m_stamper.m_imageMaskFlip;

            float noiseMaskSeed = m_stamper.m_noiseMaskSeed;
            int noiseMaskOctaves = m_stamper.m_noiseMaskOctaves;
            float noiseMaskPersistence = m_stamper.m_noiseMaskPersistence;
            float noiseMaskFrequency = m_stamper.m_noiseMaskFrequency;
            float noiseMaskLacunarity = m_stamper.m_noiseMaskLacunarity;
            float noiseZoom = m_stamper.m_noiseZoom;

            int imageMaskSmoothIterations = m_stamper.m_imageMaskSmoothIterations;
            if (areaMaskMode == GaiaConstants.ImageFitnessFilterMode.ImageAlphaChannel || 
                areaMaskMode == GaiaConstants.ImageFitnessFilterMode.ImageBlueChannel || 
                areaMaskMode == GaiaConstants.ImageFitnessFilterMode.ImageGreenChannel || 
                areaMaskMode == GaiaConstants.ImageFitnessFilterMode.ImageGreyScale || 
                areaMaskMode == GaiaConstants.ImageFitnessFilterMode.ImageRedChannel)
            {
                imageMask = (Texture2D)EditorGUILayout.ObjectField(GetLabel("Image Mask"), m_stamper.m_imageMask, typeof(Texture2D), false);
            }
            else if (areaMaskMode == GaiaConstants.ImageFitnessFilterMode.PerlinNoise || areaMaskMode == GaiaConstants.ImageFitnessFilterMode.RidgedNoise ||
                areaMaskMode == GaiaConstants.ImageFitnessFilterMode.BillowNoise)
            {
                noiseMaskSeed = EditorGUILayout.Slider(GetLabel("Noise Seed"), noiseMaskSeed, 0f, 65000f);
                noiseMaskOctaves = EditorGUILayout.IntSlider(GetLabel("Octaves"), noiseMaskOctaves, 1, 12);
                noiseMaskPersistence = EditorGUILayout.Slider(GetLabel("Persistence"), noiseMaskPersistence, 0f, 1f);
                noiseMaskFrequency = EditorGUILayout.Slider(GetLabel("Frequency"), noiseMaskFrequency, 0f, 1f);
                noiseMaskLacunarity = EditorGUILayout.Slider(GetLabel("Lacunarity"), noiseMaskLacunarity, 1.5f, 3.5f);
                noiseZoom = EditorGUILayout.Slider(GetLabel("Zoom"), noiseZoom, 1f, 1000f);
            }

            if (areaMaskMode != GaiaConstants.ImageFitnessFilterMode.None)
            {
                imageMaskSmoothIterations = EditorGUILayout.IntSlider(GetLabel("Smooth Mask"), m_stamper.m_imageMaskSmoothIterations, 0, 20);
                imageMaskNormalise = EditorGUILayout.Toggle(GetLabel("Normalise Mask"), m_stamper.m_imageMaskNormalise);
                imageMaskInvert = EditorGUILayout.Toggle(GetLabel("Invert Mask"), m_stamper.m_imageMaskInvert);
                imageMaskFlip = EditorGUILayout.Toggle(GetLabel("Flip Mask"), m_stamper.m_imageMaskFlip);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical("Helpers:", m_boxStyle);
            GUILayout.Space(20);
            //GUILayout.Label("Helpers:", EditorStyles.boldLabel);
            if (m_stamper.m_resources != null)
            {
                EditorGUILayout.LabelField("Sea Level", m_stamper.m_resources.m_seaLevel.ToString() + " m");
            }
            bool showSeaLevel = EditorGUILayout.Toggle(GetLabel("Show Sea Level"), m_stamper.m_showSeaLevel);
            //Color gizmoColour = EditorGUILayout.ColorField(GetLabel("Gizmo Colour"), m_stamper.m_gizmoColour);
            bool alwaysShow = EditorGUILayout.Toggle(GetLabel("Always Show Stamper"), m_stamper.m_alwaysShow);
            bool showRulers = m_stamper.m_showRulers = EditorGUILayout.Toggle(GetLabel("Show Rulers"), m_stamper.m_showRulers);
            bool showTerrainHelper = m_stamper.m_showTerrainHelper = EditorGUILayout.Toggle(GetLabel("Show Terrain Helper"), m_stamper.m_showTerrainHelper);
            GUILayout.EndVertical();

            //Check for changes, make undo record, make changes and let editor know we are dirty
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_stamper, "Made changes");

                //Check to see if we need to load a new stamp
                if (feature != null)
                {
                    if (m_stamper.m_stampPreviewImage == null)
                    {
                        m_stamper.m_stampPreviewImage = feature;
                        m_stamper.LoadStamp();
                        m_stamper.FitToTerrain();
                        baseLevel = m_stamper.m_baseLevel;
                        width = m_stamper.m_width;
                        height = m_stamper.m_height;
                        rotation = m_stamper.m_rotation;
                        x = m_stamper.m_x;
                        y = m_stamper.m_y;
                        z = m_stamper.m_z;
                    }
                    else if (m_stamper.m_stampPreviewImage.GetInstanceID() != feature.GetInstanceID())
                    {
                        m_stamper.m_stampPreviewImage = feature;
                        m_stamper.LoadStamp();
                        baseLevel = m_stamper.m_baseLevel;
                    }
                }

                //And invert it
                if (m_stamper.m_invertStamp != invertStamp)
                {
                    m_stamper.m_invertStamp = invertStamp;
                    m_stamper.InvertStamp();
                }

                //And normalise it
                if (m_stamper.m_normaliseStamp != normaliseStamp)
                {
                    m_stamper.m_normaliseStamp = normaliseStamp;
                    if (normaliseStamp)
                    {
                        m_stamper.NormaliseStamp();
                    }
                    else
                    {
                        m_stamper.LoadStamp();
                        baseLevel = m_stamper.m_baseLevel;
                    }
                }

                m_stamper.m_heightModifier = heightModifier;
                m_stamper.m_drawStampBase = stampBase;
                m_stamper.m_stampOperation = operation;
                m_stamper.m_blendStrength = blendStrength;
                m_stamper.m_stencilHeight = stencilHeight;
                m_stamper.m_distanceMask = distanceMask;
                m_stamper.m_smoothIterations = smoothIterations;
                m_stamper.m_resources = resources;
                m_stamper.m_x = x;
                m_stamper.m_y = y;
                m_stamper.m_z = z;
                m_stamper.m_width = width;
                m_stamper.m_height = height;
                m_stamper.m_rotation = rotation;
                m_stamper.m_stickBaseToGround = stickBaseToGround;
                m_stamper.m_alwaysShow = alwaysShow;
                m_stamper.m_showSeaLevel = showSeaLevel;
                m_stamper.m_baseLevel = baseLevel;
                m_stamper.m_showBase = showBase;
                m_stamper.m_showRulers = showRulers;
                m_stamper.m_showTerrainHelper = showTerrainHelper;
                m_stamper.m_areaMaskMode = areaMaskMode;
                m_stamper.m_imageMask = imageMask;
                m_stamper.m_imageMaskInvert = imageMaskInvert;
                m_stamper.m_imageMaskNormalise = imageMaskNormalise;
                m_stamper.m_imageMaskFlip = imageMaskFlip;
                m_stamper.m_imageMaskSmoothIterations = imageMaskSmoothIterations;

                m_stamper.m_noiseMaskSeed = noiseMaskSeed;
                m_stamper.m_noiseMaskOctaves = noiseMaskOctaves;
                m_stamper.m_noiseMaskPersistence = noiseMaskPersistence;
                m_stamper.m_noiseMaskFrequency = noiseMaskFrequency;
                m_stamper.m_noiseMaskLacunarity = noiseMaskLacunarity;
                m_stamper.m_noiseZoom = noiseZoom;

                //Update the stamp calcs
                m_stamper.UpdateStamp();

                EditorUtility.SetDirty(m_stamper);
            }

            //Terrain control
            if (showTerrainHelper)
            {
                GUILayout.BeginVertical("Terrain Helper", m_boxStyle);
                GUILayout.Space(20);

                if (GUILayout.Button(GetLabel("Show Terrain Utilities")))
                {
                    var export = EditorWindow.GetWindow<GaiaTerrainExplorerEditor>(false, "Terrain Utilities");
                    export.Show();
                }

                GUILayout.BeginHorizontal();
                if (GUILayout.Button(GetLabel("Flatten")))
                {
                    if (EditorUtility.DisplayDialog("Flatten Terrain tiles ?", "Are you sure you want to flatten all terrain tiles - this can not be undone ?", "Yes", "No"))
                    {
                        m_stamper.FlattenTerrain();
                    }
                }

                if (GUILayout.Button(GetLabel("Smooth")))
                {
                    if (EditorUtility.DisplayDialog("Smooth Terrain tiles ?", "Are you sure you want to smooth all terrain tiles - this can not be undone ?", "Yes", "No"))
                    {
                        m_stamper.SmoothTerrain();
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();

                if (GUILayout.Button(GetLabel("Clear Trees")))
                {
                    if (EditorUtility.DisplayDialog("Clear Terrain trees ?", "Are you sure you want to clear all terrain trees - this can not be undone ?", "Yes", "No"))
                    {
                        m_stamper.ClearTrees();
                    }
                }
                if (GUILayout.Button(GetLabel("Clear Details")))
                {
                    if (EditorUtility.DisplayDialog("Clear Terrain details ?", "Are you sure you want to clear all terrain details - this can not be undone ?", "Yes", "No"))
                    {
                        m_stamper.ClearDetails();
                    }
                }

                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                GUILayout.EndVertical();

            }


            //Regardless, re-enable the spawner controls
            GUI.enabled = true;

            //Display progress
            if (m_stamper.m_stampComplete != true && !m_stamper.m_cancelStamp)
            {
                GUILayout.BeginVertical("Stamp Controller", m_boxStyle);
                GUILayout.Space(20);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(GetLabel("Cancel")))
                {
                    m_stamper.CancelStamp();
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                GUILayout.EndVertical();
                ProgressBar(string.Format("Progress ({0:0.0}%)", m_stamper.m_stampProgress * 100f), m_stamper.m_stampProgress);
            }
            else
            {
                //Stamp control
                GUILayout.BeginVertical("Stamp Controller", m_boxStyle);
                GUILayout.Space(20);
                if (!m_stamper.CanPreview())
                {
                    GUI.enabled = false;
                }

                GUILayout.BeginHorizontal();
                if (GUILayout.Button(GetLabel("Ground")))
                {
                    m_stamper.AlignToGround();
                    m_stamper.UpdateStamp();
                }
                if (GUILayout.Button(GetLabel("Fit To Terrain")))
                {
                    m_stamper.FitToTerrain();
                    m_stamper.UpdateStamp();
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button(GetLabel("Preview")))
                {
                    m_stamper.TogglePreview();
                }
                GUI.enabled = true;
                if (m_stamper.m_stampPreviewImage == null)
                {
                    GUI.enabled = false;
                }
                if (GUILayout.Button(GetLabel("Stamp")))
                {
                    //Check that they have a single selected terrain
                    if (Gaia.TerrainHelper.GetActiveTerrainCount() != 1)
                    {
                        EditorUtility.DisplayDialog("OOPS!", "You must have only one active terrain in order to use a Spawner. Please either add a terrain, or deactivate all but one terrain.", "OK");
                    }
                    else
                    {
                        //Check that the centre of the terrain is at 0,0,0, and offer to move
                        bool isCentred = true;
                        Bounds b = new Bounds();
                        Terrain terrain = Gaia.TerrainHelper.GetActiveTerrain();
                        Gaia.TerrainHelper.GetTerrainBounds(terrain, ref b);
                        if ((b.center.x != 0f) || (b.min.y != 0f) || (b.center.z != 0f))
                        {
                            isCentred = false;
                            if (EditorUtility.DisplayDialog("OOPS!", "The terrain must be centered at 0,0,0 for stamping to work properly. Would you like GAIA to move it for you? You will need to reposition your stamp after this to adjust for the movement. You can move the terrain back to its original position after you have finished with the stamper.", "OK", "Cancel"))
                            {
                                terrain.transform.position = new Vector3(b.extents.x * -1f, 0f, b.extents.z * -1f);
                            }
                        }

                        if (isCentred)
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
                                if (!m_stamper.IsFitToTerrain())
                                {
                                    if (EditorUtility.DisplayDialog("WARNING!", "This feature requires your stamp to be Fit To Terrain in order to guarantee correct placement.", "Stamp Anyway", "Cancel"))
                                    {
                                        m_stamper.Stamp();
                                    }
                                }
                                else
                                {
                                    m_stamper.Stamp();
                                }
                            }
                            else
                            {
                                m_stamper.Stamp();
                            }
                        }
                    }
                }
                GUI.enabled = true;

                if (m_stamper.CanRedo())
                {
                    if (GUILayout.Button(GetLabel("Redo")))
                    {
                        m_stamper.Redo();
                    }
                }
                else
                {
                    if (!m_stamper.CanUndo())
                    {
                        GUI.enabled = false;
                    }

                    if (GUILayout.Button(GetLabel("Undo")))
                    {
                        m_stamper.Undo();
                    }
                }

                GUI.enabled = true;

                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                GUILayout.EndVertical();

                GUILayout.Space(5f);
            }
        }

        /// <summary>
        /// Display a progress bar
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
        static Dictionary<string, string> m_tooltips = new Dictionary<string,string>
        {
            { "Stamp Preview", "Preview texture for the feature being stamped. Drag and drop preview textures here." },
            { "Transform Height", "Pre-process and modify the stamp height. Can be used to further refine stamp shapes."},
            { "Invert Stamp", "Invert the stamp. Good for carving lakes and valleys."},
            { "Base Level", "Base level of the stamp."},
            { "Ground Base", "Sticks the stamp base level to the terrain base."},
            { "Stamp Base", "Applies stamp below base level, or not. Good for excluding low lying areas from the stamp."},
            { "Show Base", "Shows the base level as a yellow plane."},
            { "Normalise Stamp", "Modify stamp heights to use full height range. Essential for correct height settings when using Stencil Heights operation."},
            { "Operation Type", "The way this stamp will be applied to the terrain.\nRaise - Adds stamp to terrain if stamp height greater than terrain height.\nLower - Cuts stamp from terrain if stamp height lower than terrain height. \nBlend - Blend between terrain and stamp.\nDifference - Calculate height difference.\nStencil - Adjust by stencil height - normalise first."},
            { "Blend Strength", "Blend between terrain and stamp. 0 - all terrain - 1 - all stamp."},
            { "Stencil Height", "Adjusted height in meters that a normalised stamp will be applied to the terrain."},
            { "Distance Mask", "Masks the effect of the stamp over distance from center. Left hand side of curve is centre of stamp, right hand side of curve is outer edge of stamp. Set right hand side to zero to blend edges of stamp into existing terrain."},
            { "Area Mask", "Masks the effect of the stamp using the strength of the texture or noise function provided. A value of 1 means apply full effect, a value of 0 means apply no effect. Visually this is much the same ways as a greyscale image mask. If using a terrain texture, then paint on the terrain with the selected texture, and the painted area will be used as the mask."},
            
            { "Noise Seed", "The seed value for the noise function - the same seed will always generate the same noise for a given set of parameters."},
            { "Octaves", "The amount of detail in the noise - more octaves mean more detail and longer calculation time."},
            { "Persistence", "The roughness of the noise. Controls how quickly amplitudes diminish for successive octaves. 0..1."},
            { "Frequency", "The frequency of the first octave."},
            { "Lacunarity", "The frequency multiplier between successive octaves. Experiment between 1.5 - 3.5."},
            { "Zoom", "The zoom level of the noise. Larger zooms display the noise over larger areas."},

            { "Image Mask", "The image to use as the area mask."},
            { "Invert Mask", "Invert the image used as the area mask before using it."},
            { "Smooth Mask", "Smooth the mask before applying it. This is a nice way to clean noise up in the mask, or to soften the edges of the mask."},
            { "Normalise Mask", "Normalise the mask before applying it. Ensures that the full dynamic range of the mask is used."},
            { "Flip Mask", "Flip the mask on its x and y axis mask before applying it. Useful sometimes to match the unity terrain as this is flipped internally."},
            { "Seed", "The unique seed for this spawner. This will cause all subseqent spawns to exactly match this spawn" },
            { "Smooth Stamp", "Smooth the stamp before applying it to the terrain. Good for cleaning up noisy stamps."},
            { "Preview Material", "The material used to display the Preview mesh. Has no effect other than to make the preview viewable."},
            { "Resources", "The terrains rsources file. Changing sea level will update that resource files sea level, and this will impact where the spawners spawn."},
            { "Texture Spawner", "The texure spawner. An optional feature that enables the spawn button. Enables you to do a texure spawn and saves you from having to select the texture spawner manually."},
            { "Position X", "X location of stamp centre." },
            { "Position Y", "Y location of stamp centre." },
            { "Position Z", "Z location of stamp centre." },
            { "Width", "Modify the width of the stamp." },
            { "Height", "Modify the height of this stamp." },
            { "Rotation", "Modify the rotation of this stamp." },
            { "Stick To Groud", "Stick the stamp to the base of the terrain." },
            { "Always Show Stamper", "Always show the stamper, even when something else is selected, otherwise hide it when something else is selected." },
            { "Gizmo Colour", "The colour of the gizmo that is drawn to show the size of the stamp, used to make positioning easier." },
            { "Sea Level", "The sea level in meters. Changes to this are applied back to the resources file, and then impact the spawners, so treat with care, and only change before spawning." },
            { "Show Sea Level", "Show sea level." },
            { "Show Rulers", "Show rulers." },
            { "Show Terrain Helper", "Show the terrain helper buttons - treat these with care!" },
            { "Flatten", "Flatten all terrains." },
            { "Smooth", "Smooth all terrains." },
            { "Clear Trees", "Clear trees on all terrains." },
            { "Clear Details", "Clear details on all terrains." },
            { "Ground", "Align the stamp to the base of the terrain." },
            { "Fit To Terrain", "Fit the stamp to the terrain." },
            { "Stamp", "Apply this stamp to the terrain." },
            { "Preview", "Show or hide the stamp preview mesh." },
            { "Undo", "Undo the last stamp." },
            { "Redo", "Redo the last stamp." },
        };
    }
}