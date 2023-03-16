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

    public void PlaceObject(GameObject obj)
    {
        obj.transform.position = DemoObject.position;
    }

    public void ShowSuggestion(Vector3 groundPos)
    {
        Vector3 roundedPos = new Vector3(Mathf.RoundToInt(groundPos.x), Mathf.RoundToInt(groundPos.y * 4f) / 4f, Mathf.RoundToInt(groundPos.z));
        DemoObject.position = roundedPos;
    }
}
