using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourShape : MonoBehaviour
{

    int ListPos;
    int ListSize;

    [SerializeField]
    float DootHeight = 5.0f;
    [SerializeField]
    Vector3 DootHeightOffset = new Vector3(0.0f, 3.0f, 0.0f);

    [SerializeField]
    Transform RedDoot;
    [SerializeField]
    Transform GreenDoot;
    [SerializeField]
    Transform BlueDoot;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetValues(int listPos, int listSize)
    {
        ListPos = listPos;
        ListSize = listSize;

        float redVal = ColourValue(0);
        float greenVal = ColourValue(ListSize / 3.0f);
        float blueVal = ColourValue(2.0f * ListSize / 3.0f);

        GetComponent<Renderer>().material.color = new Color(redVal, greenVal, blueVal, 1.0f);

        RedDoot.transform.localPosition = Vector3.up * redVal * DootHeight + DootHeightOffset;
        GreenDoot.transform.localPosition = Vector3.up * greenVal * DootHeight + DootHeightOffset;
        BlueDoot.transform.localPosition = Vector3.up * blueVal * DootHeight + DootHeightOffset;
    }

    float ColourValue(float offset)
    {
        float val = (Mathf.Sin((2.0f * Mathf.PI * (ListPos + offset)) / ListSize) + 1) * 0.5f;
        Debug.Log(val);
        return val;
    }
}
