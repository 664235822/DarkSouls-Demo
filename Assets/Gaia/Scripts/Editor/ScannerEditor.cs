using UnityEngine;
using UnityEditor;
using System.IO;

namespace Gaia
{
    [CustomEditor(typeof(Scanner))]
    public class ScannerEditor : Editor
    {
		[SerializeField] Gaia.GaiaConstants.RawByteOrder m_rawByteOrder;
		[SerializeField] Gaia.GaiaConstants.RawBitDepth m_rawBitDepth = GaiaConstants.RawBitDepth.Sixteen;
		private Scanner m_scanner;
        private GUIStyle m_boxStyle;
        private GUIStyle m_wrapStyle;
		private string m_rawFilePath;
		private int m_assumedRawRes = 0;
		
		// The below are used so GUI update doesn't happen at wrong times causing layout mismatches
		private bool m_showRawInfo = false;
		private bool m_showBitDepthWarning = false;

		private readonly GUIContent BYTE_ORDER_LABEL = new GUIContent("Raw File Byte Order", "The byte order used when creating from RAW files. Try changing this if your stamp comes out flat.");
		private readonly GUIContent BIT_DEPTH_LABEL = new GUIContent("Raw File Bit Depth", "The bit depth used when creating from RAW files. Try changing this if the processing resolution is double, or half of what you expected.\n\n" +
			"NOTE: 8-bit RAW files have very poor precision and result in terraced stamps.");
		private readonly GUIContent[] BIT_DEPTHS_LABELS = new GUIContent[] {
			new GUIContent("16-bit (Recommended)", "16-bit RAW files can contain higher precision data."),
			new GUIContent("8-bit", "8-bit RAW files have low precision and result in terraced stamps/terrains."),
		};


		void OnEnable()
        {
            m_scanner = (Scanner)target;
            Gaia.GaiaUtils.CreateGaiaAssetDirectories();
        }

        void OnDisable()
        {
        }

        public override void OnInspectorGUI()
        {
            m_scanner = (Scanner)target;

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
                m_wrapStyle.fontStyle = FontStyle.Normal;
                m_wrapStyle.wordWrap = true;
            }

            //Text intro
            GUILayout.BeginVertical("Gaia Scanner", m_boxStyle);
            GUILayout.Space(20);
            EditorGUILayout.LabelField("The Gaia Scanner allows you to create new stamps from Windows R16, Windows 16 bit RAW, Mac 16 bit RAW, Terrains, Textures or Meshes. Just drag and drop the file or object onto the area below to scan it.", m_wrapStyle);
            GUILayout.EndVertical();

            DropAreaGUI();

			//Drop Options section
            GUILayout.BeginHorizontal();
			{
				EditorGUILayout.PrefixLabel(BYTE_ORDER_LABEL);
				EditorGUI.BeginChangeCheck();
				{
					m_rawByteOrder = (Gaia.GaiaConstants.RawByteOrder)GUILayout.Toolbar((int)m_rawByteOrder, new string[] { "IBM PC", "Macintosh" });
				}
				if (EditorGUI.EndChangeCheck())
				{
					ReloadRawFile();
				}
			}
			GUILayout.EndHorizontal();
			EditorGUI.BeginChangeCheck();
			{
				m_rawBitDepth = (GaiaConstants.RawBitDepth)EditorGUILayout.Popup(BIT_DEPTH_LABEL, (int)m_rawBitDepth, BIT_DEPTHS_LABELS);
			}
			if (EditorGUI.EndChangeCheck())
			{
				ReloadRawFile();
			}
            GUILayout.BeginVertical();
			if (m_showRawInfo)
			{
				EditorGUILayout.HelpBox("Assumed " + (m_rawBitDepth == GaiaConstants.RawBitDepth.Sixteen ? "16-bit" : "8-bit") + " RAW " + m_assumedRawRes + " x " + m_assumedRawRes, MessageType.Info);
			}
			if (m_showBitDepthWarning)
			{
				EditorGUILayout.HelpBox("WARNING: 8-bit RAW files have very poor precision and result in terraced stamps.", MessageType.Warning);
			}
			GUILayout.EndVertical();

			DrawDefaultInspector();

