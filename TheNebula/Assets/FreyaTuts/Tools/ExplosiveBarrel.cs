using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[ExecuteAlways]
public class ExplosiveBarrel : MonoBehaviour
{

    public BarrelType type;

    MaterialPropertyBlock mpb;
    static readonly int shPropColour = Shader.PropertyToID("_BaseColor");
    public MaterialPropertyBlock Mpb
    {
        get
        {
            if (mpb == null)
            {
                mpb = new MaterialPropertyBlock();
            }
            return mpb;
        }
    }

    void TryApplyColour()
    {
        if (type == null)
        {
            return;
        }
        MeshRenderer rnd = GetComponent<MeshRenderer>();
        Mpb.SetColor(shPropColour, type.colour);
        rnd.SetPropertyBlock(Mpb);
    }

    //private void Awake()
    //{
    //    //Shader shader = Shader.Find("Default/Diffuse");
    //    //Material mat = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };

    //    // Will duplicate the material (extra draw call, no longer batched)
    //    //GetComponent<MeshRenderer>().material.color = Color.red;

    //    // Will modify the asset
    //    //GetComponent<MeshRenderer>().sharedMaterial.color = Color.red;
    //}

    private void OnValidate()
    {
        TryApplyColour();
    }


    private void OnEnable()
    {
        TryApplyColour();
        ExplosiveBarrelManager.allTheBarrels.Add(this);
    }

    private void OnDisable() => ExplosiveBarrelManager.allTheBarrels.Remove(this);

    private void OnDrawGizmos()
    {
        if (type == null)
        {
            return;
        }
        //Gizmos.DrawWireSphere(transform.position, radius);
        Handles.color = type.colour;
        Handles.DrawWireDisc(transform.position, transform.up, type.radius);
        Handles.color = Color.white;
    }
}
