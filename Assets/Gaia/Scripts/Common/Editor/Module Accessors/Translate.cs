// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace GaiaCommon1
{
    public class Translate
    {
        public static bool Present { get; private set; }

        private static Type m_translationUtilsType;

        static Translate()
        {
            m_translationUtilsType = Utils.GetType("PWTranslation.TranslationUtils");
            Present = m_translationUtilsType != null;
        }

        public static void MissingLanguagePack(string className)
        {
            if (!Present)
            {
                return;
            }

            m_translationUtilsType.GetMethod("MissingLanguagePack", BindingFlags.Public | BindingFlags.Static)
                .Invoke(null, new object[] { className });
        }
    }
}
