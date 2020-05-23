// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using UnityEngine;

namespace GaiaCommon1
{
    /// <summary>
    /// Unity's Editor and EditorWindow does not have a common base that has interfaces we need.
    /// Repaint() for example
    /// </summary>
    public interface IPWEditor
    {
        // The position of the window in screen space.
        Rect position { get; set; }
        bool maximized { get; set; }
        bool PositionChecked { get; set; }

        void Repaint();
    }
}
