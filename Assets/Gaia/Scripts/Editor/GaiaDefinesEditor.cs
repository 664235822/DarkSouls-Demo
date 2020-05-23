using UnityEditor;

namespace Gaia
{
    /// <summary>
    /// Injects GAIA_PRESENT define into project
    /// </summary>
    [InitializeOnLoad]
    public class GaiaDefinesEditor : Editor
    {
        static GaiaDefinesEditor()
        {
            //Make sure we inject GAIA_PRESENT
            var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (!symbols.Contains("GAIA_PRESENT"))
            {
                symbols += ";" + "GAIA_PRESENT";
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
            }
        }
    }
}

