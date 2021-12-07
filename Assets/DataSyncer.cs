using Realms;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DataSyncer : MonoBehaviour
{
    [SerializeField] private RealmDataSync data;
    [SerializeField] private TMP_Text text;
    [SerializeField] private string PropertyName;

    private void OnEnable() => StartCoroutine(LinkDataSource());

    public void SetDataAsText()
    {
        text.text = 
            data.userData
            .GetType()
            .GetProperty(PropertyName)
            .GetValue(data.userData)
            .ToString();
    }

    IEnumerator LinkDataSource()
    {
        yield return new WaitUntil(() => !(data.userData == null));

        data.userData.PropertyChanged
            += (_, __) => SetDataAsText();
        SetDataAsText();
    }

}
