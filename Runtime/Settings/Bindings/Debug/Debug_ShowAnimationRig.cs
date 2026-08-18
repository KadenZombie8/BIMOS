using KadenZombie8.BIMOS.Settings.Bindings;
using System.Collections.Generic;
using UnityEngine;

namespace KadenZombie8.BIMOS
{
    public class Debug_ShowAnimationRig : SettingBinding<bool>
    {
        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private GameObject _trackerPrefab;

        private Transform _root;

        private readonly List<GameObject> _trackers = new();

        private void Start()
        {
            _root = _animator.GetBoneTransform(HumanBodyBones.Hips);
            SetAnimationRigVisible(Setting.Value);
        }

        protected override void SettingUpdated(bool value) => SetAnimationRigVisible(value);

        private void SetAnimationRigVisible(bool isVisible)
        {
            if (!_root) return;

            if (isVisible)
                AddTrackerToDescendants(_root);
            else
                DestroyAllTrackers();
        }

        private void AddTrackerToDescendants(Transform bone)
        {
            foreach (Transform child in bone)
            {
                AddTrackerToDescendants(child);
            }

            var tracker = Instantiate(_trackerPrefab, bone);
            _trackers.Add(tracker);
        }

        private void DestroyAllTrackers()
        {
            foreach (var tracker in _trackers)
            {
                Destroy(tracker);
            }

            _trackers.Clear();
        }
    }
}
