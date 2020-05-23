// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using UnityEngine;

namespace GaiaCommon1.Internal
{
    /// <summary>
    /// Internal and non internal got completely mixed up and will need clean up when we get time.
    /// </summary>sdfsf
    public static class PWConstInternal
    {
        public const string COMMON_MENU_FILENAME = "PWCommonMenuAG.cs";

        public const string COMMON_EDITOR_DLL_NAME_FORMAT = "CommEdt{0}.dll";
        public const string COMMON_RUNTIME_DLL_NAME_FORMAT = "CommRtm{0}.dll";

        public static readonly string COMMON_EDITOR_DLL_FILENAME = string.Format(COMMON_EDITOR_DLL_NAME_FORMAT, PWConst.VERSION);
        public static readonly string COMMON_RUNTIME_DLL_FILENAME = string.Format(COMMON_RUNTIME_DLL_NAME_FORMAT, PWConst.VERSION);

        public const string INACTIVE_COMMON_EXTENSION_FORMAT = ".pwc{0:00}";
        public static readonly string INACTIVE_COMMON_EXTENSION = string.Format(INACTIVE_COMMON_EXTENSION_FORMAT, PWConst.VERSION);

        /// <summary>
        /// The format of the Common Activator file name. Use it with string.Format and the NAMESPACE of the product.
        /// Example:
        ///     string.Format(PWConstInternal.COMMON_ACTIVATOR_NAME_FORMAT, appConfig.NameSpace);
        /// </summary>
        public static readonly string COMMON_ACTIVATOR_NAME_FORMAT = "{0}ComAct" + PWConst.VERSION.ToString() + ".dll";

        public static readonly string EDITOR_PATH_KEY = string.Format("{0}Common{1}EditorPath", Application.productName, PWConst.VERSION);
        public static readonly string RUNTIME_PATH_KEY = string.Format("{0}Common{1}RuntimePath", Application.productName, PWConst.VERSION);

        public static readonly string EDITOR_GUID_KEY = string.Format("{0}Common{1}EditorGUID", Application.productName, PWConst.VERSION);
        public static readonly string RUNTIME_GUID_KEY = string.Format("{0}Common{1}RuntimeGUID", Application.productName, PWConst.VERSION);
    }
}
