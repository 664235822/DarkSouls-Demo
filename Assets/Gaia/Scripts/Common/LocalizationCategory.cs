// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using System.Collections.Generic;

namespace GaiaCommon1.Localization
{
    public class LocalizationCategory
    {
        public string Name;
        public List<LocalizationItem> Items;

        public LocalizationCategory(string name)
        {
            Name = name;
            Items = new List<LocalizationItem>();
        }

        /// <summary>
        /// To String method that returns the name
        /// </summary>
        public override string ToString()
        {
            return Name;
        }
    }
}