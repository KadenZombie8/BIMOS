using UnityEngine;

namespace KadenZombie8
{
    public static class Utilities
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
    }
}