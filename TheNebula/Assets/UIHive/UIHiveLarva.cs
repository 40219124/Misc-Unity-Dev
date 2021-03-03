using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHiveLarva : MonoBehaviour
{


    private void OnEnable()
    {
        UIHiveBrain.AddOpeningUI(this);
    }

    private void OnDisable()
    {
        UIHiveBrain.RemoveClosingUI(this);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    protected void Update()
    {
        if (UIHiveBrain.IsActiveUI(this))
        {
            ActiveUIUpdate();
        }
    }

    protected virtual void ActiveUIUpdate()
    {

    }
}
