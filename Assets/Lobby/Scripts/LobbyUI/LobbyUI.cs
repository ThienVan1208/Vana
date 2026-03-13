using DG.Tweening;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : UIBase
{
    [SerializeField] private LobbyListUI lobbyList;
    [SerializeField] private MyLobbyUI myLobby;
    [SerializeField] private GameObject fadePanel;
    [SerializeField] private Button openLobbyButton;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private RectTransform loadingIcon;

    protected override void Awake()
    {
        base.Awake();
        lobbyList.Init();
        myLobby.Init();
        openLobbyButton.onClick.AddListener(OpenPage);
        lobbyList.xButton.onClick.AddListener(ClosePage);
        myLobby.xButton.onClick.AddListener(() =>
        {
            lobbyList.gameObject.SetActive(true);
            myLobby.gameObject.SetActive(false);
            LobbyManager.Instance.LeaveLobby();
        });

        LobbyManager.Instance.OnKickedFromLobby += (Lobby lobby) =>
        {
            lobbyList.gameObject.SetActive(true);
            myLobby.gameObject.SetActive(false);
        };
        LobbyManager.Instance.OnLobbyBusy += ShowLoadingPanel;
    }
    private void OnDestroy()
    {
        LobbyManager.Instance.OnKickedFromLobby -= (Lobby lobby) =>
        {
            lobbyList.gameObject.SetActive(true);
            myLobby.gameObject.SetActive(false);
        };
        LobbyManager.Instance.OnLobbyBusy -= ShowLoadingPanel;
    }

    public void OpenPage()
    {
        lobbyList.gameObject.SetActive(true);
        myLobby.gameObject.SetActive(false);
        fadePanel.SetActive(true);
    }

    public void ClosePage()
    {
        lobbyList.gameObject.SetActive(false);
        myLobby.gameObject.SetActive(false);
        fadePanel.SetActive(false);
    }

    public void ShowLoadingPanel(bool isShown = true)
    {
        loadingPanel.SetActive(isShown);
        if (isShown)
        {
            loadingIcon.DOLocalRotate(Vector3.one * 360, 3f)
            .SetLoops(-1, LoopType.Yoyo);
        }
        else
        {

            loadingIcon.DOKill();

        }
    }
}
