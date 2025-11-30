#if UNITY_EDITOR
using KadenZombie8.BIMOS.Rig;
using UnityEditor;

public class CustomEditorForRig
{
    [CustomEditor(typeof(BIMOSRig))]
    public class BIMOSRigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var rig = (BIMOSRig)target;
        }
    }
}
#endif