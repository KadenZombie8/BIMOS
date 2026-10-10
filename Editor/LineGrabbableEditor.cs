using KadenZombie8.BIMOS.Rig.Grips;
using UnityEditor;

namespace KadenZombie8.BIMOS.Rig
{
    [CustomEditor(typeof(LineGrip), true)]
    public class LineGrabbableEditor : UnityEditor.Editor
    {
        public void OnSceneGUI()
        {
            var lineGrabbable = (LineGrip)target;
            var lineOrigin = lineGrabbable.Origin;

            if (!lineOrigin) return;
            
            var start = lineOrigin.position + lineOrigin.forward * lineGrabbable.Length / 2f;
            var end = lineOrigin.position - lineOrigin.forward * lineGrabbable.Length / 2f;

            Handles.DrawDottedLine(start, end, 4f);
        }
    }
}
