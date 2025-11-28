#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace KadenZombie8.BIMOS.SDK {
    [CustomEditor(typeof(BIMOSEntity), true), CanEditMultipleObjects]
    public class BIMOSEntityEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI() {
            var target = (BIMOSEntity)this.target;
            EditorGUILayout.BeginVertical((GUIStyle)"HelpBox");
            GUILayout.Label("BIMOS Entity", EditorStyles.largeLabel);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.IntField($"Pool ID [1 .. {ushort.MaxValue}]", target.PoolId);
            EditorGUI.EndDisabledGroup();
            target.Poolee = EditorGUILayout.Toggle($"Poolee", target.Poolee);
            if (GUILayout.Button("Validate")) {
                foreach (var m_target in targets) {
                    EditorUtility.SetDirty(m_target);
                    var entity = (BIMOSEntity)m_target;
                    entity.Bodies.Clear();
                    foreach (var body in entity.GetComponentsInChildren<Rigidbody>()) {
                        entity.Bodies.Add(body.AddOrGetComponent<BIMOSBody>());
                        AssetDatabase.SaveAssetIfDirty(body);
                    }
                    foreach (var body in entity.GetComponentsInChildren<ArticulationBody>()) {
                        EditorUtility.SetDirty(body);
                        entity.Bodies.Add(body.AddOrGetComponent<BIMOSBody>());
                    }
                    AssetDatabase.SaveAssetIfDirty(m_target);
                }
            }
            EditorGUILayout.EndVertical();
            base.OnInspectorGUI();
        }
    }

    [CustomEditor(typeof(BIMOSBody), true), CanEditMultipleObjects]
    public class BIMOSBodyEditor : UnityEditor.Editor {
        public Vector3 force;
        public ForceMode forceMode;
        public override void OnInspectorGUI() {
            var target = (BIMOSBody)this.target;
            if (!target.HasPhysicsBody) {
                target.Rigidbody = target.GetComponent<Rigidbody>();
                target.ArticulationBody = target.GetComponent<ArticulationBody>();
                if (!target.HasPhysicsBody)
                    target.Rigidbody = target.AddComponent<Rigidbody>();
            }
            EditorGUILayout.BeginVertical((GUIStyle)"HelpBox");
            GUILayout.Label("BIMOS Body", EditorStyles.boldLabel);
            target.UseGravity = EditorGUILayout.Toggle($"Use Gravity", target.UseGravity);
            target.Immovable = EditorGUILayout.Toggle($"Immovable", target.Immovable);
            target.Velocity = EditorGUILayout.Vector3Field($"Velocity", target.Velocity);
            target.AngularVelocity = EditorGUILayout.Vector3Field($"Angular Velocity", target.AngularVelocity);
            DisabledGroup(target);
            EditorGUILayout.EndVertical();
            base.OnInspectorGUI();
        }

        private void DisabledGroup(BIMOSBody target) {
            EditorGUI.BeginDisabledGroup(!Application.IsPlaying(target));
            AddForceEditor(target);
            EditorGUI.EndDisabledGroup();
        }

        private void AddForceEditor(BIMOSBody target) {
            GUILayout.BeginVertical((GUIStyle)"HelpBox");
            GUILayout.Label("Force Editor", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Force"))
                target.AddForce(force, forceMode);
            if (GUILayout.Button("Add Torque"))
                target.AddTorque(force, forceMode);
            GUILayout.EndHorizontal();
            force = EditorGUILayout.Vector3Field("Force", force);
            forceMode = (ForceMode)EditorGUILayout.EnumPopup("Force Mode", forceMode);
            GUILayout.EndVertical();
        }
    }
}
#endif