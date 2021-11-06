using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetShaper : MonoBehaviour
{

    [SerializeField]
    Transform facePrefab;
    MeshFilter[] meshFilters = new MeshFilter[6];
    Mesh[] meshes = new Mesh[6];

    [SerializeField]
    int edgeVerts = 90;

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
        facePrefab.gameObject.SetActive(false);

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
        float radius = 1;

        int widthVerts = edgeVerts;

        int heightVerts = edgeVerts;

        for (int i = 0; i < 6; ++i)
        {
            List<Vector3> verts = new List<Vector3>();
            List<int> indxs = new List<int>();
            List<Vector2> uvs = new List<Vector2>();
            List<Color> colours = new List<Color>();

            Mesh mesh = meshes[i];
            Quaternion faceQuat = faceRotations[i];

            for (int y = 0; y < heightVerts; ++y)
            {
                float xAngle = 45.0f * (-1.0f + 2.0f * y / (heightVerts - 1));
                Vector3 xRot = Quaternion.Euler(xAngle, 0.0f, 0.0f) * Vector3.back;
                Vector3 newUp = Vector3.Cross(xRot, Vector3.right).normalized;

                for (int x = 0; x < widthVerts; ++x)
                {
                    Vector3 vertCircleNorm = (Quaternion.Euler(0, 90 - 45 * Mathf.Abs(2.0f * x - widthVerts + 1) / (widthVerts - 1), 0) * Vector3.back).normalized;

                    Vector3 meeting = Vector3.Cross(newUp, vertCircleNorm).normalized;
                    float alpha = Vector3.Angle(meeting, xRot);

                    verts.Add(faceQuat * (Quaternion.AngleAxis(alpha * Mathf.Sign(2.0f * x - widthVerts + 1), newUp) * xRot));
                    //verts.Add(faceQuat * new Vector3(
                    //    -1.0f + 2.0f * x / (widthVerts - 1),
                    //    -1.0f + 2.0f * y / (heightVerts - 1),
                    //    -1.0f).normalized);

                    uvs.Add(new Vector2(x / (widthVerts - 1.0f), y / (heightVerts - 1.0f)));

                    if (true)
                    {
                        colours.Add(new Color(uvs[uvs.Count - 1].x, uvs[uvs.Count - 1].y, 0));
                    }
                    else
                    {
                        colours.Add(Color.grey);
                    }

                    if (x > 0 && y > 0)
                    {
                        if ((x <= widthVerts / 2 && y <= heightVerts / 2) || (x > widthVerts / 2 && y > heightVerts / 2))
                        {
                            indxs.Add(OneDFromTwoD(x - 1, y - 0, widthVerts));
                            indxs.Add(OneDFromTwoD(x - 0, y - 1, widthVerts));
                            indxs.Add(OneDFromTwoD(x - 1, y - 1, widthVerts));

                            indxs.Add(OneDFromTwoD(x - 1, y - 0, widthVerts));
                            indxs.Add(OneDFromTwoD(x - 0, y - 0, widthVerts));
                            indxs.Add(OneDFromTwoD(x - 0, y - 1, widthVerts));
                        }
                        else
                        {
                            indxs.Add(OneDFromTwoD(x - 0, y - 0, widthVerts));
                            indxs.Add(OneDFromTwoD(x - 1, y - 1, widthVerts));
                            indxs.Add(OneDFromTwoD(x - 1, y - 0, widthVerts));

                            indxs.Add(OneDFromTwoD(x - 0, y - 0, widthVerts));
                            indxs.Add(OneDFromTwoD(x - 0, y - 1, widthVerts));
                            indxs.Add(OneDFromTwoD(x - 1, y - 1, widthVerts));
                        }
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
