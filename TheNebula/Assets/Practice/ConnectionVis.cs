using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionVis : MonoBehaviour
{
    public class ConnectionNode
    {
        public Vector3 Position;
        public Vector3 Facing;

        public ConnectionNode(Vector3 pos, Vector3 face)
        {
            Position = pos;
            Facing = face;
        }
    }

    public class ConnectionCurve
    {
        public Vector3 Centre;
        public Vector3 StartPos;
        public Vector3 StartDir;
        public Vector3 EndPos;
        public Vector3 EndDir;
        public float EndAngle;
        public float RotationDirection; // 1 = clockwise, -1 = anticlockwise

        public ConnectionCurve(Vector3 centre, Vector3 sPos, Vector3 sDir)
        {
            Centre = centre;
            StartPos = sPos;
            StartDir = sDir;
        }
    }

    [SerializeField]
    Transform Player;
    [SerializeField]
    LineRenderer Line;
    [SerializeField]
    LineRenderer LineStartDir;
    [SerializeField]
    LineRenderer LineEndDir;
    [SerializeField]
    Transform StartCircleVis;
    [SerializeField]
    Transform EndCircleVis;
    [SerializeField]
    Transform StartMacVis;
    [SerializeField]
    Transform EndMacVis;


    [Range(0.1f, 360f)]
    public float CurveTheta = 50f;
    [Range(1f, 45f)]
    public float MaxDegreesPerLine = 10f;
    bool Visualising = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!Visualising && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(Visualise());
        }
    }

    IEnumerator Visualise()
    {
        Visualising = true;
        Line.startWidth = 0.4f;
        Line.endWidth = 0.05f;

        // --- Node creation ---
        ConnectionNode startNode = MakeConnectionNode();
        DrawFacingLine(startNode, LineStartDir, StartMacVis);
        ConnectionNode endNode = new ConnectionNode(Vector3.zero, Vector3.zero);
        {
            bool endSet = false;
            while (!endSet)
            {
                yield return null;
                if (Input.GetMouseButtonDown(0))
                {
                    endNode = MakeConnectionNode();
                    endSet = true;
                }
            }
        }
        DrawFacingLine(endNode, LineEndDir, EndMacVis);


        // --- Curve creation --- 
        ConnectionCurve startCurve, endCurve;
        MakeCurves(startNode, endNode, out startCurve, out endCurve);

        StartCircleVis.localPosition = startCurve.Centre;
        EndCircleVis.localPosition = endCurve.Centre;

        Vector3 centreDiff = (endCurve.Centre - startCurve.Centre);
        Vector3 cdPerp = centreDiff.Perpendicular().normalized;

        if (startCurve.RotationDirection != endCurve.RotationDirection)
        {
            // easy
            if (Vector3.Dot(startCurve.StartDir, cdPerp) < 0)
            {
                cdPerp *= -1;
            }

            startCurve.EndPos = endCurve.EndPos = cdPerp;

            startCurve.EndAngle = Vector3.Angle(startCurve.StartPos, startCurve.EndPos);
            if (Vector3.Dot(centreDiff.NoY(), startCurve.StartDir) < 0)
            {
                // startCurve.EndAngle = 180f - startCurve.EndAngle;
            }

            endCurve.EndAngle = Vector3.Angle(endCurve.StartPos, endCurve.EndPos);
            if (Vector3.Dot(-centreDiff.NoY(), endCurve.StartDir) < 0)
            {
                // endCurve.EndAngle = 180f - endCurve.EndAngle;
            }
        }
        else
        {
            // hard
            float startToDiff = Vector3.Angle(centreDiff, startCurve.StartPos);
            if (Vector3.Dot(startCurve.StartDir, centreDiff) < 0)
            {
                startToDiff = 360f - startToDiff;
            }
            float triAngle = Mathf.Rad2Deg * Mathf.Acos((2f * 1) / centreDiff.magnitude);
            startCurve.EndAngle = startToDiff - triAngle;
            startCurve.EndPos = Quaternion.Euler(0f, startCurve.EndAngle *
                startCurve.RotationDirection, 0f) * startCurve.StartPos;

            startToDiff = Vector3.Angle(-centreDiff, endCurve.StartPos);
            if (Vector3.Dot(endCurve.StartDir, -centreDiff) < 0)
            {
                startToDiff = 360f - startToDiff;
            }
            triAngle = Mathf.Rad2Deg * Mathf.Acos((2f * 1) / centreDiff.magnitude);
            endCurve.EndAngle = startToDiff - triAngle;
            endCurve.EndPos = Quaternion.Euler(0f, endCurve.EndAngle *
                endCurve.RotationDirection, 0f) * endCurve.StartPos;
        }

        // --- Line making ---
        List<Vector3> points = new List<Vector3>();

        int curvePoints = 1 + Mathf.CeilToInt(Mathf.Abs(startCurve.EndAngle) / MaxDegreesPerLine);
        float curveRotationIncrements = startCurve.EndAngle / (curvePoints - 1);
        points.Add(startCurve.Centre + startCurve.StartPos);
        for (int i = 1; i < curvePoints; ++i)
        {
            Quaternion rot = Quaternion.Euler(0f, curveRotationIncrements * i * startCurve.RotationDirection, 0f);
            points.Add(startCurve.Centre + rot * startCurve.StartPos);
        }

        curvePoints = 1 + Mathf.CeilToInt(Mathf.Abs(endCurve.EndAngle) / MaxDegreesPerLine);
        curveRotationIncrements = endCurve.EndAngle / (curvePoints - 1);
        points.Add(endCurve.Centre + (curvePoints == 1 ? endCurve.StartPos : endCurve.EndPos));
        for (int i = 1; i < curvePoints; ++i)
        {
            Quaternion rot = Quaternion.Euler(0f, curveRotationIncrements * (curvePoints - i - 1) * endCurve.RotationDirection, 0f);
            points.Add(endCurve.Centre + rot * endCurve.StartPos);
        }




        // --- Line value setting --- 
        Line.positionCount = points.Count;
        Line.SetPositions(points.ToArray());



        // *** Curve proof of concept ***
        #region
        //float maxDeg = 10f;
        //int curvePoints = 1 + Mathf.CeilToInt(Mathf.Abs(CurveTheta) / maxDeg);
        //float curveRotationIncrements = CurveTheta / (curvePoints - 1);

        //List<Vector3> points = new List<Vector3>();
        //Vector3 circleCentre = startNode.Position + Vector3.Cross(startNode.Facing, Vector3.up).normalized;
        //Vector3 centreToEdge = startNode.Position - circleCentre;

        //for(int i = 0; i < curvePoints; ++i)
        //{
        //    Quaternion rot = Quaternion.Euler(0f, -curveRotationIncrements * i, 0f);
        //    points.Add(circleCentre + rot * centreToEdge);
        //}

        //Line.positionCount = points.Count;
        //Line.SetPositions(points.ToArray());
        #endregion

        yield return null;


        Visualising = false;
    }

    ConnectionNode MakeConnectionNode()
    {
        Vector3 startPos = Player.position + Player.forward.NoY().normalized;
        Vector3 startFace = -Player.forward.NoY().normalized;
        return new ConnectionNode(startPos, startFace);
    }

    void MakeCurves(ConnectionNode startNode, ConnectionNode endNode,
        out ConnectionCurve startCurve, out ConnectionCurve endCurve)
    {
        Vector3 nodeDiff = endNode.Position - startNode.Position;
        float ndLenght2 = nodeDiff.sqrMagnitude;

        float startToEndDirL2 = ((endNode.Position + endNode.Facing) - startNode.Position).sqrMagnitude;
        float endToStartDirL2 = ((startNode.Position + startNode.Facing) - endNode.Position).sqrMagnitude;
        Vector3 startCentre = FindBestCentre(startNode, (endNode.Position + endNode.Facing), startToEndDirL2);
        Vector3 endCentre = FindBestCentre(endNode, (startNode.Position + startNode.Facing), endToStartDirL2);

        startCurve = new ConnectionCurve(startCentre, (startNode.Position - startCentre).NoY().normalized, startNode.Facing);
        startCurve.RotationDirection = -Mathf.Sign(Vector3.Cross(startCurve.StartDir, startCurve.StartPos).y);
        endCurve = new ConnectionCurve(endCentre, (endNode.Position - endCentre).NoY().normalized, endNode.Facing);
        endCurve.RotationDirection = -Mathf.Sign(Vector3.Cross(endCurve.StartDir, endCurve.StartPos).y);
    }

    Vector3 FindBestCentre(ConnectionNode node, Vector3 otherPos, float sqrDist)
    {
        Vector3 perp = node.Facing.Perpendicular();
        Vector3 centre = node.Position + perp;
        float dist1 = (otherPos - centre).sqrMagnitude;
        float dist2 = (otherPos - (node.Position - perp)).sqrMagnitude;
        Debug.Log($"1: {dist1}, 2: {dist2}");
        if (dist1 > sqrDist && dist1 > dist2)
        {
            perp *= -1;
            centre = node.Position + perp;
        }
        return centre;
    }

    void DrawFacingLine(ConnectionNode node, LineRenderer line, Transform mac)
    {
        line.startWidth = 0.2f;
        line.endWidth = 0;
        line.positionCount = 2;
        line.SetPosition(0, node.Position);
        line.SetPosition(1, node.Position + node.Facing);

        mac.localPosition = node.Position;
        mac.rotation = Quaternion.LookRotation(node.Facing, Vector3.up);
    }
}
