using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Gaia
{
    /// <summary>
    /// Gaia extension exporter
    /// </summary>
    public class GaiaExtensionExporterEditor : EditorWindow
    {
        private GUIStyle m_boxStyle;
        private GUIStyle m_wrapStyle;
        private GUIStyle m_descWrapStyle;
        private Vector2 m_scrollPosition = Vector2.zero;
        private string m_publisherName = "Exported Extensions";
        private string m_extensionName = string.Format("Extension_{0:yyyyMMdd_HHmmss}", DateTime.Now);
        private string m_aboutText = "To use first create the resources file in the resources section and then create and use the spawners that match your requirements in the spawners sections.";
        private GaiaSettings m_settings;
        private List<GaiaResource> m_resourceList = new List<GaiaResource>();
        private List<Spawner> m_spawnerList = new List<Spawner>();

        #region Unity methods - Gui etc

        void OnEnable()
        {
            //Get or create existing settings object
            if (m_settings == null)
            {
                m_settings = (GaiaSettings)GaiaCommon1.AssetUtils.GetAssetScriptableObject("GaiaSettings");
            }

            //Grab the publisher name
            if (m_settings != null && !string.IsNullOrEmpty(m_settings.m_publisherName))
            {
                m_publisherName = m_settings.m_publisherName;
            }
        }

        void OnDisable()
        {
        }

        void OnGUI()
        {
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

            //Set up the description wrap style
            if (m_descWrapStyle == null)
            {
                m_descWrapStyle = new GUIStyle(GUI.skin.textArea);
                m_descWrapStyle.wordWrap = true;
            }

            //Text intro
            GUILayout.BeginVertical("Gaia Extension (GX) Exporter", m_boxStyle);
            GUILayout.Space(20);
            EditorGUILayout.LabelField("The Gaia Extensions exporter converts your spawners and the resources they refer to into an extension that can then be shared or used as a backup.", m_wrapStyle);
            GUILayout.EndVertical();

            //Scroll
            m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, false, false);

            m_publisherName = EditorGUILayout.TextField(GetLabel("Publisher Name"), m_publisherName);
            m_extensionName = EditorGUILayout.TextField(GetLabel("Extension Name"), m_extensionName);

            EditorGUILayout.LabelField("About Box Content");
            m_aboutText = EditorGUILayout.TextArea(m_aboutText, m_descWrapStyle, GUILayout.MinHeight(65));

            //Dump the resources list
            if (m_resourceList.Count > 0)
            {
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Resources:");
                GaiaResource resource;
                for (int resourceIdx = 0; resourceIdx < m_resourceList.Count; resourceIdx++)
                {
                    resource = m_resourceList[resourceIdx];
                    if (resource != null)
                    {
                        EditorGUILayout.LabelField(resource.m_name);
                    }
                }
            }

            //Dump the spawner list
            if (m_spawnerList.Count > 0)
            {
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Spawners:");
                Spawner spawner;
                for (int spawnerIdx = 0; spawnerIdx < m_spawnerList.Count; spawnerIdx++)
                {
                    spawner = m_spawnerList[spawnerIdx];
                    if (spawner != null)
                    {
                        EditorGUILayout.LabelField(spawner.gameObject.name);
                    }
                }
            }

            //End scroll
            GUILayout.EndScrollView();

            //Extension drop pad
            DropAreaGUI();

            GUILayout.BeginVertical("Extension Controller", m_boxStyle);
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Save Extension"))
            {
                SaveGX();
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.EndVertical();
            GUILayout.Space(5f);

        }

        public void DropAreaGUI()
        {
            //Ok - set up for drag and drop
            Event evt = Event.current;
            Rect drop_area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            GUI.Box(drop_area, "Drop Spawner Here:", m_boxStyle);

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
                            if (DragAndDrop.objectReferences.Length > 0)
                            {
                                //Debug.Log("Name is " + DragAndDrop.objectReferences[0].name);
                                //Debug.Log("Type is " + DragAndDrop.objectReferences[0].GetType());
                                if (DragAndDrop.objectReferences[0].GetType() == typeof(UnityEngine.GameObject))
                                {
                                    GameObject go = DragAndDrop.objectReferences[0] as GameObject;
                                    Spawner spawner = go.GetComponent<Spawner>();
                                    if (spawner != null)
                                    {
                                        //Only add spawners if we dont already have them
                                        for (int spIdx = 0; spIdx < m_spawnerList.Count; spIdx++)
                                        {
                                            if (m_spawnerList[spIdx].m_spawnerID == spawner.m_spawnerID && m_spawnerList[spIdx].name == spawner.name)
                                            {
                                                return;
                                            }
                                        }
                                        m_spawnerList.Add(spawner);

                                        //Check for resources that arent already there as well
                                        if (m_resourceList.Count == 0)
                                        {
                                            m_resourceList.Add(spawner.m_resources);
                                        }
                                        else
                                        {
                                            for (int resIdx = 0; resIdx < m_resourceList.Count; resIdx++)
                                            {
                                                if (m_resourceList[resIdx].m_resourcesID == spawner.m_resources.m_resourcesID && m_resourceList[resIdx].name == spawner.m_resources.name)
                                                {
                                                    return;
                                                }
                                                else
                                                {
                                                    m_resourceList.Add(spawner.m_resources);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            //If we got to here then we couldnt process it
                            //Debug.LogWarning("Object type not supported by exporter. Ignored.");
                        }

                        break;
                    }
            }
        }

        #endregion

        #region Engine

        private void SaveGX()
        {
            //Only do this if we have something to do
            if (m_resourceList.Count == 0 && m_spawnerList.Count == 0)
            {
                Debug.Log("No work to do - exiting.");
                return;
            }

            //Make sure we have basic exported extension directory
            string path = "Assets/GaiaExtensions/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path = path + FixFileName(m_publisherName) + "_" + FixFileName(m_extensionName) + ".cs";

            Template template;
            string hdrPath = GaiaCommon1.AssetUtils.GetAssetPath("GaiaXtnHeader", "txt");
            string ftrPath = GaiaCommon1.AssetUtils.GetAssetPath("GaiaXtnFooter", "txt");
            using (StreamWriter writer = File.CreateText(path))
            {
                //Write the generic header
                template = new Template(hdrPath, true);
                template.Set("ExtensionPublisher", FixFileName(m_publisherName) + "_" + FixFileName(m_extensionName));
                template.Set("ExtensionPublisherFull", m_publisherName);
                template.Set("ExtensionPackage", FixFileName(m_extensionName));
                template.Set("ExtensionPackageFull", m_extensionName);
                writer.Write(template.ToString());

                //Export the About Box
                if (!string.IsNullOrEmpty(m_aboutText))
                {
                    writer.WriteLine("\t\t");
                    writer.WriteLine("\t\t#if UNITY_EDITOR");        
                    writer.WriteLine("\t\tpublic static void GX_About_About()");
                    writer.WriteLine("\t\t{");
                    writer.WriteLine("\t\t\tEditorUtility.DisplayDialog(\"About " + m_extensionName + "\", \"" + m_aboutText + "\", \"OK\");");
                    writer.WriteLine("\t\t}");
                    writer.WriteLine("\t\t#endif");
                }

                //Export the resources
                foreach (GaiaResource resource in m_resourceList)
                {
                    if (resource == null)
                    {
                        continue;
                    }
                    resource.SetAssetAssociations();
                    writer.WriteLine("\t\t");
                    writer.WriteLine("\t\tpublic static void GX_Resources_Create" + FixFileName(resource.m_name) + "()");
                    writer.WriteLine("\t\t{");
                    writer.WriteLine(DumpObject(resource, "resource", 3));
                    writer.WriteLine("\t\t");
                    writer.WriteLine("\t\t\t#if UNITY_EDITOR");
                    writer.WriteLine("\t\t\tstring path = \"Assets/GaiaExtensions/\";");
                    writer.WriteLine("\t\t\tif (!Directory.Exists(path))");
                    writer.WriteLine("\t\t\t{");
                    writer.WriteLine("\t\t\t\tDirectory.CreateDirectory(path);");
                    writer.WriteLine("\t\t\t}");
                    writer.WriteLine("\t\t\t");
                    writer.WriteLine("\t\t\tpath = Path.Combine(path, \"" + FixFileName(m_publisherName) + "\");");
                    writer.WriteLine("\t\t\tif (!Directory.Exists(path))");
                    writer.WriteLine("\t\t\t{");
                    writer.WriteLine("\t\t\t\tDirectory.CreateDirectory(path);");
                    writer.WriteLine("\t\t\t}");
                    writer.WriteLine("\t\t\t");
                    writer.WriteLine("\t\t\tpath = Path.Combine(path, \"" + FixFileName(m_extensionName) + "\");");
                    writer.WriteLine("\t\t\tif (!Directory.Exists(path))");
                    writer.WriteLine("\t\t\t{");
                    writer.WriteLine("\t\t\t\tDirectory.CreateDirectory(path);");
                    writer.WriteLine("\t\t\t}");
                    writer.WriteLine("\t\t\t");
                    writer.WriteLine("\t\t\tpath = Path.Combine(path, \"" + FixFileName(m_publisherName) + "_" + FixFileName(m_extensionName) + "_" + FixFileName(resource.m_name) + ".asset\");");
                    writer.WriteLine("\t\t\tAssetDatabase.CreateAsset(resource, path);");
                    writer.WriteLine("\t\t\tAssetDatabase.SaveAssets();");
                    writer.WriteLine("\t\t\t#endif");
                    writer.WriteLine("\t\t}");
                }

                //Export the spawners
                foreach (Spawner spawner in m_spawnerList)
                {
                    if (spawner == null)
                    {
                        continue;
                    }
                    writer.WriteLine("\t\t");
                    writer.WriteLine("\t\tpublic static void GX_Spawners_Create" + FixFileName(spawner.gameObject.name) + "()");
                    writer.WriteLine("\t\t{");
                    writer.WriteLine(DumpObject(spawner, "spawner", 3));
                    writer.WriteLine("\t\t");
                    writer.WriteLine("\t\t\tspawner.FitToTerrain();");
                    writer.WriteLine("\t\t\tspawner.Initialise();");
                    writer.WriteLine("\t\t");
                    writer.WriteLine("\t\t\tGameObject gaiaObj = GameObject.Find(\"Gaia\");");
                    writer.WriteLine("\t\t\tif (gaiaObj == null)");
                    writer.WriteLine("\t\t\t{");
                    writer.WriteLine("\t\t\t\tgaiaObj = new GameObject(\"Gaia\");");
                    writer.WriteLine("\t\t\t}");
                    writer.WriteLine("\t\t\tgo.transform.parent = gaiaObj.transform;");
                    writer.WriteLine("\t\t}");
                }

                //Write the generic footer
                template = new Template(ftrPath, true);
                writer.Write(template.ToString());
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Remove any characters that could cause issues from the input string
        /// </summary>
        /// <param name="source">The source string</param>
        /// <returns>A destination string with rubbish removed</returns>
        string FixFileName(string source)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in source)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Extract and save the resources file
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="source"></param>
        private void SaveResources(StreamWriter writer, GaiaResource source)
        {
            if (writer == null)
            {
                Debug.LogError("Can not write resources, writer not supplied.");
                return;
            }
            if (source == null)
            {
                Debug.LogError("Can not write resources, resources not supplied.");
                return;
            }

            //Create extraction path
            string path = "Assets/GaiaExtensions/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path += m_extensionName + "/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path += Path.GetFileName(AssetDatabase.GetAssetPath(source.GetInstanceID()));

            GaiaResource resources = ScriptableObject.CreateInstance<Gaia.GaiaResource>();
            AssetDatabase.CreateAsset(resources, path);
            resources.m_beachHeight = source.m_beachHeight;
            resources.m_name = source.m_name;
            resources.m_resourcesID = source.m_resourcesID;
            resources.m_seaLevel = source.m_seaLevel;
            resources.m_terrainHeight = source.m_terrainHeight;

            //Iterate through the prototypes
            SpawnCritera srcCrit, newCrit;
            ResourceProtoTexture srcTex, newTex;
            resources.m_texturePrototypes = new ResourceProtoTexture[source.m_texturePrototypes.GetLength(0)];
            for (int resIdx = 0; resIdx < source.m_texturePrototypes.GetLength(0); resIdx++ )
            {
                srcTex = source.m_texturePrototypes[resIdx];
                newTex = new ResourceProtoTexture();
                newTex.m_metalic = srcTex.m_metalic;
                newTex.m_name = srcTex.m_name;
                newTex.m_normal = srcTex.m_normal;
                newTex.m_offsetX = srcTex.m_offsetX;
                newTex.m_offsetY = srcTex.m_offsetY;
                newTex.m_sizeX = srcTex.m_sizeX;
                newTex.m_sizeY = srcTex.m_sizeY;
                newTex.m_smoothness = srcTex.m_smoothness;
                newTex.m_texture = srcTex.m_texture;

                newTex.m_spawnCriteria = new SpawnCritera[srcTex.m_spawnCriteria.GetLength(0)];
                for (int critIdx = 0; critIdx < srcTex.m_spawnCriteria.GetLength(0); critIdx++ )
                {
                    srcCrit = srcTex.m_spawnCriteria[critIdx];

                    newCrit = new SpawnCritera();
                    newCrit.m_checkHeight = srcCrit.m_checkHeight;
                    newCrit.m_checkProximity = srcCrit.m_checkProximity;
                    newCrit.m_checkSlope = srcCrit.m_checkSlope;
                    newCrit.m_checkTexture = srcCrit.m_checkTexture;
                    newCrit.m_checkType = srcCrit.m_checkType;
                    newCrit.m_heightFitness = srcCrit.m_heightFitness;
                    newCrit.m_isActive = srcCrit.m_isActive;
                    newCrit.m_matchingTextures = srcCrit.m_matchingTextures;
                    newCrit.m_maxHeight = srcCrit.m_maxHeight;

                    newTex.m_spawnCriteria[critIdx] = newCrit;
                }
                resources.m_texturePrototypes[resIdx] = newTex;
            }
        }


        /// <summary>
        /// Write the content of the object out as C#, and assign the content of the original object to it
        /// </summary>
        /// <param name="src">Object to dump</param>
        /// <param name="name">Name of the variable to dump it to</param>
        /// <param name="tabDepth">Number of prefix tabs to prepend to each line</param>
        /// <param name="prefixType">Show the type of the object when creating it</param>
        /// <param name="instantiate">Write code to instantiate the object</param>
        /// <returns></returns>
        private string DumpObject(object src, string name, int tabDepth, bool prefixType = true, bool instantiate = true)
        {
            if (src == null)
            {
                return "";
            }

            StringBuilder retS = new StringBuilder();
            FieldInfo[] fields = src.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

            string tabs = "";
            for (int td = 0; td < tabDepth; td++ )
            {
                tabs += "\t";
            }

            if (instantiate)
            {
                if (!prefixType)
                {
                    if (src.GetType().BaseType == typeof(UnityEngine.ScriptableObject))
                    {
                        retS.AppendLine(string.Format("{0}{2} = ScriptableObject.CreateInstance<{1}>();", tabs, src.GetType(), name));
                    }
                    else if (src.GetType().BaseType == typeof(UnityEngine.MonoBehaviour))
                    {
                        retS.AppendLine(string.Format("{0}{2} = {2}.AddComponent<{1}>();", tabs, src.GetType(), name));
                    }
                    else
                    {
                        retS.AppendLine(string.Format("{0}{1} = new {2}();", tabs, name, src.GetType()));
                    }
                }
                else
                {
                    if (src.GetType().BaseType == typeof(UnityEngine.ScriptableObject))
                    {
                        retS.AppendLine(string.Format("{0}{1} {2} = ScriptableObject.CreateInstance<{1}>();", tabs, src.GetType(), name));
                    }
                    else if (src.GetType().BaseType == typeof(UnityEngine.MonoBehaviour))
                    {
                        retS.AppendLine(string.Format("{0}GameObject go = new GameObject();", tabs));
                        retS.AppendLine(string.Format("{0}go.name = \"{1}\";", tabs, ((UnityEngine.MonoBehaviour)src).gameObject.name));
                        retS.AppendLine(string.Format("{0}{1} {2} = go.AddComponent<{1}>();", tabs, src.GetType(), name));
                    }
                    else
                    {
                        retS.AppendLine(string.Format("{0}{1} {2} = new {1}();", tabs, src.GetType(), name));
                    }
                }
            }

            foreach (FieldInfo f in fields)
            {
                if (f.FieldType == typeof(System.String))
                {
                    retS.AppendLine(string.Format("{0}{1}.{2} = \"{3}\";", tabs, name, f.Name, f.GetValue(src)));
                }
                else if (f.FieldType == typeof(System.Single))
                {
                    retS.AppendLine(string.Format("{0}{1}.{2} = {3}f;", tabs, name, f.Name, f.GetValue(src)));
                }
                else if (f.FieldType == typeof(System.Double))
                {
                    retS.AppendLine(string.Format("{0}{1}.{2} = {3};", tabs, name, f.Name, f.GetValue(src)));
                }
                else if (f.FieldType == typeof(System.Int32) || f.FieldType == typeof(System.UInt32) || f.FieldType == typeof(System.Int64) || f.FieldType == typeof(System.UInt64))
                {
                    retS.AppendLine(string.Format("{0}{1}.{2} = {3};", tabs, name, f.Name, f.GetValue(src)));
                }
                else if (f.FieldType == typeof(System.Boolean))
                {
                    //Debug.Log(name + " " + f.Name + " " + f.FieldType + " " + f.GetValue(src).ToString());
                    if (f.GetValue(src).ToString() == "True")
                    {
                        retS.AppendLine(string.Format("{0}{1}.{2} = true;", tabs, name, f.Name));
                    }
                    else
                    {
                        retS.AppendLine(string.Format("{0}{1}.{2} = false;", tabs, name, f.Name));
                    }
                }
                else if (f.FieldType == typeof(UnityEngine.Texture2D))
                {
                    if (((UnityEngine.Texture2D)f.GetValue(src)) != null)
                    {
                        retS.AppendLine(string.Format("{0}{1}.{2} = GetTexture2D(\"{3}\");", tabs, name, f.Name, AssetDatabase.GetAssetPath(((UnityEngine.Texture2D)f.GetValue(src)).GetInstanceID())));
                    }
                    else
                    {
                        retS.AppendLine(string.Format("{0}{1}.{2} = null;", tabs, name, f.Name));
                    }
                }
                else if (f.FieldType == typeof(UnityEngine.AnimationCurve))
                {
                    if (((UnityEngine.AnimationCurve)f.GetValue(src)) != null)
                    {
                        UnityEngine.AnimationCurve curve = ((UnityEngine.AnimationCurve)f.GetValue(src));
                        retS.AppendLine(string.Format("{0}{1}.{2} = new UnityEngine.AnimationCurve();", tabs, name, f.Name));
                        foreach(Keyframe frame in curve.keys)
                        {
                            retS.AppendLine(string.Format("{0}{1}.{2}.AddKey({3}f,{4}f);", tabs, name, f.Name, frame.time, frame.value));
                        }
                    }
                    else
                    {
                        retS.AppendLine(string.Format("{0}{1}.{2} = null;", tabs, name, f.Name));
                    }
                }
                else if (f.FieldType == typeof(UnityEngine.Color))
                {
                    Color c = ((UnityEngine.Color)f.GetValue(src));
                    retS.AppendLine(string.Format("{0}{1}.{2} = new UnityEngine.Color({3}f,{4}f,{5}f,{6}f);", tabs, name, f.Name, c.r, c.b, c.g, c.a));
                }
                else if (f.FieldType == typeof(UnityEngine.GameObject))
                {
                    if (((UnityEngine.GameObject)f.GetValue(src)) != null)
                    {
                        retS.AppendLine(string.Format("{0}{1}.{2} = GetGameObject(\"{3}\");", tabs, name, f.Name, AssetDatabase.GetAssetPath(((UnityEngine.GameObject)f.GetValue(src)).GetInstanceID())));
                    }
                    else
                    {
                        retS.AppendLine(string.Format("{0}{1}.{2} = null;", tabs, name, f.Name));
                    }
                }
                else if (f.FieldType.IsEnum == true)
                {
                    string enumValue = "";
                    foreach (var value in Enum.GetValues(f.FieldType))
                    {
                        if ((int)value == (int)f.GetValue(src))
                        {
                            enumValue = value.ToString();
                        }
                    }

                    retS.AppendLine(string.Format("{0}{1}.{2} = {3}.{4};", tabs, name, f.Name, f.FieldType.ToString().Replace("+","."), enumValue));
                }
                else if (f.FieldType == typeof(Gaia.SpawnRuleExtension[]))
                {
                    retS.AppendLine(string.Format("{0}{1}.{2} = new {3}[{4}];", tabs, name, f.Name, f.FieldType.ToString().Replace("[]", ""), ((System.Array)f.GetValue(src)).GetLength(0)));
                    for (int idx = 0; idx < ((System.Array)f.GetValue(src)).GetLength(0); idx++)
                    {
                        retS.AppendLine(string.Format("{0}\t{1}.{2}[{3}] = GetSpawnRuleExtension(\"{4}\");", tabs, name, f.Name, idx, AssetDatabase.GetAssetPath(((Gaia.SpawnRuleExtension)(((System.Array)f.GetValue(src)).GetValue(idx))).GetInstanceID())));
                    }
                }
                else if (f.FieldType == typeof(Gaia.ResourceProtoDNA))
                {
                    retS.AppendLine(string.Format("{0}{1}.{2} = new {3}();", tabs, name, f.Name, f.FieldType.ToString().Replace("[]", "")));
                    retS.Append(DumpObject(f.GetValue(src), string.Format("{0}.{1}", name, f.Name), tabDepth+1, false, false));
                }
                else if (f.FieldType == typeof(Gaia.XorshiftPlus))
                {
                    retS.AppendLine(string.Format("{0}{1}.{2} = new {3}();", tabs, name, f.Name, f.FieldType.ToString().Replace("[]", "")));
                    retS.Append(DumpObject(f.GetValue(src), string.Format("{0}.{1}", name, f.Name), tabDepth + 1, false, false));
                }
                else if (f.FieldType == typeof(Gaia.GaiaResource))
                {
                    GaiaResource resource = ((Gaia.GaiaResource)f.GetValue(src));
                    if (resource != null)
                    {
                        retS.AppendLine(string.Format("{0}{1}.{2} = GetResource(\"{3}\",\"{4}\");", tabs, name, f.Name, 
                            FixFileName(m_publisherName) + "_" + FixFileName(m_extensionName) + "_" + FixFileName(resource.m_name) + ".asset", 
                            resource.m_resourcesID));
                    }
                    else
                    {
                        retS.AppendLine(string.Format("{0}{1}.{2} = null;", tabs, name, f.Name));
                    }
                }
                else if (f.FieldType.IsGenericType == true)
                {
                    string type = f.FieldType.ToString();
                    if (type.Contains("System.Collections.Generic.List"))
                    {
                        type = type.Substring(type.IndexOf("[") + 1);
                        type = type.Substring(0, type.IndexOf("]"));

                        retS.AppendLine(string.Format("{0}{1}.{2} = new List<{3}>();", tabs, name, f.Name, type));

                        int idx = 0;
                        IEnumerator enumerator = ((ICollection)f.GetValue(src)).GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            retS.AppendLine(string.Format("{0}{1}.{2}.Add(new {3}());", tabs, name, f.Name, type));
                            retS.Append(DumpObject(enumerator.Current , string.Format("{0}.{1}[{2}]", name, f.Name, idx++), tabDepth + 1, false, false));
                        }
                    }
                    else
                    {
                        retS.AppendLine(string.Format("{0}// ******* {1} {2} = {3};", tabs, name, f.FieldType, f.Name, f.GetValue(src)));
                    }
                }
                else if (f.FieldType.BaseType == typeof(System.Array))
                {
                    retS.AppendLine(string.Format("{0}{1}.{2} = new {3}[{4}];", tabs, name, f.Name, f.FieldType.ToString().Replace("[]",""), ((System.Array)f.GetValue(src)).GetLength(0)));
                    for (int idx = 0; idx < ((System.Array)f.GetValue(src)).GetLength(0); idx++)
                    {
                        retS.Append(DumpObject(((System.Array)f.GetValue(src)).GetValue(idx), string.Format("{0}.{1}[{2}]", name, f.Name, idx), tabDepth+1, false));
                    }
                }
                else if (f.FieldType == typeof(Gaia.HeightMap) || f.FieldType == typeof(Gaia.UnityHeightMap))
                {
                    //Dont export
                }
                else if (f.FieldType == typeof(UnityEngine.Transform))
                {
                    //Dont export
                }
                else if (f.FieldType == typeof(UnityEngine.LayerMask))
                {
                    //Dont export
                }
                else if (f.FieldType == typeof(System.Random))
                {
                    //Dont export
                }
                else if (f.FieldType == typeof(System.Collections.IEnumerator))
                {
                    //Dont export
                }
                else if (f.FieldType == typeof(UnityEngine.Bounds))
                {
                    //Dont export
                }
                else
                {
                    retS.AppendLine(string.Format("{0}// ******* {1} {2} = {3};", tabs, name, f.FieldType, f.Name, f.GetValue(src)));
                }
            }
            return retS.ToString();
        }

        #endregion

        #region Prototyping

        /// <summary>
        /// Get the resource from the ID provided
        /// </summary>
        /// <param name="path">Resource with the given ID to check</param>
        /// <param name="id">Resource with the given ID to check</param>
        /// <returns>Resource if found or null</returns>
        public static Gaia.GaiaResource GetResource(string path, string id)
        {
            Gaia.GaiaResource resource = null;

            #if UNITY_EDITOR
            string fileName = Path.GetFileNameWithoutExtension(path);
            resource = (GaiaResource)GaiaCommon1.AssetUtils.GetAssetScriptableObject(fileName);
            #endif

            return resource;
        }

        #endregion


        #region Scaffolding

        /// <summary>
        /// Get a content label - look the tooltip up if possible
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        GUIContent GetLabel(string name)
        {
            string tooltip = "";
            if (m_tooltips.TryGetValue(name, out tooltip))
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
            { "Fit To Terrain", "Fits and aligns the spawner to the terrain." },
            { "Reset", "Resets the spawner, deletes any spawned game objects, and resets the random number generator." },
            { "Spawn", "Run a single spawn iteration. You can run as many spawn iterations as you like." },
        };

        #endregion
    }

}
