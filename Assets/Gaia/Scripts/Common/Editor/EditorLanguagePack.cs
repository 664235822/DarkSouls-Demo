// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using UnityEditor;
using UnityEngine;

namespace GaiaCommon1.Localization
{
    /// <summary>
    /// Contains Localized data for an object
    /// </summary>
    public static class EditorLanguagePack
    {
        /// <summary>
        /// Loads a language pack and returns it, or null.
        /// </summary>
        /// <param name="name">Name of the lang pack.</param>
        /// <param name="language">Language of the lang pack.</param>
        /// <returns>A language pack with the name and language, or null</returns>
        public static LanguagePack Load(string name, SystemLanguage language)
        {
            string filename = name + "." + language.ToString();

#if UNITY_EDITOR
            string[] guids = AssetDatabase.FindAssets(filename);
            if (guids == null || guids.Length < 1)
            {
                return null;
            } 
#endif

            string path = null;

#if UNITY_EDITOR
            // Find the first match
            foreach (var guid in guids)
            {
                string p = AssetDatabase.GUIDToAssetPath(guid);
                if (p.EndsWith("." + PWConst.LANG_PK_EXTENSION))
                {
                    path = p;
                    break;
                }
            } 
#endif

            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            return LanguagePack.Load(path);
        }
    }
}
