using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : MonoBehaviour
{
    ForestControls InputAsset;

    [SerializeField]
    float Acceleration = 40f;
    [SerializeField]
    float Speed = 5f;
    [SerializeField]
    float JumpSpeed = 10f;

    Rigidbody Body;

    bool _grounded = false;
    float TimeSinceGrounded = 0.0f;
    bool Grounded
    {
        get
        {
            return (TimeSinceGrounded < 0.1f) || _grounded;
        }
        set
        {
            _grounded = value;
            if (!_grounded)
            {
                TimeSinceGrounded = 0.0f;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InputAsset = new ForestControls();
        InputAsset.PlayerActive.Enable();
        InputAsset.PlayerActive.Jump.started += DoJump;

        Body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        DoMovement(InputAsset.PlayerActive.Move.ReadValue<Vector2>());

        TimeSinceGrounded += Time.deltaTime;
    }

    void DoJump(InputAction.CallbackContext context)
    {
        if (!Grounded)
        {
            return;
        }
        Body.velocity += Vector3.up * JumpSpeed;
    }

    void DoMovement(Vector2 wantedDir)
    {
        if (wantedDir.sqrMagnitude > 1)
        {
            wantedDir = wantedDir.normalized;
        }

        Vector3 currentVel3D = Body.velocity;
        Vector2 currVel = new Vector2(currentVel3D.x, currentVel3D.z);

        Vector2 desVel = wantedDir * Speed;

        Vector2 diffVel = desVel - currVel;
        Vector2 changeVel = diffVel.normalized * Acceleration * (Grounded ? 1.0f : 0.1f) * Time.deltaTime;
        if (changeVel.sqrMagnitude > diffVel.sqrMagnitude)
        {
            changeVel = diffVel;
        }

        Vector3 newVel3d = currentVel3D + new Vector3(changeVel.x, 0f, changeVel.y);
        Body.velocity = newVel3d;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            Grounded = false;
            foreach (var x in collision.contacts)
            {
                Grounded |= x.normal.y > 0.7f;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            Grounded = false;
        }
    }
}
