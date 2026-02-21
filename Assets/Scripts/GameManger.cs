using System;
using Unity.Netcode;
using UnityEngine;

public class GameManger : NetworkBehaviour
{
    public enum PlayerType
    {
        None,
        Cross,
        Circle
    }
    private PlayerType localPlayerType;
    private PlayerType currentPlayerableType;    
    public override void OnNetworkSpawn()
    {
        Debug.Log(" OnNetworkSpawn " + NetworkManager.Singleton.LocalClientId);   
        if(NetworkManager.Singleton.LocalClientId == 0)
        {
            localPlayerType = PlayerType.Cross;
        }
        else
        {
            localPlayerType = PlayerType.Circle;
        }
        if(IsServer)
        {
            currentPlayerableType = PlayerType.Cross;
        }
    }
    public event EventHandler<OnClickedGridPositionEventArgs> OnGridPositionClicked;
    public class OnClickedGridPositionEventArgs : EventArgs
    {
        public int x;
        public int y;
        public PlayerType playerType;
    }   
    public static GameManger Instance { get; private set; }
    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }       
        Instance = this;    
    }

    [Rpc(SendTo.Server)]
    public void ClickedOnGridPositionRpc(int x, int y , PlayerType playerType)
    {
        if(currentPlayerableType != playerType)
        {
            Debug.Log("It's not your turn !");
            return;
        }
        Debug.Log("Clicked on grid position: " + x + ", " + y);
        OnGridPositionClicked?.Invoke(this, new OnClickedGridPositionEventArgs {
             x = x, y = y  , playerType = playerType
        });

        // Switch to the other player after a successful click
        if (currentPlayerableType == PlayerType.Cross)
        {
            currentPlayerableType = PlayerType.Circle;
        }
        else
        {
            currentPlayerableType = PlayerType.Cross;
        }
    }
    public PlayerType GetLocalPlayerType()
    {
        return localPlayerType;
    }   
}