using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BarrelType))]
public class BarrelTypeEditor : Editor
{



    public enum Things { bleep, bloop, blap }

    Things things;
    float someValue;

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();


        // explicit positioning using Rect
        // GUI
        // EditorGUI

        // implicit positioning, auto-layout
        // GUILayout
        // EditorGUILayout

        //GUILayout.Label("noot");
        //someValue = GUILayout.HorizontalSlider(someValue, -1f, 1f);
        //GUILayout.Space(10);

        //GUILayout.BeginHorizontal();
        //GUILayout.Label("Things:", GUILayout.Width(60));
        //if(GUILayout.Button("Do a thing"))
        //{
        //    Debug.Log("Did thing.");
        //}
        //things = (Things)EditorGUILayout.EnumPopup(things);
        //GUILayout.EndHorizontal();

        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Things:", GUILayout.Width(60));
                if (GUILayout.Button("Do a thing"))
                {
                    Debug.Log("Did thing.");
                }
                things = (Things)EditorGUILayout.EnumPopup(things);
            }
            GUILayout.Label("Things");
            GUILayout.Label("Things", GUI.skin.button);
            GUILayout.Space(40);
            GUILayout.Label("Category", EditorStyles.boldLabel);

            EditorGUILayout.ObjectField("Assign here:", null, typeof(Transform), true);
        }
    }
}
