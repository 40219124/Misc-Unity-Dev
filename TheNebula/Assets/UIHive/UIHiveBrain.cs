using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHiveBrain : MonoBehaviour
{

    private static List<UIHiveLarva> openUIs = new List<UIHiveLarva>();


    public static void AddOpeningUI(UIHiveLarva ui)
    {
        // add ui to end of open list
        openUIs.Add(ui);
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
    }

    public static bool IsActiveUI(UIHiveLarva ui)
    {
        if(openUIs.Count == 0)
        {
            return false;
        }
        return GetActiveUI() == ui;
    }

    public static UIHiveLarva GetActiveUI()
    {
        if(openUIs.Count == 0)
        {
            return null;
        }
        return openUIs[openUIs.Count - 1];
    }
}
