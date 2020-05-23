// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.

namespace GaiaCommon1.Localization
{
    public class LocalizationItem
    {
        public string Key;
        public string Val = "";
        public string Tooltip = "";
        public string Help = "";
        public string Context = "";

        /// <summary>
        /// To String method that returns the name
        /// </summary>
        public override string ToString()
        {
            return Key;
        }
    }
}