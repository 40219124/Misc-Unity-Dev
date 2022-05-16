using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ExplosiveBarrelManager : MonoBehaviour
{

    public static List<ExplosiveBarrel> allTheBarrels = new List<ExplosiveBarrel>();

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
        foreach (ExplosiveBarrel barrel in allTheBarrels)
        {

            if(barrel.type == null)
            {
                continue;
            }

            Vector3 managerPos = transform.position;
            Vector3 barrelPos = barrel.transform.position;
            float halfHeigh = (managerPos.y - barrelPos.y) * 0.5f;
            Vector3 offset = Vector3.up * halfHeigh;


            Handles.DrawBezier(
                managerPos, barrelPos,
                managerPos - offset, barrelPos + offset,
                barrel.type.colour,
                EditorGUIUtility.whiteTexture,
                1f
                );

            // Handles.DrawAAPolyLine(transform.position, barrel.transform.position);

            //Gizmos.DrawLine(transform.position, barrel.transform.position);
        }
    }
#endif
}
