using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHiveBrain
{
    public delegate void UIFocused(UIHiveLarva focus);
    public static event UIFocused OnUIListChanged;
    private static UIHiveLarva previousFocus = null;

    private static List<UIHiveLarva> openUIs = new List<UIHiveLarva>();

    public static void AddOpeningUI(UIHiveLarva ui)
    {
        // add ui to end of open list
        openUIs.Add(ui);
        DoNewFocusEvent();
    }

    public static void RemoveClosingUI(UIHiveLarva ui)
    {
        // remove furthest back instance of this from the list
        int remI = -1;
        for (int i = openUIs.Count - 1; i >= 0; --i)
        {
            if (openUIs[i] == ui)
            {
                remI = i;
                break;
            }
        }
        if (remI != -1)
        {
            openUIs.RemoveAt(remI);
        }
        DoNewFocusEvent();
    }

    public static bool IsActiveUI(UIHiveLarva ui)
    {
        if (openUIs.Count == 0)
        {
            return false;
        }
        return ActiveUI == ui;
    }

    public static UIHiveLarva ActiveUI
    {
        get
        {
            if (openUIs.Count == 0)
            {
                return null;
            }
            return openUIs[openUIs.Count - 1];
        }
    }

    private static void DoNewFocusEvent()
    {
        if (ActiveUI == previousFocus)
        {
            return;
        }
        OnUIListChanged?.Invoke(ActiveUI);
        previousFocus = ActiveUI;
    }
}
