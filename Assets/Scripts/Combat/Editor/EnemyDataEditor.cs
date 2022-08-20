using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
namespace RPG.Combat.Editor
{

    public class EnemyDataEditor : OdinMenuEditorWindow
    {
         const string PATH_ASSETS = "Assets/Game/Resources/Enemies/";
         private CreateNewEnemyData _createNewEnemyData;
        
        [MenuItem("Tools/Enemy Data")]
        private static void OpenWindow() 
        {
            GetWindow<EnemyDataEditor>().Show();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_createNewEnemyData != null)
                DestroyImmediate(_createNewEnemyData.enemyData);
            
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();
            tree.Selection.SupportsMultiSelect = false;

            _createNewEnemyData = new CreateNewEnemyData();
            tree.Add("Create New", _createNewEnemyData);
            tree.AddAllAssetsAtPath("Enemy Data", PATH_ASSETS, typeof(EnemyData));
            return tree;
        }

        public class CreateNewEnemyData
        {
            [InlineEditor(Expanded = true)]
            public EnemyData enemyData;
            
            public CreateNewEnemyData()
            {
                enemyData = ScriptableObject.CreateInstance<EnemyData>();
                enemyData.enemyName = "New Enemy Data";
            }


            [Button("Add New Enemy SO")]
            private void CreateNewData()
            {
                AssetDatabase.CreateAsset(enemyData, PATH_ASSETS + enemyData.enemyName + ".asset");
                AssetDatabase.SaveAssets();
                
                enemyData = ScriptableObject.CreateInstance<EnemyData>();
                enemyData.enemyName = "New Enemy Data";
            }
        }

        protected override void OnBeginDrawEditors()
        {
            OdinMenuTreeSelection selected = this.MenuTree.Selection;

            SirenixEditorGUI.BeginHorizontalToolbar();
            {
                GUILayout.FlexibleSpace();

                if (SirenixEditorGUI.ToolbarButton("Delete Current"))
                {
                    EnemyData asset = selected.SelectedValue as EnemyData;
                    string path = AssetDatabase.GetAssetPath(asset);
                    AssetDatabase.DeleteAsset(path);
                    AssetDatabase.SaveAssets();
                }

            }
            SirenixEditorGUI.EndHorizontalToolbar(); 
        }
    }

}