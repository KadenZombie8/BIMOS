using KadenZombie8.BIMOS.Rig.Grips;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    [CustomEditor(typeof(GripHapticsHandler))]
    public class GrabbableHapticsHandlerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Find Grabbables"))
            {
                var grabHapticsHandler = (GripHapticsHandler)target;
                var transform = grabHapticsHandler.transform;

                Undo.RecordObject(grabHapticsHandler, "Find Grabbables");

                HashSet<Grip> grabbables = new(transform.GetComponentsInChildren<Grip>());

                var parent = transform.parent;
                if (parent)
                {
                    var parentArticulationBody = parent.GetComponentInParent<ArticulationBody>();
                    if (parentArticulationBody)
                    {
                        foreach (var grabbable in parentArticulationBody.GetComponentsInChildren<Grip>())
                            grabbables.Add(grabbable);
                    }
                }

                grabHapticsHandler.Grips = new Grip[grabbables.Count];
                grabbables.CopyTo(grabHapticsHandler.Grips);

                EditorUtility.SetDirty(grabHapticsHandler);
            }
        }
    }
}
