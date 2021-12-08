using Realms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(DataSyncer))]
public class DataSyncerCustomEditor : Editor
{
    int selectedSourcePropertyIndex = 0;
    int selectedTargetPropertyIndex = 0;
    DataSyncer dataSyncer;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        dataSyncer = (DataSyncer)target;

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

        SerializedProperty property = serializedObject.FindProperty(nameof(dataSyncer.sourcePropertyName));
        property.stringValue = options[selectedSourcePropertyIndex];
    }

    public void DrawTargetPropertyPopup()
    {
        if (dataSyncer.targetObject == null) return;

        var options = new List<string>();

        var components = dataSyncer.targetObject.GetComponents<Component>();

        foreach (var component in components)
        {
            var componentType = component.GetType();
            var compontentName = componentType.Name;

            FillMethodPathsList(componentType, compontentName, ref options);
        }

        FillMethodPathsList(typeof(GameObject), "gameObject", ref options);


        selectedTargetPropertyIndex = options.IndexOf(dataSyncer.targetPropertyPath);

        selectedTargetPropertyIndex = EditorGUILayout.Popup("Target Property",
            selectedTargetPropertyIndex, options.ToArray());

        if (selectedTargetPropertyIndex == -1) return;

        SerializedProperty property = serializedObject.FindProperty(nameof(dataSyncer.targetPropertyPath));
        property.stringValue = options[selectedTargetPropertyIndex];

    }

    private void FillMethodPathsList(Type type, string methodOwner, ref List<string> methodPaths)
    {
        var methods = type.GetMethods();

        foreach (var method in methods)
        {
            var parameters = method.GetParameters();
            bool hasOneParameter = parameters.Length == 1;

            if (!hasOneParameter) continue;

            var parameterType = parameters[0].ParameterType;

            bool isParamaterStringOrBool = parameterType == typeof(string) || parameterType == typeof(bool);
            bool isReturnTypeVoid = method.ReturnType == typeof(void);

            if (isParamaterStringOrBool && isReturnTypeVoid)
            {
                methodPaths.Add($"{methodOwner}/{method.Name}");
            }
        }
    }

}
