using Realms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DataSyncer : MonoBehaviour
{
    public RealmDataSync data;
    public GameObject targetObject;

    [HideInInspector] public string sourcePropertyName;
    [HideInInspector] public string targetPropertyPath;
    public bool invertBoolean;

    private object propertyChangeTarget;
    private MethodInfo method;

    private void OnEnable() => StartCoroutine(LinkDataSource());

    private void InvokeMethod()
    {
        object value =
            data.userData
            .GetType()
            .GetProperty(sourcePropertyName)
            .GetValue(data.userData);

        if (!(value is bool)) 
            value = value.ToString();
        else if (invertBoolean) 
            value = !(bool)value;

        method.Invoke(propertyChangeTarget, new object[] { value });
    }

    IEnumerator LinkDataSource()
    {
        yield return new WaitUntil(() => !(data.userData == null));

        string[] pieces = targetPropertyPath.Split(new char[] { '/' });
        string methodOwnerName = pieces[0];
        string methodName = pieces[1];

        bool isGameObjectMethod = methodOwnerName == "gameObject";

        propertyChangeTarget = isGameObjectMethod ?
            (object)targetObject : (object)targetObject.GetComponent(methodOwnerName);

        method = propertyChangeTarget.GetType().GetMethod(methodName);

        data.userData.PropertyChanged += (_, __) => InvokeMethod();
        InvokeMethod();
    }



}
