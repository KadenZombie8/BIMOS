using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    [CustomEditor(typeof(GrabbableHapticsHandler))]
    public class GrabbableHapticsHandlerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Find Grabbables"))
            {
                var grabHapticsHandler = (GrabbableHapticsHandler)target;
                var transform = grabHapticsHandler.transform;

                Undo.RecordObject(grabHapticsHandler, "Find Grabbables");

                HashSet<Grabbable> grabbables = new(transform.GetComponentsInChildren<Grabbable>());

                var parent = transform.parent;
                if (parent)
                {
                    var parentArticulationBody = parent.GetComponentInParent<ArticulationBody>();
                    if (parentArticulationBody)
                    {
                        foreach (var grabbable in parentArticulationBody.GetComponentsInChildren<Grabbable>())
                            grabbables.Add(grabbable);
                    }
                }

                grabHapticsHandler.Grabbables = new Grabbable[grabbables.Count];
                grabbables.CopyTo(grabHapticsHandler.Grabbables);

                EditorUtility.SetDirty(grabHapticsHandler);
            }
        }
    }
}
