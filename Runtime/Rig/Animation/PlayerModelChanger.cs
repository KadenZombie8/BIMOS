using KadenZombie8.BIMOS.Rig;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace KadenZombie8.BIMOS.Editor
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(BIMOSRig))]
    public class PlayerModelChanger : MonoBehaviour
    {
        [SerializeField]
        private GameObject _modelPrefab;

        private BIMOSRig _player;

        public void ChangePlayerModel()
        {
            _player = GetComponent<BIMOSRig>();

            var newAvatar = _modelPrefab.GetComponent<Animator>().avatar;

            if (!newAvatar)
            {
                Debug.LogError("Character model must have an avatar");
                return;
            }

            if (!newAvatar.isHuman)
            {
                Debug.LogError("Character model's avatar must be humanoid");
                return;
            }

            int undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Change player model");

            UpdateRig(_player.AnimationRig.Transforms.Character, newAvatar);

            foreach (var renderer in _player.AnimationRig.Transforms.Character.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                Undo.DestroyObjectImmediate(renderer.gameObject);
            }

            UpdateRig(_player.AvatarRig.Character, newAvatar);

            foreach (var renderer in _player.AvatarRig.Character.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                Undo.RecordObject(renderer, "Update renderer");
                renderer.updateWhenOffscreen = true;
            }

            Undo.CollapseUndoOperations(undoGroup);
        }

        private void UpdateRig(Transform character, Avatar newAvatar)
        {
            var animator = character.GetComponent<Animator>();

            DestroyOldModel(character);

            Undo.RecordObject(animator, "Change animator avatar");
            animator.avatar = newAvatar;

            CopyModelChildren(character);
        }

        private void CopyModelChildren(Transform character)
        {
            var modelInstance = Instantiate(_modelPrefab);
            Undo.RegisterCreatedObjectUndo(modelInstance, "Created model instance");

            var children = new List<Transform>();

            foreach (Transform child in modelInstance.transform)
            {
                children.Add(child);
            }

            foreach (var child in children)
            {
                Undo.SetTransformParent(child, character, "Parent child to model");
                child.SetParent(character);
            }

            DestroyImmediate(modelInstance);
        }

        private void DestroyOldModel(Transform character)
        {
            List<Transform> characterChildren = new();
            foreach (Transform child in character)
                characterChildren.Add(child);

            foreach (var child in characterChildren)
            {
                if (child.GetComponent<UnityEngine.Animations.Rigging.Rig>()) continue;

                Undo.DestroyObjectImmediate(child.gameObject);
            }
        }
    }
}