using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListUI : MonoBehaviour
{
    public static bool IsLobbyJoinning { get; set; } = false;
    [SerializeField] private GameObject lobbySlotPrefab;
    [SerializeField] private RectTransform lobbySlotContainer;
    [SerializeField] private Button reloadButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private CreateLobbyUI createLobbyUI;

    public Button xButton;
    private List<LobbySlotUI> lobbySlotList = new List<LobbySlotUI>();
    private bool isLobbiesQueried = false;

    private void OnEnable()
    {
        RefreshLobbySlots();
    }
    public void Init()
    {
        LobbyManager.Instance.OnLobbyCreated += (Lobby lobby) => gameObject.SetActive(false);
        LobbyManager.Instance.OnJoinedLobby += (Lobby lobby) => gameObject.SetActive(false);

        reloadButton.onClick.AddListener(RefreshLobbySlots);
        createLobbyButton.onClick.AddListener(() => createLobbyUI.gameObject.SetActive(true));
        for (int i = 0; i < LobbyManager.MAX_QUERIED_LOBBIES; i++)
        {
            GameObject lobbySlotObj = Instantiate(lobbySlotPrefab, lobbySlotContainer);
            LobbySlotUI lobbySlotUI = lobbySlotObj.GetComponent<LobbySlotUI>();
            lobbySlotObj.SetActive(false);
            lobbySlotList.Add(lobbySlotUI);
        }
    }
    private void OnDestroy()
    {
        LobbyManager.Instance.OnLobbyCreated -= (Lobby lobby) => gameObject.SetActive(false);
        LobbyManager.Instance.OnJoinedLobby -= (Lobby lobby) => gameObject.SetActive(false);
    }

    private async void RefreshLobbySlots()
    {
        if (isLobbiesQueried) return;

        isLobbiesQueried = true;
        var lobbies = await LobbyManager.Instance.QueryLobbies();

        if (lobbies.Count == 0)
        {
            foreach (var lobbySlot in lobbySlotList)
            {
                lobbySlot.gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < lobbies.Count; i++)
            {
                lobbySlotList[i].SetInfor(lobbies[i]);
                lobbySlotList[i].gameObject.SetActive(true);

            }
        }


        isLobbiesQueried = false;

    }

}
