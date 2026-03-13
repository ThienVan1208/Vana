using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class MyLobbyUI : MonoBehaviour
{
    [SerializeField] private GameObject memberUiPrefab;
    [SerializeField] private RectTransform memberUiContainer;
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI lobbyNameTxt;
    [SerializeField] private TextMeshProUGUI curPlayerNumTxt;
    
    [Header("Buttons")]
    [SerializeField] private Button reloadButton;
    [SerializeField] private Button startGameButton;
    public Button xButton;
    private List<LobbyMemberUI> memberList = new List<LobbyMemberUI>();
    private bool isMemListRefreshed = false;
    private Lobby myLobby;

    private void OnDestroy()
    {
        LobbyManager.Instance.OnJoinedLobby -= OpenPage;
        LobbyManager.Instance.OnLobbyCreated -= OpenPage;
    }
    public void Init()
    {
        LobbyManager.Instance.OnJoinedLobby += OpenPage;
        LobbyManager.Instance.OnLobbyCreated += OpenPage;
        for(int i = 0; i < LobbyManager.MAX_PLAYERS; i++)
        {
            GameObject memberUiObj = Instantiate(memberUiPrefab, memberUiContainer);
            LobbyMemberUI lobbyMemberUI = memberUiObj.GetComponent<LobbyMemberUI>();
            lobbyMemberUI.Init();
            memberList.Add(lobbyMemberUI);
        }
    }
    public void RefreshMembers()
    {
        if(isMemListRefreshed) return;

        isMemListRefreshed = true;
        for(int i = 0; i < myLobby.Players.Count; i++)
        {
            memberList[i].Activate(myLobby.Players[i].Data[LobbyManager.PLAYER_NAME].Value,
                                    myLobby.Players[i].Data[LobbyManager.PLAYER_ID].Value);
            memberList[i].gameObject.SetActive(true);
        }
    }

    public void OpenPage(Lobby lobby)
    {
        myLobby = lobby;
        RefreshMembers();
        lobbyNameTxt.text = myLobby.Name;
        curPlayerNumTxt.text = myLobby.Players.Count.ToString() + "/" + myLobby.MaxPlayers.ToString();
        gameObject.SetActive(true);
    }

}
