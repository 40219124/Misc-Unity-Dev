using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Extensions 
{
    public static Vector2 XZ(this Vector3 v) => new Vector2(v.x, v.z);
    public static Vector3 AsXZ(this Vector2 v) => new Vector3(v.x, 0, v.y);

    public static Vector3 FlatPerpendicular(this Vector3 v) => Vector3.Cross(v, Vector3.up);

    public static Vector3 NoY(this Vector3 v) => new Vector3(v.x, 0f, v.z);
}
