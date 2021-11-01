using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetShaper : MonoBehaviour
{

    [SerializeField]
    Transform facePrefab;
    MeshFilter[] meshFilters = new MeshFilter[6];
    Mesh[] meshes = new Mesh[6];

    Quaternion[] faceRotations = new Quaternion[6];
    enum eDirection { none = -1, right, up, forward, left, down, back }
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 6; ++i)
        {
            meshFilters[i] = Instantiate(facePrefab, transform).GetComponent<MeshFilter>();
            meshes[i] = new Mesh();
            meshFilters[i].mesh = meshes[i];
        }
        faceRotations[(int)eDirection.right] = Quaternion.FromToRotation(Vector3.back, Vector3.right);
        faceRotations[(int)eDirection.up] = Quaternion.FromToRotation(Vector3.back, Vector3.up);
        faceRotations[(int)eDirection.forward] = Quaternion.Euler(0, 180, 0);
        faceRotations[(int)eDirection.left] = Quaternion.FromToRotation(Vector3.back, Vector3.left);
        faceRotations[(int)eDirection.down] = Quaternion.FromToRotation(Vector3.back, Vector3.down);
        faceRotations[(int)eDirection.back] = Quaternion.FromToRotation(Vector3.back, Vector3.back);
        GenerateMesh();
    }

    void GenerateMesh()
    {
        float width = 2;
        float widthHalf = width * 0.5f;
        int widthVerts = 5;
        float widthSpacer = width / (widthVerts - 1);

        float height = 2;
        float heightHalf = height * 0.5f;
        int heightVerts = 5;
        float heightSpacer = height / (heightVerts - 1);

        for (int i = 0; i < 6; ++i)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> indxs = new List<int>();
            List<Vector2> uvs = new List<Vector2>();
            List<Color> colours = new List<Color>();

            Mesh mesh = meshes[i];
            Quaternion faceQuat = faceRotations[i];

            for (float y = -heightHalf; y <= heightHalf; y += heightSpacer)
            {
                for (float x = -widthHalf; x <= widthHalf; x += widthSpacer)
                {
                    //verts.Add((faceQuat * new Vector3(x, y, -1)).normalized);
                    //verts.Add(faceQuat * Quaternion.Euler(
                    //    Mathf.Cos(Mathf.PI * y * 0.25f / height) * 45 * x / widthHalf,
                    //    Mathf.Cos(Mathf.PI * x * 0.25f / width) * 45 * y / heightHalf,
                    //    0) *
                    //    Vector3.back);
                    Vector3 xRot = Quaternion.Euler(45.0f * y / heightHalf, 0.0f, 0.0f) * Vector3.back;
                    Vector3 newUp = Vector3.Cross(xRot, Vector3.right);
                    verts.Add(faceQuat *
                        (Quaternion.AngleAxis((Mathf.Asin(1/Mathf.Sqrt(3)) * Mathf.Rad2Deg) * x / widthHalf,
                            newUp) * xRot));
                }
            }



            for (int y = 0; y < heightVerts; ++y)
            {
                for (int x = 0; x < widthVerts; ++x)
                {
                    uvs.Add(new Vector2(x / (widthVerts - 1.0f), y / (heightVerts - 1.0f)));
                    colours.Add(new Color(uvs[uvs.Count - 1].x, uvs[uvs.Count - 1].y, 0));

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
