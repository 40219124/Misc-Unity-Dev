using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="FreyaTuts/BarrelType")]
public class BarrelType : ScriptableObject
{

    [Range(1f, 8f)]
    public float radius = 1;

    public float damage = 10;
    public Color colour = Color.red;
}