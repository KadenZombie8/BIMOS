#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using KadenZombie8.BIMOS.Rig;
using KadenZombie8.BIMOS;
using KadenZombie8.BIMOS.Networking;

namespace BIMOS.Editor
{
    [CustomEditor(typeof(Grabbable), true), CanEditMultipleObjects]
    public class GripEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI () {
            base.OnInspectorGUI ();
            foreach (var target in targets) {
                var grabbable = (Grabbable)target;
                bool hasNetworkGrip = grabbable.GetComponent<GrabbableView>();
                if (!hasNetworkGrip) {
                    if (GUILayout.Button("Add Network Grip")) {
                        grabbable.AddComponent<GrabbableView>();
                        EditorUtility.SetDirty(target);
                        AssetDatabase.SaveAssetIfDirty(target);
                    }
                }
            }
        }
    }
}
#endif