// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using UnityEngine;

namespace GaiaCommon1
{
    /// <summary>
    /// To create custom welcome content
    /// </summary>
    /// <remarks>
    /// Make sure to include the app name (as it exist in AppConfig) in the classname. For example 'GaiaWelcome'
    /// </remarks>
    [AddCustomWelcome]
    public abstract class CustomWelcome : IPWEditor
    {
        public abstract AppConfig AppConfig { get; }
        public abstract bool PositionChecked { get; set; }

        public abstract void WelcomeGUI();

        public System.Action repaintDelegate;

        // These don't do anything on a CustomWelcome but need the common interface for standalone editors.
        public Rect position { get; set; }
        public bool maximized { get; set; }

        public void Repaint()
        {
            if (repaintDelegate != null)
            {
                repaintDelegate.Invoke();
            }
        }
    }
}
