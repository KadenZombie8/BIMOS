using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace KadenZombie8.BIMOS.Ragdoll
{
    [CustomEditor(typeof(PhysicsRagdoll))]
    public class PhysicsRagdollEditor : Editor
    {
        public static Transform VirtualHip {
            get;set;
        }
        public static Transform PhysicsHip {
            get; set;
        }
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            EditorGUILayout.BeginVertical((GUIStyle)"HelpBox");
            GUILayout.Label("Setup Ragdoll", EditorStyles.largeLabel);
            VirtualHip = EditorGUILayout.ObjectField("Virtual Hip", VirtualHip, typeof(Transform), true) as Transform;
            PhysicsHip = EditorGUILayout.ObjectField("Physics Hip", PhysicsHip, typeof(Transform), true) as Transform;
            if(GUILayout.Button("Auto Assign Limbs")) {
                AutoAssignLimbs();
            }
            EditorGUILayout.EndVertical();
        }

        private void AutoAssignLimbs() {
            var limbs = PhysicsHip.GetComponentsInChildren<PhysicsLimb>();
            var virtuals = VirtualHip.GetComponentsInChildren<Transform>(true);
            foreach (var virtua in virtuals.ToList()) {
                var limb = limbs.ToList().Find(x => x.name.ToLower() == virtua.name.ToLower());
                if (limb)
                    limb.SetVirtualBone(virtua);
            }
            EditorUtility.SetDirty(PhysicsHip);
            AssetDatabase.SaveAssetIfDirty(PhysicsHip);
        }
    }

    [CustomEditor(typeof(PhysicsLimb))]
    public class PhysicsLimbEditor : Editor {
        public override void OnInspectorGUI() {
            var target = (PhysicsLimb)this.target;
            EditorGUILayout.BeginVertical((GUIStyle)"HelpBox");
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Ragdoll", target.GetComponentInParent<PhysicsRagdoll>(), typeof(PhysicsRagdoll), true);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
            base.OnInspectorGUI();
        }
    }
}
