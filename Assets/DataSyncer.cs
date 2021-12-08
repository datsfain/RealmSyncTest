using Realms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DataSyncer : MonoBehaviour
{
    public RealmDataSync data;
    public GameObject targetObject;

    [HideInInspector] public string sourcePropertyName;
    [HideInInspector] public string targetPropertyPath;

    private void OnEnable() => StartCoroutine(LinkDataSource());

    IEnumerator LinkDataSource()
    {
        yield return new WaitUntil(() => !(data.userData == null));

        string[] pieces = targetPropertyPath.Split(new char[] { '/' });
        string componentName = pieces[0];
        string setterName = pieces[1];

        var component = targetObject.GetComponent(componentName);

        PropertyChangedEventHandler handler =
            (_, __) =>
            {
                component.GetType().GetMethod(setterName).Invoke(component,
                new object[] {
                data.userData
                .GetType()
                .GetProperty(sourcePropertyName)
                .GetValue(data.userData)
                .ToString()
                });
            };

        data.userData.PropertyChanged += handler;
        handler.Invoke(null, null);

    }


}
