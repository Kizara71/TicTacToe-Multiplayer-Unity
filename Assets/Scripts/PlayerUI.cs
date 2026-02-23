using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
        crossArrowGameObject.GetComponent<Image>().color = Color.black;
        circleArrowGameObject.GetComponent<Image>().color = Color.black;
        crossTextGameObject.SetActive(false);
        circleTextGameObject.SetActive(false);
        crossPlayerScoreText.text = "0";
        circlePlayerScoreText.text = "0";
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
            crossArrowGameObject.GetComponent<Image>().color = Color.white;
            circleArrowGameObject.GetComponent<Image>().color = Color.black;
        }
        else
        {
            crossArrowGameObject.GetComponent<Image>().color = Color.black;
            circleArrowGameObject.GetComponent<Image>().color = Color.white;
        }
    }
}
