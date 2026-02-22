using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameVisualManger : NetworkBehaviour
{
    private List<GameObject> visualGameObjects;
    private const float GRID_CELL_SIZE = 3.1f;
    [SerializeField ] private Transform crossPrefab;
    [SerializeField ] private Transform circlePrefab;
    [SerializeField ] private Transform lineCompletePrefab;    

    void Awake()
    {
        visualGameObjects = new List<GameObject>();
    }

    void Start()
    {
        GameManger.Instance.OnGridPositionClicked += GameManger_OnClickedGridPosition;
        GameManger.Instance.OnGameWin += GameManger_OnGameWin;
        GameManger.Instance.OnRematch += GameManger_OnRematch;
    }

    void GameManger_OnRematch(object sender, System.EventArgs e)
    {
        if(!NetworkManager.Singleton.IsServer)
        {
            return;
        }
        foreach(GameObject gameObject in visualGameObjects)
        {
            Destroy(gameObject);
        }
        visualGameObjects.Clear();
    }

    void GameManger_OnGameWin(object sender, GameManger.OnGameWinEventArgs e)
    {
        if(!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        float eulerAngleZ = 0f; 
        
        switch(e.winDirection)
        {
            default:
            case GameManger.Direction.Horizontal:
                eulerAngleZ = 0f;
                break;
            case GameManger.Direction.Vertical:
                eulerAngleZ = 90f;
                break;
            case GameManger.Direction.DiagonalLeftToRight:
                eulerAngleZ = 45f;
                break;
            case GameManger.Direction.DiagonalRightToLeft:
                eulerAngleZ = -45f;
                break;
        }   

        Transform lineComplete = Instantiate(lineCompletePrefab, (Vector3)GetGridWorldPosition(e.centerGridPosition.x, e.centerGridPosition.y), Quaternion.Euler(0f, 0f, eulerAngleZ));
        lineComplete.GetComponent<NetworkObject>().Spawn(true);
        visualGameObjects.Add(lineComplete.gameObject);
    }   

    void GameManger_OnClickedGridPosition(object sender , GameManger.OnClickedGridPositionEventArgs e)
    {
        Debug.Log("GameVisualManger received grid position click event");
        SpawnObjectRpc(e.x , e.y , e.playerType );
    }

    [Rpc(SendTo.Server)]
    void SpawnObjectRpc(int x , int y , GameManger.PlayerType playerType)
    {
        Debug.Log("Spawning object from rpc call ");
        Transform spawnedPrefab = playerType == GameManger.PlayerType.Cross ? crossPrefab : circlePrefab;
        Transform spawnedObject = Instantiate(spawnedPrefab, (Vector3)GetGridWorldPosition(x, y), Quaternion.identity);
        spawnedObject.GetComponent<NetworkObject>().Spawn(true); // spawn the object on the network so that all clients can see it
        visualGameObjects.Add(spawnedObject.gameObject);    
    }

    Vector2 GetGridWorldPosition(int x, int y  )
    {
        return new Vector2(-GRID_CELL_SIZE + x * GRID_CELL_SIZE, -GRID_CELL_SIZE + y * GRID_CELL_SIZE );
    }
}