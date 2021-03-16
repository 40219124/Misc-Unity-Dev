using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIHiveFirefly : MonoBehaviour, IEventSystemHandler
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
            Larva.OnUIUnfocus += SaveSelected;
        }

        if (UIHiveBrain.IsActiveUI(Larva))
        {
            SelectUIElement();
        }
    }

    private void OnDestroy()
    {
        Larva.OnUIFocus -= SelectUIElement;
        Larva.OnUIUnfocus -= SaveSelected;
    }

    private void OnEnable() { }

    private void OnDisable() { }

    private void SelectUIElement()
    {
        if (RememberLastSelected && LastSelected != null)
        {
            StartCoroutine(SelectElement(LastSelected));
        }
        else
        {
            StartCoroutine(SelectElement(DefaultSelected.gameObject));
        }
    }

    private IEnumerator SelectElement(GameObject selectable)
    {
        EventSystem.current.SetSelectedGameObject(null);
        yield return null;
        EventSystem.current.SetSelectedGameObject(selectable);
    }

    void SaveSelected()
    {
        if (UIHiveBrain.IsActiveUI(Larva))
        {
            LastSelected = EventSystem.current.currentSelectedGameObject;
        }
    }
}
