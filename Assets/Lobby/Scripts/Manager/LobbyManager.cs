using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
public struct PlayerLobbyInfo : INetworkSerializable
{
    public string name;
    public string id;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref name);
        serializer.SerializeValue(ref id);
    }
}

public class LobbyManager : NetworkBehaviour
{
    public const string PLAYER_NAME = "PlayerName";
    public const string PLAYER_ID = "PlayerId";
    public const string HOST_ID = "HostId";
    public const string RELAY_JOIN_CODE = "RelayJoinCode";
    public const string LOBBY_CODE = "LobbyCode";
    public const int MAX_PLAYERS = 2;
    public const int MAX_QUERIED_LOBBIES = 10;

    public Action<Lobby> OnLobbyCreated = delegate { };
    public Action<Lobby> OnJoinedLobby = delegate { };
    public Action<Lobby> OnKickedFromLobby = delegate { };
    public Action<Lobby> OnLobbyLeft = delegate{};
    public Action<Lobby> OnLobbyUpdated = delegate { };
    public Action<List<Lobby>> OnLobbyListUpdated = delegate { };
    public Action<bool> OnLobbyBusy = delegate { };

    public static LobbyManager Instance { get; private set; }
    private bool isLobbyHost = false;
    public bool IsLobbyHost => isLobbyHost;
    private PlayerLobbyInfo playerLobbyInfo;
    public PlayerLobbyInfo PlayerLobbyInfo => playerLobbyInfo;
    private Lobby currentLobby;
    public Lobby CurrentLobby => currentLobby;
    private Coroutine heartBeatLobbyCoroutine;
    private Coroutine pollLobbyCoroutine;

    private bool isLobbyBusy = false;

