using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "UserDataSO", menuName = "UserDataSO", order = 0)]
public class UserDataSO : ScriptableObject
{
    [SerializeField] private string _userID;
    [SerializeField] private string _userName;
    [SerializeField] private string _password;
    public CurrencyInfoSO currencyInfoSO;
    public LevelInfoSO levelInfoSO;

    public string GetUserID() { return _userID; }
    public void SetUserID(string id) { _userID = id; }

    public string GetUserName() { return _userName; }
    public void SetUserName(string userName) { _userName = userName; }

    public string GetPassword() { return _password; }
    public void SetPassword(string pass) { _password = pass; }

    public void SetData(string ID = "", string userName = "", string password = "", int currency = 0, int level = 1)
    {
        _userID = ID == "" ? _userID : ID;
        _userName = userName;
        _password = password;
        currencyInfoSO.SetCurrency(currency);
        levelInfoSO.SetLevel(level);
        Debug.LogWarning("set data " + userName);
    }
}
