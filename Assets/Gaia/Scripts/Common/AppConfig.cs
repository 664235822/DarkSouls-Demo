// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using UnityEngine;

namespace GaiaCommon1
{
    public class AppConfig
    {
        /// <summary>
        /// The current AppConfig version of the library.
        /// </summary>
        public const string VERSION = "1";

        /// <summary>
        /// The version of the data as it's currently serialized on disk
        /// </summary>
        public readonly string CfgVersion;

        // The last time the config was upated
        public readonly double LastUpdated;

        // Min Unity version
        public readonly string MinUnity;

        // Name, Logo
        public readonly string Name;
        public readonly Texture2D Logo;

        // Namespace, Folders
        public readonly string NameSpace;
        public readonly string Folder;
        public readonly string ScriptsFolder;
        public readonly string EditorScriptsFolder;
        public readonly string DocsFolder;
        public readonly string DocsFolderSpaced;

        // Versioning
        public readonly string MajorVersion;
        public readonly string MinorVersion;
        public readonly string PatchVersion;
        public readonly string Version;

        // Languages
        public readonly SystemLanguage[] AvailableLanguages;

        // Links
        public readonly string TutorialsLink;
        public readonly string DiscordLink;
        public readonly string SupportLink;
        public readonly string ASLink;

        // Remote settings
        public readonly string NewsURLStripped;
        public string NewsURL { get { return NewsURLStripped + "?gv=" + NameSpace + "-v." + Version; } }

        // Other settings
        public readonly bool HasWelcome;

        public AppConfig(string minUnity, string name, SystemLanguage[] availableLanguages)
        {
            CfgVersion = VERSION;

            LastUpdated = 0;

            MinUnity = minUnity;
            Name = name;

            // Name, Logo
            Logo = null;

            // Namespace, Folders
            NameSpace = name.Replace(" ", "");
            NameSpace = NameSpace.Replace("-", "");
            NameSpace = NameSpace.Replace(".", "");
            Folder = name;
            ScriptsFolder = "Scripts";
            EditorScriptsFolder = ScriptsFolder + "/Editor";
            DocsFolder = "Documentation";
            DocsFolderSpaced = DocsFolder.Replace("/", " / ");

            // Versioning
            MajorVersion = "0";
            MinorVersion = "0";
            PatchVersion = "0";
            Version = "0.0.0";

            // Languages
            AvailableLanguages = availableLanguages;

            // Links
            TutorialsLink = "http://www.procedural-worlds.com/" + NameSpace.ToLower() + "/?section=tutorials";
            DiscordLink = "https://discord.gg/rtKn8rw";
            SupportLink = "https://proceduralworlds.freshdesk.com/support/home";
            ASLink = "https://www.assetstore.unity3d.com/en/#!/search/page=1/sortby=popularity/query=publisher:15277";

            // Remote settings
            NewsURLStripped = "http://www.procedural-worlds.com/gaiajson.php";

            // Other settings
            HasWelcome = true;

            Debug.LogWarning("Created a blank config for " + name);
        }

        public AppConfig(
            string cfgVersion,
            double lastUpdated,

            string minUnity,
            string name,
            Texture2D logo,

            string nameSpace,
            string folder,
            string scriptsFolder,
            string editorScriptsFolder,
            string docsFolder,

            string majorVer,
            string minorVer,
            string patchVer,

            SystemLanguage[] availableLang,

            string tutorialsLink,
            string discordLink,
            string supportLink,
            string asLink,

            string newsURL,

            bool hasWelcome
            )
        {
            this.CfgVersion = cfgVersion;

            LastUpdated = lastUpdated;

            // Min Unity version
            MinUnity = minUnity;

            // Name, Logo
            Name = name;
            Logo = logo;

            // Namespace, Folders
            NameSpace = nameSpace;
            Folder = folder;
            ScriptsFolder = scriptsFolder;
            EditorScriptsFolder = editorScriptsFolder;
            DocsFolder = docsFolder;
            DocsFolderSpaced = DocsFolder.Replace("/", " / ");

            // Versioning
            MajorVersion = majorVer.ToString();
            MinorVersion = minorVer.ToString();
            PatchVersion = patchVer.ToString();
            Version = MajorVersion + "." + MinorVersion + "." + PatchVersion;

            // Languages
            AvailableLanguages = availableLang;

            // Links
            TutorialsLink = tutorialsLink;
            DiscordLink = discordLink;
            SupportLink = supportLink;
            ASLink = asLink;

            // Remote settings
            NewsURLStripped = newsURL;

            // Other settings
            HasWelcome = hasWelcome;
        }
    }
}
