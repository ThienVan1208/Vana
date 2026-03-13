using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMemberUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI memberNameTxt;
    [SerializeField] private GameObject hostIcon;
    [SerializeField] private Button kickButton;
    private string playerId;

    public void Init()
    {
        gameObject.SetActive(false);
        CheckHost();
        LobbyManager.Instance.OnLobbyLeft += CheckHost;
        kickButton.onClick.AddListener(() => LobbyManager.Instance.KickPlayer(playerId));
    }
    private void OnDestroy()
    {
        LobbyManager.Instance.OnLobbyLeft -= CheckHost;
    }

    public void Activate(string name = "New Player", string id = "")
    {
        memberNameTxt.text = name;
        playerId = id;
        CheckHost();
        
        
    }

    public void SetHost(bool isHost = true)
    {
        hostIcon.SetActive(isHost);
        kickButton.gameObject.SetActive(LobbyManager.Instance.IsLobbyHost && !isHost);
    }

    private void CheckHost(Lobby lobby)
    {
        
        if(lobby.HostId == playerId)
        {
            SetHost();
        }
        else
        {
            SetHost(false);
        }

    }

    private void CheckHost()
    {
        if(LobbyManager.Instance.CurrentLobby == null) return;

        CheckHost(LobbyManager.Instance.CurrentLobby);
    }
}