    [SerializeField] private UserDataSO userDataSO;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        Init();
    }
    private async void Init()
    {
        try
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            Debug.Log("Authentication: " + AuthenticationService.Instance.PlayerId);
        }
        catch (ServicesInitializationException e)
        {
            Debug.LogException(e);
        }
        finally
        {
            playerLobbyInfo = new PlayerLobbyInfo
            {
                name = userDataSO.GetUserName(),
                id = AuthenticationService.Instance.PlayerId
            };

            OnKickedFromLobby += HandleKickEvent;
        }
    }

    void OnDisable()
    {
        OnKickedFromLobby -= HandleKickEvent;
    }
    private void SetLobbyBusy(bool isBusy = true)
    {
        isLobbyBusy = isBusy;
        OnLobbyBusy.Invoke(isLobbyBusy);
    }

    #region Create Lobby
    public async UniTask CreateLobby(string lobbyName = "New Lobby")
    {
        try
        {
            if (isLobbyBusy) return;
            SetLobbyBusy(true);

            isLobbyHost = true;
            var createLobbyOption = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = GetPlayer(playerLobbyInfo.name, playerLobbyInfo.id),
                Data = new Dictionary<string, DataObject>
                {
                    {HOST_ID, new DataObject(DataObject.VisibilityOptions.Member, playerLobbyInfo.id)},
                    {RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Public, "")}
                }

            };
            currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MAX_PLAYERS, createLobbyOption);
            currentLobby = await RelayManager.Instance.SetupRelay(currentLobby);
            Debug.Log($"Lobby created: {currentLobby.Id}, Code: {currentLobby.LobbyCode}");
            heartBeatLobbyCoroutine = StartCoroutine(HandleHeartBeatLobby());
            pollLobbyCoroutine = StartCoroutine(HandlePollLobby());

        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
        finally
        {
            SetLobbyBusy(false);
            OnLobbyCreated.Invoke(currentLobby);
        }
    }
    #endregion

    #region Join Lobby
    public async UniTask JoinLobbyByCode(string lobbyCode, string playerName = "New Player", string id = "")
    {
        try
        {
            if (isLobbyBusy) return;
            SetLobbyBusy(true);

            isLobbyHost = false;
            var joinLobbyOption = new JoinLobbyByCodeOptions
            {
                Player = id == "" ? GetPlayer(playerLobbyInfo.name, playerLobbyInfo.id) : GetPlayer(playerName, id)
            };
            currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyOption);
            Debug.Log($"Joined lobby: {currentLobby.Id}");
            await RelayManager.Instance.JoinRelay(currentLobby);
            heartBeatLobbyCoroutine = StartCoroutine(HandleHeartBeatLobby());
            pollLobbyCoroutine = StartCoroutine(HandlePollLobby());
            OnJoinedLobby?.Invoke(currentLobby);

        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Can not join lobby");
            Debug.LogException(e);
        }
        finally
        {
            SetLobbyBusy(false);
            
        }
    }
    #endregion


    #region Leave Lobby
    public async void LeaveLobby()
    {
        try
        {
            if (isLobbyBusy) return;
            SetLobbyBusy(true);

            if (currentLobby == null) return;

            isLobbyHost = false;

            if (heartBeatLobbyCoroutine != null)
            {
                StopCoroutine(heartBeatLobbyCoroutine);
                heartBeatLobbyCoroutine = null;
            }
            if (pollLobbyCoroutine != null)
            {
                StopCoroutine(pollLobbyCoroutine);
                pollLobbyCoroutine = null;
            }

            await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, playerLobbyInfo.id);
            if(currentLobby.Players.Count <= 1)
            {
                Debug.Log("delete lobby");
                await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);

            }
            OnLobbyLeft.Invoke(currentLobby);

        }
        catch (LobbyServiceException e)
        {
            if (e.Reason == LobbyExceptionReason.LobbyNotFound)
            {
                Debug.Log("Lobby already deleted by server (normal when host leaves). Ignoring.");
                return;
            }


            Debug.LogException(e);
        }
        finally
        {
            NetworkManager.Singleton.Shutdown();
            currentLobby = null;
            SetLobbyBusy(false);
        }
    }

    public async void KickPlayer(string playerId)
    {
        try
        {
            if (!isLobbyHost)
            {
                Debug.Log("Only Host can kick players");
                return;
            }

            await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, playerId);

            OnLobbyUpdated.Invoke(currentLobby);
        }
        catch(LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }
    
    private void HandleKickEvent(Lobby lobby)
    {
        lobby = null;
        NetworkManager.Singleton.Shutdown();
    }
    #endregion

    #region Query
    public async UniTask<List<Lobby>> QueryLobbies()
    {
        try
        {
            var queryOptions = new QueryLobbiesOptions
            {
                Count = MAX_QUERIED_LOBBIES,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };
            var response = await LobbyService.Instance.QueryLobbiesAsync(queryOptions);
            OnLobbyListUpdated.Invoke(response.Results);
            return response.Results;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
            return new List<Lobby>();
        }
    }
    #endregion

    public Player GetPlayer(string playerName = "", string playerId = "")
    {
        if (playerId.Length == 0)
        {
            var player = new Player
            {
                Data = new Dictionary<string, PlayerDataObject>
            {
                {PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerLobbyInfo.name)},
                {PLAYER_ID, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerLobbyInfo.id)}
            }
            };
            return player;
        }
        else
        {
            var player = new Player
            {
                Data = new Dictionary<string, PlayerDataObject>
            {
                {PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)},
                {PLAYER_ID, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerId)}
            }
            };
            return player;
        }

    }

    public void StartGame()
    {
        NetworkManager.Singleton.SceneManager.LoadScene(Constant.PLAY_SCENE, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    #region HeartBeat&Poll
    private IEnumerator HandleHeartBeatLobby()
    {
        if (currentLobby == null) yield return null;
        while (true)
        {
            yield return new WaitForSeconds(15f);
            try
            {
                LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
            }
        }
    }

    private IEnumerator HandlePollLobby()
    {
        if (currentLobby == null) yield return null;
        while (true)
        {
            yield return new WaitForSeconds(5f);
            PollLobby();

        }
    }

    private async void PollLobby()
    {
        try
        {
            Lobby updatedLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);
            if (updatedLobby != null)
            {
                bool isPlayerExist = updatedLobby.Players.Exists(p => p.Id == playerLobbyInfo.id);
                if (!isPlayerExist)
                {
                    Debug.Log("You have been kicked out of lobby");
                    if (heartBeatLobbyCoroutine != null)
                    {
                        StopCoroutine(heartBeatLobbyCoroutine);
                        heartBeatLobbyCoroutine = null;
                    }
                    if (pollLobbyCoroutine != null)
                    {
                        StopCoroutine(pollLobbyCoroutine);
                        pollLobbyCoroutine = null;
                    }
                    currentLobby = null;
                    OnKickedFromLobby.Invoke(updatedLobby);
                    return;
                }
                OnLobbyUpdated.Invoke(updatedLobby);
                currentLobby = updatedLobby;
            }

        }
        catch (LobbyServiceException e)
        {
            if (e.Reason == LobbyExceptionReason.LobbyNotFound)
            {
                Debug.Log("Lobby no longer exists - probably deleted by host");
                if (heartBeatLobbyCoroutine != null)
                {
                    StopCoroutine(heartBeatLobbyCoroutine);
                    heartBeatLobbyCoroutine = null;
                }
                if (pollLobbyCoroutine != null)
                {
                    StopCoroutine(pollLobbyCoroutine);
                    pollLobbyCoroutine = null;
                }
                currentLobby = null;
                OnKickedFromLobby.Invoke(currentLobby);
                return;
            }

            Debug.LogException(e);
        }

    }
    #endregion


}
