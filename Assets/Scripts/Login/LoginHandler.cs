using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
public enum LoginState
{
    Success, // userName and password are all correct.
    Fail, // userName is correct, password is incorrect.
    NotFound // userName can not be found.
}
public class LoginHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField _accInput, _pwInput;
    private DatabaseReference _dbRef;
    public string _userID = "";

    private void Awake()
    {
        _dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // Used thru button.
    public async void Login()
    {
        // Can get some effects here.
        await UniTask.WaitForEndOfFrame();
        if (string.IsNullOrEmpty(_accInput.text) || string.IsNullOrEmpty(_pwInput.text))
        {
            Debug.Log("Please login your account.");
            return;
        }

        await _dbRef.Child(Constant.UsersNode).GetValueAsync().ContinueWithOnMainThread(async task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogWarning("Error to query users");
                return;
            }
            else
            {
                switch (CheckLoginState(task.Result.Children))
                {
                    case LoginState.NotFound:
                        CreateNewAccount(task.Result.ChildrenCount);
                        break;
                    case LoginState.Success:
                        await LoadDataEvent.RaiseAction(_userID);
                        LoadSceneHandler.LoadSceneByIndex(Constant.HomeScene);
                        break;
                    default:
                        break;
                }
            }
        });
    }

    private LoginState CheckLoginState(IEnumerable<DataSnapshot> task)
    {
        foreach (DataSnapshot data in task)
        {
            // If userName is incorrect.
            if (_accInput.text != data.Child(Constant.UserName).Value?.ToString()) continue;

            // If password is incorrect.
            if (_pwInput.text != data.Child(Constant.Password).Value?.ToString())
            {
                Debug.Log("Password is incorrect.");
                return LoginState.Fail;
            }

            // Login successfully.
            Debug.Log("Login successfully.");
            _userID = data.Key;
            return LoginState.Success;
        }

        // Can not find userName -> create new account.
        return LoginState.NotFound;
    }

    private async void CreateNewAccount(long childCount)
    {
        // Create new object for saving.
        DataSaver dataSaver = new DataSaver
        {
            userName = _accInput.text,
            password = _pwInput.text,
            currency = 0,
            level = 1
        };

        // Turn saving object to json and then add it to database.
        SaveDataEvent.RaiseAction(dataSaver, "user" + childCount.ToString());

        Debug.Log("Create new account");

        // Load data and enter game.
        await LoadDataEvent.RaiseAction();
        LoadSceneHandler.LoadSceneByIndex(1);
    }
}
