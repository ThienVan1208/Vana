using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using UnityEngine;
public class RelayManager : MonoBehaviour
{
    public static RelayManager Instance;
    private UnityTransport unityTransport;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
        unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
    }
    public async UniTask<Lobby> SetupRelay(Lobby lobby)
    {
        try
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(LobbyManager.MAX_PLAYERS - 1);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            await LobbyService.Instance.UpdateLobbyAsync(lobby.Id,
                new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        {LobbyManager.RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Public, joinCode)},
                        {LobbyManager.LOBBY_CODE, new DataObject(DataObject.VisibilityOptions.Public, lobby.LobbyCode)}
                    }
                }
            );

            unityTransport.SetRelayServerData(allocation.RelayServer.IpV4,
                                            (ushort)allocation.RelayServer.Port,
                                            allocation.AllocationIdBytes,
                                            allocation.Key,
                                            allocation.ConnectionData);
            NetworkManager.Singleton.StartHost();

            lobby = await LobbyService.Instance.GetLobbyAsync(lobby.Id);
            return lobby;
        }
        catch (RelayServiceException e)
        {
            Debug.LogException(e);
            return null;
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
            return null;
        }
    }

    public async UniTask JoinRelay(Lobby lobby)
    {
        try
        {
            var joinCode = lobby.Data[LobbyManager.RELAY_JOIN_CODE].Value;
            var joinCodeAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            unityTransport.SetRelayServerData(joinCodeAllocation.RelayServer.IpV4,
                                            (ushort)joinCodeAllocation.RelayServer.Port,
                                            joinCodeAllocation.AllocationIdBytes,
                                            joinCodeAllocation.Key,
                                            joinCodeAllocation.ConnectionData);
            NetworkManager.Singleton.StartClient();

        }
        catch (RelayServiceException e)
        {
            Debug.LogException(e);
        }
    }
}