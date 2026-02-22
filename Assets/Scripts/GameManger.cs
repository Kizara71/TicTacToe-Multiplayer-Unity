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
    private NetworkVariable<int> crossPlayerScore = new NetworkVariable<int>(); 
    private NetworkVariable<int> circlePlayerScore = new NetworkVariable<int>(); 

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
        public PlayerType winnerPlayerType;
    }
    public event EventHandler OnRematch;
    public event EventHandler OnGameDraw;
    public event EventHandler OnScoreChanged;
    public event EventHandler OnPlacedSymbol;
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

        crossPlayerScore.OnValueChanged += (int oldValue, int newValue) => {
            OnScoreChanged?.Invoke(this, EventArgs.Empty);
            Debug.Log("Cross player score changed from " + oldValue + " to " + newValue);
        };

        circlePlayerScore.OnValueChanged += (int oldValue, int newValue) => {
            OnScoreChanged?.Invoke(this, EventArgs.Empty);
            Debug.Log("Circle player score changed from " + oldValue + " to " + newValue);
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
        TriggerOnPlacedSymbolRpc();
        TestWinner();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnPlacedSymbolRpc()
    {
        OnPlacedSymbol?.Invoke(this, EventArgs.Empty);
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
                PlayerType winnerPlayerType = playerTypeArray[0, y];
                Debug.Log("Winner is : " + playerTypeArray[0, y]);
                
                if (winnerPlayerType == PlayerType.Cross)
                {
                    crossPlayerScore.Value++;
                }
                else
                {
                    circlePlayerScore.Value++;
                }
                
                TriggerOnGameWinRpc(new Vector2Int(1, y), Direction.Horizontal, winnerPlayerType);
                return;
            }
        }

        // Test columns
        for(int x = 0; x < 3; x++)
        {
            if(TestWinnerLine(playerTypeArray[x, 0], playerTypeArray[x, 1], playerTypeArray[x, 2]))
            {
                PlayerType winnerPlayerType = playerTypeArray[x, 0];
                Debug.Log("Winner is : " + playerTypeArray[x, 0]);
                
                if (winnerPlayerType == PlayerType.Cross)
                {
                    crossPlayerScore.Value++;
                }
                else
                {
                    circlePlayerScore.Value++;
                }
                
                TriggerOnGameWinRpc(new Vector2Int(x, 1), Direction.Vertical , winnerPlayerType);
                return;
            }
        }

        // Test diagonals
        if(TestWinnerLine(playerTypeArray[0, 0], playerTypeArray[1, 1], playerTypeArray[2, 2]))
        {
            PlayerType winnerPlayerType = playerTypeArray[0, 0];
             if (winnerPlayerType == PlayerType.Cross)
                {
                    crossPlayerScore.Value++;
                }
                else
                {
                    circlePlayerScore.Value++;
                }
            Debug.Log("Winner is : " + playerTypeArray[0, 0]);
            TriggerOnGameWinRpc(new Vector2Int(1, 1), Direction.DiagonalLeftToRight , winnerPlayerType);
            return;
        }
        if(TestWinnerLine(playerTypeArray[2, 0], playerTypeArray[1, 1], playerTypeArray[0, 2]))
        {
            PlayerType winnerPlayerType = playerTypeArray[2, 0];
             if (winnerPlayerType == PlayerType.Cross)
                {
                    crossPlayerScore.Value++;
                }
                else
                {
                    circlePlayerScore.Value++;
                }
            Debug.Log("Winner is : " + playerTypeArray[2, 0]);
            TriggerOnGameWinRpc(new Vector2Int(1, 1), Direction.DiagonalRightToLeft , winnerPlayerType  );
            return;
        }

        bool isDraw = true;
        for(int x = 0; x < 3; x++)
        {
            for(int y = 0; y < 3; y++)
            {
                if(playerTypeArray[x, y] == PlayerType.None)
                {
                    isDraw = false;
                    break;
                }
            }
        }
        if(isDraw)
        {
             Debug.Log("Game is a draw !");
             TriggerOnGameDrawRpc();
        }
    }   

    [Rpc(SendTo.ClientsAndHost)]
    void TriggerOnGameDrawRpc()
    {
        OnGameDraw?.Invoke(this, EventArgs.Empty);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TriggerOnGameWinRpc(Vector2Int centerGridPosition, Direction winDirection , PlayerType winnerPlayerType)
    {
        OnGameWin?.Invoke(this, new OnGameWinEventArgs {
            centerGridPosition = centerGridPosition,
            winDirection = winDirection,
            winnerPlayerType = winnerPlayerType
        });
    }

    // only the server can call this method and it will reset the game state and notify all clients to update their UI accordingly
    [Rpc(SendTo.Server)]
    public void RematchRpc()
    {
        for(int x = 0; x < 3; x++)
        {
            for(int y = 0; y < 3; y++)
            {
                playerTypeArray[x, y] = PlayerType.None;
            }
        }
        // can make it random or keep the same player starting first
        currentPlayerableType.Value = PlayerType.Cross;
        TriggerOnRematchRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    void TriggerOnRematchRpc()
    {
        OnRematch?.Invoke(this, EventArgs.Empty);
    }

    public PlayerType GetLocalPlayerType()
    {
        return localPlayerType;
    }   

    public PlayerType GetCurrentPlayerableType()
    {
        return currentPlayerableType.Value;
    }

    public void GetScore(out int crossScore, out int circleScore)
    {
        crossScore = crossPlayerScore.Value;
        circleScore = circlePlayerScore.Value;
    }
}