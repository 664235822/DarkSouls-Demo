// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using UnityEditor;
using GaiaCommon1;

namespace Gaia.Internal
{
    [InitializeOnLoad]
    public static class PWApp
    {
        private const string CONF_NAME = "Gaia";

        //old way:

        //public static readonly AppConfig CONF;
        //static PWApp()
        //{
        //    CONF = AssetUtils.GetConfig(CONF_NAME, true);
        //    if (CONF != null)
        //    {
        //        Prod.Checkin(CONF);
        //    }
        //}

        //new way:

        private static AppConfig conf;
        public static AppConfig CONF { get
                                            {
                                                if (conf != null)
                                                {
                                                    return conf;
                                                }
                                                else
                                                {
                                                    conf = AssetUtils.GetConfig(CONF_NAME, true);
                                                    if (conf != null)
                                                    {
                                                        Prod.Checkin(conf);
                                                        return conf;
                                                    }
                                                    else
                                                    {
                                                        return null;
                                                    }
                                                }
                                            } }
        static PWApp()
        {
            conf = AssetUtils.GetConfig(CONF_NAME,true);
            if (conf != null)
            {
                Prod.Checkin(conf);
            }
        }

        /// <summary>
        /// Get an editor utils object that can be used for common Editor stuff - DO make sure to Dispose() the instance.
        /// </summary>
        /// <param name="editorObj">The class that uses the utils. Just pass in "this".</param>
        /// <param name="customUpdateMethod">(Optional) The method to be called when the GUI needs to be updated. (Repaint will always be called.)</param>
        /// <returns>Editor Utils</returns>
        public static EditorUtils GetEditorUtils(IPWEditor editorObj, System.Action customUpdateMethod = null)
        {
            return new EditorUtils(CONF, editorObj, customUpdateMethod);
        }
    }
}
