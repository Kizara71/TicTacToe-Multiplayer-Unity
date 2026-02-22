using System;
using Unity.Netcode;
using UnityEngine;

public class GameManger : NetworkBehaviour
{
    // Enums and Variables
    public enum PlayerType
    {
        None,
        Cross,
        Circle
    }
    public enum Direction
    {
        Horizontal,
        Vertical,
        DiagonalLeftToRight,
        DiagonalRightToLeft
    }
    private PlayerType localPlayerType;
    private NetworkVariable<PlayerType> currentPlayerableType = new NetworkVariable<PlayerType>();    
    private PlayerType[,] playerTypeArray;


    // Events
    public event EventHandler<OnClickedGridPositionEventArgs> OnGridPositionClicked;
    public class OnClickedGridPositionEventArgs : EventArgs
    {
        public int x;
        public int y;
        public PlayerType playerType;
    }   
    public event EventHandler OnGameStarted;
    public event EventHandler OnCurrentPlayerableTypeChanged;
    public event EventHandler<OnGameWinEventArgs> OnGameWin;
    public class OnGameWinEventArgs : EventArgs
    {
        public Vector2Int centerGridPosition;
        public Direction winDirection;
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

        playerTypeArray = new PlayerType[3, 3];
    }


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
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManger_OnClientConnectedCallback;
        }

        currentPlayerableType.OnValueChanged += (PlayerType oldValue, PlayerType newValue) => {
            OnCurrentPlayerableTypeChanged?.Invoke(this, EventArgs.Empty);
            Debug.Log("Current playerable type changed from " + oldValue + " to " + newValue);
        };
    }

    private void NetworkManger_OnClientConnectedCallback(ulong clientId)
    {
        currentPlayerableType.Value = PlayerType.Cross  ;
        if(NetworkManager.Singleton.ConnectedClients.Count == 2)
        {
            TriggerOnGameStartedRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameStartedRpc()
    {
        OnGameStarted?.Invoke(this, EventArgs.Empty);
    }

    [Rpc(SendTo.Server)]
    public void ClickedOnGridPositionRpc(int x, int y , PlayerType playerType)
    {
        if(currentPlayerableType.Value != playerType)
        {
            Debug.Log("It's not your turn !");
            return;
        }

        if (playerTypeArray[x, y] != PlayerType.None)
        {
            Debug.Log("This grid position is already occupied !");
            return;
        }

        playerTypeArray[x, y] = playerType; 

        Debug.Log("Clicked on grid position: " + x + ", " + y);
        OnGridPositionClicked?.Invoke(this, new OnClickedGridPositionEventArgs {
             x = x, y = y  , playerType = playerType
        });

        // Switch to the other player after a successful click
        if (currentPlayerableType.Value == PlayerType.Cross)
        {
            currentPlayerableType.Value = PlayerType.Circle;
        }
        else
        {
            currentPlayerableType.Value  = PlayerType.Cross;
        }

        TestWinner();
    }

    private bool TestWinnerLine(PlayerType aPlayerType , PlayerType bPlayerType , PlayerType cPlayerType)
    {
        return aPlayerType == bPlayerType && bPlayerType == cPlayerType && aPlayerType != PlayerType.None;  
    }   

    private void TestWinner()
    {
        // Test rows
        for(int y = 0; y < 3; y++)
        {
            if(TestWinnerLine(playerTypeArray[0, y], playerTypeArray[1, y], playerTypeArray[2, y]))
            {
                Debug.Log("Winner is : " + playerTypeArray[0, y]);
                OnGameWin?.Invoke(this, new OnGameWinEventArgs {
                    centerGridPosition = new Vector2Int(1, y),
                    winDirection = Direction.Horizontal
                });
                return;
            }
        }

        // Test columns
        for(int x = 0; x < 3; x++)
        {
            if(TestWinnerLine(playerTypeArray[x, 0], playerTypeArray[x, 1], playerTypeArray[x, 2]))
            {
                Debug.Log("Winner is : " + playerTypeArray[x, 0]);
                OnGameWin?.Invoke(this, new OnGameWinEventArgs {
                    centerGridPosition = new Vector2Int(x, 1),
                    winDirection = Direction.Vertical
                });
                return;
            }
        }

        // Test diagonals
        if(TestWinnerLine(playerTypeArray[0, 0], playerTypeArray[1, 1], playerTypeArray[2, 2]))
        {
            Debug.Log("Winner is : " + playerTypeArray[0, 0]);
            OnGameWin?.Invoke(this, new OnGameWinEventArgs {
                centerGridPosition = new Vector2Int(1, 1),
                winDirection = Direction.DiagonalLeftToRight
            });
            return;
        }
        if(TestWinnerLine(playerTypeArray[2, 0], playerTypeArray[1, 1], playerTypeArray[0, 2]))
        {
            Debug.Log("Winner is : " + playerTypeArray[2, 0]);
            OnGameWin?.Invoke(this, new OnGameWinEventArgs {
                centerGridPosition = new Vector2Int(1, 1),
                winDirection = Direction.DiagonalRightToLeft
            });
            return;
        }
    }   

    public PlayerType GetLocalPlayerType()
    {
        return localPlayerType;
    }   

    public PlayerType GetCurrentPlayerableType()
    {
        return currentPlayerableType.Value;
    }
}