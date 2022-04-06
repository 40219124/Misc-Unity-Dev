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

    [SerializeField]
    bool generateMesh = false;

    Quaternion[] faceRotations = new Quaternion[6]{Quaternion.FromToRotation(Vector3.back, Vector3.right),
        Quaternion.FromToRotation(Vector3.back, Vector3.up),
        Quaternion.Euler(0, 180, 0),
        Quaternion.FromToRotation(Vector3.back, Vector3.left),
        Quaternion.FromToRotation(Vector3.back, Vector3.down),
        Quaternion.FromToRotation(Vector3.back, Vector3.back)
    };
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

        GenerateSphereFaces();
    }

    void GenerateSphereFaces()
    {
        int widthVerts = edgeVerts;
        int heightVerts = edgeVerts;

        // Create default mesh on the Vector3.back side of the sphere
        {
            Mesh mesh = meshes[(int)eDirection.back];
            mesh.Clear();
            List<Vector3> verts = new List<Vector3>();
            List<int> indxs = new List<int>();
            List<Vector2> uvs = new List<Vector2>();
            List<Color> colours = new List<Color>();

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

                    verts.Add(Quaternion.AngleAxis(alpha * Mathf.Sign(2.0f * x - widthVerts + 1), newUp) * xRot);

                    uvs.Add(new Vector2(x / (widthVerts - 1.0f), y / (heightVerts - 1.0f)));

                    if (false)
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

        // Duplicate and rotate existing face to create other 5 faces
        for (int i = 0; i < 6; ++i)
        {
            if (i == (int)eDirection.back)
            {
                continue;
            }
            Mesh copyFromMesh = meshes[(int)eDirection.back];
            Mesh copyToMesh = meshes[i];
            copyToMesh.Clear();
            Quaternion faceQuat = faceRotations[i];

            copyToMesh.vertices = copyFromMesh.vertices;
            Vector3[] verts = copyToMesh.vertices;
            for (int v = 0; v < verts.Length; ++v)
            {
                verts[v] = faceQuat * verts[v];
            }
            copyToMesh.vertices = verts;

            copyToMesh.triangles = copyFromMesh.triangles;
            copyToMesh.uv = copyFromMesh.uv;
            copyToMesh.colors = copyFromMesh.colors;
            copyToMesh.RecalculateNormals();
        }


        List<Vector3>[] norms = new List<Vector3>[6];
        for (int i = 0; i < norms.Length; ++i)
        {
            norms[i] = new List<Vector3>();
            meshes[i].GetNormals(norms[i]);
        }

        {
            int x0y0 = 0;
            int x1y0 = edgeVerts - 1;
            int x0y1 = (edgeVerts - 1) * edgeVerts;
            int x1y1 = edgeVerts * edgeVerts - 1;

            // Edges
            for (int i = 0; i < edgeVerts; ++i)
            {
                int x0yi = i * edgeVerts;
                int x1yi = i * edgeVerts + x1y0;
                int xiy0 = i;
                int xjy0 = x1y0 - i;
                int xiy1 = x0y1 + i;
                int xjy1 = x1y1 - i;

                AverageNormals(ref norms, MakeDIList(eDirection.left, x1yi, eDirection.back, x0yi));
                AverageNormals(ref norms, MakeDIList(eDirection.back, x1yi, eDirection.right, x0yi));
                AverageNormals(ref norms, MakeDIList(eDirection.right, x1yi, eDirection.forward, x0yi));
                AverageNormals(ref norms, MakeDIList(eDirection.forward, x1yi, eDirection.left, x0yi));

                AverageNormals(ref norms, MakeDIList(eDirection.up, xiy0, eDirection.back, xiy1));
                AverageNormals(ref norms, MakeDIList(eDirection.up, x1yi, eDirection.right, xiy1));
                AverageNormals(ref norms, MakeDIList(eDirection.up, xiy1, eDirection.forward, xjy1));
                AverageNormals(ref norms, MakeDIList(eDirection.up, x0yi, eDirection.left, xjy1));

                AverageNormals(ref norms, MakeDIList(eDirection.down, xiy1, eDirection.back, xiy0));
                AverageNormals(ref norms, MakeDIList(eDirection.down, x1yi, eDirection.right, xjy0));
                AverageNormals(ref norms, MakeDIList(eDirection.down, xiy0, eDirection.forward, xjy0));
                AverageNormals(ref norms, MakeDIList(eDirection.down, x0yi, eDirection.left, xiy0));
            }

            // Corners
            AverageNormals(ref norms, MakeDIList(eDirection.up, x0y0, eDirection.left, x1y1, eDirection.back, x0y1));
            AverageNormals(ref norms, MakeDIList(eDirection.up, x1y0, eDirection.back, x1y1, eDirection.right, x0y1));
            AverageNormals(ref norms, MakeDIList(eDirection.up, x1y1, eDirection.right, x1y1, eDirection.forward, x0y1));
            AverageNormals(ref norms, MakeDIList(eDirection.up, x0y1, eDirection.forward, x1y1, eDirection.left, x0y1));

            AverageNormals(ref norms, MakeDIList(eDirection.down, x0y1, eDirection.left, x1y0, eDirection.back, x0y0));
            AverageNormals(ref norms, MakeDIList(eDirection.down, x1y1, eDirection.back, x1y0, eDirection.right, x0y0));
            AverageNormals(ref norms, MakeDIList(eDirection.down, x1y0, eDirection.right, x1y0, eDirection.forward, x0y0));
            AverageNormals(ref norms, MakeDIList(eDirection.down, x0y0, eDirection.forward, x1y0, eDirection.left, x0y0));
        }

        for (int i = 0; i < norms.Length; ++i)
        {
            meshes[i].SetNormals(norms[i]);
        }
    }

    List<KeyValuePair<eDirection, int>> MakeDIList(eDirection d1, int i1, eDirection d2, int i2, eDirection d3 = eDirection.none, int i3 = -1)
    {
        List<KeyValuePair<eDirection, int>> outList = new List<KeyValuePair<eDirection, int>>();
        outList.Add(new KeyValuePair<eDirection, int>(d1, i1));
        outList.Add(new KeyValuePair<eDirection, int>(d2, i2));
        if (d3 != eDirection.none)
        {
            outList.Add(new KeyValuePair<eDirection, int>(d3, i3));
        }
        return outList;
    }

    void AverageNormals(ref List<Vector3>[] norms, List<KeyValuePair<eDirection, int>> dirIndexes)
    {
        Vector3 average = Vector3.zero;
        foreach (var pair in dirIndexes)
        {
            average += norms[(int)pair.Key][pair.Value];
        }
        average = average.normalized;
        foreach (var pair in dirIndexes)
        {
            norms[(int)pair.Key][pair.Value] = average;
        }
    }

    eDirection RotateAroundFaces(eDirection from)
    {
        return from switch
        {
            eDirection.back => eDirection.right,
            eDirection.right => eDirection.forward,
            eDirection.forward => eDirection.left,
            eDirection.left => eDirection.back,
            _ => eDirection.none,
        };
        ;
    }

    int OneDFromTwoD(int x, int y, int width)
    {
        return y * width + x;
    }
    // Update is called once per frame
    void Update()
    {

    }

    private void OnValidate()
    {
        if (generateMesh)
        {
            generateMesh = false;
            GenerateSphereFaces();
        }
    }
}
