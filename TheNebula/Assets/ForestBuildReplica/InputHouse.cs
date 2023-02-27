using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHouse : MonoBehaviour
{
    static InputHouse _instance;
    public static InputHouse Instance { get { return _instance; } }
    ForestControls _inputAsset;
    public ForestControls InputAsset { get { return _inputAsset; } }

    private void Awake()
    {
        _instance = this;
        _inputAsset = new ForestControls();
        _inputAsset.PlayerActive.Enable();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
