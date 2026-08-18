using UnityEditor;
using UnityEngine;

namespace KadenZombie8.BIMOS.Editor
{
    [CustomEditor(typeof(PlayerModelChanger))]
    class PlayerModelChangerEditor : UnityEditor.Editor
    {
        private PlayerModelChanger _target;
        private SerializedProperty _modelPrefab;

        private void OnEnable()
        {
            _modelPrefab = serializedObject.FindProperty("_modelPrefab");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            _target = (PlayerModelChanger)target;
            EditorGUILayout.PropertyField(_modelPrefab);

            if (GUILayout.Button("Set Character Model"))
                _target.ChangePlayerModel();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
