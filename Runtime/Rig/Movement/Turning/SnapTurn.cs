using System.Collections;
using UnityEngine;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    /// <summary>
    /// Turn type with stepped rotation
    /// </summary>
    [RequireComponent(typeof(VirtualTurning))]
    public class SnapTurn : MonoBehaviour
    {
        [Tooltip("The angle (in degrees) the player turns when they move the turn stick horizontally")]
        public float TurnIncrement = 45;

        private VirtualTurning _virtualTurning;
        private ControllerRig _controllerRig;
        private bool _isTurning;

        private void OnEnable() => _virtualTurning.TurnEvent += Turn;

        private void OnDisable() => _virtualTurning.TurnEvent -= Turn;

        private void Awake()
        {
            _virtualTurning = GetComponent<VirtualTurning>();
            _controllerRig = _virtualTurning.ControllerRig;
        }

        private void Turn(float direction)
        {
            var turnVector = direction;
            var wasTurning = _isTurning;
            _isTurning = turnVector != 0f;

            if (wasTurning || !_isTurning)
                return;
            
            var turnDirection = turnVector / Mathf.Abs(turnVector);
            StartCoroutine(Snap(turnDirection));
        }

        private IEnumerator Snap(float turnDirection)
        {
            var degreesLeftToTurn = TurnIncrement;
            while (degreesLeftToTurn > 0f)
            {
                var degreesToTurn = Mathf.Min(degreesLeftToTurn, _virtualTurning.TurnRate * Time.deltaTime);
                degreesLeftToTurn -= degreesToTurn;
                _controllerRig.transform.Rotate(0f, degreesToTurn * turnDirection, 0f);
                yield return null;
            }
        }
    }
}