using System;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using RPG.Utils;

namespace RPG.Inventories.Editor
{
    public class InventoryDataManager : OdinMenuEditorWindow
    {
        private static Type[] typesToDisplay = TypeCache.GetTypesWithAttribute<InventorynManageableDataAttribute>()
            .OrderBy(m => m.Name)
            .ToArray();

        private Type selectedType;

        [MenuItem("Tools/Inventory Data Manager")]
        private static void OpenEditor() => GetWindow<InventoryDataManager>();

        protected override void OnGUI()
        {
            //draw menu tree for SOs and other assets
            if (GUIUtils.SelectButtonList(ref selectedType, typesToDisplay))
                this.ForceMenuTreeRebuild();

            base.OnGUI();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();
            if(selectedType != null)
                tree.AddAllAssetsAtPath(selectedType.Name, "Assets/", selectedType, true, true);
            return tree;
        }
    }
}