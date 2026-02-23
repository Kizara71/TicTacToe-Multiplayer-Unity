using System;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using System.Threading.Tasks;

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

    public async void StartHost() {
        NetworkManager.Singleton.StartHost();
        
        // Give the Relay connection half a second to securely bind
        await Task.Delay(500); 
        
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
            
            RelayServerData relayServerData = AllocationUtils.ToRelayServerData(allocation, "udp");
            
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
            Debug.Log("Client is executing JoinRelay with code: " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            
            RelayServerData relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, "udp");
            
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            
            Debug.Log("Client successfully configured transport. Starting Client...");
            StartClient();
        } 
        catch (RelayServiceException e) 
        {
            Debug.Log(e);
        }
    }
}