using UnityEngine;
using System.Collections.Generic;

// Script initial de MiddleVR 2, juste la physique de la rotation a ete changee,
// Script de deplacement classique

[RequireComponent(typeof(CharacterController))]
public class SVRNavigationController : MonoBehaviour
{
    public float WalkSpeed = 1.0f;
    public float OriginalSpeed = 1.0f;
    public int SpeedMultiplier = 1;
    public float RotationSpeed = 100.0f;
    public float GravityMultiplier = 9.81f;
    public bool isFlying;
    private string MovementDirectionNodeName = "HandNode";
    private float HeadProtectionRadius = 0.2f;

    private GameObject _head;
    private GameObject _movementDirectionNode;
    private CharacterController _characterController;

    [HideInInspector] public CollisionFlags _collisionFlags;
    private Vector2 _input;
    private Vector3 _movement = Vector3.zero;
    private List<GameObject> ignoredObject = new List<GameObject>();

    private void Start()
    {
        OriginalSpeed = WalkSpeed;
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (_head == null)
        {
            _head = GameObject.Find("HeadNode");
            return;
        }

        _characterController.height = HeadProtectionRadius + (_head.transform.position.y - transform.position.y);

        Vector3 center = transform.InverseTransformVector(_head.transform.position - transform.position);
        center.y = _characterController.height / 2.0f;

        _characterController.center = center;

        // Follow ground
        _movement.y = 0f;
    }

    private void FixedUpdate()
    {
        float speed;
        GetInput(out speed);

        if (_movementDirectionNode == null)
        {
            _movementDirectionNode = GameObject.Find(MovementDirectionNodeName);
            return;
        }

        Vector3 movementForward = _movementDirectionNode.transform.forward;
        Vector3 movementRight = _movementDirectionNode.transform.right;

        // No fly for the moment
        if(!isFlying)
            movementForward.y = 0.0f;
        movementForward.Normalize();
        if (!isFlying)
            movementRight.y = 0.0f;
        movementRight.Normalize();

        //Vector3 desiredMove = movementForward * _input.y + movementRight * _input.x;
        Vector3 desiredMove = movementForward * _input.y;

        _movement.x = desiredMove.x * speed;
        _movement.y = desiredMove.y * speed;
        _movement.z = desiredMove.z * speed;


        if (!_characterController.isGrounded)
            _movement += Physics.gravity * GravityMultiplier * Time.fixedDeltaTime;

        _collisionFlags = _characterController.Move(_movement * Time.fixedDeltaTime);

        transform.RotateAround(_head.transform.position, Vector3.up, _input.x * RotationSpeed * Time.fixedDeltaTime);

        if(!isFlying && ignoredObject.Count > 0)
        {
            for(int i = 0; i < ignoredObject.Count; i++)
                Physics.IgnoreCollision(_characterController, ignoredObject[i].GetComponent<Collider>(), false);
            ignoredObject.Clear();
        }
    }

    private void GetInput(out float speed)
    {
        // Read input
        float horizontal = MiddleVR.VRDeviceMgr.GetWandHorizontalAxisValue();
        float vertical = MiddleVR.VRDeviceMgr.GetWandVerticalAxisValue();

        speed = WalkSpeed;
        _input = new Vector2(horizontal, vertical);

        // normalize input if it exceeds 1 in combined length:
        if (_input.sqrMagnitude > 1)
            _input.Normalize();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (isFlying)
        {
            Physics.IgnoreCollision(_characterController, hit.collider);
            ignoredObject.Add(hit.gameObject);
        }

        Rigidbody body = hit.collider.attachedRigidbody;

        if (_collisionFlags == CollisionFlags.Below )
            return;

        if (body == null || body.isKinematic)
            return;

        body.AddForceAtPosition(_characterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
    }

    public void increaseSpeed()
    {
        if (SpeedMultiplier == 1)
            SpeedMultiplier++;
        else
            SpeedMultiplier += 2;
        SetSpeed();
    }
    public void decreaseSpeed()
    {
        SpeedMultiplier-=2;
        SetSpeed();
    }

    private void SetSpeed()
    {
        switch (SpeedMultiplier)
        {
            case 1:
                WalkSpeed = OriginalSpeed;
                break;
            case 2:
                WalkSpeed = OriginalSpeed * SpeedMultiplier;
                break;
            case 4:
                WalkSpeed = OriginalSpeed * SpeedMultiplier;
                break;
            case 6:
                WalkSpeed = OriginalSpeed * SpeedMultiplier;
                break;
            case 8:
                WalkSpeed = OriginalSpeed * SpeedMultiplier;
                break;
            default:
                SpeedMultiplier = 1;
                break;
        }
    }
}
