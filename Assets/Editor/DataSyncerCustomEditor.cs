using Realms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

static class TypeExtensions
{
    public static bool IsCastableTo(this Type from, Type to)
    {
        if (to.IsAssignableFrom(from))
        {
            return true;
        }
        return from.GetMethods(BindingFlags.Public | BindingFlags.Static)
                          .Any(
                              m => m.ReturnType == to &&
                                   (m.Name == "op_Implicit" ||
                                    m.Name == "op_Explicit")
                          );
    }
}


[CustomEditor(typeof(DataSyncer))]
public class DataSyncerCustomEditor : Editor
{
    int selectedSourcePropertyIndex = 0;
    int selectedTargetPropertyIndex = 0;
    DataSyncer dataSyncer;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        dataSyncer = (DataSyncer)base.target;

        DrawSourcePropertyPopup();
        DrawTargetPropertyPopup();

        serializedObject.ApplyModifiedProperties();
    }

    public void DrawSourcePropertyPopup()
    {
        List<string> options = new List<string>();
        var datumFields =
            typeof(UserDatum).
            GetProperties(
            BindingFlags.Public |
            BindingFlags.Instance |
            BindingFlags.DeclaredOnly);

        foreach (var f in datumFields) options.Add(f.Name);

        selectedSourcePropertyIndex = options.IndexOf(dataSyncer.sourcePropertyName);

        selectedSourcePropertyIndex = EditorGUILayout.Popup("Source Property", selectedSourcePropertyIndex, options.ToArray());
        if (selectedSourcePropertyIndex == -1) return;


        serializedObject.FindProperty(nameof(dataSyncer.sourcePropertyName)).stringValue = options[selectedSourcePropertyIndex];
    }

    public void DrawTargetPropertyPopup()
    {
        if (dataSyncer.targetObject == null) return;

        var options = new List<string>();

        foreach (var c in dataSyncer.targetObject.GetComponents<Component>())
        {
            string compontentName = c.GetType().Name;

            foreach (var f in c.GetType().GetProperties())
            {
                foreach (var a in f.GetAccessors())
                {
                    if (a.GetParameters().Length == 1 && a.GetParameters()[0].ParameterType == typeof(string))
                    {
                        options.Add($"{compontentName}/{a.Name}");
                    }
                }
            }
        }

        selectedTargetPropertyIndex = options.IndexOf(dataSyncer.targetPropertyPath);


        selectedTargetPropertyIndex = EditorGUILayout.Popup("Target Property",
            selectedTargetPropertyIndex, options.ToArray());

        if (selectedTargetPropertyIndex == -1) return;

        serializedObject.FindProperty(nameof(dataSyncer.targetPropertyPath))
            .stringValue = options[selectedTargetPropertyIndex];

    }

}