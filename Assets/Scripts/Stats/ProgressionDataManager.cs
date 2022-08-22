using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace RPG.Stats
{
    public class ProgressionDataManager : OdinMenuEditorWindow
    {

        [MenuItem("Tools/Progression Data Manager")]
        private static void OpenEditor() => GetWindow<ProgressionDataManager>();
        

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();
            tree.AddAllAssetsAtPath(nameof(ProgressionCharacterClass), "Assets/", typeof(ProgressionCharacterClass), true, true);
            return tree;
        }
    }
}