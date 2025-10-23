using UnityEditor;

namespace KadenZombie8.BIMOS.Rig
{
    [CustomEditor(typeof(LineGrabbable))]
    public class LineGrabbableEditor : UnityEditor.Editor
    {
        public void OnSceneGUI()
        {
            var lineGrabbable = (LineGrabbable)target;
            var lineOrigin = lineGrabbable.Origin;

            if (!lineOrigin) return;
            
            var start = lineOrigin.position + lineOrigin.up * lineGrabbable.Length / 2f;
            var end = lineOrigin.position - lineOrigin.up * lineGrabbable.Length / 2f;

            Handles.DrawDottedLine(start, end, 4f);
        }
    }
}
