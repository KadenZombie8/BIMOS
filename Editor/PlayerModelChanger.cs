using System.Collections.Generic;
using KadenZombie8.BIMOS.Rig;
using UnityEditor;
using UnityEngine;

namespace KadenZombie8.BIMOS.Editor
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(BIMOSRig))]
    public class PlayerModelChanger : MonoBehaviour
    {
        [SerializeField]
        private GameObject _characterModel;

        private BIMOSRig _player;

        public void ChangePlayerModel()
        {
            _player = GetComponent<BIMOSRig>();

            Animator animator = _characterModel.GetComponent<Animator>();

            if (!animator.avatar)
            {
                Debug.LogError("Character model must have an avatar");
                return;
            }

            if (!animator.avatar.isHuman)
            {
                Debug.LogError("Character model's avatar must be humanoid");
                return;
            }

            Transform character = _player.AnimationRig.Transforms.Character;
            List<Transform> characterChildren = new();
            foreach (Transform child in character.transform)
                characterChildren.Add(child);

            foreach (Transform child in characterChildren)
                if (!child.GetComponent<UnityEngine.Animations.Rigging.Rig>())
                    DestroyImmediate(child.gameObject);

            _player.AnimationRig.Transforms.Character.GetComponent<Animator>().avatar = animator.avatar;

            GameObject characterModel = Instantiate(_characterModel);

            List<Transform> characterModelChildren = new();
            foreach (Transform child in characterModel.transform)
                characterModelChildren.Add(child);

            foreach (Transform child in characterModelChildren)
                child.parent = character;

            foreach (var renderer in character.GetComponentsInChildren<SkinnedMeshRenderer>())
                renderer.updateWhenOffscreen = true;

            DestroyImmediate(characterModel);
        }
    }

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