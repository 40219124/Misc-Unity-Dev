using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridColumn
{
    public GridColumn(Vector2Int gridPos, float rawY)
    {
        GridPos = gridPos;
        FloorYRaw = rawY;
        FloorY = Mathf.FloorToInt(rawY * 4f) / 4f;
    }

    public Vector2Int GridPos;
    public float FloorYRaw;
    public float FloorY;
    public int BuildStages = 0;
    public float CurrentY
    {
        get
        {
            return FloorY + (0.5f * BuildStages);
        }
    }

    public Vector3 BasePos
    {
        get
        {
            return new Vector3(GridPos.x, FloorY, GridPos.y);
        }
    }
    public Vector3 BasePosRaw
    {
        get
        {
            return new Vector3(GridPos.x, FloorYRaw, GridPos.y);
        }
    }
    public Vector3 TopPos
    {
        get
        {
            return new Vector3(GridPos.x, CurrentY, GridPos.y);
        }
    }
}

public class BuildingGrid
{
    Dictionary<Vector2Int, GridColumn> Columns = new Dictionary<Vector2Int, GridColumn>();

    public Vector2Int ClosestColumnKey(Vector3 pos)
    {
        return new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z));
    }

    public GridColumn GetColumn(Vector3 pos)
    {
        Vector2Int closest = ClosestColumnKey(pos);
        if (!Columns.ContainsKey(closest))
        {
            Vector3 columnRayStart = new Vector3(closest.x, pos.y, closest.y) + (3f * Vector3.up);
            Ray forRay = new Ray(columnRayStart, Vector3.down);
            RaycastHit hit;
            Physics.Raycast(forRay, out hit, 10f);
            if (hit.collider.CompareTag("Ground"))
            {
                Columns.Add(closest, new GridColumn(closest, hit.point.y));
            }
        }
        return Columns[closest];
    }
}

public class BuildingSuggestion : MonoBehaviour
{
    private static BuildingSuggestion _instance;
    public static BuildingSuggestion Instance { get { return _instance; } }

    private void Awake()
    {
        _instance = this;
    }

    [SerializeField]
    Transform DemoObject;

    Camera Eyes = null;

    private bool SuggestionFound = false;
    private float TimeSinceSuggestionMove = 5f;
    private float SuggestionMoveCD = 0.3f;

    BuildingGrid Grid = new BuildingGrid();

    public void Initialise(Camera eyes)
    {
        Eyes = eyes;
    }

    public bool CanPlaceObject
    {
        get
        {
            return SuggestionFound;
        }
    }

    public void PlaceObject(GameObject obj)
    {
        obj.transform.position = DemoObject.position;
        Grid.GetColumn(obj.transform.position).BuildStages += 4;
    }

    void Update()
    {
        if (Eyes == null)
        {
            return;
        }

        TimeSinceSuggestionMove += Time.deltaTime;

        if (PlayerInventory.LogCount == 0)
        {
            if (SuggestionFound)
            {
                DemoObject.gameObject.SetActive(false);
                SuggestionFound = false;
            }
            return;
        }

        if (TimeSinceSuggestionMove >= SuggestionMoveCD)
        {
            Ray forRay = Eyes.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;
            Physics.Raycast(forRay, out hit, 4f);

            SuggestionFound = hit.collider != null && hit.collider.CompareTag("Ground");
            DemoObject.gameObject.SetActive(SuggestionFound);
            if (SuggestionFound)
            {
                Vector3 groundPos = hit.point;
                GridColumn column = Grid.GetColumn(groundPos);
                //Vector3 roundedPos = new Vector3(Mathf.RoundToInt(groundPos.x), Mathf.RoundToInt(groundPos.y * 4f) / 4f, Mathf.RoundToInt(groundPos.z));
                DemoObject.position = column.TopPos;
            }
        }
    }
}
