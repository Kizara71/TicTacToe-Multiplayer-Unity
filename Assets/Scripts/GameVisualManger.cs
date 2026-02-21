using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class GameVisualManger : NetworkBehaviour
{
    private const float GRID_CELL_SIZE = 3.1f;
    [SerializeField ] private Transform crossPrefab;
    [SerializeField ] private Transform circlePrefab;

    void Start()
    {
        GameManger.Instance.OnGridPositionClicked += GameManger_OnClickedGridPosition;
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
        Transform spawnedObject = Instantiate (spawnedPrefab , GetGridWorldPosition(x, y) , quaternion.identity);
        spawnedObject.GetComponent<NetworkObject>().Spawn(true); // spawn the object on the network so that all clients can see it
    }

    Vector2 GetGridWorldPosition(int x, int y  )
    {
        return new Vector2(-GRID_CELL_SIZE + x * GRID_CELL_SIZE, -GRID_CELL_SIZE + y * GRID_CELL_SIZE );
    }
}