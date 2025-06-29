using System;

using Cysharp.Threading.Tasks;
using Firebase.Database;
using Firebase.Extensions;

using UnityEngine;
public static class SaveDataEvent
{
    public static Action<DataSaver, string> action;
    public static void RaiseAction(DataSaver arg1 = null, string arg2 = "")
    {
        action?.Invoke(arg1, arg2);
    }
}
public static class LoadDataEvent
{
    public static Func<string, UniTask> action; // Use Func<string, UniTask> instead of Action<string>
    public static async UniTask RaiseAction(string arg2 = "") // Use UniTask return type
    {
        await action.Invoke(arg2);
    }
}
public class SaveLoadDataManager : MonoBehaviour
{
    [SerializeField] private UserDataSO _userDataSO;
    private DatabaseReference _dbRef;
    private void Awake()
    {
        _dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    }
    private void OnEnable()
    {
        SaveDataEvent.action += SaveData;
        LoadDataEvent.action += LoadData;
    }
    private void OnDisable()
    {
        SaveDataEvent.action -= SaveData;
        LoadDataEvent.action -= LoadData;
    }
    public void SaveData(DataSaver dataSaver = null, string userID = "")
    {
        if (dataSaver == null)
        {
            dataSaver = new DataSaver
            {
                userName = _userDataSO.GetUserName(),
                password = _userDataSO.GetPassword(),
                currency = _userDataSO.currencyInfoSO.GetCurrency(),
                level = _userDataSO.levelInfoSO.GetLevel()
            };
        }
        string json = JsonUtility.ToJson(dataSaver);
        if (userID != "")
        {
            _dbRef.Child(Constant.UsersNode).Child(userID).SetRawJsonValueAsync(json);
        }
        else
        {
            _dbRef.Child(Constant.UsersNode).Child(_userDataSO.GetUserID()).SetRawJsonValueAsync(json);
        }

        Debug.Log("Save successfully");
    }
    public async UniTask LoadData(string userID = "")
    {
        if (userID == "")
        {
            await _dbRef.Child(Constant.UsersNode).Child(_userDataSO.GetUserID()).GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogWarning("Error to query users");
                    return;
                }
                else
                {
                    _userDataSO.SetData(
                        userName: (string)task.Result.Child(Constant.UserName).Value,
                        password: (string)task.Result.Child(Constant.Password).Value,
                        currency: (int)task.Result.Child(Constant.Currency).Value,
                        level: (int)task.Result.Child(Constant.Level).Value
                    );
                }
            });
        }
        else
        {
            try
            {
                DataSnapshot snapshot = await _dbRef.Child(Constant.UsersNode).Child(userID).GetValueAsync();
                if (snapshot.Exists)
                {
                    _userDataSO.SetData(
                        ID: userID,
                        userName: snapshot.Child(Constant.UserName).Value != null ? (string)snapshot.Child(Constant.UserName).Value : "",
                        password: snapshot.Child(Constant.Password).Value != null ? (string)snapshot.Child(Constant.Password).Value : "",
                        currency: snapshot.Child(Constant.Currency).Value != null ? Convert.ToInt32(snapshot.Child(Constant.Currency).Value) : 0,
                        level: snapshot.Child(Constant.Level).Value != null ? Convert.ToInt32(snapshot.Child(Constant.Level).Value) : 0
                    );
                }
                else
                {
                    Debug.LogWarning("No data found for userID: " + userID);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error loading data: " + e.Message);
            }
        }

        Debug.Log("load data successfully.");
    }

}
