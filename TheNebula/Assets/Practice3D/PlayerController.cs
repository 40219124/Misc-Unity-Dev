using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed = 5f;
    float Acceleration;
    float Deceleration;

    public float JumpVelocity = 5f;

    private Vector2 inputDir = Vector2.zero;
    private bool SpacePushed = false;

    [SerializeField]
    Rigidbody Body;

    private void Awake()
    {
        Acceleration = Speed * 10f;
        Deceleration = Acceleration * 2f;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        InputUpdate();
    }

    private void InputUpdate()
    {
        inputDir.x = Input.GetAxisRaw("Horizontal");
        inputDir.y = Input.GetAxisRaw("Vertical");

        if (inputDir != Vector2.zero)
        {
            Vector3 camToPlayer = transform.position - CameraController.Camera.transform.position;
            camToPlayer.y = 0;
            inputDir = (Quaternion.FromToRotation(Vector3.forward, camToPlayer.normalized) * inputDir.AsXZ()).XZ();
        }

        if (!SpacePushed)
        {
            SpacePushed = Input.GetKeyDown(KeyCode.Space);
        }
    }

    private void FixedUpdate()
    {
        PhysicsInputResponse();
        GravityUpdate();
    }

    private void PhysicsInputResponse()
    {
        Vector3 bodyVel = Body.velocity;
        Vector2 currVel = new Vector2(bodyVel.x, bodyVel.z);
        Vector2 desiredVel = inputDir * Speed;
        if (desiredVel != currVel)
        {
            Vector2 changeVel = desiredVel - currVel;
            if (desiredVel != Vector2.zero)
            {
                transform.rotation = Quaternion.LookRotation(desiredVel.AsXZ(), Vector3.up);
            }

            Vector2 accel = changeVel.normalized * Time.fixedDeltaTime * (inputDir.Equals(Vector2.zero) ? Deceleration : Acceleration);
            if (accel.sqrMagnitude > changeVel.sqrMagnitude)
            {
                accel = changeVel;
            }

            Body.velocity += new Vector3(accel.x, 0, accel.y);
        }

        if (SpacePushed)
        {
            Body.velocity += Vector3.up * JumpVelocity;
            SpacePushed = false;
        }
    }

    private void GravityUpdate()
    {
        Vector3 gravity = Physics.gravity;
        if (Body.velocity.y < 0)
        {
            gravity *= 2f;
        }
        Body.AddForce(gravity * Body.mass);
        if(Body.velocity.y < -5f)
        {
            Body.velocity += Vector3.up * (-5f - Body.velocity.y);
        }
    }
}
