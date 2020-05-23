using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Gaia
{
    /// <summary>
    /// Editor for reource manager
    /// </summary>
    [CustomEditor(typeof(GaiaSessionManager))]
    public class GaiaSessionManagerEditor : Editor
    {
        GUIStyle m_boxStyle;
        GUIStyle m_wrapStyle;
        GUIStyle m_descWrapStyle;
        private Vector2 m_scrollPosition = Vector2.zero;
        GaiaSessionManager m_manager;
        private int m_lastSessionID = -1;
        private string m_lastPreviewImgName = "";
        private bool m_showTooltips = true;

        void OnEnable()
        {
            //Get the settings and update tooltips
            GaiaSettings settings = Gaia.GaiaUtils.GetGaiaSettings();
            if (settings != null)
            {
                m_showTooltips = settings.m_showTooltips;
            }

            //Get our resource
            m_manager = (GaiaSessionManager)target;
        }

        public override void OnInspectorGUI()
        {
            //Get our resource
            m_manager = (GaiaSessionManager)target;

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

            //Set up the description wrap style
            if (m_descWrapStyle == null)
            {
                m_descWrapStyle = new GUIStyle(GUI.skin.textArea);
                m_descWrapStyle.wordWrap = true;
            }

            //Scroll
            m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, false, false);

            //Create a nice text intro
            GUILayout.BeginVertical("Gaia Session Manager", m_boxStyle);
            GUILayout.Space(20);
            EditorGUILayout.LabelField("Track and control session creation and playback.", m_wrapStyle);
            GUILayout.Space(4);
            EditorGUILayout.BeginHorizontal();
            m_manager.m_session = (GaiaSession)EditorGUILayout.ObjectField(GetLabel("Session"), m_manager.m_session, typeof(GaiaSession), false);

            if (GUILayout.Button(GetLabel("New"), GUILayout.Width(45)))
            {
                m_manager.CreateSession();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (m_manager.m_session == null)
            {
                GUILayout.EndScrollView();
                return;
            }

            //Track changes
            EditorGUI.BeginChangeCheck();

            //Make some space
            GUILayout.Space(4);

            //Wrap it up in a box
            GUILayout.BeginVertical(m_boxStyle);
            GUILayout.BeginVertical("Summary:", m_boxStyle);
            GUILayout.Space(20);


            //Display the basic details
            EditorGUILayout.LabelField("Name");
            if (m_manager.IsLocked())
            {
                GUI.enabled = false;
            }
            string name = EditorGUILayout.TextArea(m_manager.m_session.m_name, m_descWrapStyle, GUILayout.MinHeight(15));
            GUI.enabled = true;

            EditorGUILayout.LabelField("Description");
            if (m_manager.IsLocked())
            {
                GUI.enabled = false;
            }
            string description = EditorGUILayout.TextArea(m_manager.m_session.m_description, m_descWrapStyle, GUILayout.MinHeight(45));

            Texture2D previewImage = m_manager.GetPreviewImage();
            if (!m_manager.IsLocked())
            {
                previewImage = (Texture2D)EditorGUILayout.ObjectField(GetLabel("Preview Image"), m_manager.m_session.m_previewImage, typeof(Texture2D), false, GUILayout.MaxHeight(15f));
            }

            //Detect change in session and handle changes to preview image
            float width, height;
            if (m_manager.m_session.GetInstanceID() != m_lastSessionID)
            {
                m_lastPreviewImgName = "";
                m_lastSessionID = m_manager.m_session.GetInstanceID();
                if (m_manager.HasPreviewImage())
                {
                    previewImage = m_manager.GetPreviewImage();
                    m_lastPreviewImgName = previewImage.name;
                }
            }
            else //Process changes to preview image
            {
                if (previewImage == null)
                {
                    if (m_manager.IsLocked()) //Undo change if locked
                    {
                        if (m_manager.HasPreviewImage())
                        {
                            previewImage = m_manager.GetPreviewImage();
                            m_lastPreviewImgName = previewImage.name;
                            Debug.LogWarning("You can not change the image on a locked session");
                        }
                    }
                    else
                    {
                        if (m_manager.HasPreviewImage())
                        {
                            m_manager.RemovePreviewImage();
                            m_lastPreviewImgName = "";
                        }
                    }
                }
                else
                {
                    //Handle changes to preview image
                    if (previewImage.name != m_lastPreviewImgName) 
                    {
                        if (m_manager.IsLocked()) //Revert
                        {
                            if (m_manager.HasPreviewImage())
                            {
                                previewImage = m_manager.GetPreviewImage();
                                m_lastPreviewImgName = previewImage.name;
                                Debug.LogWarning("You can not change the image on a locked session");
                            }
                            else
                            {
                                previewImage = null;
                                m_lastPreviewImgName = "";
                            }
                        }
                        else
                        {
                            //Make it readable
                            Gaia.GaiaUtils.MakeTextureReadable(previewImage);

                            //Make a new texture from it
                            Texture2D newTexture = new Texture2D(previewImage.width, previewImage.height, TextureFormat.ARGB32, false);
                            newTexture.name = m_manager.m_session.m_name;
                            newTexture.SetPixels(previewImage.GetPixels(0));
                            newTexture.Apply();

                            //Resize and scale it
                            width = 320;
                            height = previewImage.height * (width / previewImage.width);
                            Gaia.ScaleTexture.Bilinear(newTexture, (int)width, (int)height);

                            //Compress it
                            //newTexture.Compress(true);

                            //And store its content
                            m_manager.AddPreviewImage(newTexture);

                            //Assign back to the texture for the scene
                            previewImage = newTexture;
                            m_lastPreviewImgName = previewImage.name;
                        }
                    }
                }
            }

            GUI.enabled = true; //In response to locked above

            if (previewImage != null)
            {
                //Get aspect ratio and available space and display the image
                width = Screen.width - 43f;
                height = previewImage.height * (width / previewImage.width);
                GUILayout.Label(previewImage, GUILayout.MaxWidth(width), GUILayout.MaxHeight(height));
            }

            EditorGUILayout.LabelField("Created", m_manager.m_session.m_dateCreated);
            EditorGUILayout.LabelField("Dimensions", string.Format("w{0} d{1} h{2}", m_manager.m_session.m_terrainWidth, m_manager.m_session.m_terrainDepth, m_manager.m_session.m_terrainHeight));

            if (m_manager.IsLocked())
            {
                GUI.enabled = false;
            }
            m_manager.m_session.m_seaLevel = EditorGUILayout.Slider(GetLabel("Sea Level"), m_manager.m_session.m_seaLevel, 0f, m_manager.m_session.m_terrainDepth);
            GUI.enabled = true; //In response to locked above

            bool locked = EditorGUILayout.Toggle(GetLabel("Locked"), m_manager.m_session.m_isLocked);
            GUILayout.EndVertical();

            //Iterate through the operations
            GUILayout.BeginVertical("Operations:", m_boxStyle);
            GUILayout.Space(20);

            if (m_manager.m_session.m_operations.Count == 0)
            {
                GUILayout.Space(5);
                GUILayout.Label("No operations yet...");
                GUILayout.Space(5);
            }
            else
            {
                GaiaOperation op;
                EditorGUI.indentLevel++;
                for (int opIdx = 0; opIdx < m_manager.m_session.m_operations.Count; opIdx++)
                {
                    op = m_manager.m_session.m_operations[opIdx];

                    if (op.m_isActive)
                    {
                        op.m_isFoldedOut = EditorGUILayout.Foldout(op.m_isFoldedOut, op.m_description, true);
                    }
                    else
                    {
                        op.m_isFoldedOut = EditorGUILayout.Foldout(op.m_isFoldedOut, op.m_description + " [inactive]", true);
                    }

                    if (op.m_isFoldedOut)
                    {
                        EditorGUI.indentLevel++;

                        EditorGUILayout.LabelField("Description", op.m_description, m_wrapStyle);
                        EditorGUILayout.LabelField("Created", op.m_operationDateTime);
                        if (m_manager.m_session.m_isLocked)
                        {
                            GUI.enabled = false;
                        }
                        op.m_isActive = EditorGUILayout.Toggle(GetLabel("Active"), op.m_isActive);
                        GUI.enabled = true;

                        int dataLength = 0;
                        for (int idx = 0; idx < op.m_operationDataJson.GetLength(0); idx++)
                        {
                            dataLength += op.m_operationDataJson[idx].Length;
                        }
                        EditorGUILayout.LabelField("Data", dataLength.ToString() + " bytes");

                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (m_manager.m_session.m_isLocked)
                        {
                            GUI.enabled = false;
                        }
                        if (GUILayout.Button(GetLabel("Delete")))
                        {
                            if (EditorUtility.DisplayDialog("Delete Operation ?", "Are you sure you want to delete this operation ?", "Yes", "No"))
                            {
                                m_manager.RemoveOperation(opIdx);
                            }
                        }
                        GUI.enabled = true;
                        if (GUILayout.Button(GetLabel("Apply")))
                        {
                            m_manager.Apply(opIdx);
                        }
                        if (GUILayout.Button(GetLabel("Play")))
                        {
                            m_manager.PlayOperation(opIdx);
                        }
                        GUILayout.EndHorizontal();

                        EditorGUI.indentLevel--;
                    }
                    //EditorGUILayout.Space();
                }
                EditorGUI.indentLevel--;
            }
            GUILayout.EndVertical();

            //Create a nice text intro
            if (!m_manager.m_session.m_isLocked)
            {
                GUILayout.BeginVertical(m_boxStyle);
                m_manager.m_genShowRandomGenerator = EditorGUILayout.BeginToggleGroup(GetLabel(" Random Terrain Generator"), m_manager.m_genShowRandomGenerator);
                if (m_manager.m_genShowRandomGenerator)
                {
                    m_manager.m_genGridSize = EditorGUILayout.IntSlider(GetLabel("Stamp Grid"), m_manager.m_genGridSize, 1, 5);
                    if (m_manager.m_genGridSize == 1)
                    {
                        m_manager.m_genNumStampsToGenerate = 1;
                    }
                    else
                    {
                        m_manager.m_genNumStampsToGenerate = (m_manager.m_genGridSize * m_manager.m_genGridSize) + 1;
                    }
                    EditorGUILayout.LabelField(new GUIContent("Stamps Generated"), new GUIContent(m_manager.m_genNumStampsToGenerate.ToString()));
                    //m_manager.m_genNumStampsToGenerate = EditorGUILayout.IntSlider(GetLabel("Stamps"), m_manager.m_genNumStampsToGenerate, 1, 26);
                    m_manager.m_genScaleWidth = EditorGUILayout.Slider(GetLabel("Width Scale"), m_manager.m_genScaleWidth, 0.5f, 100f);
                    m_manager.m_genScaleHeight = EditorGUILayout.Slider(GetLabel("Height Scale"), m_manager.m_genScaleHeight, 0.5f, 100f);
                    m_manager.m_genBorderStyle = (Gaia.GaiaConstants.GeneratorBorderStyle)EditorGUILayout.EnumPopup(GetLabel("Border Style"), m_manager.m_genBorderStyle);
                    m_manager.m_genChanceOfHills = EditorGUILayout.Slider(GetLabel("Hill Chance"), m_manager.m_genChanceOfHills, 0f, 1f);
                    m_manager.m_genChanceOfIslands = EditorGUILayout.Slider(GetLabel("Island Chance"), m_manager.m_genChanceOfIslands, 0f, 1f);
                    m_manager.m_genChanceOfLakes = EditorGUILayout.Slider(GetLabel("Lake Chance"), m_manager.m_genChanceOfLakes, 0f, 1f);
                    m_manager.m_genChanceOfMesas = EditorGUILayout.Slider(GetLabel("Mesa Chance"), m_manager.m_genChanceOfMesas, 0f, 1f);
                    m_manager.m_genChanceOfMountains = EditorGUILayout.Slider(GetLabel("Mountain Chance"), m_manager.m_genChanceOfMountains, 0f, 1f);
                    m_manager.m_genChanceOfPlains = EditorGUILayout.Slider(GetLabel("Plains Chance"), m_manager.m_genChanceOfPlains, 0f, 1f);
                    m_manager.m_genChanceOfRivers = EditorGUILayout.Slider(GetLabel("River Chance"), m_manager.m_genChanceOfRivers, 0f, 1f);
                    m_manager.m_genChanceOfValleys = EditorGUILayout.Slider(GetLabel("Valley Chance"), m_manager.m_genChanceOfValleys, 0f, 1f);
                    m_manager.m_genChanceOfVillages = EditorGUILayout.Slider(GetLabel("Village Chance"), m_manager.m_genChanceOfVillages, 0f, 1f);
                    m_manager.m_genChanceOfWaterfalls = EditorGUILayout.Slider(GetLabel("Waterfall Chance"), m_manager.m_genChanceOfWaterfalls, 0f, 1f);

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(GetLabel("Reset Session")))
                    {
                        if (EditorUtility.DisplayDialog("Reset Session ?", "Are you sure you want to reset your session - this can not be undone ?", "Yes", "No"))
                        {
                            m_manager.ResetSession();
                        }
                    }
                    if (GUILayout.Button(GetLabel("Add Stamps")))
                    {
                        m_manager.RandomiseStamps();
                    }
                    GUILayout.EndHorizontal();
                }
                EditorGUILayout.EndToggleGroup();
                GUILayout.EndVertical();
            }

            //Create a nice text intro
            GUILayout.BeginVertical(m_boxStyle);
            m_manager.m_genShowTerrainHelper = EditorGUILayout.BeginToggleGroup(GetLabel(" Terrain Helper"), m_manager.m_genShowTerrainHelper);
            if (m_manager.m_genShowTerrainHelper)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(GetLabel("Flatten Terrain")))
                {
                    if (EditorUtility.DisplayDialog("Flatten Terrain?", "Are you sure you want to flatten your terrain - this can not be undone ?", "Yes", "No"))
                    {
                        GaiaWorldManager wm = new GaiaWorldManager(Terrain.activeTerrains);
                        wm.FlattenWorld();
                    }
                }
                if (GUILayout.Button(GetLabel("Smooth Terrain")))
                {
                    if (EditorUtility.DisplayDialog("Smooth Terrain?", "Are you sure you want to smooth your terrain - this can not be undone ?", "Yes", "No"))
                    {
                        GaiaWorldManager wm = new GaiaWorldManager(Terrain.activeTerrains);
                        wm.SmoothWorld();
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button(GetLabel("Clear Trees")))
                {
                    if (EditorUtility.DisplayDialog("Clear Trees?", "Are you sure you want to clear your trees - this can not be undone ?", "Yes", "No"))
                    {
                        TerrainHelper.ClearTrees();
                    }
                }
                if (GUILayout.Button(GetLabel("Clear Details")))
                {
                    if (EditorUtility.DisplayDialog("Clear Details?", "Are you sure you want to clear your details / grass - this can not be undone ?", "Yes", "No"))
                    {
                        TerrainHelper.ClearDetails();
                    }
                }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndToggleGroup();
            GUILayout.EndVertical();


          

            GUILayout.BeginVertical("Session Controller", m_boxStyle);
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            bool focusSceneView = EditorGUILayout.Toggle(GetLabel("Focus Scene View"), m_manager.m_focusSceneView);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            if (m_manager.m_updateSessionCoroutine == null && m_manager.m_updateOperationCoroutine == null)
            {
               


                if (GUILayout.Button(GetLabel("Play Session")))
                {
                    if (EditorUtility.DisplayDialog("Playback Session ?", "Are you sure you want to playback your session - this can not be undone ?", "Yes", "No"))
                    {
                        m_manager.PlaySession();
                    }
                }
                if (GUILayout.Button(GetLabel("Export Resources")))
                {
                    m_manager.ExportSessionResources();
                }
            }
            else
            {
                if (GUILayout.Button(GetLabel("Cancel")))
                {
                    m_manager.CancelPlayback();
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(5);


            //Check for changes, make undo record, make changes and let editor know we are dirty
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(m_manager, "Made changes");
                m_manager.m_session.m_name = name;
                m_manager.m_session.m_description = description;
                m_manager.m_session.m_isLocked = locked;
                m_manager.m_session.m_previewImage = previewImage;
                m_manager.m_focusSceneView = focusSceneView;
                EditorUtility.SetDirty(m_manager.m_session);
                EditorUtility.SetDirty(m_manager);
            }

            //Debug.Log(m_manager.m_lastUpdateDateTime);

            //Draw the various spawner progress bars
            if (m_manager.m_currentStamper != null)
            {
                if (m_manager.m_currentStamper.IsStamping())
                {
                    ProgressBar(string.Format("{0}:{1} ({2:0.0}%)", m_manager.m_currentStamper.gameObject.name, m_manager.m_currentStamper.m_stampPreviewImage.name, m_manager.m_currentStamper.m_stampProgress * 100f), m_manager.m_currentStamper.m_stampProgress);
                }
            }
            if (m_manager.m_currentSpawner != null)
            {
                if (m_manager.m_currentSpawner.IsSpawning())
                {
                    ProgressBar(string.Format("{0} ({1:0.0}%)", m_manager.m_currentSpawner.gameObject.name, m_manager.m_currentSpawner.m_spawnProgress * 100f), m_manager.m_currentSpawner.m_spawnProgress);
                }
            }

            GUILayout.EndVertical();


            GUILayout.EndVertical();

            //End scroll
            GUILayout.EndScrollView();
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
            { "Sea Level", "The sea level the session will be rendered at. Changing this will also change the resource files when it is played." },
            { "Locked", "When activated then this stamp is locked and no further changes can be made." },
            { "Delete", "Delete the step." },
            { "Apply", "Apply the step to the relevant object, but don't execute it. Great for seeing how something was configured." },
            { "Play", "Apply the step and play it in the scene." },
        
            { "Flatten Terrain", "Flatten all terrains." },
            { "Smooth Terrain", "Smooth all terrains." },
            { "Clear Trees", "Clear trees on all terrains and reset all tree spawners." },
            { "Clear Details", "Clear details on all terrains." },

            { "Terrain Helper", "Show the terrain helper controls." },
            { "Focus Scene View", "Focus the scene view on the terrain during session Playback." },
            { "Play Session", "Play the session from end to end." },
            { "Export Resources", "Export the embedded session resources to the Assest\\Gaia Sessions\\SessionName directory." },
            { "Session", "The way this spawner runs. Design time : At design time only. Runtime Interval : At run time on a timed interval. Runtime Triggered Interval : At run time on a timed interval, and only when the tagged game object is closer than the trigger range from the center of the spawner." },
        };

    }
}