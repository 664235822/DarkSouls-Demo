// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace GaiaCommon1
{
    public class Dev
    {
        public static bool Present { get; private set; }

        private static Type m_devUtilsType;

        static Dev()
        {
            m_devUtilsType = Utils.GetType("PWDev.DevUtils");
            Present = m_devUtilsType != null;
        }

        public static void NoLocalizationPkg(string className, SystemLanguage lang1, SystemLanguage lang2)
        {
            if (!Present)
            {
                return;
            }

            m_devUtilsType.GetMethod("NoLocalizationPkg", BindingFlags.Public | BindingFlags.Static)
                .Invoke(null, new object[] { className, lang1, lang2 });
        }
    }
}
