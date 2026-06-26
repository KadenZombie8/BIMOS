using UnityEngine;

namespace KadenZombie8.BIMOS.Rig.Movement
{
    /// <summary>
    /// Turn type with continuous rotation
    /// </summary>
    [RequireComponent(typeof(VirtualTurning))]
    public class SmoothTurn : MonoBehaviour
    {
        private VirtualTurning _virtualTurning;
        private ControllerRig _controllerRig;
        private float _turnVector;

        private void OnEnable() => _virtualTurning.TurnEvent += OnTurn;

        private void OnDisable() => _virtualTurning.TurnEvent -= OnTurn;

        private void Awake()
        {
            _virtualTurning = GetComponent<VirtualTurning>();
            _controllerRig = _virtualTurning.ControllerRig;
        }

        private void OnTurn(float direction) => _turnVector = direction;

        private void Update()
        {
            if (_turnVector == 0f) return;
            var degreesToTurn = _virtualTurning.TurnRate * Time.deltaTime;
            _controllerRig.transform.Rotate(0f, degreesToTurn * _turnVector, 0f);
        }
    }
}