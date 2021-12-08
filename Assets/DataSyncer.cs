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
    public bool invertBoolean;

    private void OnEnable() => StartCoroutine(LinkDataSource());

    IEnumerator LinkDataSource()
    {
        yield return new WaitUntil(() => !(data.userData == null));

        string[] pieces = targetPropertyPath.Split(new char[] { '/' });
        string componentName = pieces[0];
        string methodName = pieces[1];

        bool isGameObjectMethod = componentName == "gameObject";

        UnityEngine.Component component = null;
        if (!isGameObjectMethod)
            component = targetObject.GetComponent(componentName);

        PropertyChangedEventHandler handler;

        if (!isGameObjectMethod)
        {
            handler =
            (_, __) =>
                {
                    object value = 
                    data.userData
                    .GetType()
                    .GetProperty(sourcePropertyName)
                    .GetValue(data.userData);

                    if (!(value is string) && !(value is bool))
                        value = value.ToString();

                    if(value is bool && invertBoolean)
                    {
                        value = !((bool)value);

                    }

                    component.GetType().GetMethod(methodName).Invoke(component, new object[] { value });
                };
        }
        else
        {
            handler =
                (_, __) =>
                {
                    object value = 
                    data.userData
                    .GetType()
                    .GetProperty(sourcePropertyName)
                    .GetValue(data.userData);

                    if (!(value is string) && !(value is bool))
                        value = value.ToString();

                    if (value is bool && invertBoolean)
                    {
                        value = !((bool)value);
                    }

                    typeof(GameObject).GetMethod(methodName).Invoke(targetObject, new object[] { value });
                };
        }

        data.userData.PropertyChanged += handler;
        handler.Invoke(null, null);

    }


}
