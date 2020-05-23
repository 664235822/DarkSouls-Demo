// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using GaiaCommon1.Internal;

namespace GaiaCommon1
{
    public static class Prod
    {
        public static List<AppConfig> AppsConfigs { get; private set; }

        static Prod()
        {
            if (AppsConfigs == null)
            {
                AppsConfigs = new List<AppConfig>();
            }
        }

        /// <summary>
        /// Installed Apps check-in here.
        /// </summary>
        /// <param name="appConfig"></param>
        public static void Checkin(AppConfig appConfig)
        {
            if (appConfig == null)
            {
                Debug.LogError("AppConfig is null. App checkin failed.");
                return;
            }
#if PW_DEBUG
            Debug.Log("[C.Prod-" + appConfig.NameSpace + "]: " + appConfig.Name + " just Checked in.");
#endif
            AppsConfigs.Add(appConfig);

            if (AppsConfigs.Count > 0)
            {
                // This was essentially triggered by the last app checking in
                CreateCommonMenu(AppsConfigs[AppsConfigs.Count - 1]);
            }
            else
            {
                // How did we get here?
                Debug.LogError("Lost the apps that checked in.");
            }
        }

        /// <summary>
        /// Checks if this product should implement the common menu and creates it accordingly.
        /// </summary>
        /// <param name="appConfig"></param>
        private static void CreateCommonMenu(AppConfig appConfig)
        {
            string verKey = Application.productName + "CommonMenuVer";
            string pathKey = Application.productName + "CommonMenuPath";
            int menuVer = EditorPrefs.GetInt(verKey, 0);
            string path = EditorPrefs.GetString(pathKey, null);

#if PW_DEBUG
            Debug.LogFormat("[C.Prod-{0}]: From EditorPrefs: menuVer(key: '{1}'): '{2}'; path(key: '{3}'): '{4}'", appConfig.NameSpace, verKey, menuVer, pathKey, path);
#endif

            if (!string.IsNullOrEmpty(path) && !File.Exists(path))
            {
#if PW_DEBUG
                Debug.Log("[C.Prod-" + appConfig.NameSpace + "]: Path in EditorPrefs was not empty, but the file was not found at path: " + path + "'");
                Debug.Log("[C.Prod-" + appConfig.NameSpace + "]: Searching by filename...");
#endif
                path = AssetUtils.GetAssetPath(PWConstInternal.COMMON_MENU_FILENAME);
#if PW_DEBUG
                Debug.Log("[C.Prod-" + appConfig.NameSpace + "]: Filename search result: " + path + "'");
#endif
                if (!string.IsNullOrEmpty(path) && !File.Exists(path))
                {
                    // Something fishy is going on.
                    Debug.LogErrorFormat("Could not find the asset file that's in the Asset Database with path '" + path + "'");
                    path = null;
                }
            }

            if (!string.IsNullOrEmpty(path) && menuVer >= PWConst.VERSION)
            {
                // Nothing to do here
#if PW_DEBUG
                Debug.Log("[C.Prod-" + appConfig.NameSpace + "]: Nothing to do here. This guy is king: " + path + "'");
#endif
                return;
            }
#if PW_DEBUG
            else
            {
                Debug.Log("[C.Prod-" + appConfig.NameSpace + "]: Taking over...");
            }
#endif

            // This is a newer version. Let's remove any existing menu files and add the updated one
            DeleteExistingMenuFiles(appConfig);

            // The new path - Check if this is a custom common embedded into a product
            string[] fullNameBits = typeof(Prod).FullName.Split('.');

            // If regular Common
            if (fullNameBits[fullNameBits.Length - 2].StartsWith("PWCommon"))
            {
                // This is a quick solution we may improve later.
                // Looks up the resource file in order to get the path for a Common Editor folder
                // It will target Custom Common folders embedded in the projects as well but we don't mind that for now.
                string resourceFile = "CommonMenu" + PWConst.VERSION_IN_FILENAMES + ".txt";
                path = AssetUtils.GetAssetPath(resourceFile);
#if PW_DEBUG
                Debug.LogFormat("[C.Prod-{0}]: Looked for the Common({1}) Resources folder and found: '{2}'", appConfig.NameSpace, PWConst.VERSION, path.Replace(resourceFile, ""));
#endif
                if (string.IsNullOrEmpty(path))
                {
#if PW_DEBUG
                    Debug.LogError("Unable to locate path for core component generation. Were any Procedural Worlds files removed renamed, or did we not wait enough for the import?");
#endif
                    return;
                }
                path = path.Replace("/Resources/" + resourceFile, "");
                if (string.IsNullOrEmpty(path))
                {
                    // Something really weird is going on
                    Debug.LogError("Something has gone wrong while looking for the Common Editor folder.");
                    return;
                }
            }
            // or custom Common embedded into a legacy product
            else
            {
                path = Utils.GetEditorScriptsPath(appConfig);
#if PW_DEBUG
                Debug.Log("[C.Prod-" + appConfig.NameSpace + "]: Looked for the Editor scripts folder and found: '" + path + "'");
#endif

                if (string.IsNullOrEmpty(path))
                {
                    Debug.LogError("Unable to locate path for core component generation. Was the directory structure of " +
                        appConfig.Name + " changed?");
                    return;
                }
            }


            path = path + "/" + PWConstInternal.COMMON_MENU_FILENAME;

            if(!AddCommonMenu(appConfig, path))
            {
#if PW_DEBUG
            Debug.LogWarning("[C.Prod-" + appConfig.NameSpace + "]: New Menu file could not be created at: " + path + "'");
#endif
                return;
            }
#if PW_DEBUG
            Debug.Log("[C.Prod-" + appConfig.NameSpace + "]: New Menu file was created at: " + path + "'");
#endif

            EditorPrefs.SetInt(Application.productName + "CommonMenuVer", PWConst.VERSION);
            EditorPrefs.SetString(Application.productName + "CommonMenuPath", path);
#if PW_DEBUG
            Debug.Log("[C.Prod-" + appConfig.NameSpace + "]: Set EditorPrefs: '" + Application.productName + "CommonMenuVer" + "'::'" + PWConst.VERSION + "';  '" +
                Application.productName + "CommonMenuPath" + "'::'" + path + "'");
            Debug.Log("[C.Prod-" + appConfig.NameSpace + "]: Refreshing the Asset Database...");
#endif
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Deletes any existing menu files. Usually used when a product replaces an outdated Common Menu.
        /// </summary>
        private static void DeleteExistingMenuFiles(AppConfig appConfig)
        {
            List<string> menuFilePaths = AssetUtils.GetAllAssetPathsWithFilename(PWConstInternal.COMMON_MENU_FILENAME);

#if PW_DEBUG
            Debug.Log("[C.Prod-" + appConfig.NameSpace + "]: Deleting all existing menu files...");
#endif
            for (int i = 0; i < menuFilePaths.Count; i++)
            {
                AssetDatabase.DeleteAsset(menuFilePaths[i]);
#if PW_DEBUG
                Debug.Log("[C.Prod-" + appConfig.NameSpace + "]: Deleted menu file at path " + menuFilePaths[i] + "'.");
#endif
            }
        }

        /// <summary>
        /// Adds the common menu at the path
        /// </summary>
        /// <param name="path">Path to add the common menu to</param>
        private static bool AddCommonMenu(AppConfig appConfig, string path)
        {
            return CreateFromTemplate(appConfig, "CommonMenu" + PWConst.VERSION_IN_FILENAMES + ".txt", path);
        }

        /// <summary>
        /// Creates project files from templates and return <see langword="true"/> upon success
        /// </summary>
        /// <param name="refreshADB">Refresh the Asset Database when complete</param>
        /// <returns>True if the file was created</returns>
        private static bool CreateFromTemplate(AppConfig appConfig, string templateFilename, string targetFile)
        {            
            string templatePath = AssetUtils.GetAssetPath(templateFilename);

            if (string.IsNullOrEmpty(templatePath))
            {
                if (Dev.Present)
                {
                    // Unity can error out due to the txt not being imported the first time round this is executed.
                    // This is not nice but we don't have time. Error in dev to prompt for a better solution. Probably best to go around unity altogether.
                    Debug.LogError(templateFilename + " was not found");
                }
                return false;
            }

            string text = File.ReadAllText(templatePath);

            //var assembly = Assembly.GetExecutingAssembly();
            
            //using (Stream stream = assembly.GetManifestResourceStream("CommonMenu.txt"))
            //{
            //    using(StreamReader reader = new StreamReader(stream))
            //    {
            //        string
            //    }
            //}

            text = text.Replace("TPL_NAMESPACE", appConfig.NameSpace);
            text = text.Replace("%NAMESPACE%", appConfig.NameSpace);
            text = text.Replace("TPL_NAME", appConfig.Name);
            text = text.Replace("%NAME%", appConfig.Name);
            text = text.Replace("%FOLDER%", appConfig.Folder);

            text = text.Replace("%COMMON_VERSION%", PWConst.VERSION.ToString());
            text = text.Replace("%COMMON_EDITOR_DLL_FILENAME%", PWConstInternal.COMMON_EDITOR_DLL_FILENAME);
            text = text.Replace("%COMMON_RUNTIME_DLL_FILENAME%", PWConstInternal.COMMON_RUNTIME_DLL_FILENAME);
            text = text.Replace("%INACTIVE_COMMON_EXTENSION%", PWConstInternal.INACTIVE_COMMON_EXTENSION);
            text = text.Replace("//%REMOVE_COMMENTING_TO_ACTIVATE%", "");

            string targetDir = new FileInfo(targetFile).Directory.FullName;
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            File.WriteAllText(targetFile, text, System.Text.Encoding.UTF8);

            return true;
        }
    }
}
