// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using UnityEngine;
using GaiaCommon1.Internal;

namespace GaiaCommon1
{
    /// <summary>
    /// Internal and non internal got completely mixed up and will need clean up when we get time.
    /// </summary>
    public static class PWConst
    {
        public const int VERSION = 1;
        public static readonly string VERSION_IN_FILENAMES = "_c" + VERSION.ToString();

        public const string CFG_EXTENSION = "pwcfg";
        public const string LANG_PK_EXTENSION = "pwlpk";

        public static readonly Color LINK_COLOR = new Color(0.251f, 0.392f, 1f);

        public const string DISCORD_LINK = "https://discord.gg/rtKn8rw";
        public const string ASSETS_LINK = "https://www.assetstore.unity3d.com/en/#!/search/page=1/sortby=popularity/query=publisher:15277";
        public const string SECTR_ASSETS_LINK = "https://www.assetstore.unity3d.com/en/?stay#!/search/page=1/sortby=popularity/query=publisher:6087";

        public const string LOCALIZATION_FOLDER_NAME = "Localization";
        public static readonly string PW_CONF_NAME = "PW" + VERSION_IN_FILENAMES;
        public static readonly AppConfig COMMON_APP_CONF = AssetUtils.GetConfig(PW_CONF_NAME);

        public const string COMMON_MENU = "Procedural Worlds";

        public const int WELCOME_FREE_HOURS = 24;
    }
}
