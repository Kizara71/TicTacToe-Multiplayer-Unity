using Unity.VisualScripting;
using UnityEngine;

public class SoundManger : MonoBehaviour
{
    [SerializeField] private Transform placeSFXPrefab;
    [SerializeField] private Transform winSFXPrefab;
    [SerializeField] private Transform loseSFXPrefab;
    void Start()
    {
        GameManger.Instance.OnPlacedSymbol += GameManger_OnPlacedSymbol;
        GameManger.Instance.OnGameWin += GameManger_OnGameWin;
    }

    private void GameManger_OnGameWin(object sender, GameManger.OnGameWinEventArgs e)
    {
        if(e.winnerPlayerType == GameManger.Instance.GetLocalPlayerType())
        {
            Transform winSFXTransform = Instantiate(winSFXPrefab);
            Destroy(winSFXTransform.gameObject, 3f);
        }
        else
        {
            Transform loseSFXTransform = Instantiate(loseSFXPrefab);
            Destroy(loseSFXTransform.gameObject, 2f);
        }
    }

    private void GameManger_OnPlacedSymbol(object sender, System.EventArgs e)
    {
        Transform placeSFXTransform = Instantiate(placeSFXPrefab);
        Destroy(placeSFXTransform.gameObject, 1f);
    }
}
