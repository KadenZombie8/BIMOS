using UnityEditor;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    [CustomEditor(typeof(BoxGrabbable))]
    public class BoxGrabbableEditor : UnityEditor.Editor
    {
        private Color _enabledColor = new(1f, 1f, 1f, 0f);
        private Color _disabledColor = new(1f, 0f, 0f, 0.05f);
        private Color _outlineColor = new(1f, 1f, 1f, 0f);

        private struct BoxCorners
        {
            public Vector3 FrontTopLeft;
            public Vector3 FrontTopRight;
            public Vector3 FrontBottomLeft;
            public Vector3 FrontBottomRight;
            public Vector3 BackTopLeft;
            public Vector3 BackTopRight;
            public Vector3 BackBottomLeft;
            public Vector3 BackBottomRight;
        }

        public void OnSceneGUI()
        {
            var boxGrabbable = (BoxGrabbable)target;
            var enabledFaces = boxGrabbable.EnabledFaces;

            var transform = boxGrabbable.transform;
            var scale = 0.5f * transform.localScale;
            var forward = transform.forward * scale.z;
            var right = transform.right * scale.x;
            var up = transform.up * scale.y;
            var corners = new BoxCorners()
            {
                FrontTopLeft = transform.position + forward + up - right,
                FrontTopRight = transform.position + forward + up + right,
                FrontBottomLeft = transform.position + forward - up - right,
                FrontBottomRight = transform.position + forward - up + right,
                BackTopLeft = transform.position + -forward + up - right,
                BackTopRight = transform.position + -forward + up + right,
                BackBottomLeft = transform.position + -forward - up - right,
                BackBottomRight = transform.position + -forward - up + right
            };

            Handles.DrawSolidRectangleWithOutline(new Vector3[]
            {
                corners.FrontTopLeft,
                corners.FrontTopRight,
                corners.FrontBottomRight,
                corners.FrontBottomLeft
            }, enabledFaces.HasFlag(BoxGrabbable.BoxFaces.Front) ? _enabledColor : _disabledColor, _outlineColor);

            Handles.DrawSolidRectangleWithOutline(new Vector3[]
            {
                corners.BackTopLeft,
                corners.BackTopRight,
                corners.BackBottomRight,
                corners.BackBottomLeft
            }, enabledFaces.HasFlag(BoxGrabbable.BoxFaces.Back) ? _enabledColor : _disabledColor, _outlineColor);

            Handles.DrawSolidRectangleWithOutline(new Vector3[]
            {
                corners.FrontTopRight,
                corners.BackTopRight,
                corners.BackBottomRight,
                corners.FrontBottomRight
            }, enabledFaces.HasFlag(BoxGrabbable.BoxFaces.Right) ? _enabledColor : _disabledColor, _outlineColor);

            Handles.DrawSolidRectangleWithOutline(new Vector3[]
            {
                corners.FrontTopLeft,
                corners.BackTopLeft,
                corners.BackBottomLeft,
                corners.FrontBottomLeft
            }, enabledFaces.HasFlag(BoxGrabbable.BoxFaces.Left) ? _enabledColor : _disabledColor, _outlineColor);

            Handles.DrawSolidRectangleWithOutline(new Vector3[]
            {
                corners.FrontTopLeft,
                corners.BackTopLeft,
                corners.BackTopRight,
                corners.FrontTopRight
            }, enabledFaces.HasFlag(BoxGrabbable.BoxFaces.Top) ? _enabledColor : _disabledColor, _outlineColor);

            Handles.DrawSolidRectangleWithOutline(new Vector3[]
            {
                corners.FrontBottomLeft,
                corners.BackBottomLeft,
                corners.BackBottomRight,
                corners.FrontBottomRight
            }, enabledFaces.HasFlag(BoxGrabbable.BoxFaces.Bottom) ? _enabledColor : _disabledColor, _outlineColor);
        }
    }
}
