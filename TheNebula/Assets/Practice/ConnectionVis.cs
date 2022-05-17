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

        public ConnectionCurve MakeConnectionCurve(ConnectionCurve.eRotationDirection wind, float radius = 1f)
        {
            Vector3 perpDir = Facing.FlatPerpendicular().normalized;
            if (wind == ConnectionCurve.eRotationDirection.clockwise)
            {
                perpDir *= -1f;
            }

            Vector3 centre = Position + perpDir * radius;
            Vector3 toNode = Position - centre;

            return new ConnectionCurve(centre, toNode, Facing);
        }
    }

    public class ConnectionCurve
    {
        public enum eRotationDirection { none = -1, clockwise, anticlockwise }

        public Vector3 Centre;
        public Vector3 StartPos;
        public Vector3 StartDir;
        public Vector3 EndPos;
        public Vector3 EndDir;
        public float EndAngle;

        public Vector3[] PotentialEnds = new Vector3[2];
        public Vector3[] PotentialFacing = new Vector3[2];
        public float[] PotentialAngles = new float[2];
        private int _decidedIndex = -1;
        public int DecidedIndex
        {
            get { return _decidedIndex; }
            set
            {
                _decidedIndex = value;
                EndPos = PotentialEnds[_decidedIndex];
                EndDir = PotentialFacing[_decidedIndex];
                EndAngle = PotentialAngles[_decidedIndex];
            }
        }

        public float RotationDirection
        {
            get
            { return RotDirEnum == eRotationDirection.clockwise ? 1 : -1; }
        }
        public eRotationDirection RotDirEnum;
        public float Radius = 1f;

        public ConnectionCurve(Vector3 centre, Vector3 sPos, Vector3 sDir)
        {
            Centre = centre;
            StartPos = sPos;
            StartDir = sDir;
            RotDirEnum = (-Mathf.Sign(Vector3.Cross(StartDir, StartPos).y)) == 1 ? eRotationDirection.clockwise : eRotationDirection.anticlockwise;
        }

        public void CalculateAngleOfPotentials()
        {
            for (int i = 0; i < PotentialAngles.Length; ++i)
            {
                PotentialAngles[i] = Vector3.SignedAngle(StartPos, PotentialEnds[i], RotDirEnum == eRotationDirection.anticlockwise ? Vector3.down : Vector3.up);
                if (PotentialAngles[i] < 0)
                {
                    PotentialAngles[i] += 360f;
                }
                PotentialFacing[i] = Quaternion.Euler(0, PotentialAngles[i] * RotationDirection, 0) * StartDir;
            }
        }

        public int GetFirstIndex()
        {
            return (PotentialAngles[0] <= PotentialAngles[1]) ? 0 : 1;
        }

        public int GetSecondIndex()
        {
            return 1 - GetFirstIndex();
        }

        public bool UsingFirstChoice { get { return DecidedIndex == GetFirstIndex(); } }
    }

    public class ConnectionScenario
    {
        public enum eScenarioType { none = -1, perpendicular, crossed }

        public ConnectionCurve StartCurve;
        public ConnectionCurve EndCurve;
        public ConnectionNode StartNode;
        public ConnectionNode EndNode;
        public eScenarioType Type;

        public ConnectionScenario(ConnectionNode sNode, ConnectionNode eNode,
            ConnectionCurve sCurve, ConnectionCurve eCurve)
        {
            StartCurve = sCurve;
            EndCurve = eCurve;
            StartNode = sNode;
            EndNode = eNode;
            if (sCurve.RotationDirection == eCurve.RotationDirection)
            {
                Type = eScenarioType.crossed;
            }
            else
            {
                Type = eScenarioType.perpendicular;
            }

            EvaluateScenario();
        }

        public void EvaluateScenario()
        {
            Vector3 centreDiff = EndCurve.Centre - StartCurve.Centre;
            if (Type == eScenarioType.perpendicular)
            {
                Vector3 perp = centreDiff.FlatPerpendicular().normalized;
                StartCurve.PotentialEnds[0] = EndCurve.PotentialEnds[0] = perp;
                StartCurve.PotentialEnds[1] = EndCurve.PotentialEnds[1] = -perp;
                StartCurve.CalculateAngleOfPotentials();
                EndCurve.CalculateAngleOfPotentials();
            }
            else
            {
                float hypAngle = Mathf.Acos(StartCurve.Radius * 2f / centreDiff.magnitude) * Mathf.Rad2Deg;
                Vector3 diffNorm = centreDiff.NoY().normalized;
                Vector3 pot1 = Quaternion.Euler(0, hypAngle, 0) * diffNorm;
                Vector3 pot2 = Quaternion.Euler(0, -hypAngle, 0) * diffNorm;


                StartCurve.PotentialEnds[0] = pot1;
                StartCurve.PotentialEnds[1] = pot2;
                EndCurve.PotentialEnds[0] = -pot1;
                EndCurve.PotentialEnds[1] = -pot2;
                StartCurve.CalculateAngleOfPotentials();
                EndCurve.CalculateAngleOfPotentials();
            }

            if (Vector3.Dot(StartCurve.PotentialFacing[StartCurve.GetFirstIndex()], EndCurve.Centre - StartCurve.Centre) > 0)
            {
                StartCurve.DecidedIndex = StartCurve.GetFirstIndex();
            }
            else
            {
                StartCurve.DecidedIndex = StartCurve.GetSecondIndex();
            }
            if (Vector3.Dot(EndCurve.PotentialFacing[EndCurve.GetFirstIndex()], StartCurve.Centre - EndCurve.Centre) > 0)
            {
                EndCurve.DecidedIndex = EndCurve.GetFirstIndex();
            }
            else
            {
                EndCurve.DecidedIndex = EndCurve.GetSecondIndex();
            }
        }

        public float Length()
        {
            return Mathf.PI * StartCurve.Radius * 2f * StartCurve.EndAngle / 360f
                + ((EndCurve.Centre + EndCurve.EndPos) - (StartCurve.Centre + StartCurve.EndPos)).magnitude
                + Mathf.PI * EndCurve.Radius * 2f * EndCurve.EndAngle / 360f;
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

    // 0 all left ; 1 left right ; 2 right left ; 3 all right
    ConnectionScenario[] Scenarios = new ConnectionScenario[4];

    // Update is called once per frame
    void Update()
    {
        if (!Visualising && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(VisualiseContinuous());
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
        Vector3 cdPerp = centreDiff.FlatPerpendicular().normalized;

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

    IEnumerator VisualiseContinuous()
    {
        Visualising = true;
        Line.startWidth = 0.4f;
        Line.endWidth = 0.05f;

        // --- Node creation ---
        ConnectionNode startNode = MakeConnectionNode();
        DrawFacingLine(startNode, LineStartDir, StartMacVis);

        bool placementFinished = false;
        bool stopTracking = false;
        Camera camera = Camera.main;
        Vector3 endFacing = Vector3.forward;

        int currentScenario = 0;
        bool useDynamicScenario = true;

        while (!placementFinished)
        {
            yield return null;
            if (Input.GetMouseButtonDown(0))
            {
                placementFinished = true;
            }
            if (Input.GetMouseButtonDown(1))
            {
                placementFinished = !placementFinished;
                placementFinished = !placementFinished;
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                currentScenario = (currentScenario + 1) % Scenarios.Length;
            }

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                useDynamicScenario = !useDynamicScenario;
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                stopTracking = !stopTracking;
            }
            if (stopTracking)
            {
                continue;
            }


            if (Input.mouseScrollDelta.y != 0f)
            {
                endFacing = Quaternion.Euler(0f, Input.mouseScrollDelta.y * 5f, 0f) * endFacing;
            }

            var raycasts = Physics.RaycastAll(camera.ScreenPointToRay(Input.mousePosition), 100);
            Vector3 rayHitPos = startNode.Position;
            foreach (var r in raycasts)
            {
                if (r.collider.name.Equals("Terrain"))
                {
                    rayHitPos = r.point;
                    break;
                }
            }
            if ((rayHitPos - startNode.Position).sqrMagnitude <= 4f * 4f)
            {
                continue;
            }


            ConnectionNode endNode = new ConnectionNode(rayHitPos, endFacing);
            DrawFacingLine(endNode, LineEndDir, EndMacVis);


            // --- Scenarios ---
            Scenarios[0] = new ConnectionScenario(startNode, endNode,
                startNode.MakeConnectionCurve(ConnectionCurve.eRotationDirection.anticlockwise),
                endNode.MakeConnectionCurve(ConnectionCurve.eRotationDirection.anticlockwise));
            Scenarios[1] = new ConnectionScenario(startNode, endNode,
                startNode.MakeConnectionCurve(ConnectionCurve.eRotationDirection.anticlockwise),
                endNode.MakeConnectionCurve(ConnectionCurve.eRotationDirection.clockwise));
            Scenarios[2] = new ConnectionScenario(startNode, endNode,
                startNode.MakeConnectionCurve(ConnectionCurve.eRotationDirection.clockwise),
                endNode.MakeConnectionCurve(ConnectionCurve.eRotationDirection.anticlockwise));
            Scenarios[3] = new ConnectionScenario(startNode, endNode,
                startNode.MakeConnectionCurve(ConnectionCurve.eRotationDirection.clockwise),
                endNode.MakeConnectionCurve(ConnectionCurve.eRotationDirection.clockwise));

            string scenarioString = "";
            int doubleCount = 0;
            int shortestLengthI = 0;

            float minDist = float.MaxValue;
            for (int i = 0; i < Scenarios.Length; ++i)
            {
                float newDist = Mathf.Min(minDist, Scenarios[i].Length());
                if (newDist < minDist)
                {
                    minDist = newDist;
                    shortestLengthI = i;
                }
                scenarioString += $"S{i}: {Scenarios[i].StartCurve.UsingFirstChoice} {Scenarios[i].EndCurve.UsingFirstChoice}.  ";
            }


            Debug.Log(scenarioString);
            Debug.Log($"Double trues: {doubleCount}. Active index: {shortestLengthI}.");

            // --- Line making ---
            List<Vector3> points = new List<Vector3>();

            int scenarioI = useDynamicScenario ? shortestLengthI : currentScenario;

            ConnectionCurve startCurve = Scenarios[scenarioI].StartCurve;
            ConnectionCurve endCurve = Scenarios[scenarioI].EndCurve;

            StartCircleVis.localPosition = startCurve.Centre;
            EndCircleVis.localPosition = endCurve.Centre;

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

        }


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
        endCurve = new ConnectionCurve(endCentre, (endNode.Position - endCentre).NoY().normalized, endNode.Facing);
    }
    void MakeCurvesSimple
        (ConnectionNode startNode, ConnectionNode endNode,
        out ConnectionCurve startCurve, out ConnectionCurve endCurve)
    {
        Vector3 nodeDiff = endNode.Position - startNode.Position;
        float ndLenght2 = nodeDiff.sqrMagnitude;

        Vector3 startCentre = FindBestCentre(startNode, endNode.Position, ndLenght2);
        Vector3 endCentre = FindBestCentre(endNode, startNode.Position, ndLenght2);

        startCurve = new ConnectionCurve(startCentre, (startNode.Position - startCentre).NoY().normalized, startNode.Facing);
        endCurve = new ConnectionCurve(endCentre, (endNode.Position - endCentre).NoY().normalized, endNode.Facing);
    }

    Vector3 FindBestCentre(ConnectionNode node, Vector3 otherPos, float sqrDist)
    {
        Vector3 perp = node.Facing.FlatPerpendicular();
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
