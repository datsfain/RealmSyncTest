using MongoDB.Bson;
using Realms;
using Realms.Sync;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserDatum : RealmObject
{
    [PrimaryKey]
    [MapTo("_id")]
    public ObjectId? Id { get; set; }
    [MapTo("_partition")]
    [Required]
    public string Partition { get; set; }
    [MapTo("coins")]
    public int? Coins { get; set; }
    [MapTo("hearts")]
    public int? Hearts { get; set; }
}
public class RealmDataSync : MonoBehaviour
{
    Realm _realm;
    User _user;
    App _app;
    public UserDatum userData;
    private readonly string APPLICATION_ID = "application-0-ybxkm";
    void OnEnable() => StartCoroutine(LoginToRealm());
    private void OnDisable() => _realm?.Dispose();

    private IEnumerator LoginToRealm()
    {
        _app = App.Create(new AppConfiguration(APPLICATION_ID)
        {
            MetadataPersistenceMode = MetadataPersistenceMode.NotEncrypted
        });

        var logInTask = _app.LogInAsync(Credentials.Anonymous());

        yield return new WaitUntil(() => logInTask.IsCompleted);

        _user = logInTask.Result;

        _realm = Realm.GetInstance(new SyncConfiguration(_user.Id, _user));

        var users = _realm.All<UserDatum>();
        if (users.Count() == 0)
        {
            Debug.Log("No User Data Found, Creating one...");
            _realm.Write(() =>
            {
                userData = _realm.Add(new UserDatum()
                {
                    Coins = 0,
                    Hearts = 0,
                    Id = ObjectId.GenerateNewId(),
                    Partition = _user.Id
                });
            });
        }

        if (users.Count() == 0)
        {
            Debug.LogError("No User Data");
            yield return null;
        }

        userData = users.First();
    }
}
