// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GaiaCommon1.Localization
{
    /// <summary>
    /// Contains Localized data for an object
    /// </summary>
    public class LanguagePack
    {
        /// <summary>
        /// We use the dictionary, so we don't load the same pack twice.
        /// This way we can use the same object in EditorUtils and also edit it.
        /// </summary>
        private static IDictionary<string, LanguagePack> ms_loadedPacks = new Dictionary<string, LanguagePack>();

        /// <summary>
        /// The current Language Pack version of the library.
        /// </summary>
        public const string VERSION = "1";

        /// <summary>
        /// The version of the data as it's currently serialized on disk
        /// </summary>
        public string Version;

        // The last time the config was upated
        public double LastUpdated;

        // Categories and their content
        public List<LocalizationCategory> Categories;

        // Item dictionary for quick access
        public IDictionary<string, LocalizationItem> Items;

        // The path of the when loaded from file
        private string m_path;

        /// <summary>
        /// Create a blank Language pack
        /// </summary>
        public LanguagePack()
        {
            Version = LanguagePack.VERSION;

            LastUpdated = Utils.GetFrapoch();

            Categories = new List<LocalizationCategory>();

            Items = new Dictionary<string, LocalizationItem>();
        }

        /// <summary>
        /// Create a Lang pack with a set of items that will be added to a "Main" category
        /// </summary>
        public LanguagePack(LocalizationItem[] items)
        {
            Version = LanguagePack.VERSION;

            LastUpdated = Utils.GetFrapoch();

            Categories = new List<LocalizationCategory>();

            LocalizationCategory mainCategory = new LocalizationCategory("Main")
            {
                Items = new List<LocalizationItem>(items)
            };
            Categories.Add(mainCategory);

            Items = new Dictionary<string, LocalizationItem>();
            foreach (var category in Categories)
            {
                for (int i = 0; i < category.Items.Count; i++)
                {
                    if (category.Items[i] != null)
                    {
                        Items[category.Items[i].Key] = category.Items[i];
                    }
                }
            }
        }

        /// <summary>
        /// Create a Lang pack with a set of categories and their content
        /// </summary>
        public LanguagePack(LocalizationCategory[] categories)
        {
            Version = LanguagePack.VERSION;

            LastUpdated = Utils.GetFrapoch();

            Categories = new List<LocalizationCategory>(categories);

            Items = new Dictionary<string, LocalizationItem>();
            foreach (var category in Categories)
            {
                if (category == null)
                {
                    continue;
                }

                for (int i = 0; i < category.Items.Count; i++)
                {
                    if (category.Items[i] != null)
                    {
                        Items[category.Items[i].Key] = category.Items[i];
                    }
                }
            }
        }

        /// <summary>
        /// Loads a language pack and returns it, or null.
        /// </summary>
        /// <param name="path">Path of the lang pack file.</param>
        /// <returns>A language pack with the name and language, or null</returns>
        public static LanguagePack Load(string path)
        {
            if (ms_loadedPacks.ContainsKey(path) && ms_loadedPacks[path] != null)
            {
                return ms_loadedPacks[path];
            }

            LanguagePack langPack = new LanguagePack();
            
            BinaryFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // Version
                langPack.Version = (string)formatter.Deserialize(stream);

                switch (langPack.Version)
                {
                    case "1":
                        langPack.LoadV1(formatter, stream);
                        ms_loadedPacks[path] = langPack;
                        break;
                    default:
                        throw new ApplicationException("Unknown language pack v" + langPack.Version);
                }
            }

            langPack.m_path = path;
            return ms_loadedPacks[path];
        }

        /// <summary>
        /// Reloads the pack from file.
        /// </summary>
        public void ReLoad()
        {
            if (string.IsNullOrEmpty(m_path))
            {
                Debug.LogWarning("Can only reload language packs that were loaded from file.");
                return;
            }

            // Reset things before loading
            Categories = new List<LocalizationCategory>();
            Items = new Dictionary<string, LocalizationItem>();

            BinaryFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(m_path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // Version
                Version = (string)formatter.Deserialize(stream);

                switch (Version)
                {
                    case "1":
                        LoadV1(formatter, stream);
                        break;
                    default:
                        throw new ApplicationException("Unknown language pack v" + Version);
                }
            }
        }
        
        private void LoadV1(BinaryFormatter formatter, Stream stream)
        {
            // The last time the pack was upated
            LastUpdated = (double)formatter.Deserialize(stream);

            // Number of categories
            int numCategories = (int)formatter.Deserialize(stream);

            for (int ci = 0; ci < numCategories; ci++)
            {
                // Category Name
                var category = new LocalizationCategory((string)formatter.Deserialize(stream));

                // Number of items in category
                int numItems = (int)formatter.Deserialize(stream);

                // Loop through categories
                for (int i = 0; i < numItems; i++)
                {
                    var item = new LocalizationItem();

                    // Item Key
                    item.Key = (string)formatter.Deserialize(stream);
                    // Item Val
                    item.Val = (string)formatter.Deserialize(stream);
                    // Item Tooltip
                    item.Tooltip = (string)formatter.Deserialize(stream);
                    // Item Help
                    item.Help = (string)formatter.Deserialize(stream);
                    // Item Context (Used proved context for translation where needed)
                    item.Context = (string)formatter.Deserialize(stream);

                    category.Items.Add(item);
                    Items[item.Key] = item;
                }

                Categories.Add(category);
            }
        }

        private event Action OnChange;

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public void Validate()
        {
            if (OnChange != null)
            {
                OnChange.Invoke();
            }
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public void AddOnChangeAction(Action action)
        {
            OnChange += action;
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public void RemoveOnChangeAction(Action action)
        {
            OnChange -= action;
        }
    }
}
