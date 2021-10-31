using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetShaper : MonoBehaviour
{

    [SerializeField]
    MeshFilter meshFilter;
    Mesh mesh;
    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        meshFilter.mesh = mesh;
        GenerateMesh();
    }

    void GenerateMesh()
    {
        mesh.Clear();

        List<Vector3> verts = new List<Vector3>();
        List<int> indxs = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        List<Color> colours = new List<Color>();

        float width = 2;
        float widthHalf = width * 0.5f;
        int widthVerts = 5;
        float widthSpacer = width / (widthVerts - 1);

        float height = 2;
        float heightHalf = height * 0.5f;
        int heightVerts = 5;
        float heightSpacer = height / (heightVerts - 1);

        for (float y = -heightHalf; y <= heightHalf; y += heightSpacer)
        {
            for (float x = -widthHalf; x <= widthHalf; x += widthSpacer)
            {
                verts.Add(new Vector3(x, y, 0));
            }
        }

        for (int y = 0; y < heightVerts; ++y)
        {
            for (int x = 0; x < widthVerts; ++x)
            {
                uvs.Add(new Vector2(x / (widthVerts - 1.0f), y / (heightVerts - 1.0f)));
                colours.Add(new Color(uvs[uvs.Count-1].x, uvs[uvs.Count - 1].y, 0));

                if (x > 0 && y > 0)
                {
                    indxs.Add(OneDFromTwoD(x - 1, y - 0, widthVerts));
                    indxs.Add(OneDFromTwoD(x - 0, y - 1, widthVerts));
                    indxs.Add(OneDFromTwoD(x - 1, y - 1, widthVerts));

                    indxs.Add(OneDFromTwoD(x - 1, y - 0, widthVerts));
                    indxs.Add(OneDFromTwoD(x - 0, y - 0, widthVerts));
                    indxs.Add(OneDFromTwoD(x - 0, y - 1, widthVerts));
                }
            }
        }

        mesh.SetVertices(verts);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(indxs, 0);
        mesh.SetColors(colours);
        mesh.RecalculateNormals();
    }

    int OneDFromTwoD(int x, int y, int width)
    {
        return y * width + x;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
