// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GaiaCommon1
{
    public class PWWelcome
    {
        private struct ProductWelcome
        {
            public Type Type;
            public AddCustomWelcomeAttribute Attribute;

            public ProductWelcome(Type type, AddCustomWelcomeAttribute attribute)
            {
                Type = type;
                Attribute = attribute;
            }
        }

        private const int WELCOME_FREE_PERIOD = PWConst.WELCOME_FREE_HOURS * 60 * 60;

        private static float m_loadTime = -1f;

        /// <summary>
        /// Called On Load
        /// </summary>
        [InitializeOnLoadMethod]
        private static void OnLoad()
        {
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.update += OnEditorUpdate;
        }


        /// <summary>
        /// We apply a little delay before showing welcome to ensure everything is ready.
        /// </summary>
        private static void OnEditorUpdate()
        {
            float waitTime = 3f;

            if (m_loadTime < 0f && !EditorApplication.isCompiling)
            {
                m_loadTime = Time.realtimeSinceStartup;
#if PW_DEBUG
            Debug.LogFormat("[PWWelcome]: Waiting '{0}' seconds.", waitTime); 
#endif
                return;
            }

            // Check every .6 seconds that the Editor didn't start some compiling as part of import/prod creation
            if (Time.realtimeSinceStartup - m_loadTime > 0.6f && EditorApplication.isCompiling)
            {
                m_loadTime = Time.realtimeSinceStartup;
#if PW_DEBUG
            Debug.LogFormat("[PWWelcome]: Editor is compiling. Reseted timer to wait '{0}' seconds.", waitTime); 
#endif
                return;
            }

            // Waiting <waitTime> seconds.
            if (Time.realtimeSinceStartup - m_loadTime > waitTime)
            {
                ShowWelcomeOnLoad();
                EditorApplication.update -= OnEditorUpdate;
            }
        }

        /// <summary>
        /// Shows Welcome OnLoad if it's enabled and enough time passed by since it was last shown.
        /// </summary>
        private static void ShowWelcomeOnLoad()
        {
            List<AppConfig> defaultWelcomeList = new List<AppConfig>(Prod.AppsConfigs);
            List<CustomWelcome> customWelcomeList = new List<CustomWelcome>();
            string productNames = "";
#if PW_DEBUG
            Debug.LogFormat("[PWWelcome]: {0} apps checked in. Looking for Custom Welcomes...", defaultWelcomeList.Count); 
#endif

            foreach (ProductWelcome prodWelcome in GetTypesWithWelcomeAttribute())
            {
                // Skipping the abstract class
                if(prodWelcome.Type == typeof(CustomWelcome))
                {
                    continue;
                }
                var custWelcome = (CustomWelcome)Activator.CreateInstance(prodWelcome.Type);
                defaultWelcomeList.Remove(custWelcome.AppConfig);
                if (IsTimeToShowWelcome(custWelcome.AppConfig))
                {
                    customWelcomeList.Add(custWelcome);
                    productNames = string.IsNullOrEmpty(productNames) ? custWelcome.AppConfig.Name : ", " + custWelcome.AppConfig.Name;
                }
            }

#if PW_DEBUG
            Debug.LogFormat("[PWWelcome]: Looking through the number of apps which are left with default welcome ({0}) " +
                "to see if it's time to show welcome for them...", defaultWelcomeList.Count); 
#endif
            for (int i = defaultWelcomeList.Count -1 ; i >= 0; i--)
            {
                if (IsTimeToShowWelcome(defaultWelcomeList[i]))
                {
                    productNames = string.IsNullOrEmpty(productNames) ? defaultWelcomeList[i].Name : ", " + defaultWelcomeList[i].Name;
                }
                else
                {
                    defaultWelcomeList.RemoveAt(i);
                }
            }

            if (string.IsNullOrEmpty(productNames) == false)
            {
                var welcomeScreen = EditorWindow.GetWindow<PWWelcomeScreen>(true, "Welcome To Procedural Worlds!");
                welcomeScreen.DefaultWelcomeList = defaultWelcomeList;
                welcomeScreen.CustomWelcomeList = customWelcomeList;
            }
        }

        /// <summary>
        /// Show welcome for a product on demand. This is calleed from the menu for example.
        /// </summary>
        /// <param name="appConfig"></param>
        public static void ShowWelcome(AppConfig appConfig)
        {
            if (appConfig == null)
            {
                Debug.LogWarning("AppConfig is null. Welcome will not be shown.");
                return;
            }

            Type customType = null;
            foreach (ProductWelcome prodWelcome in GetTypesWithWelcomeAttribute())
            {
                // Skipping the abstract class
                if (prodWelcome.Type == typeof(CustomWelcome))
                {
                    continue;
                }

                if (prodWelcome.Type.ToString().Contains(appConfig.NameSpace))
                {
                    customType = prodWelcome.Type;
                    break;
                }
            }

            var welcomeScreen = EditorWindow.GetWindow<PWWelcomeScreen>(true, "Welcome To Procedural Worlds!");
            if(customType == null)
            {
                welcomeScreen.DefaultWelcomeList = new List<AppConfig> { appConfig };
            }
            else
            {
                welcomeScreen.CustomWelcomeList = new List<CustomWelcome> { (CustomWelcome)Activator.CreateInstance(customType) };
            }
        }

        /// <summary>
        /// Checks if it's time to show welcome for an App.
        /// </summary>
        private static bool IsTimeToShowWelcome(AppConfig appConfig)
        {
            if (appConfig == null)
            {
                Debug.LogWarning("AppConfig is null. Can't check welcome status.");
                return false;
            }
            string name = appConfig.Name;
#if PW_DEBUG
            Debug.LogFormat("[PWWelcome]: Welcome for App '{0}' is {1} in CFG and {2} by the user.", name,
                    appConfig.HasWelcome ? "ENABLED" : "DISABLED", WelcomeEnabledForApp(name) ? "ENABLED" : "DISABLED"); 
#endif
            if (appConfig.HasWelcome && WelcomeEnabledForApp(name))
            {
#if PW_DEBUG
                int lastShown = EditorPrefs.GetInt(name + "WelcomeLastShown", 0);
                int currTime = Utils.GetFrapoch();
                Debug.LogFormat("[PWWelcome]: '{0}' Welcome was last shown {2}({1}).", name, lastShown, Utils.FrapochToLocalDate(lastShown));
                if (currTime - lastShown > WELCOME_FREE_PERIOD)
                {
                    Debug.LogFormat("[PWWelcome]: The next time to show Welcome for App '{0}' is {2}({1}). Showing the welcome now.", name, lastShown + WELCOME_FREE_PERIOD,
                        Utils.FrapochToLocalDate(lastShown + WELCOME_FREE_PERIOD));
                }
                else
                {
                    Debug.LogFormat("[PWWelcome]: The next time to show Welcome for App '{0}' is {2}({1}).", name, lastShown + WELCOME_FREE_PERIOD,
                        Utils.FrapochToLocalDate(lastShown + WELCOME_FREE_PERIOD));
                } 
#endif
                return (Utils.GetFrapoch() - EditorPrefs.GetInt(name + "WelcomeLastShown", 0) > WELCOME_FREE_PERIOD);
            }
            return false;
        }

        /// <summary>
        /// Checks if welcome is enabled for an App.
        /// </summary>
        public static bool WelcomeEnabledForApp(string appName)
        {
            return EditorPrefs.GetBool("Show" + appName + "Welcome", true);
        }

        /// <summary>
        /// Finds custome Welcomes that has the attribute.
        /// </summary>
        private static IEnumerable<ProductWelcome> GetTypesWithWelcomeAttribute()
        {
            foreach (var type in typeof(CustomWelcome).Assembly.GetTypes())
            {
                Attribute attribute = Attribute.GetCustomAttribute(type, typeof(AddCustomWelcomeAttribute), true);
                if (attribute != null)
                {
                    yield return new ProductWelcome(type, (AddCustomWelcomeAttribute)attribute);
                }
            }
        }
    }
}
