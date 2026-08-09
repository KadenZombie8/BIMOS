using UnityEngine;

namespace KadenZombie8
{
    public static class BodyUtilities
    {
        public static Transform GetBody(Transform current, out Rigidbody rigidbody, out ArticulationBody articulationBody)
        {
            rigidbody = null;
            articulationBody = null;
            Transform body = null;

            if (!current)
                return null;

            while (!body)
            {
                rigidbody = current.GetComponent<Rigidbody>();
                articulationBody = current.GetComponent<ArticulationBody>();

                if (rigidbody || articulationBody)
                    return current;

                current = current.parent;
            }
            return null;
        }

        public static void AddForceAtPosition(Component body, Vector3 force, Vector3 position, ForceMode mode)
        {
            switch (body)
            {
                case Rigidbody otherRigidbody:
                    otherRigidbody.AddForceAtPosition(force, position, mode);
                    break;
                case ArticulationBody otherArticulationBody:
                    otherArticulationBody.AddForceAtPosition(force, position, mode);
                    break;
            }
        }

        public static float GetMass(Component body)
        {
            return body switch
            {
                Rigidbody rigidbody => rigidbody.mass,
                ArticulationBody articulationBody => articulationBody.mass,
                _ => 0f,
            };
        }
    }
}