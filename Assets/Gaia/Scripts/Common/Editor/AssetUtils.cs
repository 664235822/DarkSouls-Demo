// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using GaiaCommon1.Internal;

namespace GaiaCommon1
{
    public partial class AssetUtils : MonoBehaviour
    {
        #region File helpers

        /// <summary>
        /// Get config by name
        /// </summary>
        /// <param name="name">Name of the config</param>
        /// <param name="silent">Fail silently</param>
        /// <returns>Config or null</returns>
        public static AppConfig GetConfig(string name, bool silent = false, bool nameIsPath = false)
        {
            string path = null;

            if (nameIsPath)
            {
                path = name;
            }
            else
            {
                path = GetAssetPath(name + "." + PWConst.CFG_EXTENSION);
                if (string.IsNullOrEmpty(path))
                {
                    if (!silent)
                    {
                        Debug.LogError("Files are missing or corrupt. Was the import completed? Please reimport your Procedural Worlds products if this message persist.");
                    }
                    return null;
                }
            }

            BinaryFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                object firstObj = formatter.Deserialize(stream);
                string cfgVersion = "1";

                if (firstObj.GetType() == typeof(string))
                {
                    cfgVersion = (string)firstObj;
                    firstObj = null;
                }
                else if (firstObj.GetType() != typeof(double))
                {
                    Debug.LogError("Unknown cfg file 0: " + firstObj.GetType());
                    return null;
                }

                switch (cfgVersion)
                {
                    case "1":
                        return LoadV1(cfgVersion, firstObj, formatter, stream);
                    default:
                        throw new ApplicationException("Unknown language pack v" + cfgVersion);
                }
            }
        }

        private static AppConfig LoadV1(string cfgVersion, object firstObj, BinaryFormatter formatter, Stream stream)
        {
            double lastUpdated = 0;

            // The last time the config was upated
            if (firstObj == null)
            {
                lastUpdated = (double)formatter.Deserialize(stream);
            }
            else
            {
                lastUpdated = (double)firstObj;
            }

            // Min Unity version
            var minUnity = (string)formatter.Deserialize(stream);

            // Name
            var pName = (string)formatter.Deserialize(stream);

            // Logo
            var width = (int)formatter.Deserialize(stream);
            var height = (int)formatter.Deserialize(stream);
            var format = (TextureFormat)formatter.Deserialize(stream);
            var mipmap = (bool)formatter.Deserialize(stream);
            var bytes = (byte[])formatter.Deserialize(stream);
            Texture2D logo = null;
            if (width < 1 || height < 1 || bytes.Length < 1)
            {
            }
            else
            {
                logo = new Texture2D(width, height, format, mipmap);
                logo.LoadRawTextureData(bytes);
                logo.Apply();
            }

            // Namespace, Folders
            var nameSpace = (string)formatter.Deserialize(stream);
            var folder = (string)formatter.Deserialize(stream);
            var scriptsFolder = (string)formatter.Deserialize(stream);
            var editorScriptsFolder = (string)formatter.Deserialize(stream);
            var docsFolder = (string)formatter.Deserialize(stream);

            // Versioning
            var majorVersion = (string)formatter.Deserialize(stream);
            var minorVersion = (string)formatter.Deserialize(stream);
            var patchVersion = (string)formatter.Deserialize(stream);

            // Languages
            var availableLanguages = (SystemLanguage[])formatter.Deserialize(stream);

            // Links
            var tutorialsLink = (string)formatter.Deserialize(stream);
            var discordLink = (string)formatter.Deserialize(stream);
            var supportLink = (string)formatter.Deserialize(stream);
            var asLink = (string)formatter.Deserialize(stream);

            // Remote settings
            var newsURL = (string)formatter.Deserialize(stream);

            // Other settings
            var hasWelcome = (bool)formatter.Deserialize(stream);

            return new AppConfig(
                cfgVersion,
                lastUpdated,

                // Min Unity version
                minUnity,

                // Name
                pName,

                // Logo
                logo,

                // Namespace, Folders
                nameSpace,
                folder,
                scriptsFolder,
                editorScriptsFolder,
                docsFolder,

                // Versioning
                majorVersion,
                minorVersion,
                patchVersion,

                // Languages
                availableLanguages,

                // Links
                tutorialsLink,
                discordLink,
                supportLink,
                asLink,

                // Remote settings
                newsURL,

                // Other settings
                hasWelcome
            );
        }

        #endregion

        #region Asset, Scriptable Object, GameObject helpers

        /// <summary>
        /// Write a scriptable object out into a new asset that can be shared
        /// </summary>
        /// <typeparam name="T">The scriptable object to be saved as an asset</typeparam>
        public static void CreateAsset<T>() where T : ScriptableObject
        {
#if UNITY_EDITOR
            T asset = ScriptableObject.CreateInstance<T>();

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");
            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
#endif
        }

        /// <summary>
        /// Get the path of the unity object supplied
        /// </summary>
        /// <param name="uo"></param>
        /// <returns></returns>
        public static string GetAssetPath(UnityEngine.Object uo)
        {
            string path = "";
#if UNITY_EDITOR
            path = Application.dataPath + "/" + AssetDatabase.GetAssetPath(uo);
            path = path.Replace("/Assets", "");
            path = path.Replace("\\", "/");
#endif
            return path;
        }

        /// <summary>
        /// Wrap the scriptable object up so that it can be transferred without causing unity errors
        /// </summary>
        /// <param name="so"></param>
        public static string WrapScriptableObject(ScriptableObject so)
        {
            string newpath = "";
#if UNITY_EDITOR
            string path = GetAssetPath(so);
            if (File.Exists(path))
            {
                newpath = Path.ChangeExtension(path, "bytes");
                UnityEditor.FileUtil.CopyFileOrDirectory(path, newpath);
            }
            else
            {
                Debug.LogError("There is no file at the path supplied: " + path);
            }
#endif
            return newpath;
        }


