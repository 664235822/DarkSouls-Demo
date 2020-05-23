using UnityEditor;

namespace Gaia
{
    //Automates removal of the gaia define
    public class GaiaRemovalEditor : UnityEditor.AssetModificationProcessor
    {
        public static AssetDeleteResult OnWillDeleteAsset(string AssetPath, RemoveAssetOptions rao)
        {
            if (AssetPath.Contains("Gaia"))
            {
                string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
                if (symbols.Contains("GAIA_PRESENT"))
                {
                    symbols = symbols.Replace("GAIA_PRESENT;", "");
                    symbols = symbols.Replace("GAIA_PRESENT", "");
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
                }
            }
            return AssetDeleteResult.DidNotDelete;
        }
    }
}