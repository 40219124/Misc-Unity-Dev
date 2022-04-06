using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTFacing : MonoBehaviour
{
    public Rigidbody rb;
    public Transform targetObject;
    public bool hardCoded = false;
    public void FixedUpdate()
    {
        Vector3 lookDirection;
        if (hardCoded)
        {
            lookDirection = Quaternion.FromToRotation(rb.transform.forward, (Vector3.zero - rb.transform.position).normalized).eulerAngles;
        }
        else
        {
            lookDirection = Quaternion.FromToRotation(rb.transform.forward, (targetObject.position - rb.transform.position).normalized).eulerAngles;
        }
        float[] lds = new float[] { lookDirection.x, lookDirection.y, lookDirection.z };
        for (int i = 0; i < lds.Length; ++i)
        {
            if (lds[i] > 180f)
            {
                lds[i] -= 360f;
            }
        }
        lookDirection = new Vector3(lds[0], lds[1], lds[2]);
        rb.angularVelocity = lookDirection * Mathf.Deg2Rad;
    }
}
