using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIHiveFirefly : MonoBehaviour, IEventSystemHandler, ISelectHandler
{
    [SerializeField]
    private Selectable DefaultSelected;
    [SerializeField]
    private bool RememberLastSelected;
    private GameObject LastSelected;

    UIHiveLarva Larva;
    private void Awake()
    {
        Larva = GetComponent<UIHiveLarva>();

        if (Larva != null)
        {
            Larva.OnUIFocus += SelectUIElement;
        }
    }

    private void OnDestroy()
    {
        Larva.OnUIFocus -= SelectUIElement;
    }

    private void OnEnable() { }

    private void OnDisable() { }

    private void SelectUIElement()
    {
        if (RememberLastSelected)
        {
            SelectElement(LastSelected);
        }
        else
        {
            SelectElement(DefaultSelected.gameObject);
        }
    }

    private void SelectElement(GameObject selectable)
    {
        EventSystem.current.SetSelectedGameObject(selectable);
    }

    void ISelectHandler.OnSelect(BaseEventData eventData)
    {
        if (UIHiveBrain.IsActiveUI(Larva))
        {
            LastSelected = eventData.selectedObject;
        }
    }
}