            //Terraform section
            GUILayout.BeginVertical("Scanner Controller", m_boxStyle);
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Save Scan"))
            {
                m_scanner.SaveScan();
                AssetDatabase.Refresh();
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.EndVertical();
            GUILayout.Space(5f);

			m_showRawInfo = m_assumedRawRes > 0;
			m_showBitDepthWarning = m_rawBitDepth == GaiaConstants.RawBitDepth.Eight;
	}

        public void DropAreaGUI()
        {
            //Ok - set up for drag and drop
            Event evt = Event.current;
            Rect drop_area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            GUI.Box(drop_area, "Drop Here To Scan", m_boxStyle);

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    {
                        if (!drop_area.Contains(evt.mousePosition))
                        {
                            break;
                        }

                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                        if (evt.type == EventType.DragPerform)
                        {
                            DragAndDrop.AcceptDrag();
							m_rawFilePath = null;
							m_assumedRawRes = 0;

							//First lets determine whether we got something we are interested in

							//Is it a saved file - only raw files are processed this way
							if (DragAndDrop.paths.Length > 0)
                            {
                                string filePath = DragAndDrop.paths[0];

                                //Update in case unity has messed with it 
                                if (filePath.StartsWith("Assets"))
                                {
                                    filePath = Path.Combine(Application.dataPath, filePath.Substring(7)).Replace('\\', '/');
                                }

                                //Check file type and process as we can
                                string fileType = Path.GetExtension(filePath).ToLower();

                                //Handle raw files
                                if (fileType == ".r16" || fileType == ".raw")
                                {
									m_rawFilePath = filePath;
									m_scanner.LoadRawFile(filePath, m_rawByteOrder, ref m_rawBitDepth, ref m_assumedRawRes);
                                    return;
                                }
                            }

                            //Is it something that unity knows about - may or may not have been saved
                            if (DragAndDrop.objectReferences.Length > 0)
                            {

                                //Debug.Log("Name is " + DragAndDrop.objectReferences[0].name);
                                //Debug.Log("Type is " + DragAndDrop.objectReferences[0].GetType());

                                //Check for textures
                                if (DragAndDrop.objectReferences[0].GetType() == typeof(UnityEngine.Texture2D))
                                {
                                    GaiaUtils.MakeTextureReadable(DragAndDrop.objectReferences[0] as Texture2D);
                                    GaiaUtils.MakeTextureUncompressed(DragAndDrop.objectReferences[0] as Texture2D);
                                    m_scanner.LoadTextureFile(DragAndDrop.objectReferences[0] as Texture2D);
                                    return;
                                }

                                //Check for terrains
                                if (DragAndDrop.objectReferences[0].GetType() == typeof(UnityEngine.GameObject))
                                {
                                    GameObject go = DragAndDrop.objectReferences[0] as GameObject;
                                    Terrain t = go.GetComponentInChildren<Terrain>();

                                    //Handle a terrain
                                    if (t != null)
                                    {
                                        m_scanner.LoadTerain(t);
                                        return;
                                    }
                                }

                                //Check for something with a mesh
                                if (DragAndDrop.objectReferences[0].GetType() == typeof(UnityEngine.GameObject))
                                {
                                    GameObject go = DragAndDrop.objectReferences[0] as GameObject;

                                    //Check for a mesh - this means we can scan it
                                    MeshFilter[] filters = go.GetComponentsInChildren<MeshFilter>();
                                    for (int idx = 0; idx < filters.Length; idx++)
                                    {
                                        if (filters[idx].mesh != null)
                                        {
                                            m_scanner.LoadGameObject(go);
                                            return;
                                        }
                                    }
                                }
                            }

                            //If we got to here then we couldnt process it
                            Debug.LogWarning("Object type not supported by scanner. Ignored");
                        }

                        break;
                    }
            }
        }

		void ReloadRawFile() {
			if (string.IsNullOrEmpty(m_rawFilePath)) {
				return;
			}
			m_assumedRawRes = 0;
			m_scanner.LoadRawFile(m_rawFilePath, m_rawByteOrder, ref m_rawBitDepth, ref m_assumedRawRes);
		}
    }
}
