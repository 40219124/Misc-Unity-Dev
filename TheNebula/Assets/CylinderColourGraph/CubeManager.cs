using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    [SerializeField]
    Transform CubePrefab;
    [SerializeField]
    int CubeCount = 21;
    [SerializeField]
    float Radius = 5.0f;

    List<GameObject> Cubes = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        GenerateCubes();    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GenerateCubes()
    {
        foreach(GameObject go in Cubes)
        {
            Destroy(go);
        }
        Cubes.Clear();

        for(int i = 0; i < CubeCount; ++i)
        {
            Vector3 shapePos = new Vector3(Mathf.Sin(Mathf.PI * 2.0f * i / CubeCount), 0.0f, Mathf.Cos(Mathf.PI * 2.0f * i / CubeCount));
            shapePos *= Radius;
            Quaternion shapeRot = Quaternion.AngleAxis(360.0f * i / CubeCount, Vector3.up);
            ColourShape shape = Instantiate(CubePrefab, shapePos, shapeRot, transform).GetComponent<ColourShape>();
            shape.SetValues(i, CubeCount);
        }
    }
}
