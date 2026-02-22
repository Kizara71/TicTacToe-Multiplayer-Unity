using System;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;

public class StartGameManager : MonoBehaviour 
{
    private void Start() 
    {
        LobbyManager.Instance.OnLobbyStartGame += LobbyManager_OnLobbyStartGame;
    }

    private void LobbyManager_OnLobbyStartGame(object sender, LobbyManager.LobbyEventArgs e) 
    {
        // Start Game!
        if (LobbyManager.Instance.IsLobbyHost()) // Note: Changed from LobbyManager.IsHost to match your current LobbyManager script
        {
            CreateRelay();
        } 
        else 
        {
            JoinRelay(LobbyManager.Instance.GetRelayJoinCode()); // Note: Adjusted to use Instance 
        }
    }

    public void StartHost() {
        NetworkManager.Singleton.StartHost();
        // Force the network to load the GameScene for all connected players!
        NetworkManager.Singleton.SceneManager.LoadScene("GameScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void StartClient() 
    {
        NetworkManager.Singleton.StartClient();
    }

    private async void CreateRelay() 
    {
        try 
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
            
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("Allocated Relay JoinCode: " + joinCode);
            
            RelayServerData relayServerData = AllocationUtils.ToRelayServerData(allocation, "dtls");
            
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            
            await LobbyManager.Instance.SetRelayJoinCode(joinCode);
            
            StartHost();
        } 
        catch (RelayServiceException e) 
        {
            Debug.Log(e);
        }
    }

    private async void JoinRelay(string joinCode) 
    {
        try 
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            
            RelayServerData relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");
            
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            
            StartClient();
        } 
        catch (RelayServiceException e) 
        {
            Debug.Log(e);
        }
    }
}