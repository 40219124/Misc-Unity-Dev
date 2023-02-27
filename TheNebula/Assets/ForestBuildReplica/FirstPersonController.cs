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
    Camera Eyes;

    bool _grounded = false;
    float TimeSinceGrounded = 0.0f;
    float CoyoteTime = 0.1f;
    float TimeSinceJump = 0.0f;
    float JumpCD = 0.3f;
    bool Grounded
    {
        get
        {
            return (TimeSinceGrounded < CoyoteTime) || _grounded;
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
        InputAsset = InputHouse.Instance.InputAsset;
        InputAsset.PlayerActive.Jump.started += DoJump;

        Body = GetComponent<Rigidbody>();

        Eyes = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        DoMovement(InputAsset.PlayerActive.Move.ReadValue<Vector2>());
        DoCamera(InputAsset.PlayerActive.Look.ReadValue<Vector2>());

        TimeSinceGrounded += Time.deltaTime;
        TimeSinceJump += Time.deltaTime;
    }

    void DoJump(InputAction.CallbackContext context)
    {
        if (!Grounded || (TimeSinceJump < JumpCD))
        {
            return;
        }
        Body.velocity += Vector3.up * JumpSpeed;
        TimeSinceJump = 0f;
    }

    void DoMovement(Vector2 wantedDir)
    {
        if (wantedDir.sqrMagnitude > 1)
        {
            wantedDir = wantedDir.normalized;
        }

        wantedDir = Quaternion.Euler(new Vector3(0f, 0f, -transform.eulerAngles.y)) * wantedDir;

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

    void DoCamera(Vector2 lookChangeVector)
    {
        transform.Rotate(Vector3.up, lookChangeVector.x * 120f * Time.deltaTime);

        Vector3 currentRot = Eyes.transform.localEulerAngles;
        currentRot.x += lookChangeVector.y * 90f * Time.deltaTime;
        float clampX = currentRot.x;
        if (clampX > 180)
        {
            clampX -= 360f;
        }
        clampX = Mathf.Clamp(clampX, -90f, 90f);
        currentRot.x = clampX;
        Eyes.transform.localEulerAngles = currentRot;
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
