using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject crossArrowGameObject;
    [SerializeField] private GameObject circleArrowGameObject;  
    [SerializeField] private GameObject crossTextGameObject;  
    [SerializeField] private GameObject circleTextGameObject;
    void Awake()
    {
        crossArrowGameObject.SetActive(false);
        circleArrowGameObject.SetActive(false);
        crossTextGameObject.SetActive(false);
        circleTextGameObject.SetActive(false);
    }

    void Start()
    {
        GameManger.Instance.OnGameStarted += GameManger_OnGameStarted;
        GameManger.Instance.OnCurrentPlayerableTypeChanged += GameManger_OnCurrentPlayerableTypeChanged;
    }

    private void GameManger_OnCurrentPlayerableTypeChanged(object sender, System.EventArgs e)
    {
        UpdatePlayerArrow();
    }   
    
    private void GameManger_OnGameStarted(object sender, System.EventArgs e)
    {
        if(GameManger.Instance.GetLocalPlayerType() == GameManger.PlayerType.Cross)
        {
            crossTextGameObject.SetActive(true);
        }
        else
        {
            circleTextGameObject.SetActive(true);
        }
    }
    void UpdatePlayerArrow()
    {
        if(GameManger.Instance.GetCurrentPlayerableType() == GameManger.PlayerType.Cross)
        {
            crossArrowGameObject.SetActive(true);
            circleArrowGameObject.SetActive(false);
        }
        else
        {
            crossArrowGameObject.SetActive(false);
            circleArrowGameObject.SetActive(true);
        }
    }
}
