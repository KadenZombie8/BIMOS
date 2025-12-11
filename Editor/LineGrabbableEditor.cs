using UnityEditor;

namespace KadenZombie8.BIMOS.Rig
{
    [CustomEditor(typeof(LineGrabbable), true)]
    public class LineGrabbableEditor : UnityEditor.Editor
    {
        public void OnSceneGUI()
        {
            var lineGrabbable = (LineGrabbable)target;
            var lineOrigin = lineGrabbable.Origin;

            if (!lineOrigin) return;
            
            var start = lineOrigin.position + lineOrigin.forward * lineGrabbable.Length / 2f;
            var end = lineOrigin.position - lineOrigin.forward * lineGrabbable.Length / 2f;

            Handles.DrawDottedLine(start, end, 4f);
        }
    }
}
