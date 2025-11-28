using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    public class TeleportLocomotion : MonoBehaviour
    {
        private BIMOSRig _player;

        private bool _isTeleporting; //Whether the player is teleporting or not

        private LayerMask _mask;
        private float _gravityConstant = 0.1f, _resolution = 0.5f;
        private int _maxIterations = 20;

        [SerializeField]
        private Transform _teleportFeetTransform; //The transform for the feet

        private LineRenderer _lineRenderer; //The line that connects the hand and feet
        private MeshRenderer _teleportFeetRenderer; //The renderer for the feet

        [SerializeField]
        private Gradient _successGradient, _failGradient; //The gradient shown when a teleport is/isn't possible

        private void Awake()
        {
            _player = GetComponentInParent<BIMOSRig>();
        }

        private void Start()
        {
            _mask = ~LayerMask.GetMask("BIMOSRig");
            _lineRenderer = _teleportFeetTransform.GetComponent<LineRenderer>();
            _teleportFeetRenderer = _teleportFeetTransform.Find("Feet").GetComponent<MeshRenderer>();
        }

        private void OnDisable() //Reset important variables
        {
            _isTeleporting = false;
            _teleportFeetRenderer.enabled = false;
            _lineRenderer.enabled = false;
        }

        private void FixedUpdate()
        {
            _player.PhysicsRig.Rigidbodies.LocomotionSphere.angularVelocity = Vector3.zero;
        }

        private void Update()
        {
            bool wasTeleporting = _isTeleporting;

            //if (wasTeleporting)
            //    _isTeleporting = _player.ControllerRig.InputReader.MoveVector.magnitude > 0.25f;
            //else
            //    _isTeleporting = _player.ControllerRig.InputReader.MoveVector.magnitude > 0.5f;

            _lineRenderer.enabled = _isTeleporting;

            if (_isTeleporting)
                CalculateCurve();

            if (wasTeleporting && !_isTeleporting)
                Teleport();
        }

        private void CalculateCurve()
        {
            RaycastHit hitData = new RaycastHit();
            Vector3 rayStart = _player.ControllerRig.Transforms.LeftPalm.position; //The start of the mini ray
            Vector3 rayEnd; //The end of the mini ray
            Vector3 rayVelocity = _player.ControllerRig.Transforms.LeftPalm.up * _resolution; //The velocity when the ray starts

            bool hitObject = false;
            bool hitFloor = false;
            _lineRenderer.positionCount = 0;
            for (int i = 0; i < _maxIterations; i++)
            {
                Ray ray = new(rayStart, rayVelocity);
                rayEnd = rayStart + rayVelocity;
                _lineRenderer.positionCount++;
                _lineRenderer.SetPosition(i, rayStart);

                //Fail - Off a large ledge?
                float verticalBoundary = _player.PhysicsRig.Rigidbodies.LocomotionSphere.position.y - 0.2f - 4f;
                if (rayEnd.y < verticalBoundary)
                {
                    hitData.point = Vector3.Lerp(rayEnd, rayStart, (verticalBoundary - rayEnd.y) / (rayStart.y - rayEnd.y));
                    hitObject = true; //Hit edge of vertical displacement
                    break;
                }

                //Hit - Reached something!
                if (Physics.Raycast(ray, out hitData, rayVelocity.magnitude, _mask, QueryTriggerInteraction.Ignore))
                {
                    hitObject = true;
                    //Success criterion: is it a floor? Use hit angle with normal maybe
                    if (Vector3.Angle(hitData.normal, Vector3.up) < 50)
                    {
                        hitFloor = true;
                    }
                    break;
                }

                //Prepare for next ray
                rayVelocity += new Vector3(0, -_gravityConstant, 0);
                rayStart = rayEnd;
            }

            if (hitObject)
            {
                _lineRenderer.positionCount++;
                _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, hitData.point);
            }
            if (hitFloor)
            {
                _teleportFeetRenderer.enabled = true;

                //Draw success ray
                _lineRenderer.colorGradient = _successGradient;
                _teleportFeetTransform.position = hitData.point + hitData.normal * 0.2f;

                Quaternion headYaw = Quaternion.LookRotation(Vector3.Cross(_player.ControllerRig.Transforms.Camera.right, Vector3.up)); //Gets player direction
                _teleportFeetTransform.rotation = headYaw;
            }
            else
            {
                //Draw fail ray
                _lineRenderer.colorGradient = _failGradient;
                _teleportFeetRenderer.enabled = false;
            }
        }

        private void Teleport()
        {
            if (_teleportFeetRenderer.enabled)
            {
                Rigidbody[] rigidbodies = transform.GetComponentsInChildren<Rigidbody>();
                Vector3 rootPosition = _player.PhysicsRig.Rigidbodies.LocomotionSphere.position;
                foreach (Rigidbody rigidbody in rigidbodies)
                {
                    Vector3 offset = rigidbody.position - rootPosition; //Calculates the offset between the locoball and the rigidbody
                    rigidbody.position = _teleportFeetTransform.position + offset; //Sets the rigidbody's position
                    rigidbody.transform.position = _teleportFeetTransform.position + offset; //Sets the transform's position
                }

                _teleportFeetRenderer.enabled = false;
                _lineRenderer.enabled = false;

                //Update the animation rig's position
                _player.AnimationRig.Transforms.Hips.position += _teleportFeetTransform.position - rootPosition;

                //Move the player's animated feet to the new position
                _player.AnimationRig.Feet.TeleportFeet();
            }
        }
    }
}