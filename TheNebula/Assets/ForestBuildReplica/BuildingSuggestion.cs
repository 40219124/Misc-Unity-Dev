using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    void Update()
    {
        if (Eyes == null )
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
                Vector3 roundedPos = new Vector3(Mathf.RoundToInt(groundPos.x), Mathf.RoundToInt(groundPos.y * 4f) / 4f, Mathf.RoundToInt(groundPos.z));
                DemoObject.position = roundedPos;
            }
        }
    }
}
