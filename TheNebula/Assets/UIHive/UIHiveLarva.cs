using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHiveLarva : MonoBehaviour
{
    public event Action OnUIFocus;
    public event Action OnUIUnfocus;

    private void Awake()
    {
        UIHiveBrain.OnUIListChanged += OnFocusChange;
    }

    private void OnDestroy()
    {
        UIHiveBrain.OnUIListChanged -= OnFocusChange;
    }

    protected virtual void OnEnable()
    {
        UIHiveBrain.AddOpeningUI(this);
        OnUIFocus?.Invoke();
    }

    protected virtual void OnDisable()
    {
        OnUIUnfocus?.Invoke();
        UIHiveBrain.RemoveClosingUI(this);
    }

    public virtual void OpenUI()
    {
        gameObject.SetActive(true);
    }

    public virtual void CloseUI()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (UIHiveBrain.IsActiveUI(this))
        {
            ActiveUIUpdate();
        }
    }

    protected virtual void ActiveUIUpdate()
    {

    }

    protected virtual void OnFocusChange(UIHiveLarva newFocus)
    {

    }
}
