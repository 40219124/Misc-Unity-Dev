using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHiveSecretary : MonoBehaviour
{
    [SerializeField]
    private string OpenInput = "";
    [SerializeField]
    private string CloseInput = "";
    [SerializeField]
    private bool RequireFocusToClose = true;
    [SerializeField]
    private UIHiveLarva Target;

    private void Awake()
    {
        if (Target == null)
        {
            Debug.LogWarning("Secretary has no target.");
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown(OpenInput) && CanOpen())
        {
            Target.OpenUI();
        }
        else if (Input.GetButtonDown(CloseInput) && CanClose())
        {
            Target.CloseUI();
        }
    }

    private bool CanOpen()
    {
        return Target.isActiveAndEnabled == false;
    }

    private bool CanClose()
    {
        if (Target.isActiveAndEnabled == false)
        {
            return false;
        }
        if (RequireFocusToClose && !UIHiveBrain.IsActiveUI(Target))
        {
            return false;
        }
        return true;
    }
}
