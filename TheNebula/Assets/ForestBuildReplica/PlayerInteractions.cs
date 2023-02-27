using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractions : MonoBehaviour
{
    Camera Eyes;

    ForestControls InputAsset;

    [SerializeField]
    Transform Log;

    // Start is called before the first frame update
    void Start()
    {
        Eyes = GetComponentInChildren<Camera>();
        InputAsset = InputHouse.Instance.InputAsset;
        InputAsset.PlayerActive.LeftClick.started += DoLeftClick;
        InputAsset.PlayerActive.RightClick.started += DoRightClick;
    }

    void DoLeftClick(InputAction.CallbackContext context)
    {
        Ray forRay = Eyes.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;
        Physics.Raycast(forRay, out hit);
        if (hit.collider != null)
        {
            Debug.Log($"{hit.collider.name}");
            if (hit.collider.name.ToLower().Contains("tree"))
            {
                Instantiate(Log, transform.position + Vector3.up * 2f, Quaternion.identity);
            }
        }
    }

    void DoRightClick(InputAction.CallbackContext context)
    {

    }
}
