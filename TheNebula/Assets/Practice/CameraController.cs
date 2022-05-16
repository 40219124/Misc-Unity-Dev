using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static Camera Camera;
    public float Speed = 10f;
    float Acceleration;
    float Deceleration;
    [SerializeField]
    public Transform Target;
    private Vector3 TargetVelocity = Vector3.zero;
    private Vector3 TargetLastPos;

    public Vector2 DeadZone = new Vector2(5f, 10f);
    public Vector2 StopFollowZone;
    public float MaxExcess = 2f;
    private float PermittedExcess = 0f;
    Vector3 Velocity = Vector3.zero;

    private void Awake()
    {
        Acceleration = Speed * 1.5f;
        Deceleration = Acceleration;
        StopFollowZone = new Vector2(DeadZone.x + 1, DeadZone.y - 1);
    }

    private void OnEnable()
    {
        Camera = GetComponent<Camera>();
    }

    private void OnDisable()
    {
        Camera = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        TargetLastPos = Target.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        TargetVelocity = (Target.position - TargetLastPos) / Time.fixedDeltaTime;
        TargetLastPos = Target.position;

        float targetDistance = (Target.position - transform.position).XZ().magnitude;
        Vector2 inputDir = Vector2.zero;
        float excess = 0;
        float maxSpeed = Speed;
        if (targetDistance < StopFollowZone.x
            || targetDistance > StopFollowZone.y)
        {
            if (targetDistance < DeadZone.x
                || targetDistance > DeadZone.y
                || PermittedExcess > 0f)
            {
                inputDir = (Target.position - transform.position).XZ();
                if (targetDistance < DeadZone.x)
                {
                    inputDir *= -1f;
                }
                if (inputDir.sqrMagnitude > 1f)
                {
                    inputDir = inputDir.normalized;
                }
            }
            if (targetDistance < StopFollowZone.x)
            {
                excess = Mathf.Abs(targetDistance - StopFollowZone.x);
            }
            else
            {
                excess = targetDistance - StopFollowZone.y;
            }
        }
        else if (PermittedExcess > 0f && Velocity == Vector3.zero)
        {

            PermittedExcess = 0f;
            //Velocity = Vector3.zero;
        }

        if (excess > 1f || Velocity != Vector3.zero)
        {
            PermittedExcess = Mathf.Max(PermittedExcess, 0.2f + 0.8f * Mathf.Clamp01(excess / 2f));
        }

        if (PermittedExcess > 0f)
        {
            maxSpeed = Mathf.Lerp(0f, TargetVelocity.magnitude, excess);
        }

        if (maxSpeed == 0f)
        {
            maxSpeed = excess;
        }

        Vector3 bodyVel = Velocity;
        Vector2 currVel = new Vector2(bodyVel.x, bodyVel.z);
        Vector2 desiredVel = inputDir * maxSpeed;

        //if (desiredVel != currVel)
        //{
        //    Vector2 changeVel = desiredVel - currVel;

        //    Vector2 accel = Acceleration * Time.fixedDeltaTime * changeVel.normalized;
        //    if (accel.sqrMagnitude > changeVel.sqrMagnitude)
        //    {
        //        accel = changeVel;
        //    }

        //    Velocity += accel.AsXZ();
        //}
        Velocity = desiredVel.AsXZ();

        transform.position += Velocity * Time.fixedDeltaTime;

        Quaternion targetRotation = Quaternion.LookRotation((Target.position - transform.position).normalized);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 90f * Time.fixedDeltaTime);
    }
}
