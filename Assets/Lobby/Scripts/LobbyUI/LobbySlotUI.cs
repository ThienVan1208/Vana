using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbySlotUI : MonoBehaviour 
{
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI lobbyCodeTxt;
    [SerializeField] private Button joinLobbyButton;
    private Lobby lobby;

    private void Awake() {
        joinLobbyButton.onClick.AddListener(JoinLobby);
    }

    public async void JoinLobby()
    {
        Debug.LogWarning(lobby.Data[LobbyManager.LOBBY_CODE].Value);
        await LobbyManager.Instance.JoinLobbyByCode(lobby.Data[LobbyManager.LOBBY_CODE].Value.ToString(), 
                                                    LobbyManager.Instance.PlayerLobbyInfo.name,
                                                    LobbyManager.Instance.PlayerLobbyInfo.id);
    }

    public void SetInfor(string name = "New Lobby", string lobbyCode = "")
    {
        nameTxt.text = name;
        lobbyCodeTxt.text = lobbyCode;
    }

    public void SetInfor(Lobby lobby)
    {
        this.lobby = lobby;
        SetInfor(lobby.Name, lobby.LobbyCode);
    }
}