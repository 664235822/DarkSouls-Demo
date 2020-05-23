// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GaiaCommon1.Localization
{
    /// <summary>
    /// Class that holds and sets the current PW Culture. Includes helper classes for language change GUI.
    /// </summary>
    public static class Culture
    {
        #region Private Data
        
        private static string[] m_languageNamesArray;
        private static IDictionary<string, SystemLanguage> m_languageMapping;
        private static int m_selectedLangIndex;

        #endregion

        #region Public Data Access

        /// <summary>
        /// The default language used by PW products
        /// </summary>
        public const SystemLanguage DEFAULT_LANGUAGE = SystemLanguage.English;

        /// <summary>
        /// The language currently set for PW products
        /// (those which does not have the language installed will fail back to the default language)
        /// </summary>
        public static SystemLanguage Language { get; private set; }

        #endregion

        #region Public Events Access

        /// <summary>
        /// Use this to sign up for Language changed events - It's called after the language changes.
        /// For example if the user selects a different language.
        /// </summary>
        public static event System.Action LanguageChanged;

        #endregion

        #region ctor

        static Culture()
        {
            if (!Translate.Present)
            {
                m_languageNamesArray = new string[] { DEFAULT_LANGUAGE.ToString() };
                m_languageMapping = new Dictionary<string, SystemLanguage>
                {
                    { DEFAULT_LANGUAGE.ToString(), DEFAULT_LANGUAGE }
                };
                m_selectedLangIndex = 0;
            }
            Language = DEFAULT_LANGUAGE;
        }

        #endregion

        #region We don't currently do this, but might consider later

        /// <summary>
        /// Set the Cultures so we can utilize it in different ways, for examle date formatting
        /// </summary>
        //[InitializeOnLoadMethod]
        //public static void SetCultures()
        //{
        //    //Debug.Log(Application.systemLanguage.ToString());
        //    //Debug.Log(System.Threading.Thread.CurrentThread.CurrentCulture);
        //    //Debug.Log(System.Threading.Thread.CurrentThread.CurrentUICulture);

        //    // We could set the CultureInfo but we don't do for now in case it could break stuff in Unity

        //    //CultureInfo correspondingCultureInfo = CultureInfo.GetCultures(CultureTypes.AllCultures).FirstOrDefault(x => x.EnglishName.Equals(m_language.ToString()));
        //    //use this to set: CultureInfo.CreateSpecificCulture(correspondingCultureInfo.TwoLetterISOLanguageName);

        //    //Can test with
        //    //Debug.Log(System.DateTime.Now.ToString("dd MMMM", GetCurrentCultureInfo()));
        //}

        #endregion

        #region Setup Methods

        /// <summary>
        /// All PW Apps languages will be available as a Union. If a language does not exist for an App, the system will fallback to the <see cref="DEFAULT_LANGUAGE"/>
        /// </summary>
        /// <param name="appLanguages"></param>
        public static void AddToLanguageSet(SystemLanguage[] appLanguages)
        {
            if (!Translate.Present)
            {
                if (appLanguages == null)
                {
                    if (Dev.Present)
                    {
                        Debug.LogError("Houston, we got a corrupt App Config file! App languages is null when attempting to add them.");
                    }
                    return;
                }

                List<string> languageNamesList = new List<string>(m_languageNamesArray);

                foreach (var language in appLanguages)
                {
                    if (languageNamesList.Contains(language.ToString()) == false)
                    {
                        languageNamesList.Add(language.ToString());
                        m_languageMapping[language.ToString()] = language;
                    }
                }

                languageNamesList.Sort();
                m_languageNamesArray = languageNamesList.ToArray();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get the language pack for the class with <paramref name="className"/>. 
        /// If the language does not exist, the system will fallback to the <see cref="DEFAULT_LANGUAGE"/>
        /// </summary>
        /// <param name="className">Name of the class to get the lang pack for</param>
        /// <returns>Lang pack</returns>
        public static LanguagePack GetLanguagePackOrDefault(string className)
        {
            LanguagePack langPack = EditorLanguagePack.Load(className, Language);

            if (langPack == null)
            {
                langPack = EditorLanguagePack.Load(className, DEFAULT_LANGUAGE);

                if (Translate.Present)
                {
                    if (langPack != null)
                    {
                        Translate.MissingLanguagePack(className);
                    }
                }
            }

            if (langPack == null)
            {
                if (Dev.Present)
                {
                    Dev.NoLocalizationPkg(className, Culture.Language, Culture.DEFAULT_LANGUAGE);
                }
                else
                {
                    Debug.LogError("No localization package was found for " + className + "!");
                }
            }

            return langPack;
        }

        /// <summary>
        /// This can be used to refresh the Localization System, the same as when the language changes
        /// </summary>
        public static void Refresh()
        {
            OnLanguageChange();
        }

        /// <summary>
        /// Draws a langugage selector dropdown and handles all the functionality as well
        /// </summary>
        public static void LanguageSelectorGUI()
        {
            if (Translate.Present)
            {
                EditorGUI.BeginChangeCheck();
                {
                    Language = (SystemLanguage)EditorGUILayout.EnumPopup(Language, GUILayout.Width(120f));
                }
                if (EditorGUI.EndChangeCheck())
                {
                    OnLanguageChange();
                }
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                {
                    m_selectedLangIndex = EditorGUILayout.Popup(m_selectedLangIndex, m_languageNamesArray, GUILayout.Width(120f));
                }
                if (EditorGUI.EndChangeCheck())
                {
                    Language = m_languageMapping[m_languageNamesArray[m_selectedLangIndex]];
                    OnLanguageChange();
                }
            }
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// OnLanguageChange
        /// </summary>
        private static void OnLanguageChange()
        {
            if (LanguageChanged != null)
            {
                LanguageChanged.Invoke();
            }
        }

        #endregion
    }
}