using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public static class LoginEvent
{
    public static Action<string, string> LoginAction;
    public static void RaiseLoginAction(string arg1, string arg2)
    {
        LoginAction?.Invoke(arg1, arg2);
    }

    public static Action<string, string> RegisterAction;
    public static void RaiseRegisterAction(string arg1, string arg2)
    {
        RegisterAction?.Invoke(arg1, arg2);
    }
}
public enum LoginState
{
    Success, // userName and password are all correct.
    Fail, // userName is correct, password is incorrect.
    NotFound // userName can not be found.
}
public class LoginHandler : MonoBehaviour
{

    private DatabaseReference _dbRef;
    private string _userID = "";

    private void Awake()
    {
        _dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    private void OnEnable()
    {
        LoginEvent.LoginAction += Login;
        LoginEvent.RegisterAction += CreateNewAccount;
    }
    private void OnDisable()
    {
        LoginEvent.LoginAction -= Login;
        LoginEvent.RegisterAction -= CreateNewAccount;
    }
    public async void Login(string accText, string pwText)
    {
        ////////////////////////////////////
        // Can get some effects here.
        ///////////////////////////////////

        await UniTask.WaitForEndOfFrame();
        if (string.IsNullOrEmpty(accText) || string.IsNullOrEmpty(pwText))
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
                switch (CheckLoginState(task.Result.Children, accText, pwText))
                {
                    case LoginState.NotFound:
                        // CreateNewAccount(task.Result.ChildrenCount, accText, pwText);
                        Debug.LogWarning("Account do not exist, please register.");
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

    private LoginState CheckLoginState(IEnumerable<DataSnapshot> task, string accText, string pwText)
    {
        foreach (DataSnapshot data in task)
        {
            // If userName is incorrect.
            if (accText != data.Child(Constant.UserName).Value?.ToString()) continue;

            // If password is incorrect.
            if (pwText != data.Child(Constant.Password).Value?.ToString())
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

    private async void CreateNewAccount(string accText, string pwText)
    {
        await _dbRef.Child(Constant.UsersNode).GetValueAsync().ContinueWithOnMainThread(async task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Error to query users");
                return;
            }

            // Create new object for saving.
            DataSaver dataSaver = new DataSaver
            {
                userName = accText,
                password = pwText,
                currency = 0,
                level = 1
            };
            _userID = "user" + task.Result.ChildrenCount.ToString();
            // Turn saving object to json and then add it to database.
            SaveDataEvent.RaiseAction(dataSaver, _userID);

            Debug.Log("Create new account");

            // Load data and enter game.
            await LoadDataEvent.RaiseAction(_userID);
            LoadSceneHandler.LoadSceneByIndex(Constant.HomeScene);
        });

    }
}
