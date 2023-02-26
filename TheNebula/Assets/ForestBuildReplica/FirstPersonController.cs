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

    Rigidbody Body;


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
    }

    void DoJump(InputAction.CallbackContext context)
    {
        transform.Translate(Vector3.up * 3f);
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
        Vector2 changeVel = diffVel.normalized * Acceleration * Time.deltaTime;
        if (changeVel.sqrMagnitude > diffVel.sqrMagnitude)
        {
            changeVel = diffVel;
        }

        Vector3 newVel3d = currentVel3D + new Vector3(changeVel.x, 0f, changeVel.y);
        Body.velocity = newVel3d;
    }
}
