using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject crossArrowGameObject;
    [SerializeField] private GameObject circleArrowGameObject;  
    [SerializeField] private GameObject crossTextGameObject;  
    [SerializeField] private GameObject circleTextGameObject;
    [SerializeField] private TextMeshProUGUI crossPlayerScoreText;
    [SerializeField] private TextMeshProUGUI circlePlayerScoreText;
    void Awake()
    {
        crossArrowGameObject.SetActive(false);
        circleArrowGameObject.SetActive(false);
        crossTextGameObject.SetActive(false);
        circleTextGameObject.SetActive(false);
        crossPlayerScoreText.text = "";
        circlePlayerScoreText.text = "";
    }

    void Start()
    {
        GameManger.Instance.OnGameStarted += GameManger_OnGameStarted;
        GameManger.Instance.OnCurrentPlayerableTypeChanged += GameManger_OnCurrentPlayerableTypeChanged;
        GameManger.Instance.OnScoreChanged += GameManger_onScoreChanged;
    }

    private void GameManger_onScoreChanged(object sender, System.   EventArgs e)
    {
        int crossScore, circleScore;
        GameManger.Instance.GetScore(out crossScore, out circleScore);
        crossPlayerScoreText.text = crossScore.ToString();
        circlePlayerScoreText.text = circleScore.ToString();
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
