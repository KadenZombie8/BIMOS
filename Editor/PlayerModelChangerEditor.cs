using UnityEditor;
using UnityEngine;

namespace KadenZombie8.BIMOS.Editor
{
    [CustomEditor(typeof(PlayerModelChanger))]
    class PlayerModelChangerEditor : UnityEditor.Editor
    {
        private PlayerModelChanger _target;
        private SerializedProperty _characterModel;

        private void OnEnable()
        {
            _characterModel = serializedObject.FindProperty("_characterModel");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            _target = (PlayerModelChanger)target;
            EditorGUILayout.PropertyField(_characterModel);

            if (GUILayout.Button("Set Character Model"))
                _target.ChangePlayerModel();

            serializedObject.ApplyModifiedProperties();

        }
    }
}