        public static void UnwrapScriptableObject(string path, string newpath)
        {
#if UNITY_EDITOR
            if (File.Exists(path))
            {
                if (!File.Exists(newpath))
                {
                    UnityEditor.FileUtil.CopyFileOrDirectory(path, newpath);
                }
                else
                {
                    Debug.LogError("There is already a file with this name at the path supplied: " + newpath);
                }
            }
            else
            {
                Debug.LogError("There is no file at the path supplied: " + path);
            }
#endif
        }

        public static string WrapGameObjectAsPrefab(GameObject go)
        {
#if UNITY_EDITOR

#if UNITY_2018_3_OR_NEWER
            UnityEngine.Object prefab = PrefabUtility.SaveAsPrefabAsset(go, "Assets/" + go.name + ".prefab");
#else
            UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab("Assets/" + go.name + ".prefab");
            PrefabUtility.ReplacePrefab(go, prefab);
#endif
            AssetDatabase.Refresh();
            return AssetDatabase.GetAssetPath(prefab);
#else
            return "";
#endif
        }

        /// <summary>
        /// Get all the assets that matches the name
        /// </summary>
        /// <param name="fileName">File name to search for</param>
        /// <returns></returns>
        public static List<string> GetAllAssetPathsWithFilename(string fileName)
        {
            List<string> paths = new List<string>();
#if UNITY_EDITOR
            string fName = Path.GetFileNameWithoutExtension(fileName);
            string[] assets = AssetDatabase.FindAssets(fName, null);
            for (int idx = 0; idx < assets.Length; idx++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assets[idx]);
                if (Path.GetFileName(path) == fileName)
                {
                    paths.Add(path);
                }
            }
#endif
            return paths;
        }

        /// <summary>
        /// Get the asset path of the first thing that matches the name
        /// </summary>
        /// <param name="fileName">File name to search for</param>
        /// <returns></returns>
        public static string GetAssetPath(string fileName)
        {
#if UNITY_EDITOR
            string fName = Path.GetFileNameWithoutExtension(fileName);
            string[] assets = AssetDatabase.FindAssets(fName, null);
            for (int idx = 0; idx < assets.Length; idx++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assets[idx]);
                if (Path.GetFileName(path) == fileName)
                {
                    return path;
                }
            }
#endif
            return "";
        }

        /// <summary>
        /// Get the asset path of the first thing that matches the name
        /// </summary>
        /// <param name="name">Name to search for</param>
        /// <param name="name">Type to search for</param>
        /// <returns></returns>
        public static string GetAssetPath(string name, string type)
        {
#if UNITY_EDITOR
            string[] assets = AssetDatabase.FindAssets(name, null);
            string[] file;
            for (int idx = 0; idx < assets.Length; idx++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assets[idx]);
                //Make sure its an exact match
                file = Path.GetFileName(path).Split('.');
                if (file.GetLength(0) != 2)
                {
                    continue;
                }
                if (file[0] != name)
                {
                    continue;
                }
                if (file[1] != type)
                {
                    continue;
                }
                return path;
            }
#endif
            return "";
        }

        /// <summary>
        /// Returns the first asset that matches the file path and name passed. Will try
        /// full path first, then will try just the file name.
        /// </summary>
        /// <param name="fileNameOrPath">File name as standalone or fully pathed</param>
        /// <returns>Object or null if it was not found</returns>
        public static UnityEngine.Object GetAsset(string fileNameOrPath, Type assetType)
        {
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(fileNameOrPath))
            {
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(fileNameOrPath, assetType);
                if (obj != null)
                {
                    return obj;
                }
                else
                {
                    string path = GetAssetPath(Path.GetFileName(fileNameOrPath));
                    if (!string.IsNullOrEmpty(path))
                    {
                        return AssetDatabase.LoadAssetAtPath(path, assetType);
                    }
                }
            }
#endif
            return null;
        }

        /// <summary>
        /// Return the first prefab that exactly matches the given name from within the current project
        /// </summary>
        /// <param name="name">Asset to search for</param>
        /// <returns>Returns the prefab or null</returns>
        public static GameObject GetAssetPrefab(string name)
        {
#if UNITY_EDITOR
            string path = GetAssetPath(name, "prefab");
            if (!string.IsNullOrEmpty(path))
            {
                return AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
#endif
            return null;
        }

        /// <summary>
        /// Return the first scriptable that exactly matches the given name from within the current project
        /// </summary>
        /// <param name="name">Asset to search for</param>
        /// <returns>Returns the prefab or null</returns>
        public static ScriptableObject GetAssetScriptableObject(string name)
        {
#if UNITY_EDITOR
            string path = GetAssetPath(name, "asset");
            if (!string.IsNullOrEmpty(path))
            {
                return AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            }
#endif
            return null;
        }

        /// <summary>
        /// Return the first texture that exactly matches the given name from within the current project
        /// </summary>
        /// <param name="name">Asset to search for</param>
        /// <returns>Returns the texture or null</returns>
        public static Texture2D GetAssetTexture2D(string name)
        {
#if UNITY_EDITOR
            string[] assets = AssetDatabase.FindAssets(name, null);
            for (int idx = 0; idx < assets.Length; idx++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assets[idx]);
                if (path.Contains(".jpg") || path.Contains(".psd") || path.Contains(".png"))
                {
                    //Make sure its an exact match
                    string filename = Path.GetFileNameWithoutExtension(path);
                    if (filename == name)
                    {
                        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    }
                }
            }
#endif
            return null;
        }

        #endregion
    }
}

#endif